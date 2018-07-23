using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using Cyient.MDT.WebAPI.Core.Interface.SubSystem;
using Cyient.MDT.WebAPI.Core.Entities.SubSystem;
using Cyient.MDT.WebAPI.Core.Common;

namespace Cyient.MDT.Infrastructure.SubSystemRepository
{
    public class SubSystemRepository : ISubSystem
    {
        public SubSystemRepository() { }

        /// <summary>
        /// It will return the Sub System details in landing page for sales based on Package ID
        /// </summary>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        //public MDTTransactionInfo GetPackageSystemDetails(int PackageID)
        //{

        //    MDTTransactionInfo mdt = new MDTTransactionInfo();
        //    List<PackageSystem> PackageSystems = null;
        //    List<SqlParameter> prm = new List<SqlParameter>();
        //    SqlParameter package_Id = new SqlParameter("@PackageID", PackageID);
        //    prm.Add(package_Id);
        //    SqlParameter status = new SqlParameter("@Status", 0);
        //    status.Direction = ParameterDirection.Output;
        //    prm.Add(status);
        //    DataTable dt = DatabaseSettings.GetDataSet("sp_GetPackageSystems", out APIHelper.StatusValue, prm).Tables[0];
        //    if (APIHelper.StatusValue == 1)
        //    {
        //        if (dt.Rows.Count > 0)
        //        {
        //            PackageSystem packageSystem;
        //            PackageSystems = new List<PackageSystem>();
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                packageSystem = new PackageSystem();

        //                packageSystem.PACKAGE_ID = Convert.ToInt32(row["PACKAGE_ID"]);
        //                packageSystem.SYSTEM_ID = Convert.ToInt32(row["SYSTEM_ID"]);
        //                packageSystem.SYSTEM_VARIANT_ID = Convert.ToInt32(row["SYSTEM_VARIANT_ID"]);
        //                packageSystem.SELECT = Convert.ToBoolean(row["SELECT"]);
        //                packageSystem.SYSTEM_IMAGE = row["SYSTEM_IMAGE"].ToString();
        //                packageSystem.SYSTEM_NAME = row["SYSTEM_NAME"].ToString();
        //                packageSystem.EQUIPMENT_COST = float.Parse(row["EQUIPMENT_COST"].ToString());
        //                packageSystem.ELECTRICAL_COST = float.Parse(row["ELECTRICAL_COST"].ToString());
        //                packageSystem.MECHANICAL_COST = float.Parse(row["MECHANICAL_COST"].ToString());
        //                packageSystem.COMMENTS = row["COMMENTS"].ToString();
        //                packageSystem.REMARKS = row["REMARKS"].ToString();
        //                if (Convert.ToInt32(row["SYSTEM_VARIANT_ID"]) != 0)
        //                {
        //                    packageSystem.SystemVariants = GetSystemVariants(Convert.ToInt32(row["SYSTEM_VARIANT_ID"]));
        //                }
        //                else
        //                {
        //                    packageSystem.SystemVariants = null;
        //                }
        //                PackageSystems.Add(packageSystem);
        //            }
        //        }
        //        mdt.transactionObject = PackageSystems;
        //        mdt.status = HttpStatusCode.OK;
        //        mdt.msgCode = MessageCode.Success;
        //        mdt.message = "Package System Details Fetched Successfully";
        //    }
        //    else  if(APIHelper.StatusValue == 5)
        //    {
        //        ErrorInfoFromSQL eInfo = null;
        //        if (dt.Rows.Count > 0)
        //        {
        //            eInfo = new ErrorInfoFromSQL();
        //            eInfo = DatabaseSettings.GetError(dt);
        //            mdt.status = HttpStatusCode.BadRequest;
        //            mdt.transactionObject = eInfo;
        //            mdt.msgCode = (eInfo.Status == 1) ? MessageCode.Success : MessageCode.Failed;
        //            mdt.message = eInfo.ErrorMessage;
        //            mdt.LineNumber = eInfo.ErrorLineNo;
        //            mdt.ProcedureName = eInfo.Procedure;
        //        }
        //    }

