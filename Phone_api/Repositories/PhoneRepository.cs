using Microsoft.EntityFrameworkCore;
using Phone_api.Common;
using Phone_api.Data;
using Phone_api.Entities;
using System.Data;

namespace Phone_api.Repositories
{
    /// <summary>
    /// Repository cho Phone, kế thừa Generic Repository
    /// </summary>
    public class PhoneRepository : Repository<Phone>, IPhoneRepository
    {
        public PhoneRepository(PhoneContext context) : base(context)
        {
        }

        public async Task<bool> AddPhoneAsync(Phone phone)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = SqlConstants.AddPhoneProcedure; // Sử dụng hằng số
                command.CommandType = CommandType.StoredProcedure;

                // Thêm các tham số
                var modelParam = command.CreateParameter();
                modelParam.ParameterName = SqlConstants.ParamModel; // Sử dụng hằng số
                modelParam.Value = phone.Model ?? (object)DBNull.Value;
                command.Parameters.Add(modelParam);

                var priceParam = command.CreateParameter();
                priceParam.ParameterName = SqlConstants.ParamPrice; // Sử dụng hằng số
                priceParam.Value = phone.Price;
                command.Parameters.Add(priceParam);

                var stockParam = command.CreateParameter();
                stockParam.ParameterName = SqlConstants.ParamStock; // Sử dụng hằng số
                stockParam.Value = phone.Stock;
                command.Parameters.Add(stockParam);

                var brandIdParam = command.CreateParameter();
                brandIdParam.ParameterName = SqlConstants.ParamBrandId; // Sử dụng hằng số
                brandIdParam.Value = phone.BrandId.HasValue ? (object)phone.BrandId.Value : DBNull.Value;
                command.Parameters.Add(brandIdParam);

                var resultParam = command.CreateParameter();
                resultParam.ParameterName = SqlConstants.ParamResult; // Sử dụng hằng số
                resultParam.Value = 0;
                resultParam.Direction = ParameterDirection.Output;
                resultParam.DbType = DbType.Boolean;
                command.Parameters.Add(resultParam);

                if (command is null && command?.Connection is null)
                {
                    return false;
                }

                // Mở kết nối và thực thi
                if (command.Connection!.State != ConnectionState.Open)
                {
                    await command.Connection.OpenAsync();
                }

                await command.ExecuteNonQueryAsync();

                // Lấy giá trị trả về
                var result = (bool)command.Parameters[SqlConstants.ParamResult].Value!;
                return result;
            }
        }
    }
}
