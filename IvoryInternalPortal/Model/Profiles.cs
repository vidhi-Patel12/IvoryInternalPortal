namespace IvoryInternalPortal.Model
{
    public class Profiles
    {
        public int? ProfileId { get; set; }
        public string Name { get; set; }
        public string ProfessionalSummary { get; set; }
        public string CoreSkills { get; set; }
        public string ProfessionalExperience { get; set; }
        public string? ProjectNames { get; set; }
        public string? ProjectDomains { get; set; }
        public string? ProjectTechnologies { get; set; }
        public string? ProjectDescriptions { get; set; }
        public string? ProjectResponsibilities { get; set; }
        public string KeySkills { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsActive { get; set; }
    }
}
