using IvoryInternalPortal.Model;

namespace IvoryInternalPortal.Interface
{
    public interface IProfile
    {
        Task<List<Profiles>> GetAllAsync();
        Task<Profiles?> GetByIdAsync(int profileId);
        Task<Profiles> InsertUpdateAsync(Profiles model);
        Task<bool> DeleteAsync(int profileId);
    }
}
