using IvoryInternalPortal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using iText.Html2pdf;
using System.IO;

namespace IvoryInternalPortal.Controllers
{
    public class MsaController : Controller
    {
        public IActionResult Msa()
        {
            return View();
        }
        //public IActionResult Download()
        //{
        //    var pdfBytes = MsaPdfGenerator.Generate();

        //    return File(
        //        pdfBytes,
        //        "application/pdf",
        //        "MasterServicesAgreement.pdf"
        //    );
        //}

        [HttpPost]
        public IActionResult GeneratePdf(
     string AgreementDate,
     string ProviderDetails,
     string ConfidentialInfo,
     string ServiceProviderText,
     string ivorySignatureBase64,
     string SignatureBase64)
        {
            ServiceProviderText = string.IsNullOrWhiteSpace(ServiceProviderText)
           ? "Ivory Technolab | Service Provider"
           : ServiceProviderText;

            var pdf = MsaPdfGenerator.Generate(
                AgreementDate,
                ProviderDetails,
                ConfidentialInfo,
                ServiceProviderText,
                ivorySignatureBase64,
                SignatureBase64
            );

            return File(pdf, "application/pdf", "MSA.pdf");
        }


        //        [HttpPost]
        //        public IActionResult GeneratePdf(
        //              string AgreementDate,
        //              string ProviderDetails,
        //              string ConfidentialInfo)
        //        {
        //            string html = $@"
        //<!DOCTYPE html>
        //<html>
        //<head>
        //<link href='css/msa.css' rel='stylesheet' />
        //</head>
        //<body>

        //<div class='top-bar'>
        //    <div class='logo'>
        //        <img src='images/ivory-logo.png' />
        //    </div>
        //    <div class='website'>www.ivorytechnolab.com</div>
        //</div>

        //<h1>MASTER SERVICES AGREEMENT</h1>

        //<div class='subtitle'>
        //(Ivory Technolab | Tabdelta Solutions as Service Provider)
        //</div>

        //<p>This Agreement is entered into on <b>{AgreementDate}</b></p>

        //<p class='section-title'>BETWEEN</p>

        //<p><b>Ivory Technolab Private Limited</b>, Ahmedabad, Gujarat, India</p>

        //<p class='section-title'>AND</p>

        //<p>{ProviderDetails.Replace("\n", "<br/>")}</p>

        //<p class='section-title'>1. DEFINITIONS</p>
        //<p>{ConfidentialInfo}</p>

        //<div class='footer'>
        //Registered Office Address : Isquare Corporate Park, Ahmedabad<br/>
        //www.ivorytechnolab.com
        //</div>

        //</body>
        //</html>";

        //            var baseUri = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + Path.DirectorySeparatorChar;

        //            var props = new ConverterProperties();
        //            props.SetBaseUri(baseUri);

        //            using var ms = new MemoryStream();
        //            HtmlConverter.ConvertToPdf(html, ms, props);

        //            return File(ms.ToArray(), "application/pdf", "Ivory-MSA.pdf");
        //        }
    }
}