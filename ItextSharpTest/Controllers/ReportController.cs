using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItextSharpTest.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public FileResult DownloadWithCssFileReport()
        {
            return File(GetPDFWithCssFile(), "application/pdf", "InvoiceWithCssFile.pdf");
        }

        public FileResult DownloadWithCssInlineReport()
        {
            return File(GetPDFCssInline(), "application/pdf", "InvoiceWithCssInline.pdf");
        }

        private byte[] GetPDFWithCssFile()
        {
            //Create a byte array that will eventually hold our final PDF
            Byte[] bytes;

            //Boilerplate iTextSharp setup here
            //Create a stream that we can write to, in this case a MemoryStream
            using (var ms = new MemoryStream())
            {

                //Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                using (var doc = new Document())
                {

                    //Create a writer that's bound to our PDF abstraction and our stream
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        //Open the document for writing
                        doc.Open();

                        //Our sample HTML and CSS
                        string htmlTemplate = System.IO.File.ReadAllText(Server.MapPath(ConfigurationManager.AppSettings["TemplatePath"]));
                        string cssTemplate = System.IO.File.ReadAllText(Server.MapPath(ConfigurationManager.AppSettings["TemplateCssPath"]));
                        string logoPath = Server.MapPath(ConfigurationManager.AppSettings["LogoPath"]);

                        htmlTemplate = htmlTemplate.Replace("@TemplatePath", logoPath);


                        using (var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssTemplate)))
                        {
                            using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlTemplate)))
                            {
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss);
                            }
                        }


                        doc.Close();
                    }
                }

                //After all of the PDF "stuff" above is done and closed but **before** we
                //close the MemoryStream, grab all of the active bytes from the stream
                bytes = ms.ToArray();
            }

            return bytes;
        }

        private byte[] GetPDFCssInline()
        {
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        //Open the document for writing
                        doc.Open();

                        //Our sample HTML and CSS
                        string htmlTemplate = System.IO.File.ReadAllText(Server.MapPath(ConfigurationManager.AppSettings["TemplateCssInlinePath"]));
                        string logoPath = Server.MapPath(ConfigurationManager.AppSettings["LogoPath"]);

                        htmlTemplate = htmlTemplate.Replace("@TemplatePath", logoPath);

                        using (var srHtml = new StringReader(htmlTemplate))
                        {

                            //Parse the HTML
                            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                        }

                        doc.Close();
                    }
                }
                bytes = ms.ToArray();
            }

            return bytes;
        }
    }
}