using IvoryInternalPortal.Model;

namespace IvoryInternalPortal.Interface
{
    public interface IUser
    {
        int CreateUser(Users user);
        Users GetByEmail(string email);

        int GetUserIdByEmail(string email);

    }
}
