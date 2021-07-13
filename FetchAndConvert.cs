using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using IronPdf;

namespace html2pdf_win_csharp
{
    public static class FetchAndConvert
    {
        [Function("FetchAndConvert")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("FetchAndConvert");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse();

            var queryString = req.Url.Query;
            var queries = System.Web.HttpUtility.ParseQueryString(queryString);
            var url = queries.Get("url");
            if (url == null) {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("query string \"url\" is required");
                return response;
            }

            var Renderer = new HtmlToPdf();
            Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen;
            Renderer.PrintOptions.FitToPaperWidth = true;
            Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
            Renderer.PrintOptions.MarginBottom = 5;
            Renderer.PrintOptions.MarginLeft = 5;
            Renderer.PrintOptions.MarginRight = 5;
            Renderer.PrintOptions.MarginTop = 5;
            Renderer.PrintOptions.RenderDelay = 2500;
            Renderer.PrintOptions.PrintHtmlBackgrounds = true;
            Renderer.PrintOptions.DPI = 96;
            Renderer.PrintOptions.EnableJavaScript = true;

            var PDF = Renderer.RenderUrlAsPdf(url);
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/pdf");
            response.WriteBytes(PDF.BinaryData);

            return response;
        }
    }
}
