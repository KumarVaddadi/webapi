using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Cyient.MDT.Infrastructure.BasicInputRepository;
using Cyient.MDT.WebAPI.Core.Common;
using Cyient.MDT.WebAPI.Core.Entities.BasicInput;
namespace Cyient.MDT.WebAPI.Controllers.BasicInput
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[EnableCors("*", "*", "*")]
    public class BasicInputController : ApiController
    {

        BasicInputRepository _service = new BasicInputRepository();
        // GET api/values
        [HttpGet]
        [Route("api/BasicInput/GetBasicInput")]
        public IHttpActionResult Get()
        {
            MDTTransactionInfo tInfo = null;
            try
            {
                tInfo = _service.GetBasicInputs();
            }
            catch (Exception ex)
            {
                tInfo = new MDTTransactionInfo { msgCode = MessageCode.Failed, message = ex.Message, status = HttpStatusCode.InternalServerError };
            }
            return Ok(tInfo);
        }

    }
}
