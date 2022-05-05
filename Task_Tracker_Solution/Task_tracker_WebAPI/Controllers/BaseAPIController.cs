using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_tracker_WebAPI.Areas.Services.Controllers;
using Task_tracker_WebAPI.Filters;

namespace Task_tracker_WebAPI.Controllers
{
    [StrideAPIAuthValidator]
    public class BaseAPIController : ApiController
    {
        protected HttpClient webClient = null;

        public BaseAPIController()
        {
            webClient = new HttpClient();
        }

        protected List<string> FileUpload(int uploadSyscode, int uploadedBy, bool isTrailCall = false)
        {
            List<string> UploadedFileNames = new List<string>();

            if (HttpContext.Current.Request.Files.Count <= 0)
            {
                return UploadedFileNames;
            }
            //webClient.BaseAddress = new Uri(mongoBaseUri);

            ///ToDo: Find Better approach for below file upload and file trail creation code.
            //HttpPostedFile file = fileCols[0];
            HttpFileCollection fileCols = HttpContext.Current.Request.Files;
            OperationDetailsDTO od = new OperationDetailsDTO();
            for (int indx = 0; indx < fileCols.Count; indx++)
            {
                //ToDo: Try to find better approach, this reinitialisation of client can be avoided.
                webClient = null;
                webClient = new HttpClient();
                FileService fs = new FileService(webClient);

                HttpPostedFile file = fileCols[indx];
                if (!(string.IsNullOrEmpty(file.FileName) || file.ContentLength == 0))
                {
                    od = fs.UploadFiles(file, "Task", uploadSyscode, uploadedBy);
                }

                if (od.opStatus)
                    UploadedFileNames.Add(file.FileName);
            }
            webClient = null;
            return UploadedFileNames;
        }
    }
}
