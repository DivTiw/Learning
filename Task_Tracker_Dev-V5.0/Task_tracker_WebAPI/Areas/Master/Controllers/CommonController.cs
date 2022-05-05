using Common_Components;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_Library.Repository;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Master.Controllers
{
    public class CommonController : BaseAPIController
    {
        [HttpPost]
        public IHttpActionResult GetDDLData([FromBody] DDLDTO ddlData)
        {
            if (ddlData == null)
            {
                return Content(HttpStatusCode.BadRequest,"");//Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request object can not be null.Please check your request and try again.", new Exception("The Request is null which can not be processed.", new Exception("This is inner exception!")));
                //var httpResp = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                //httpResp.ReasonPhrase = "Request object can not be null. Please check your request and try again.";
                //throw new HttpResponseException(httpResp);//400, "Request object can not be null. Please check your request and try again."
            }
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    uow.CommonRepo.fillDDLdata(ddlData);
                    ddlData.opStatus = true;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetDDLData", "CommonController");
                ddlData.opStatus = false;
                ddlData.opMsg = ex.Message;
                ddlData.opInnerException = ex;
            }

            return Ok(ddlData);//Request.CreateResponse(HttpStatusCode.OK, ddlData);
        }

    }
}
