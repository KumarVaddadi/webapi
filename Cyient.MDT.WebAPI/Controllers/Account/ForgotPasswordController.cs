using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http;
using Cyient.MDT.Infrastructure.AccountRepository;
using Cyient.MDT.WebAPI.Core.Entities.Account;
using Cyient.MDT.WebAPI.Core.Common;
using System.Threading.Tasks;
namespace Cyient.MDT.WebAPI.Controllers.Account
{
    public class ForgotPasswordController : ApiController
    {
        UserAccountRepository _service = new UserAccountRepository();

        // GET api/values
        [HttpPost]
        [Route("api/ForgotPassword/ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            MDTTransactionInfo transactionInfo = null;
            transactionInfo = await Task.Run(() =>
            {
                try
                {
                    return _service.ForgotPassword(forgotPassword);
                }
                catch (Exception ex)
                {
                    return new MDTTransactionInfo { msgCode = MessageCode.Failed, message = ex.Message, status = HttpStatusCode.InternalServerError };
                }
            });

            return Ok(transactionInfo);
        }
    }
}