        //    return mdt;
        //}
        /// <summary>
        /// It will return the Sub System details in landing page for sales based on Package ID
        /// </summary>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        public MDTTransactionInfo GetPackageSystemDetails(int PackageID)
        {

            MDTTransactionInfo mdt = new MDTTransactionInfo();
            IEnumerable<PackageSystem> PackageSystems = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter package_Id = new SqlParameter("@PackageID", PackageID);
            prm.Add(package_Id);
            SqlParameter status = new SqlParameter("@Status", 0);
            status.Direction = ParameterDirection.Output;
            prm.Add(status);
            int StatusValue = 0;
            DataTable dt = DatabaseSettings.GetDataSet("sp_GetPackageSystems", out StatusValue, prm).Tables[0];
            if (StatusValue == 1)
            {
                if (dt.Rows.Count > 0)
                {

                    PackageSystems = from d in dt.AsEnumerable()
                                     select new PackageSystem
                                     {
                                         PACKAGE_ID = d.Field<int>("PACKAGE_ID"),
                                         SYSTEM_ID = d.Field<int>("SYSTEM_ID"),
                                         SYSTEM_VARIANT_ID = d.Field<int>("SYSTEM_VARIANT_ID"),
                                         SELECT = Convert.ToBoolean(d.Field<int>("SELECT")),
                                         SYSTEM_IMAGE = d.Field<string>("SYSTEM_IMAGE"),
                                         SYSTEM_NAME = d.Field<string>("SYSTEM_NAME"),
                                         EQUIPMENT_COST = d.Field<double>("EQUIPMENT_COST"),
                                         ELECTRICAL_COST = d.Field<double>("ELECTRICAL_COST"),
                                         MECHANICAL_COST = d.Field<double>("MECHANICAL_COST"),
                                         COMMENTS = d.Field<string>("COMMENTS"),
                                         REMARKS = d.Field<string>("REMARKS"),
                                         SystemVariants = GetSystemVariants(d.Field<int>("SYSTEM_VARIANT_ID"))
                                     };

                }
            }
            mdt = DatabaseSettings.GetTransObject(PackageSystems, StatusValue, "Package System Details Fetched Successfully", dt);
            return mdt;
        }
        private IEnumerable<SystemVariants> GetSystemVariants(int VariantID)
        {
            IEnumerable<SystemVariants> systemVariants = null;
            List<SqlParameter> prm = new List<SqlParameter>();
            SqlParameter System_Variant_ID = new SqlParameter("@System_Variant_ID", VariantID);
            prm.Add(System_Variant_ID);
            SqlParameter status = new SqlParameter("@Status", 0);
            status.Direction = ParameterDirection.Output;
            prm.Add(status);
            int StatusValue = 0;
            DataTable dt = DatabaseSettings.GetDataSet("sp_GetSystemVariants", out StatusValue, prm).Tables[0];
            if (StatusValue == 1)
            {
                if (dt.Rows.Count > 0)
                {
                    systemVariants = from d in dt.AsEnumerable()
                                     select new SystemVariants
                                     {
                                         SYSTEM_ID = d.Field<int>("SYSTEM_ID"),
                                         SYSTEM_VARIANT_ID = d.Field<int>("SYSTEM_VARIANT_ID"),
                                         SYSTEM_IMAGE = d.Field<string>("SYSTEM_IMAGE"),
                                         SYSTEM_NAME = d.Field<string>("SYSTEM_NAME"),
                                         EQUIPMENT_COST = d.Field<double>("EQUIPMENT_COST"),
                                         ELECTRICAL_COST = d.Field<double>("ELECTRICAL_COST"),
                                         MECHANICAL_COST = d.Field<double>("MECHANICAL_COST"),
                                         COMMENTS = d.Field<string>("COMMENTS"),
                                         REMARKS = d.Field<string>("REMARKS")
                                     };
                }
            }
            return systemVariants;
        }
    }
}
