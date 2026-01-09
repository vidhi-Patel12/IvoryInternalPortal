using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using System.Data;
using System.Data.SqlClient;

namespace IvoryInternalPortal.Repository
{
    public class UserRepository : IUser
    {
        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _config = config;
        }

        public int CreateUser(Users user)
        {
            using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_CreateUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@IsActive", true);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

            con.Open();
            int result = Convert.ToInt32(cmd.ExecuteScalar());

            if (result == -1)
                throw new Exception("Email already exists");

            return result;
        }

        public Users GetByEmail(string email)
        {
            // Optional: Implement when needed
            return null;
        }

        public int GetUserIdByEmail(string email)
        {
            using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("sp_GetUserByEmail", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            con.Open();
            object result = cmd.ExecuteScalar();

            return result != null ? Convert.ToInt32(result) : 0;
        }

    }

}