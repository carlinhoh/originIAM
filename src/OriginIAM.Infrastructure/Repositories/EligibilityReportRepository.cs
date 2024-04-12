using Dapper;
using Npgsql;
using NpgsqlTypes;
using OriginIAM.Domain.Common;
using OriginIAM.Domain.Entities;
using OriginIAM.Domain.Interfaces;
using System.Data;

namespace OriginIAM.Infrastructure.Repositories
{
    public class EligibilityReportRepository : IEligibilityReportRepository
    {
        private readonly string _connectionString;

        public EligibilityReportRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<EligibilityRecordReport>> GetReportsByEmployerIdAsync(string employerId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM EligibilityFileReport WHERE EmployerId = @EmployerId";
                return await connection.QueryAsync<EligibilityRecordReport>(query, new { EmployerId = employerId });
            }
        }

        public async Task<PaginatedResult<EligibilityRecordReport>> GetReportsByEmployerIdAsync(string employerId, int pageNumber, int pageSize)
        {
            employerId = employerId.ToUpper();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT * FROM EligibilityFileReport WHERE UPPER(EmployerId) = @EmployerId
                    ORDER BY ProcessedAt
                    LIMIT @PageSize OFFSET @Offset;

                    SELECT COUNT(*) FROM EligibilityFileReport WHERE Upper(EmployerId) = @EmployerId;
                ";

                using var multi = await connection.QueryMultipleAsync(query, new { EmployerId = employerId, PageSize = pageSize, Offset = (pageNumber - 1) * pageSize });

                var items = await multi.ReadAsync<EligibilityRecordReport>();

                var totalCount = await multi.ReadFirstAsync<int>();

                return new PaginatedResult<EligibilityRecordReport>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }


        public async Task AddReportAsync(EligibilityRecordReport report)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO EligibilityFileReport (EmployerId, RecordData, Status, ProcessedAt)
                      VALUES (@EmployerId, @RecordData::jsonb, @Status, @ProcessedAt);";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("EmployerId", report.EmployerId);
                    command.Parameters.AddWithValue("RecordData", NpgsqlDbType.Jsonb, report.RecordData);
                    command.Parameters.AddWithValue("Status", report.Status);
                    command.Parameters.AddWithValue("ProcessedAt", report.ProcessedAt);

                    await command.ExecuteNonQueryAsync();
                }

            }
        }

        public async Task RemoveLastReport(string employerId)
        {
            employerId = employerId.ToUpper();
            
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"DELETE FROM EligibilityFileReport WHERE UPPER(EmployerId) = @EmployerId;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("EmployerId", employerId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}