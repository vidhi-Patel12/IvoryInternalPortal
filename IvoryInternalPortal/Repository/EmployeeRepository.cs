using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using Microsoft.AspNetCore.Connections;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Umbraco.Core.Composing;

namespace IvoryInternalPortal.Repository
{
    public class EmployeeRepository : IEmployee
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;


        public EmployeeRepository(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );
        }


        public async Task<List<Employees>> GetAllAsync()
        {
            var list = new List<Employees>();

            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Employees_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapEmployee(reader));
            }

            return list;
        }

        public async Task<Employees> InsertUpdateAsync(Employees m,IFormFile? profileFile,IFormFile? aadhaarFile,
        IFormFile? panFile, IFormFile? agreementFile, IFormFile? signatureFile)
        {
            try
            {
                if (m is null) throw new ArgumentNullException(nameof(m));

                // STEP 1: FILE UPLOAD (INLINE – NO HELPER METHOD)

                string uploadPath = Path.Combine(_env.ContentRootPath, "wwwroot", "upload");
                Directory.CreateDirectory(uploadPath);

                if (profileFile != null && profileFile.Length > 0)
                {
                    var fileName = Path.GetFileName(profileFile.FileName);
                    var path = Path.Combine(uploadPath, fileName);

                    await using var fs = new FileStream(path, FileMode.Create);
                    await profileFile.CopyToAsync(fs);

                    m.UploadProfile = $"/upload/{fileName}";
                }

                //if (profileFile != null)
                //{
                //    var fileName = $"{Path.GetFileName(profileFile.FileName)}";
                //    var fullPath = Path.Combine(uploadPath, fileName);

                //    await using var stream = new FileStream(fullPath, FileMode.Create);
                //    await profileFile.CopyToAsync(stream);

                //    m.UploadProfile = $"/upload/{fileName}";
                //}

                if (aadhaarFile != null && aadhaarFile.Length > 0)
                {
                    var fileName = Path.GetFileName(aadhaarFile.FileName);
                    var path = Path.Combine(uploadPath, fileName);

                    await using var fs = new FileStream(path, FileMode.Create);
                    await aadhaarFile.CopyToAsync(fs);

                    m.AadhaarDocumentPath = $"/upload/{fileName}";
                }

                if (panFile != null && panFile.Length > 0)
                {
                    var fileName = Path.GetFileName(panFile.FileName);
                    var path = Path.Combine(uploadPath, fileName);

                    await using var fs = new FileStream(path, FileMode.Create);
                    await panFile.CopyToAsync(fs);

                    m.PanDocumentPath = $"/upload/{fileName}";
                }


                //if (aadhaarFile != null)
                //{
                //    var fileName = $"{Path.GetFileName(aadhaarFile.FileName)}";
                //    var fullPath = Path.Combine(uploadPath, fileName);

                //    await using var stream = new FileStream(fullPath, FileMode.Create);
                //    await aadhaarFile.CopyToAsync(stream);

                //    m.AadhaarDocumentPath = $"/upload/{fileName}";
                //}

                //if (panFile != null)
                //{
                //    var fileName = $"{Path.GetFileName(panFile.FileName)}";
                //    var fullPath = Path.Combine(uploadPath, fileName);

                //    await using var stream = new FileStream(fullPath, FileMode.Create);
                //    await panFile.CopyToAsync(stream);

                //    m.PanDocumentPath = $"/upload/{fileName}";
                //}

                //if (agreementFile != null)
                //{
                //    var fileName = $"{Path.GetFileName(agreementFile.FileName)}";
                //    var fullPath = Path.Combine(uploadPath, fileName);

                //    await using var stream = new FileStream(fullPath, FileMode.Create);
                //    await agreementFile.CopyToAsync(stream);

                //    m.AgreementPdfPath = $"/upload/{fileName}";
                //}

                //if (signatureFile != null)
                //{
                //    var fileName = $"{Path.GetFileName(signatureFile.FileName)}";
                //    var fullPath = Path.Combine(uploadPath, fileName);

                //    await using var stream = new FileStream(fullPath, FileMode.Create);
                //    await signatureFile.CopyToAsync(stream);

                //    m.DigitalSignaturePath = $"/upload/{fileName}";
                //}

                if (agreementFile != null && agreementFile.Length > 0)
                {
                    var fileName = Path.GetFileName(agreementFile.FileName);
                    var path = Path.Combine(uploadPath, fileName);

                    await using var fs = new FileStream(path, FileMode.Create);
                    await agreementFile.CopyToAsync(fs);

                    m.AgreementPdfPath = $"/upload/{fileName}";
                }

                //if (signatureFile != null && signatureFile.Length > 0)
                //{
                //    var fileName = Path.GetFileName(signatureFile.FileName);
                //    var path = Path.Combine(uploadPath, fileName);

                //    await using var fs = new FileStream(path, FileMode.Create);
                //    await signatureFile.CopyToAsync(fs);

                //    m.DigitalSignaturePath = $"/upload/{fileName}";
                //}

                // ========== SIGNATURE SAVE (CANVAS OR FILE) ==========
                if (!string.IsNullOrEmpty(m.SignatureData))
                {
                    // 🔹 PRIORITY 1: CANVAS SIGNATURE
                    var cleanBase64 = m.SignatureData.Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(cleanBase64);

                    string fileName = $"{Guid.NewGuid()}.png";
                    string filePath = Path.Combine(uploadPath, fileName);

                    await File.WriteAllBytesAsync(filePath, bytes);
                    m.DigitalSignaturePath = $"/upload/{fileName}";
                }
                else if (signatureFile != null && signatureFile.Length > 0)
                {
                    // 🔹 PRIORITY 2: FILE UPLOAD
                    var fileName = $"{Path.GetFileName(signatureFile.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    await using var fs = new FileStream(filePath, FileMode.Create);
                    await signatureFile.CopyToAsync(fs);

                    m.DigitalSignaturePath = $"/upload/{fileName}";
                }



                // STEP 2: DATABASE INSERT / UPDATE

                await using var conn = CreateConnection();
                await conn.OpenAsync();


                await using var cmd = new SqlCommand("dbo.sp_Employees_Upsert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@employee_id",
            (m.EmployeeId.HasValue && m.EmployeeId > 0) ? m.EmployeeId : DBNull.Value);


                cmd.Parameters.Add(new SqlParameter("@employee_name", SqlDbType.VarChar, 255) { Value = m.EmployeeName });
                cmd.Parameters.Add(new SqlParameter("@employee_email", SqlDbType.VarChar, 255) { Value = (object?)m.EmployeeEmail ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@employee_phone", SqlDbType.VarChar, 20) { Value = (object?)m.EmployeePhone ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@employee_address", SqlDbType.VarChar, -1) { Value = (object?)m.EmployeeAddress ?? DBNull.Value });

                cmd.Parameters.Add(new SqlParameter("@designation", SqlDbType.VarChar, 100) { Value = (object?)m.Designation ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@department", SqlDbType.VarChar, 100) { Value = (object?)m.Department ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@employment_type", SqlDbType.VarChar, 50) { Value = (object?)m.EmploymentType ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@skills", SqlDbType.VarChar, -1) { Value = (object?)m.Skills ?? DBNull.Value });

                //cmd.Parameters.Add(new SqlParameter("@upload_profile", SqlDbType.VarChar, -1) { Value = (object?)m.UploadProfile ?? DBNull.Value });
                cmd.Parameters.AddWithValue("@upload_profile", (object?)m.UploadProfile ?? DBNull.Value);


                cmd.Parameters.Add(new SqlParameter("@bank_name", SqlDbType.VarChar, 255) { Value = (object?)m.BankName ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@account_number", SqlDbType.VarChar, 50) { Value = (object?)m.AccountNumber ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@ifsc_code", SqlDbType.VarChar, 20) { Value = (object?)m.IFSCCode ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@account_holder_name", SqlDbType.VarChar, 255) { Value = (object?)m.AccountHolderName ?? DBNull.Value });

                cmd.Parameters.Add(new SqlParameter("@aadhaar_number", SqlDbType.VarChar, 20) { Value = (object?)m.AadhaarNumber ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@pan_number", SqlDbType.VarChar, 20) { Value = (object?)m.PANNumber ?? DBNull.Value });

                //cmd.Parameters.Add(new SqlParameter("@aadhaar_document_path", SqlDbType.VarChar, -1) { Value = (object?)m.AadhaarDocumentPath ?? DBNull.Value });
                //cmd.Parameters.Add(new SqlParameter("@pan_document_path", SqlDbType.VarChar, -1) { Value = (object?)m.PanDocumentPath ?? DBNull.Value });

                cmd.Parameters.AddWithValue("@aadhaar_document_path", (object?)m.AadhaarDocumentPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@pan_document_path", (object?)m.PanDocumentPath ?? DBNull.Value);


                cmd.Parameters.Add(new SqlParameter("@salary", SqlDbType.Decimal) { Value = (object?)m.Salary ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@payroll_cycle", SqlDbType.VarChar, 50) { Value = (object?)m.PayrollCycle ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@paymentstatus", SqlDbType.VarChar, 50) { Value = (object?)m.PaymentStatus ?? DBNull.Value });

                //cmd.Parameters.Add(new SqlParameter("@agreement_pdf_path", SqlDbType.VarChar, -1) { Value = (object?)m.AgreementPdfPath ?? DBNull.Value });
                //cmd.Parameters.Add(new SqlParameter("@digital_signature_path", SqlDbType.VarChar, -1) { Value = (object?)m.DigitalSignaturePath ?? DBNull.Value });

                cmd.Parameters.AddWithValue("@agreement_pdf_path", (object?)m.AgreementPdfPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@digital_signature_path", (object?)m.DigitalSignaturePath ?? DBNull.Value);


                //cmd.Parameters.Add(new SqlParameter("@agreement_signed_at", SqlDbType.DateTime) { Value = (object?)m.AgreementSignedAt ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.VarChar, 50) { Value = (object?)m.Status ?? "ACTIVE" });

                // STEP 3: RETURN SAVED ROW

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return MapEmployee(reader);

                throw new InvalidOperationException("Insert/Update did not return Employee row.");
            }
            catch (Exception ex)
            {
                // Log ex here if required
                throw new ApplicationException("Error while saving employee details.", ex);
            }
        }


        public async Task<Employees?> GetByIdAsync(long employeeId)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Employees_GetById";

            cmd.Parameters.Add(new SqlParameter("@employee_id", SqlDbType.BigInt)
            {
                Value = employeeId
            });

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (await reader.ReadAsync())
                return MapEmployee(reader);

            return null;
        }

        public async Task<bool> DeleteAsync(long employeeId)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Employees_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@employee_id", SqlDbType.BigInt).Value = employeeId;

            var result = await cmd.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        private static Employees MapEmployee(IDataRecord reader)
        {
            return new Employees
            {
                EmployeeId = reader["employee_id"] != DBNull.Value
            ? Convert.ToInt64(reader["employee_id"])
            : null,

                EmployeeName = reader["employee_name"]?.ToString(),

                EmployeeEmail = reader["employee_email"] != DBNull.Value
            ? reader["employee_email"].ToString()
            : null,

                EmployeePhone = reader["employee_phone"] != DBNull.Value
            ? reader["employee_phone"].ToString()
            : null,

                EmployeeAddress = reader["employee_address"] != DBNull.Value
            ? reader["employee_address"].ToString()
            : null,

                Designation = reader["designation"] != DBNull.Value
            ? reader["designation"].ToString()
            : null,

                Department = reader["department"] != DBNull.Value
            ? reader["department"].ToString()
            : null,

                EmploymentType = reader["employment_type"] != DBNull.Value
            ? reader["employment_type"].ToString()
            : null,

                Skills = reader["skills"] != DBNull.Value
            ? reader["skills"].ToString()
            : null,

                UploadProfile = reader["upload_profile"] != DBNull.Value
            ? reader["upload_profile"].ToString()
            : null,

                BankName = reader["bank_name"] != DBNull.Value
            ? reader["bank_name"].ToString()
            : null,

                AccountNumber = reader["account_number"] != DBNull.Value
            ? reader["account_number"].ToString()
            : null,

                IFSCCode = reader["ifsc_code"] != DBNull.Value
            ? reader["ifsc_code"].ToString()
            : null,

                AccountHolderName = reader["account_holder_name"] != DBNull.Value
            ? reader["account_holder_name"].ToString()
            : null,

                AadhaarNumber = reader["aadhaar_number"] != DBNull.Value
            ? reader["aadhaar_number"].ToString()
            : null,

                PANNumber = reader["pan_number"] != DBNull.Value
            ? reader["pan_number"].ToString()
            : null,

                AadhaarDocumentPath = reader["aadhaar_document_path"] != DBNull.Value
            ? reader["aadhaar_document_path"].ToString()
            : null,

                PanDocumentPath = reader["pan_document_path"] != DBNull.Value
            ? reader["pan_document_path"].ToString()
            : null,

                Salary = reader["salary"] != DBNull.Value
            ? Convert.ToDecimal(reader["salary"])
            : null,

                PayrollCycle = reader["payroll_cycle"] != DBNull.Value
            ? reader["payroll_cycle"].ToString()
            : null,

                PaymentStatus = reader["paymentstatus"] != DBNull.Value
            ? reader["paymentstatus"].ToString()
            : null,

                AgreementPdfPath = reader["agreement_pdf_path"] != DBNull.Value
            ? reader["agreement_pdf_path"].ToString()
            : null,

                DigitalSignaturePath = reader["digital_signature_path"] != DBNull.Value
            ? reader["digital_signature_path"].ToString()
            : null,

                AgreementSignedAt = reader["agreement_signed_at"] != DBNull.Value
            ? Convert.ToDateTime(reader["agreement_signed_at"])
            : null,

                Status = reader["status"] != DBNull.Value
            ? reader["status"].ToString()
            : null,

                CreatedAt =  reader["created_at"] != DBNull.Value
            ? Convert.ToDateTime(reader["created_at"])
            : null,

                UpdatedAt =  reader["updated_at"] != DBNull.Value
            ? Convert.ToDateTime(reader["updated_at"])
            : null
            };
        }
    }

    //public void CreateEmployee(Employees employee)
    //{
    //    using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    //    using SqlCommand cmd = new SqlCommand("sp_CreateEmployee", con);
    //    cmd.CommandType = CommandType.StoredProcedure;

    //    cmd.Parameters.AddWithValue("@UserId", employee.UserId);
    //    cmd.Parameters.AddWithValue("@FullName", employee.FullName);
    //    cmd.Parameters.AddWithValue("@Domain", employee.Domain);

    //    con.Open();
    //    cmd.ExecuteNonQuery();
    //}
}

