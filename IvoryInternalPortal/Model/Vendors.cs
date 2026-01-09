using System.Drawing;

namespace IvoryInternalPortal.Model
{
    public class Vendors
    {
            public long? VendorId { get; set; }

            public string? CompanyName { get; set; }
            public string? CompanyAddress { get; set; }
            public string? CompanyEmail { get; set; }
            public string? CompanyPhone { get; set; }

            public string? PocPhone { get; set; }

            public string? GstNumber { get; set; }
            public string? RocNumber { get; set; }
            public string? UanNumber { get; set; }

            public string? BankName { get; set; }
            public string? AccountNumber { get; set; }
            public string? IFSCCode { get; set; }
            public string? AccountHolderName { get; set; }

            public string? AgreementPdfPath { get; set; }
            public string? DigitalSignaturePath { get; set; }

            public string? Status { get; set; }

            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public string? SignatureData { get; set; }
    

    }
}
