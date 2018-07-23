using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using Cyient.MDT.Infrastructure.SubSystemRepository;
using Cyient.MDT.WebAPI.Core.Common;
namespace Cyient.MDT.WebAPI.Controllers.SubSystem
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SubSystemController : ApiController
    {
        SubSystemRepository _service = new SubSystemRepository();

        [HttpGet]
        [Route("api/SubSystem/GetPackageSystem/{PackageID}")]
        public IHttpActionResult GetPackageSystem(int PackageID)
        {
            MDTTransactionInfo tInfo = null;
            try
            {
                tInfo = (PackageID == 0) ? new MDTTransactionInfo { msgCode = MessageCode.Failed, message = "Invalid Data", status = HttpStatusCode.BadRequest }
                                            : _service.GetPackageSystemDetails(PackageID);
            }
            catch (Exception ex)
            {
                tInfo = new MDTTransactionInfo { msgCode = MessageCode.Failed, message = ex.Message, status = HttpStatusCode.InternalServerError };
            }

            return Ok(tInfo);
        }

    }
}
