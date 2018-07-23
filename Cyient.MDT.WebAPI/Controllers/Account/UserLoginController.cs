using System;
using System.Net;
using System.Web.Http.Cors;
using System.Web.Http;
using Cyient.MDT.Infrastructure.AccountRepository;
using Cyient.MDT.WebAPI.Core.Entities.Account;
using Cyient.MDT.WebAPI.Core.Common;
using System.Threading.Tasks;

namespace Cyient.MDT.WebAPI.Controllers.Account
{
   [EnableCors("*", "*", "*")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[RoutePrefix("api/UserLogin")]
    public class UserLoginController : ApiController
    {
        UserAccountRepository _service = new UserAccountRepository();
        // GET api/values
        [HttpPost]
        [Route("api/UserLogin/Login")]
        //[Route("login", Name = "Login")]
        public async Task<IHttpActionResult> Login(UserLogin userLogin)
        {
            MDTTransactionInfo transactionInfo = null;
            transactionInfo = await Task.Run(() =>
            {
                try
                {
                    return _service.Login(userLogin);
                }
                catch (Exception ex)
                {
                    return new MDTTransactionInfo { msgCode = MessageCode.Failed, message = ex.Message, status = HttpStatusCode.InternalServerError };
                }
            });

            return Ok(transactionInfo);
        }


        [HttpPost]
        [Route("api/UserLogin/GetPost")]
        public string GetPost()
        {
            return "This is post method called from Angular";
        }
    }
}
