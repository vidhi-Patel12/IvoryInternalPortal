using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using System.Data;
using System.Data.SqlClient;

namespace IvoryInternalPortal.Repository
{
    public class ProfileRepository : IProfile
    {
        private readonly IConfiguration _configuration;

        public ProfileRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        // GET ALL
        public async Task<List<Profiles>> GetAllAsync()
        {
            var list = new List<Profiles>();

            try
            {
                await using var conn = CreateConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("dbo.sp_Profile_GetAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(MapProfile(reader));
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while fetching profiles.", ex);
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<Profiles?> GetByIdAsync(int profileId)
        {
            try
            {
                await using var conn = CreateConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("dbo.sp_Profile_GetById", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ProfileId", profileId);

                await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

                if (await reader.ReadAsync())
                    return MapProfile(reader);

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error while fetching profile with ID {profileId}.", ex);
            }
        }

        // =========================
        // INSERT / UPDATE
        // =========================
        public async Task<Profiles> InsertUpdateAsync(Profiles model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                await using var conn = CreateConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("dbo.sp_Profile_Upsert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ProfileId",
                    (object?)model.ProfileId ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@ProfessionalSummary", (object?)model.ProfessionalSummary ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CoreSkills", (object?)model.CoreSkills ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProfessionalExperience", (object?)model.ProfessionalExperience ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectNames", (object?)model.ProjectNames ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectDomains", (object?)model.ProjectDomains ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectTechnologies", (object?)model.ProjectTechnologies ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectDescriptions", (object?)model.ProjectDescriptions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectResponsibilities", (object?)model.ProjectResponsibilities ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@KeySkills", (object?)model.KeySkills ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive ?? true);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return MapProfile(reader);

                throw new InvalidOperationException("Profile insert/update did not return any data.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while saving profile details.", ex);
            }
        }

        // DELETE (SOFT DELETE)
        public async Task<bool> DeleteAsync(int profileId)
        {
            try
            {
                await using var conn = CreateConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("dbo.sp_Profile_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ProfileId", profileId);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error while deleting profile with ID {profileId}.", ex);
            }
        }

        // MAP DATA
        private static Profiles MapProfile(IDataRecord reader)
        {
            return new Profiles
            {
                ProfileId = reader["ProfileId"] != DBNull.Value
                    ? Convert.ToInt32(reader["ProfileId"])
                    : null,

                Name = reader["Name"]?.ToString(),
                ProfessionalSummary = reader["ProfessionalSummary"]?.ToString(),
                CoreSkills = reader["CoreSkills"]?.ToString(),
                ProfessionalExperience = reader["ProfessionalExperience"]?.ToString(),
                ProjectNames = reader["ProjectNames"]?.ToString(),
                ProjectDomains = reader["ProjectDomains"]?.ToString(),
                ProjectTechnologies = reader["ProjectTechnologies"]?.ToString(),
                ProjectDescriptions = reader["ProjectDescriptions"]?.ToString(),
                ProjectResponsibilities = reader["ProjectResponsibilities"]?.ToString(),

                KeySkills = reader["KeySkills"]?.ToString(),

                CreatedOn = reader["CreatedOn"] != DBNull.Value
                    ? Convert.ToDateTime(reader["CreatedOn"])
                    : null,

                UpdatedOn = reader["UpdatedOn"] != DBNull.Value
                    ? Convert.ToDateTime(reader["UpdatedOn"])
                    : null,

                IsActive = reader["IsActive"] != DBNull.Value
                    ? Convert.ToBoolean(reader["IsActive"])
                    : null
            };
        }
    }
}
