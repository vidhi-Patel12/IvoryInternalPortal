using IvoryInternalPortal.Model;

namespace IvoryInternalPortal.Interface
{
    public interface IEmployee
    {
        //void CreateEmployee(Employees employee);

        Task<List<Employees>> GetAllAsync();

        Task<Employees> InsertUpdateAsync(Employees m,IFormFile? profileFile, IFormFile? aadhaarFile,IFormFile? panFile,
        IFormFile? agreementFile, IFormFile? signatureFile );
        Task<Employees?> GetByIdAsync(long employeeId);
        Task<bool> DeleteAsync(long employeeId);

    }
}
