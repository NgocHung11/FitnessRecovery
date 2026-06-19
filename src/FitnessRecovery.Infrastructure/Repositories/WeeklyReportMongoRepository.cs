using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessRecovery.Features.Dashboard.Contracts;
using FitnessRecovery.Features.Dashboard.Domain;
using FitnessRecovery.Infrastructure.Persistence.Mongo;
using FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;
using MongoDB.Driver;

namespace FitnessRecovery.Infrastructure.Repositories;

public class WeeklyReportMongoRepository : IWeeklyReportMongoRepository
{
    private readonly IMongoCollection<WeeklyReportDocument> _collection;

    public WeeklyReportMongoRepository(MongoDbContext mongoContext)
    {
        _collection = mongoContext.GetCollection<WeeklyReportDocument>("weekly_reports");
    }

    public async Task UpsertAsync(WeeklyReport report)
    {
        var doc = new WeeklyReportDocument
        {
            Id = report.Id,
            UserId = report.UserId,
            StartDate = report.StartDate,
            EndDate = report.EndDate,
            AverageRecoveryScore = report.AverageRecoveryScore,
            AverageSleepHours = report.AverageSleepHours,
            TotalWorkoutDuration = report.TotalWorkoutDuration,
            TotalCaloriesBurned = report.TotalCaloriesBurned,
            TotalSteps = report.TotalSteps,
            GeneratedAt = report.GeneratedAt
        };

        var filter = Builders<WeeklyReportDocument>.Filter.And(
            Builders<WeeklyReportDocument>.Filter.Eq(w => w.UserId, doc.UserId),
            Builders<WeeklyReportDocument>.Filter.Eq(w => w.StartDate, doc.StartDate)
        );

        await _collection.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true });
    }

    public async Task<List<WeeklyReport>> GetByUserIdAsync(Guid userId)
    {
        var filter = Builders<WeeklyReportDocument>.Filter.Eq(w => w.UserId, userId);
        var docs = await _collection.Find(filter)
            .SortByDescending(w => w.StartDate)
            .ToListAsync();

        return docs.Select(doc => new WeeklyReport(
            doc.UserId,
            doc.StartDate,
            doc.EndDate,
            doc.AverageRecoveryScore,
            doc.AverageSleepHours,
            doc.TotalWorkoutDuration,
            doc.TotalCaloriesBurned,
            doc.TotalSteps
        )).ToList();
    }
}
