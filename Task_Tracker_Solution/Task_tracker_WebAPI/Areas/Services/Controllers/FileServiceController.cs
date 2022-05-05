using Common_Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Services.Controllers
{
    public class FileServiceController : BaseAPIController
    {
        FileService fs = null;
        TokenServer ts = null;
        public FileServiceController()
        {
            fs = new FileService(webClient);
            ts = new TokenServer();
        }

        [HttpPost]
        public void UploadFiles([FromBody] TaskAttachment ta)
        {
            throw new NotImplementedException("Upload via this is currently disabled!");

            //string sDwnldObj = HttpContext.Current.Request["DownloadObj"];
            //ta = JsonConvert.DeserializeObject<TaskAttachment>(sDwnldObj);
            //HttpPostedFile file = HttpContext.Current.Request.Files[0];
            //fs.UploadFiles(file, ta);
        }

        [HttpPost]
        public IHttpActionResult DownloadFile(TaskAttachment ta)
        {
            string _ExceptionMSG = string.Empty;
            try
            {
                var response = new HttpResponseMessage();

                TokenModel tm = new TokenModel();
                //tm.CreatedBy = ta.created_by;
                //tm.AttachmentGUID = ta.attachment_identifier.ToString();
                //tm.RecordSyscode = ta.attachment_syscode;
                //tm.MongoID = ta.mongo_file_id;
                //tm.UserRole = "User";
                //tm.EmployeeSyscode = ta.created_by;
                tm.UserID = ta.created_by;

                tm.Payload = new Dictionary<string, object>();
                tm.Payload.Add("record_syscode", ta.attachment_identifier.ToString());
                tm.Payload.Add("file_id", ta.mongo_file_id);

                string token = string.Empty;
                token = ts.GetToken(tm);

                if (ts.ValidateToken(token))
                {
                    string decToken = ts.DecodeToken(token); //Just for testing.
                }
                else
                {
                    throw new Exception("Download token generation failed or the generated token is invalid. Token is: " + token);
                }

                var retResp = webClient.GetAsync("api/Files/downloadFile?token=" + token);
                response = retResp.Result;
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    IHttpActionResult fileResult = ResponseMessage(response);
                    return fileResult;
                }
                else
                {
                    throw new HttpException(Convert.ToInt32(response.StatusCode), "Request to mongo API failed");
                }
            }
            catch (Exception ex)
            {
                _ExceptionMSG = ex.Message;
                Log.LogError(_ExceptionMSG, "", null, "DownloadFiles", "FileServiceController");
                throw new HttpException(500, "Exception occured in API while fetching the file!");
            }
            finally
            {
            }
        }

    }
}
