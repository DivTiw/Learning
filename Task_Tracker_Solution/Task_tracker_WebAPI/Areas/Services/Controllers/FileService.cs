using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;

namespace Task_tracker_WebAPI.Areas.Services.Controllers
{
    public class FileService
    {
        HttpClient fsWebClient;
        TokenServer ts;
        private string mongoBaseUri = ConfigurationManager.AppSettings["MongoFileServerBaseURL"].ToString();

        public FileService(HttpClient _client)
        {
            fsWebClient = _client;
            fsWebClient.BaseAddress = new Uri(mongoBaseUri);
            ts = new TokenServer();
        }
        public OperationDetailsDTO UploadFiles(HttpPostedFile file, string attachType, int attachSyscode, int userId)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            if (string.IsNullOrEmpty(file.FileName))
            {
                od.opStatus = false;
                od.opMsg = "Upload File name is blank.";
                return od;
            }
            if (file.ContentLength == 0)
            {
                od.opStatus = false;
                od.opMsg = "Upload file content length is 0.";
                return od;
            }

            try
            {
                using (var uow = new UnitOfWork())
                {
                    string fileName = file.FileName;
                    string fileType = Path.GetExtension(fileName);
                    string displayName = fileName.Replace("." + fileType, "");
                    TaskAttachment ta = new TaskAttachment
                    {
                        attachment_filename = fileName,
                        attachment_display_name = displayName,
                        attachment_identifier = Guid.NewGuid(),
                        created_by = userId,
                        type_detail = attachType,
                        type_syscode = attachSyscode,
                        created_on = DateTime.Now,
                        is_deleted = false
                    };
                    uow.AttachmentRepo.Add(ta);
                    uow.commitTT();

                    if (ta.attachment_syscode > 0)
                    {
                        var response = new HttpResponseMessage();

                        string strAttachmentGuid = ta.attachment_identifier.ToString();
                        TokenModel tm = new TokenModel();                        
                        tm.UserID = ta.created_by;

                        tm.Payload = new Dictionary<string, object>();
                        tm.Payload.Add("record_syscode", strAttachmentGuid);

                        string token = ts.GetToken(tm);

                        #region Testing the token
                        if (ts.ValidateToken(token))
                        {
                            string decToken = ts.DecodeToken(token);
                            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(ts.Decode(token));
                            Log.LogDebug($"{Environment.NewLine}Token is: {token} {Environment.NewLine} Decoded token:  {decToken} {Environment.NewLine} JSON value:  {values}", "", null, "UploadFiles", "FileService");
                        }
                        else
                        {
                            Log.LogDebug($"{Environment.NewLine}Generated token is: {token} {Environment.NewLine}", "", null, "UploadFiles", "FileService");
                            throw new Exception("Token generation failed or the generated token is invalid.");
                        }
                        #endregion
                        using (var content = new MultipartFormDataContent())
                        {
                            HttpPostedFile uploadFile = file;//HttpContext.Current.Request.Files[0];
                            var streamContent = new StreamContent(uploadFile.InputStream);
                            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(uploadFile.ContentType);
                            streamContent.Headers.ContentLength = uploadFile.ContentLength;
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = "fileToUpload",
                                FileName = uploadFile.FileName
                            };
                            content.Add(streamContent);

                            fsWebClient.DefaultRequestHeaders.Add("Token", token);
                            response = fsWebClient.PostAsync("api/Files/Upload", content).Result;

                            //response.EnsureSuccessStatusCode();
                            if (response.IsSuccessStatusCode)
                            {
                                string responseString = response.Content.ReadAsStringAsync().Result;
                                if (!responseString.Contains(uploadFile.FileName))
                                {
                                    //Delete file metadata and throw exception!                                  
                                    ta.is_deleted = true;
                                    uow.AttachmentRepo.Update(ta);
                                    uow.commitTT();
                                    throw new HttpException(responseString);
                                }
                                var settings = new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore
                                };
                                var res = JsonConvert.DeserializeObject(responseString, settings);
                                ///ToDo: Implement below code for better handling the response from the mongo file server.
                                //Dictionary<string, string> retValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.ToString());
                                //string strDictReturn = string.Empty;
                                //if (retValue.TryGetValue(uploadFile.FileName, out strDictReturn))
                                //{
                                //    string mongoFileId = strDictReturn;//((Newtonsoft.Json.Linq.JContainer)res).First.Value<string>(uploadFile.FileName);
                                //}
                                //else
                                //{
                                //    retValue.TryGetValue("message", out strDictReturn);
                                //    if (string.IsNullOrEmpty(strDictReturn))
                                //    {
                                //        strDictReturn = "Error occurred in Mongo file server, No messages returned with the cause. Check with Mongo server owner.";
                                //    }
                                //    throw new Exception("Exception: " + strDictReturn);
                                //}
                                string mongoFileId = ((Newtonsoft.Json.Linq.JContainer)res).First.Value<string>(uploadFile.FileName);

                                ta = uow.AttachmentRepo.Get(ta.attachment_syscode);
                                ta.mongo_file_id = mongoFileId;
                                uow.AttachmentRepo.Update(ta);
                                uow.commitTT();

                                od.opStatus = true;
                                od.opMsg = "File successfully uploaded!";
                            }
                            else
                            {
                                //Delete file metadata and throw exception!                                  
                                ta.is_deleted = true;
                                uow.AttachmentRepo.Update(ta);
                                uow.commitTT();
                                throw new HttpException(Convert.ToInt32(response.StatusCode), "Request to mongo API failed");
                            }
                        }
                    }
                    else
                    {
                        throw new HttpException("File Metadata upload failed!");
                    }
                }
            }
            catch (Exception ex)
            {
                string _ExceptionMSG = ex.Message;
                Log.LogError(_ExceptionMSG, "", null, "UploadFiles", "FileServiceController");
                od.opStatus = false;
                od.opMsg = "Some error occurred while uploading file, see opInnerexception for more details!";
                od.opInnerException = ex;
            }
            finally
            {
            }
            return od;
        }
    }
}