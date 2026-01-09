using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using System.Data;
using System.Data.SqlClient;

namespace IvoryInternalPortal.Repository
{
    public class RegisterRepository : IRegister
    {
        private readonly IConfiguration _configuration;

        public RegisterRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<Register>> GetAllAsync()
        {
            var list = new List<Register>();

            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Register_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapRegister(reader));
            }

            return list;
        }

        public async Task<Register> InsertUpdateAsync(Register r)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Register_Upsert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@register_id", (object?)r.RegisterId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@first_name", r.FirstName);
            cmd.Parameters.AddWithValue("@last_name", r.LastName);
            cmd.Parameters.AddWithValue("@mobile", r.Mobile);
            cmd.Parameters.AddWithValue("@email", r.Email);

            // Hash password
            cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(r.Password));

            cmd.Parameters.AddWithValue("@role", r.Role);
            cmd.Parameters.AddWithValue("@is_active", r.IsActive);

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Register insert/update failed.");

            return MapRegister(reader);
        }

        public async Task<Register?> GetByIdAsync(int registerId)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Register_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@register_id", registerId);

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            return await reader.ReadAsync() ? MapRegister(reader) : null;
        }

        public async Task<bool> DeleteAsync(int registerId)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Register_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@register_id", registerId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static Register MapRegister(IDataRecord r)
        {
            return new Register
            {
                RegisterId = Convert.ToInt32(r["RegisterId"]),
                FirstName = r["FirstName"].ToString()!,
                LastName = r["LastName"].ToString()!,
                Mobile = Convert.ToInt64(r["Mobile"]),
                Email = r["Email"].ToString()!,
                Password = r["Password"].ToString()!,
                Role = Convert.ToInt32(r["Role"]),
                IsActive = Convert.ToBoolean(r["IsActive"])
            };
        }
    }
}