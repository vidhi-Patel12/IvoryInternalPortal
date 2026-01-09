using IvoryInternalPortal.Interface;
using Microsoft.AspNetCore.Identity.Data;
using System.Data;
using System.Data.SqlClient;

namespace IvoryInternalPortal.Repository
{
    public class LoginRepository : ILogin
    {
        private readonly IConfiguration _configuration;

        public LoginRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<object?> LoginAsync(LoginRequest request)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Register_Login", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@email", request.Email);

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync())
                return null;

            string hashedPassword = reader["Password"].ToString()!;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword))
                return null;

            int roleId = Convert.ToInt32(reader["Role"]);

            return new
            {
                RegisterId = Convert.ToInt32(reader["RegisterId"]),
                Email = reader["Email"].ToString(),
                FullName = $"{reader["FirstName"]} {reader["LastName"]}",
                RoleId = roleId,
                RoleName = GetRoleName(roleId)
            };
        }

        private static string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "CEO",
                3 => "Vendor",
                4 => "Employee",
                _ => "Unknown"
            };
        }
    }
}