namespace IvoryInternalPortal.Model
{
    public class Register
    {
        public int? RegisterId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public long Mobile { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; }   // 1 Admin, 2 CEO, 3 Vendor, 4 Employee
        public bool IsActive { get; set; }
    }
}
