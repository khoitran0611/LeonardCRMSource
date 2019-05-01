
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Text;
//using Codaxy.WkHtmlToPdf;
//using WkHtmlToXDotNet;
using TuesPechkin;

namespace Eli.Common
{
    public class HtmlPdfConverter
    {
        private static readonly IConverter Converter =
            new ThreadSafeConverter(
                new RemotingToolset<PdfToolset>(
                    new WinAnyCPUEmbeddedDeployment(
                        new TempFolderDeployment())));

        //private static readonly IConverter ImageConverter =
        //    new ThreadSafeConverter(
        //        new RemotingToolset<ImageToolset>(
        //            new WinAnyCPUEmbeddedDeployment(
        //                new TempFolderDeployment())));

        public static string RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process {StartInfo = {FileName = filename}};

            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + filename + " - " + arguments + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(filename + " - " + arguments + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        ///// <summary>
        ///// Convert html from an webpage to pdf
        ///// <para />This method requires 'wkhtmltopdf' 32bit to be installed on server.
        ///// </summary>
        ///// <param name="filePath">File path to save PDF</param>
        ///// <param name="url">Url to download html and convert to pdf</param>
        //public static void ConvertWebpageToPdf(string filePath, string url)
        //{
        //    PdfConvert.ConvertHtmlToPdf(new PdfDocument
        //    {
        //        Url = url,
        //        //HeaderLeft = "[title]",
        //        //HeaderRight = "[date] [time]",
        //        //FooterCenter = "Page [page] of [topage]"

        //    }, new PdfOutput
        //    {
        //        OutputFilePath = filePath
        //    });
        //}

        ///// <summary>
        ///// Convert html string to pdf
        ///// <para />This method requires 'wkhtmltox.dll' to be put in bin folder
        ///// </summary>
        ///// <param name="htmlContent">Html string to convert to PDF</param>
        ///// <param name="filePath">File path to save PDF</param>
        //public static void ConvertHtmlToPdf(string htmlContent, string filePath)
        //{
        //    var pdfData = HtmlToXConverter.ConvertToPdf(htmlContent);
        //    File.WriteAllBytes(filePath, pdfData);
        //}

        ///// <summary>
        ///// Convert html string to PNG
        ///// <para />This method requires 'wkhtmltox.dll' to be put in bin folder
        ///// </summary>
        ///// <param name="htmlContent">Html string to convert to PDF</param>
        ///// <param name="filePath">File path to save PDF. This is optional</param>
        //public static byte[] ConvertHtmlToImage(string htmlContent, string filePath = null)
        //{
        //    var pdfData = HtmlToXConverter.ConvertToPng(htmlContent);
        //    if(!string.IsNullOrEmpty(filePath))
        //        File.WriteAllBytes(filePath, pdfData);
        //    return pdfData;
        //}

        ///// <summary>
        ///// Convert html string to PNG
        ///// <para />This method requires 'wkhtmltox.dll' to be put in bin folder
        ///// </summary>
        ///// <param name="htmlContent">Html string to convert to PDF</param>
        ///// <param name="filePath">File path to save PDF. This is optional</param>
        ///// <param name="width">Image width</param>
        ///// <param name="height">Image height</param>
        //public static byte[] ConvertHtmlToImage(string htmlContent, int width, int height, string filePath = null)
        //{
        //    var pdfData = HtmlToXConverter.ConvertToPng(htmlContent, width, height);
        //    if (!string.IsNullOrEmpty(filePath))
        //        File.WriteAllBytes(filePath, pdfData);
        //    return pdfData;
        //}

        /// <summary>
        /// Convert html from an webpage to pdf
        /// </summary>
        /// <param name="filePath">File path to save PDF</param>
        /// <param name="url">Url to download html and convert to pdf</param>
        public static void ConvertWebpageToPdf(string filePath, string url)
        {           
            var doc = new HtmlToPdfDocument();
            doc.GlobalSettings.Margins.Left = 0;
            doc.GlobalSettings.Margins.Right = 0;
            var os = new ObjectSettings { PageUrl = url };
            os.WebSettings.PrintBackground = true;
            doc.Objects.Add(os);
            var result = Converter.Convert(doc);
            File.WriteAllBytes(filePath, result);
        }

        /// <summary>
        /// Convert html string to pdf
        /// </summary>
        /// <param name="htmlContent">Html string to convert to PDF</param>
        /// <param name="filePath">File path to save PDF</param>
        public static void ConvertHtmlToPdf(string htmlContent, string filePath)
        {
            var doc = new HtmlToPdfDocument();

            //set default font 
            var startPos = htmlContent.IndexOf("<head");
            var len = htmlContent.IndexOf("</head>") + 6 - startPos + 1;
            var header = htmlContent.Substring(startPos, len);
            htmlContent = htmlContent.Replace(header, string.Format("<head>{0}</head>", Constant.DefaultHtmlFont));
                       
            //wrap content
            var bodyStartPos = htmlContent.IndexOf("<body");
            var contentStartPos = bodyStartPos + htmlContent.Substring(bodyStartPos, htmlContent.IndexOf(">", bodyStartPos) + 1 - bodyStartPos).Length;
            var bodyContent = htmlContent.Substring(contentStartPos, htmlContent.IndexOf("</body>") - contentStartPos);
            htmlContent = htmlContent.Replace(bodyContent, string.Format(Constant.BodyStandardWrapper, bodyContent));

            doc.Objects.Add( new ObjectSettings { HtmlText = htmlContent });

            var result = Converter.Convert(doc);
            File.WriteAllBytes(filePath, result);
        }
    }
}
