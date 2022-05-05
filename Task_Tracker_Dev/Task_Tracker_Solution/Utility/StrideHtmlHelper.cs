using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;

namespace Task_Tracker_Solution.Utility
{
    public static class StrideHtmlHelper
    {
        public static IHtmlString EnumToString<T>(this HtmlHelper helper)
        {
            return new MvcHtmlString(typeof(T).ToJSONString());
        }
        public static IHtmlString FileLinkFor(this HtmlHelper linkHtmlHelper, List<TaskAttachment> lstAttachments)
        {
            if (linkHtmlHelper == null || lstAttachments == null || lstAttachments.Count <= 0)
            {
                return MvcHtmlString.Create(string.Empty);
            }

            StringBuilder htmlBuilder = new StringBuilder();

            var ul = new TagBuilder("ul");
            ul.AddCssClass("p-0 attachmentul");
            htmlBuilder.Append(ul.ToString(TagRenderMode.StartTag));

            foreach (var attachment in lstAttachments)
            {
                htmlBuilder.AppendFileLinkTags(attachment);
            }
            htmlBuilder.Append(ul.ToString(TagRenderMode.EndTag));

            var html = htmlBuilder.ToString();

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString FileLinkFor(this HtmlHelper linkHtmlHelper, List<TaskAttachment> lstAttachments, string trailComment)
        {
            if (string.IsNullOrEmpty(trailComment))
                return MvcHtmlString.Create(string.Empty);
            else if (linkHtmlHelper == null || lstAttachments == null || lstAttachments.Count <= 0 || !trailComment.Contains("File(s) :"))
            //    return MvcHtmlString.Create(string.Empty);
            //else if ()
                return MvcHtmlString.Create("<span style='white-space:pre-wrap;'>" + trailComment + "</span>");

            var fnSplit = trailComment.Split(':');

            if (fnSplit.Length < 2)
                return MvcHtmlString.Create(trailComment);

            var files = fnSplit[1].Split(',');

            if (files.Length < 1)
                return MvcHtmlString.Create(trailComment);

            StringBuilder htmlBuilder = new StringBuilder();

            var ul = new TagBuilder("ul");
            ul.AddCssClass("p-0 attachmentul");
            htmlBuilder.Append(ul.ToString(TagRenderMode.StartTag));

            foreach (var fName in files)
            {
                var attachment = lstAttachments.FirstOrDefault(x => x.attachment_filename == fName.Trim());
                if (attachment == null)
                    htmlBuilder.Append(fName);
                else
                    htmlBuilder.AppendFileLinkTags(attachment);
            }
            htmlBuilder.Append(ul.ToString(TagRenderMode.EndTag));

            var html = htmlBuilder.ToString();

            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// This helper method was made for replacing the upload file button user control.
        /// But currently that is kept as it is and not used anywhere else.
        /// </summary>
        /// <param name="uploadFileHelper"></param>
        /// <returns></returns>
        public static IHtmlString UploadFile(this HtmlHelper uploadFileHelper)
        {
            if (uploadFileHelper == null)
                return MvcHtmlString.Create(string.Empty);

            StringBuilder htmlBuilder = new StringBuilder();
            var lbl = new TagBuilder("label");
            var fileInpt = new TagBuilder("input");
            fileInpt.MergeAttribute("name", "UploadFile");
            fileInpt.MergeAttribute("id", "UploadFile");
            fileInpt.MergeAttribute("type", "file");
            fileInpt.MergeAttribute("onchange", "GetFileNames(this);");
            fileInpt.MergeAttribute("accept", ".pdf, .xls, .xlsx, .jpg, .png, .gif, .jpeg, .docx, .doc, .txt, .msg, .sql, .zip");
            fileInpt.MergeAttribute("class", "form-control-file");

            htmlBuilder.Append(lbl.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(fileInpt.ToString(TagRenderMode.Normal));

            string shtmlString = htmlBuilder.ToString();

            return MvcHtmlString.Create(shtmlString);
        }

        private static void AppendFileLinkTags(this StringBuilder htmlBuilder, TaskAttachment attachment)
        {
            if (attachment == null)
                return;

            var li = new TagBuilder("li");
            var anchor = new TagBuilder("a");
            string aHref = $"{cWebApiNames.AppDownloadAttachments}?AttachGuid={attachment.attachment_identifier}&MFileId={attachment.mongo_file_id}&FileName={attachment.attachment_filename}";
            anchor.MergeAttribute("href", aHref);

            var icon = new TagBuilder("i");
            icon.AddCssClass("fa fa-paperclip btn-icon-wrapper");

            //var textbox = InputExtensions.TextBoxFor(htmlHelper, m => m.ImageName, new { type = "file", style = "display:none" });

            htmlBuilder.Append(li.ToString(TagRenderMode.StartTag));
            htmlBuilder.Append(anchor.ToString(TagRenderMode.StartTag));
            htmlBuilder.Append(icon.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(attachment.attachment_filename);
            htmlBuilder.Append(anchor.ToString(TagRenderMode.EndTag));
            htmlBuilder.Append(li.ToString(TagRenderMode.EndTag));
        }

    }
}