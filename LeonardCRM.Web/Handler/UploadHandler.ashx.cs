using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Eli.Common;

namespace LeonardCRM.Web.Handler
{
    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private readonly JavaScriptSerializer js;

        private string StorageRoot
        {
            get { return HttpContext.Current.Server.MapPath("~/" + ConfigValues.UPLOAD_DIRECTORY); }
        }

        public UploadHandler()
        {
            js = new JavaScriptSerializer { MaxJsonLength = 41943040 };
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    else ListCurrentFiles(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            var filePath = StorageRoot + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var filePath = StorageRoot + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                var statuses = new List<FilesStatus>();
                var headers = context.Request.Headers;

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    UploadWholeFile(context, statuses);
                }
                else
                {
                    UploadPartialFile(headers["X-File-Name"], context, statuses);
                }
                WriteJsonIframeSafe(context, statuses);
            }


            //Process Sale Customers
            //if (context.Request["customerId"] != null)
            //{
            //    try
            //    {
            //        int customerId = int.Parse(context.Request["customerId"]);
            //        SalesCustomer customer = SalesCustomerBM.Instance.GetById(customerId);
            //        if (customer != null)
            //        {
            //            customer.Photo = context.Request.Headers["X-File-Name"];
            //            SalesCustomerBM.Instance.Update(customer);
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var folder = (!string.IsNullOrEmpty(context.Request["folder"]) ? (context.Request["folder"] + "/") : "");
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var fullPath = StorageRoot + folder;
            if (File.Exists(fullPath + fileNameWithoutExt + ext))
            {
                fileNameWithoutExt = fileNameWithoutExt + "_" + DateTime.Now.Ticks;
                fullPath = StorageRoot + folder + fileNameWithoutExt + ext;
            }

            using (var fs = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new FilesStatus(new FileInfo(fullPath)));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                var folder = (!string.IsNullOrEmpty(context.Request["folder"]) ? (context.Request["folder"] + "/") : "");
                if (!Directory.Exists(StorageRoot + folder)) Directory.CreateDirectory(StorageRoot + folder);

                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
                if (fileNameWithoutExt.Length > Constant.MaxFileNameLength)
                    fileNameWithoutExt = fileNameWithoutExt.Substring(0, Constant.MaxFileNameLength);
                var ext = Path.GetExtension(file.FileName);
                var fullPath = StorageRoot + folder + fileNameWithoutExt + ext;
                if (File.Exists(fullPath))
                {
                    fileNameWithoutExt = fileNameWithoutExt + "_" + DateTime.Now.Ticks;
                    fullPath = StorageRoot + folder + fileNameWithoutExt + ext;
                }
                file.SaveAs(fullPath);

                var fullName = fileNameWithoutExt + ext;
                statuses.Add(new FilesStatus(folder + fullName, file.ContentLength, fullPath));
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];
            var filePath = StorageRoot + filename;

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            var files =
                new DirectoryInfo(StorageRoot)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new FilesStatus(f))
                    .ToArray();

            string jsonObj = js.Serialize(files);
            context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }

    }
}