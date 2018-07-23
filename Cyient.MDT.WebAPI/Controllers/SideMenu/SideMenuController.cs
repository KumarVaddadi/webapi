using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Cyient.MDT.Infrastructure.SideMenuRepository;
using Cyient.MDT.WebAPI.Core.Common;

namespace Cyient.MDT.WebAPI.Controllers.SideMenu
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SideMenuController : ApiController
    {
        SideMenuRepository _service = new SideMenuRepository();

        [HttpGet]
        [Route("api/SideMenu/GetSideMenu/{UserID}")]
        public IHttpActionResult GetSideMenu(int UserID)
        {
            MDTTransactionInfo tInfo = null;
            try
            {
                tInfo = (UserID == 0) ? new MDTTransactionInfo { msgCode = MessageCode.Failed, message = "Invalid Data", status = HttpStatusCode.BadRequest }
                                            : _service.GetSideMenu(UserID);
            }
            catch (Exception ex)
            {
                tInfo = new MDTTransactionInfo { msgCode = MessageCode.Failed, message = ex.Message, status = HttpStatusCode.InternalServerError };
            }

            return Ok(tInfo);
        }
    }
}
