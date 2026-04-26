using ACCIFCConverter.Domain.Contracts;
using ACCIFCConverter.Domain.Models;
using Microsoft.Data.Sqlite;

namespace ACCIFCConverter.Infrastructure.Persistence;

public sealed class SqliteHistoryRepository(string connectionString) : IHistoryRepository
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = new SqliteConnection(connectionString);
        await conn.OpenAsync(cancellationToken);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ExportHistory (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            JobId TEXT NOT NULL,
            FileName TEXT NOT NULL,
            StartUtc TEXT NOT NULL,
            EndUtc TEXT NOT NULL,
            DurationSeconds INTEGER NOT NULL,
            Result TEXT NOT NULL,
            OutputPath TEXT NOT NULL,
            Logs TEXT NOT NULL
        );";
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task AddAsync(ExportHistoryRecord record, CancellationToken cancellationToken = default)
    {
        await using var conn = new SqliteConnection(connectionString);
        await conn.OpenAsync(cancellationToken);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO ExportHistory (JobId,FileName,StartUtc,EndUtc,DurationSeconds,Result,OutputPath,Logs)
            VALUES ($jobId,$fileName,$start,$end,$duration,$result,$output,$logs);";
        cmd.Parameters.AddWithValue("$jobId", record.JobId.ToString());
        cmd.Parameters.AddWithValue("$fileName", record.FileName);
        cmd.Parameters.AddWithValue("$start", record.StartUtc.ToString("O"));
        cmd.Parameters.AddWithValue("$end", record.EndUtc.ToString("O"));
        cmd.Parameters.AddWithValue("$duration", record.DurationSeconds);
        cmd.Parameters.AddWithValue("$result", record.Result);
        cmd.Parameters.AddWithValue("$output", record.OutputPath);
        cmd.Parameters.AddWithValue("$logs", record.Logs);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExportHistoryRecord>> GetRecentAsync(int limit = 500, CancellationToken cancellationToken = default)
    {
        var items = new List<ExportHistoryRecord>();
        await using var conn = new SqliteConnection(connectionString);
        await conn.OpenAsync(cancellationToken);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, JobId, FileName, StartUtc, EndUtc, DurationSeconds, Result, OutputPath, Logs FROM ExportHistory ORDER BY Id DESC LIMIT $limit;";
        cmd.Parameters.AddWithValue("$limit", limit);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new ExportHistoryRecord
            {
                Id = reader.GetInt64(0),
                JobId = Guid.Parse(reader.GetString(1)),
                FileName = reader.GetString(2),
                StartUtc = DateTimeOffset.Parse(reader.GetString(3)),
                EndUtc = DateTimeOffset.Parse(reader.GetString(4)),
                DurationSeconds = reader.GetInt64(5),
                Result = reader.GetString(6),
                OutputPath = reader.GetString(7),
                Logs = reader.GetString(8)
            });
        }
        return items;
    }
}
