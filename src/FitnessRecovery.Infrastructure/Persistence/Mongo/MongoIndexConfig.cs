using System.Threading.Tasks;
using FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;
using MongoDB.Driver;

namespace FitnessRecovery.Infrastructure.Persistence.Mongo;

public static class MongoIndexConfig
{
    public static async Task CreateIndexesAsync(MongoDbContext mongoContext)
    {
        // 1. Health Records Index
        var healthCol = mongoContext.GetCollection<HealthRecordDocument>("health_records");
        var healthIndexKeys = Builders<HealthRecordDocument>.IndexKeys.Combine(
            Builders<HealthRecordDocument>.IndexKeys.Ascending(h => h.UserId),
            Builders<HealthRecordDocument>.IndexKeys.Ascending(h => h.RecordDate)
        );
        await healthCol.Indexes.CreateOneAsync(new CreateIndexModel<HealthRecordDocument>(
            healthIndexKeys, 
            new CreateIndexOptions { Unique = true, Name = "IX_health_records_UserId_RecordDate" }
        ));

        // 2. Recovery Analyses Index
        var recoveryCol = mongoContext.GetCollection<RecoveryAnalysisDocument>("recovery_analyses");
        var recoveryIndexKeys = Builders<RecoveryAnalysisDocument>.IndexKeys.Combine(
            Builders<RecoveryAnalysisDocument>.IndexKeys.Ascending(r => r.UserId),
            Builders<RecoveryAnalysisDocument>.IndexKeys.Ascending(r => r.AnalysisDate)
        );
        await recoveryCol.Indexes.CreateOneAsync(new CreateIndexModel<RecoveryAnalysisDocument>(
            recoveryIndexKeys, 
            new CreateIndexOptions { Unique = true, Name = "IX_recovery_analyses_UserId_AnalysisDate" }
        ));

        // 3. Weekly Reports Index
        var reportCol = mongoContext.GetCollection<WeeklyReportDocument>("weekly_reports");
        var reportIndexKeys = Builders<WeeklyReportDocument>.IndexKeys.Combine(
            Builders<WeeklyReportDocument>.IndexKeys.Ascending(w => w.UserId),
            Builders<WeeklyReportDocument>.IndexKeys.Ascending(w => w.StartDate)
        );
        await reportCol.Indexes.CreateOneAsync(new CreateIndexModel<WeeklyReportDocument>(
            reportIndexKeys,
            new CreateIndexOptions { Unique = true, Name = "IX_weekly_reports_UserId_StartDate" }
        ));
    }
}
