using IvoryInternalPortal.Model;

namespace IvoryInternalPortal.Interface
{
   
    public interface IVendor
    {
        //void CreateVendor(Vendors vendor);

        Task<List<Vendors>> GetAllAsync();

        Task<Vendors> InsertUpdateAsync(
       Vendors vendor,
       IFormFile? agreementFile,
       IFormFile? signatureFile);

        Task<Vendors?> GetByIdAsync(long vendorId);

        Task<bool> DeleteAsync(long vendorId);

    }
}
