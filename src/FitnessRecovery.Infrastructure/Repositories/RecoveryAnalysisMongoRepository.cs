using System;
using System.Threading.Tasks;
using FitnessRecovery.Features.Recovery.Contracts;
using FitnessRecovery.Features.Recovery.Domain;
using FitnessRecovery.Infrastructure.Persistence.Mongo;
using FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;
using MongoDB.Driver;

namespace FitnessRecovery.Infrastructure.Repositories;

public class RecoveryAnalysisMongoRepository : IRecoveryAnalysisMongoRepository
{
    private readonly IMongoCollection<RecoveryAnalysisDocument> _collection;

    public RecoveryAnalysisMongoRepository(MongoDbContext mongoContext)
    {
        _collection = mongoContext.GetCollection<RecoveryAnalysisDocument>("recovery_analyses");
    }

    public async Task UpsertAsync(RecoveryAnalysis analysis)
    {
        var doc = new RecoveryAnalysisDocument
        {
            Id = analysis.Id,
            UserId = analysis.UserId,
            AnalysisDate = analysis.AnalysisDate,
            RecoveryScore = analysis.RecoveryScore.Value,
            RecoveryStatus = analysis.RecoveryStatus.ToString(),
            SleepScore = analysis.SleepScore,
            HeartRateScore = analysis.HeartRateScore,
            WorkoutLoadScore = analysis.WorkoutLoadScore,
            ActivityScore = analysis.ActivityScore,
            GeneratedAt = analysis.GeneratedAt,
            CreatedAt = analysis.CreatedAt,
            UpdatedAt = analysis.UpdatedAt
        };

        var filter = Builders<RecoveryAnalysisDocument>.Filter.And(
            Builders<RecoveryAnalysisDocument>.Filter.Eq(r => r.UserId, doc.UserId),
            Builders<RecoveryAnalysisDocument>.Filter.Eq(r => r.AnalysisDate, doc.AnalysisDate)
        );

        await _collection.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true });
    }
}
