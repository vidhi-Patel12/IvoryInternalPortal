using IvoryInternalPortal.Interface;
using IvoryInternalPortal.Model;
using System.Data;
using System.Data.SqlClient;

namespace IvoryInternalPortal.Repository
{
    public class VendorRepository : IVendor
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public VendorRepository(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private SqlConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<Vendors>> GetAllAsync()
        {
            var list = new List<Vendors>();

            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Vendors_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapVendor(reader));
            }

            return list;
        }


        public async Task<Vendors> InsertUpdateAsync(
            Vendors v,
            IFormFile? agreementFile,
            IFormFile? signatureFile)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));

            // FILE UPLOAD
            string uploadPath = Path.Combine(_env.ContentRootPath, "wwwroot", "upload");
            Directory.CreateDirectory(uploadPath);

            if (agreementFile != null && agreementFile.Length > 0)
            {
                var fileName = Path.GetFileName(agreementFile.FileName);
                var path = Path.Combine(uploadPath, fileName);

                await using var fs = new FileStream(path, FileMode.Create);
                await agreementFile.CopyToAsync(fs);

                v.AgreementPdfPath = $"/upload/{fileName}";
            }

            //if (signatureFile != null && signatureFile.Length > 0)
            //{
            //    var fileName = Path.GetFileName(signatureFile.FileName);
            //    var path = Path.Combine(uploadPath, fileName);

            //    await using var fs = new FileStream(path, FileMode.Create);
            //    await signatureFile.CopyToAsync(fs);

            //    v.DigitalSignaturePath = $"/upload/{fileName}";
            //}

            if (!string.IsNullOrEmpty(v.SignatureData))
            {
                //  PRIORITY 1: CANVAS SIGNATURE
                var cleanBase64 = v.SignatureData.Split(',')[1];
                byte[] bytes = Convert.FromBase64String(cleanBase64);

                string fileName = $"{Guid.NewGuid()}.png";
                string filePath = Path.Combine(uploadPath, fileName);

                await File.WriteAllBytesAsync(filePath, bytes);
                v.DigitalSignaturePath = $"/upload/{fileName}";
            }
            else if (signatureFile != null && signatureFile.Length > 0)
            {
                //  PRIORITY 2: FILE UPLOAD
                var fileName = $"{Path.GetFileName(signatureFile.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                await using var fs = new FileStream(filePath, FileMode.Create);
                await signatureFile.CopyToAsync(fs);

                v.DigitalSignaturePath = $"/upload/{fileName}";
            }


            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Vendors_Upsert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@vendor_id", (object?)v.VendorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@company_name", v.CompanyName);
            cmd.Parameters.AddWithValue("@company_address", (object?)v.CompanyAddress ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@company_email", (object?)v.CompanyEmail ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@company_phone", (object?)v.CompanyPhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@poc_phone", (object?)v.PocPhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@gst_number", (object?)v.GstNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roc_number", (object?)v.RocNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@uan_number", (object?)v.UanNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@bank_name", (object?)v.BankName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@account_number", (object?)v.AccountNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ifsc_code", (object?)v.IFSCCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@account_holder_name", (object?)v.AccountHolderName ?? DBNull.Value);
            
            //cmd.Parameters.AddWithValue("@agreement_pdf_path", (object?)v.AgreementPdfPath ?? DBNull.Value);
            //cmd.Parameters.AddWithValue("@digital_signature_path", (object?)v.DigitalSignaturePath ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@agreement_pdf_path", (object?)v.AgreementPdfPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@digital_signature_path", (object?)v.DigitalSignaturePath ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@status", (object?)v.Status ?? "ACTIVE");


            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await reader.ReadAsync())
                throw new InvalidOperationException("Vendor insert/update failed.");

            return MapVendor(reader);
        }

        public async Task<Vendors?> GetByIdAsync(long vendorId)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Vendors_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@vendor_id", vendorId);

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            return await reader.ReadAsync() ? MapVendor(reader) : null;
        }

        public async Task<bool> DeleteAsync(long vendorId)
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("dbo.sp_Vendors_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@vendor_id", vendorId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        private static Vendors MapVendor(IDataRecord r)
        {
            return new Vendors
            {
                VendorId = r["vendor_id"] != DBNull.Value ? Convert.ToInt64(r["vendor_id"]) : null,
                CompanyName = r["company_name"].ToString(),
                CompanyAddress = r["company_address"]?.ToString(),
                CompanyEmail = r["company_email"]?.ToString(),
                CompanyPhone = r["company_phone"]?.ToString(),
                PocPhone = r["poc_phone"]?.ToString(),
                GstNumber = r["gst_number"]?.ToString(),
                RocNumber = r["roc_number"]?.ToString(),
                UanNumber = r["uan_number"]?.ToString(),
                BankName = r["bank_name"]?.ToString(),
                AccountNumber = r["account_number"]?.ToString(),
                IFSCCode = r["ifsc_code"]?.ToString(),
                AccountHolderName = r["account_holder_name"]?.ToString(),
                AgreementPdfPath = r["agreement_pdf_path"]?.ToString(),
                DigitalSignaturePath = r["digital_signature_path"]?.ToString(),
                Status = r["status"]?.ToString(),
                CreatedAt = r["created_at"] as DateTime?,
                UpdatedAt = r["updated_at"] as DateTime?
            };
        }





        //public void CreateVendor(Vendors vendor)
        //{
        //    using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        //    using SqlCommand cmd = new SqlCommand("sp_CreateVendor", con);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@UserId", vendor.UserId);
        //    cmd.Parameters.AddWithValue("@CompanyName", vendor.CompanyName);
        //    cmd.Parameters.AddWithValue("@CompanyAddress", vendor.CompanyAddress);
        //    cmd.Parameters.AddWithValue("@GSTNumber", vendor.GSTNumber);
        //    cmd.Parameters.AddWithValue("@UANNumber", vendor.UANNumber);
        //    cmd.Parameters.AddWithValue("@Email", vendor.Email);
        //    cmd.Parameters.AddWithValue("@Phone", vendor.Phone);
        //    cmd.Parameters.AddWithValue("@PointOfContactPerson", vendor.PointOfContactPerson);
        //    cmd.Parameters.AddWithValue("@Status", "Pending");
        //    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //}
    }
}
