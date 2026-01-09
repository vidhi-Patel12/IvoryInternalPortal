using IvoryInternalPortal.Model;
using Microsoft.AspNetCore.Identity.Data;

namespace IvoryInternalPortal.Interface
{
    public interface ILogin
    {
        Task<object?> LoginAsync(LoginRequest request);
    }
}
