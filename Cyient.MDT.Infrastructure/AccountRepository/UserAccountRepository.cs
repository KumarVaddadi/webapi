using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Cyient.MDT.WebAPI.Core.Entities.Account;
using Cyient.MDT.WebAPI.Core.Interface.Account;
using Cyient.MDT.WebAPI.Core.Common;
using Cyient.MDT.WebAPI.Notification.Product;
using Cyient.MDT.WebAPI.Notification.ConcreteProduct;
using Cyient.MDT.WebAPI.Notification.Interface;
using Cyient.MDT.WebAPI.Notification;
using System.Net;
using System.Web;
using System.Net.Http;

namespace Cyient.MDT.Infrastructure.AccountRepository
{
    public class UserAccountRepository : IUserAccount
    {

        public UserAccountRepository() { }

        /// <summary>
        /// This is login method and it will connect to DB and check if user is exists or not. If user exists then it will display the user detail
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public MDTTransactionInfo Login(UserLogin userLogin)
        {
            MDTTransactionInfo mdt = new MDTTransactionInfo();
            UserLoginDetails loginDetails = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter email = new SqlParameter("@email", userLogin.UserName);
            prm.Add(email);
            SqlParameter pwd = new SqlParameter("@pwd", userLogin.Password);
            prm.Add(pwd);

            SqlParameter status = new SqlParameter("@Status", 0);
            status.Direction = ParameterDirection.Output;
            prm.Add(status);

           DataTable dt = DatabaseSettings.GetDataSet("sp_LoginUser", out APIHelper.StatusValue, prm).Tables[0];

            if (APIHelper.StatusValue == 1)
            {
                if (dt.Rows.Count > 0)
                {
                    loginDetails = new UserLoginDetails();
                    loginDetails.USER_ID = Convert.ToInt32(dt.Rows[0]["USER_ID"]);
                    loginDetails.FIRST_NAME = dt.Rows[0]["FIRST_NAME"].ToString();
                    loginDetails.LAST_NAME = dt.Rows[0]["LAST_NAME"].ToString();
                    loginDetails.EMAIL_ADDRESS = dt.Rows[0]["EMAIL_ADDRESS"].ToString();
                    loginDetails.FORCE_PWD_CHNG = Convert.ToBoolean(dt.Rows[0]["FORCE_PWD_CHNG"]);
                    loginDetails.PHOTO = dt.Rows[0]["PHOTO"].ToString();
                    loginDetails.ROLE_NAME = dt.Rows[0]["ROLE_NAME"].ToString();
                    loginDetails.ROLE_ID = Convert.ToInt32(dt.Rows[0]["ROLE_ID"]);
                }
                mdt.status = HttpStatusCode.OK;
                mdt.transactionObject = loginDetails;
                mdt.msgCode = MessageCode.Success;
                mdt.message = "Login Successfully";
            }
            else if (APIHelper.StatusValue == 5)
            {
                ErrorInfoFromSQL eInfo = null;
                if (dt.Rows.Count > 0)
                {
                    eInfo = new ErrorInfoFromSQL();
                    eInfo = DatabaseSettings.GetError(dt);
                    mdt.status = HttpStatusCode.BadRequest;
                    mdt.transactionObject = eInfo;
                    mdt.msgCode = (eInfo.Status == 1) ? MessageCode.Success : MessageCode.Failed;
                    mdt.message = eInfo.ErrorMessage;
                    mdt.LineNumber = eInfo.ErrorLineNo;
                    mdt.ProcedureName = eInfo.Procedure;
                }
            }
            return mdt;
        }

