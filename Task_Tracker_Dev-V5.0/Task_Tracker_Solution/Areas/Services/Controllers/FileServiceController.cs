using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Mime;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Services.Controllers
{
    public class FileServiceController : TaskTrackerBaseController
    {
        public FileServiceController()
        {
            //SessionExpiredMsg = ConfigurationManager.AppSettings["SessionExpiredMsg"].ToString();
        }        

        [HttpGet]
        public FileResult DownloadFile(string AttachGuid, string MFileId, string FileName)
        {
            byte[] mByte = null;
            //string FileName = string.Empty;
            //string FileContent = string.Empty;

            string mReturnValue = string.Empty;
            string mErrMsg = string.Empty;

            TaskAttachment ta = null;
            try
            {
                ta = new TaskAttachment();
                ta.attachment_identifier = new Guid(AttachGuid);
                ta.attachment_filename = FileName;
                ta.mongo_file_id = MFileId;
                ta.created_by = Convert.ToInt32(this.Session["UserSyscode"]);                

                var response = new HttpResponseMessage();
                response = client.PostAsJsonAsync(cWebApiNames.APIDownloadAttachments, ta).Result;
                if (response.IsSuccessStatusCode)
                {
                    mByte = response.Content.ReadAsByteArrayAsync().Result;//ReadAsStringAsync().Result;
                    return File(mByte, MediaTypeNames.Application.Octet, FileName);
                }
                else
                {
                    //ModelState.AddModelError("DownloadError", ConfigurationManager.AppSettings["APITimeOutError"].ToString());

                    mReturnValue = response.Content.ReadAsStringAsync().Result;                    
                    ModelState.AddModelError("DownloadError", new Exception(mReturnValue));
                    ViewBag.ErrorMessage = mReturnValue;
                    TempData["ErrorMessage"] = mReturnValue;
                    //mErrMsg = Common.Get_ErrMsg_WebAPI_IsSuccessStatusCode_Failed(mReturnValue);

                    //Log.LogError(mErrMsg, Convert.ToString(this.Session["UserName"]), Convert.ToString(this.Session["EmployeeSyscode"]), this.ControllerContext,  );
                    //return ;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("DownloadError", ex.Message);
                ViewBag.ErrorMessage = mReturnValue;
                TempData["ErrorMessage"] = mReturnValue;
                //Log.LogError(ex.Message, Convert.ToString(this.Session["UserName"]), Convert.ToString(this.Session["EmployeeSyscode"]), this.ControllerContext, null, null);
            }
            //finally
            //{
            //    client = null;
            //}
            return File(new Byte[100], MediaTypeNames.Application.Octet, "");
        }
    }
}

//[HttpPost]
//[ValidateInput(false)]
//public JsonResult UploadFiles(HttpPostedFileBase file, string AttachFieldDetails)
//{
//    string PEDetails_Syscode = "";

//    //DownloadAttachment mDownloadAttachment = null;
//    //AttachmentsData ad = null;

//    try
//    {
//        if (this.Session["UserSession"] != null)
//        {
//            //mDownloadAttachment = Common.ReadFileBinary(file);
//            TaskAttachment ta = new TaskAttachment { };
//            //mDownloadAttachment.PEDetails_Syscode = Convert.ToInt32(Session["PEDetails_Syscode"]);
//            //mDownloadAttachment.CreatedBy = Convert.ToInt32(this.Session["UserSyscode"]);// //PEDetails_syscode
//            //mDownloadAttachment.UserRole = Session["UserRole"].ToString();
//            //mDownloadAttachment.EmployeeSyscode = Convert.ToInt32(Session["EmployeeSyscode"]);

//            //mDownloadAttachment.CreatedBy = mDownloadAttachment.CreatedBy == 0 && mDownloadAttachment.EmployeeSyscode > 0 ? mDownloadAttachment.EmployeeSyscode : mDownloadAttachment.CreatedBy;
//            //mDownloadAttachment.EmployeeSyscode = mDownloadAttachment.EmployeeSyscode == 0 && mDownloadAttachment.CreatedBy > 0 ? mDownloadAttachment.CreatedBy : mDownloadAttachment.EmployeeSyscode;

//            //var PageDetails = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(AttachFieldDetails)[0];

//            //mDownloadAttachment.PEDetails_Syscode = Convert.ToInt32(PageDetails["PEDetails_Syscode"]);
//            //mDownloadAttachment.Page_Code = Convert.ToInt32(PageDetails["Page_Syscode"]);
//            //mDownloadAttachment.Section_Code = Convert.ToInt32(PageDetails["Section_Syscode"]);
//            //mDownloadAttachment.Field_Code = Convert.ToInt32(PageDetails["Field_Syscode"]);

//            var response = new HttpResponseMessage();
//            //Calling Mongo upload......
//            using (var content = new MultipartFormDataContent())
//            {
//                var jsonDownloadObj = JsonConvert.SerializeObject(ta);
//                StringContent scJsonDwnldObj = new StringContent(jsonDownloadObj, UnicodeEncoding.UTF8, "application/json");
//                content.Add(scJsonDwnldObj, "DownloadObj");

//                file.InputStream.Seek(0, SeekOrigin.Begin);
//                var streamContent = new StreamContent(file.InputStream);
//                //byte[] myfileByte = new BinaryReader(file.InputStream).ReadBytes(96694);
//                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
//                streamContent.Headers.ContentLength = file.ContentLength;
//                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
//                {
//                    Name = "fileToUpload",
//                    FileName = file.FileName
//                };
//                content.Add(streamContent);//new StreamContent(stream) //CreateFileContent(stream, file.FileName)

//                Dictionary<string, string> payload = new Dictionary<string, string>();
//                payload.Add("CreatedBy", ta.created_by.ToString());
//                payload.Add("SysCode_Temp", ta.attachment_identifier.ToString());

//                HttpContent PayloadItems = new FormUrlEncodedContent(payload);

//                content.Add(PayloadItems, "payloadItems");

//                response = client.PostAsync("api/FileService/UploadFiles", content).Result;
//                response.EnsureSuccessStatusCode();
//            }
//            if (response.IsSuccessStatusCode)
//            {
//                var clientResponse = response.Content.ReadAsStringAsync().Result;
//                var settings = new JsonSerializerSettings
//                {
//                    NullValueHandling = NullValueHandling.Ignore,
//                    MissingMemberHandling = MissingMemberHandling.Ignore
//                };

//                //mDownloadAttachment.SysCode_Temp = Convert.ToInt32(JsonConvert.DeserializeObject<int>(clientResponse, settings));
//                ad = JsonConvert.DeserializeObject<TaskAttachment>(clientResponse, settings);
//            }
//            else
//            {
//                mDownloadAttachment.SysCode_Temp = 0;
//                ModelState.AddModelError("AttachSaveTempError", ConfigurationManager.AppSettings["APITimeOutError"].ToString());

//                string mReturnValue = response.Content.ReadAsStringAsync().Result;
//                //string mErrMsg = Common.Get_ErrMsg_WebAPI_IsSuccessStatusCode_Failed(mReturnValue);

//                //Log.LogError(mErrMsg, Convert.ToString(this.Session["UserName"]), this.ControllerContext, null, null);

//                return Json(new { IsSuccess = "N", Errors = ModelState.Errors(), Msg = "" }, JsonRequestBehavior.AllowGet);
//            }
//            mDownloadAttachment.AttachContent = string.Empty;

//            if (Session["PEDetails_Syscode"] != null)
//            {
//                PEDetails_Syscode = Session["PEDetails_Syscode"].ToString();
//            }

//            return Json(new
//            {
//                IsSuccess = "Y",
//                PEDetails_syscode = ad.PEDetails_syscode,
//                SysCode_Temp = ad.Attachment_Syscode,
//                //FileContent = mDownloadAttachment.AttachContent,
//                OriginalFileName = ad.OriginalFileName,
//                MFileID = ad.File_MongoID,
//                FileName = mDownloadAttachment.FileName
//            }, JsonRequestBehavior.AllowGet);
//        }
//        else
//        {
//            return Json(new
//            {
//                IsSuccess = "SessionExpired",
//                Errors = ModelState.Errors(),
//                Msg = ""
//            }, JsonRequestBehavior.AllowGet);
//        }
//    }
//    catch (Exception ex)
//    {
//        ModelState.AddModelError("ExErr", ex.Message);
//        return Json(new
//        {
//            IsSuccess = "N",
//            Errors = ModelState.Errors(),
//            Msg = ""
//        }, JsonRequestBehavior.AllowGet);
//    }
//    finally
//    {
//        mDownloadAttachment = null;
//        client = null;
//    }
//}