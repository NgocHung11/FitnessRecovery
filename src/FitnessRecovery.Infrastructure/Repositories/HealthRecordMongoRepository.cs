using System;
using System.Threading.Tasks;
using FitnessRecovery.Features.Health.Contracts;
using FitnessRecovery.Features.Health.Domain;
using FitnessRecovery.Infrastructure.Persistence.Mongo;
using FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;
using MongoDB.Driver;

namespace FitnessRecovery.Infrastructure.Repositories;

public class HealthRecordMongoRepository : IHealthRecordMongoRepository
{
    private readonly IMongoCollection<HealthRecordDocument> _collection;

    public HealthRecordMongoRepository(MongoDbContext mongoContext)
    {
        _collection = mongoContext.GetCollection<HealthRecordDocument>("health_records");
    }

    public async Task UpsertAsync(HealthRecord record)
    {
        var doc = new HealthRecordDocument
        {
            Id = record.Id,
            UserId = record.UserId,
            RecordDate = record.RecordDate,
            SleepHours = record.SleepHours.Value,
            SleepQuality = record.SleepQuality.ToString(),
            RestingHeartRate = record.RestingHeartRate.Value,
            AverageHeartRate = record.AverageHeartRate.Value,
            Steps = record.Steps.Value,
            Weight = record.Weight,
            CaloriesBurned = record.CaloriesBurned,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };

        var filter = Builders<HealthRecordDocument>.Filter.And(
            Builders<HealthRecordDocument>.Filter.Eq(h => h.UserId, doc.UserId),
            Builders<HealthRecordDocument>.Filter.Eq(h => h.RecordDate, doc.RecordDate)
        );

        await _collection.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true });
    }

    public async Task DeleteAsync(Guid userId, DateOnly date)
    {
        var filter = Builders<HealthRecordDocument>.Filter.And(
            Builders<HealthRecordDocument>.Filter.Eq(h => h.UserId, userId),
            Builders<HealthRecordDocument>.Filter.Eq(h => h.RecordDate, date)
        );

        await _collection.DeleteOneAsync(filter);
    }
}