        /// <summary>
        /// Change password method which will interact with database and change the password
        /// </summary>
        /// <param name="changePassword">You need to pass ChangePassword type object to process the request</param>
        /// <returns></returns>
        public MDTTransactionInfo ChangePassword(ChangePassword changePassword)
        {
            MDTTransactionInfo mdt = new MDTTransactionInfo();
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter email = new SqlParameter("@email", changePassword.Email);
            prm.Add(email);

            SqlParameter oldPwd = new SqlParameter("@oldPwd", changePassword.OldPassword);
            prm.Add(oldPwd);

            SqlParameter NewPwd = new SqlParameter("@newPwd", changePassword.NewPassword);
            prm.Add(NewPwd);

            SqlParameter status = new SqlParameter("@Status", 0);
            status.Direction = ParameterDirection.Output;
            prm.Add(status);

            DataTable dt = DatabaseSettings.GetDataSet("sp_UpdatePassword", out APIHelper.StatusValue, prm).Tables[0];
            if (APIHelper.StatusValue == 1)
            {
                mdt.msgCode = MessageCode.Success;
                mdt.status = HttpStatusCode.OK;
            }
            else if (APIHelper.StatusValue == 5)
            {
                mdt.status = HttpStatusCode.BadRequest;
                mdt.msgCode = MessageCode.Failed;
            }
            if (dt.Rows.Count == 1)
            {
                ErrorInfoFromSQL eInfo = DatabaseSettings.GetError(dt);
                mdt.transactionObject = eInfo;
                mdt.message = eInfo.ErrorMessage;
                mdt.LineNumber = eInfo.ErrorLineNo;
                mdt.ProcedureName = eInfo.Procedure;
            }
            return mdt;
        }

        /// <summary>
        /// It will auto generate new random password and send to user on his email.
        /// </summary>
        /// <param name="forgotPassword"></param>
        /// <returns></returns>
        public MDTTransactionInfo ForgotPassword(ForgotPassword forgotPassword)
        {
            MDTTransactionInfo mdt = new MDTTransactionInfo(); ;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter email = new SqlParameter("@email", forgotPassword.Email);
            prm.Add(email);
            SqlParameter status = new SqlParameter("@Status", 0);
            status.Direction = ParameterDirection.Output;
            prm.Add(status);
            int StatusValue = 0;
            DataSet ds = DatabaseSettings.GetDataSet("sp_UpdatePassword",out StatusValue, prm);

            DataTable dt = ds.Tables[0];
            ErrorInfoFromSQL eInfo = null;
            if(StatusValue == 1)
            {
                mdt.status = HttpStatusCode.OK;
            }else if (StatusValue == 5)
            {
                mdt.status = HttpStatusCode.BadRequest;
            }
            if (dt.Rows.Count == 1)
            {
                eInfo = DatabaseSettings.GetError(dt);
                mdt.transactionObject = eInfo;
                mdt.msgCode = (eInfo.Status == 1) ? MessageCode.Success : MessageCode.Failed;
                mdt.message = eInfo.ErrorMessage;
                mdt.LineNumber = eInfo.ErrorLineNo;
                mdt.ProcedureName = eInfo.Procedure;
            }
            // If above call success then sending an email to user with latest password.
            if (eInfo.Status == 1)
            {
                dt = ds.Tables[1];
                if (dt.Rows.Count > 0)
                {
                    SendMailRequest sendMailRequest = new SendMailRequest();
                    sendMailRequest.recipient = dt.Rows[0]["Email"].ToString();
                    sendMailRequest.subject = "MDT Password Reset";
                    sendMailRequest.body = "Dear User," + Environment.NewLine + "Your password has been reset successfully. please login with new password given below " + Environment.NewLine + "New Password : " + dt.Rows[0]["New Password"].ToString();
                    IMessager messager = new Email();
                    var Notification = new Notification(messager);
                    mdt.message = Notification.DoNotify(sendMailRequest);
                }
            }

            return mdt;
        }


        public MDTTransactionInfo UploadProfilePic(List<string> files)
        {
            HttpResponseMessage result = null;
            MDTTransactionInfo mdt = new MDTTransactionInfo();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    docfiles.Add(filePath);
                }
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
                
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
    }
}
