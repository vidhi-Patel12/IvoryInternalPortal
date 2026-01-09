namespace IvoryInternalPortal.Model
{
    public class Employees
    {
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
        public string? EmployeePhone { get; set; }
        public string? EmployeeAddress { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? EmploymentType { get; set; }
        public string? Skills { get; set; }

        public string? UploadProfile { get; set; }

        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AadhaarNumber { get; set; }
        public string? PANNumber { get; set; }

        public string? AadhaarDocumentPath { get; set; }
        public string? PanDocumentPath { get; set; }

        public decimal? Salary { get; set; }
        public string? PayrollCycle { get; set; }
        public string? PaymentStatus { get; set; }

        public string? AgreementPdfPath { get; set; }
        public string? DigitalSignaturePath { get; set; }

        public DateTime? AgreementSignedAt { get; set; }
        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? SignatureData { get; set; }

    }
}
