using IvoryInternalPortal.Model;

namespace IvoryInternalPortal.Interface
{
    public interface IRegister
    {
        Task<List<Register>> GetAllAsync();
        Task<Register> InsertUpdateAsync(Register register);
        Task<Register?> GetByIdAsync(int registerId);
        Task<bool> DeleteAsync(int registerId);
    }
}
