using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Task_Tracker_Solution.Utility
{
    public static class StrideControllerHelpers
    {
        public static MultipartFormDataContent returnMultipartFormData(this HttpRequestBase req, object viewmodel)
        {
            var formCont = new MultipartFormDataContent();

            var vm = JsonConvert.SerializeObject(viewmodel);
            StringContent vmContent = new StringContent(vm, UnicodeEncoding.UTF8, "application/json");
            formCont.Add(vmContent, "ViewModel");

            int fileCount = req.Files.Count;

            for (int i = 0; i < fileCount; i++)
            {
                var file = req.Files[i];
                if (string.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                    continue;
                var streamContent = new StreamContent(file.InputStream);
                //byte[] myfileByte = new BinaryReader(file.InputStream).ReadBytes(96694);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                streamContent.Headers.ContentLength = file.ContentLength;
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "fileToUpload - " + i,
                    FileName = file.FileName
                };
                formCont.Add(streamContent);
            }
            return formCont;
        }
    }
}