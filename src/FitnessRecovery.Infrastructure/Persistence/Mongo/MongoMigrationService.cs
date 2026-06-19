using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessRecovery.Features.Dashboard.Contracts;
using FitnessRecovery.Features.Dashboard.Domain;
using FitnessRecovery.Features.Health.Contracts;
using FitnessRecovery.Features.Recovery.Contracts;
using FitnessRecovery.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessRecovery.Infrastructure.Persistence.Mongo;

public class MongoMigrationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHealthRecordMongoRepository _healthRecordMongo;
    private readonly IRecoveryAnalysisMongoRepository _recoveryAnalysisMongo;
    private readonly IWeeklyReportMongoRepository _weeklyReportMongo;

    public MongoMigrationService(
        ApplicationDbContext dbContext,
        IHealthRecordMongoRepository healthRecordMongo,
        IRecoveryAnalysisMongoRepository recoveryAnalysisMongo,
        IWeeklyReportMongoRepository weeklyReportMongo)
    {
        _dbContext = dbContext;
        _healthRecordMongo = healthRecordMongo;
        _recoveryAnalysisMongo = recoveryAnalysisMongo;
        _weeklyReportMongo = weeklyReportMongo;
    }

    public async Task MigrateAsync()
    {
        var users = await _dbContext.Users.ToListAsync();
        foreach (var user in users)
        {
            // 1. Sync Health Records
            var healthRecords = await _dbContext.HealthRecords
                .Where(h => h.UserId == user.Id)
                .ToListAsync();

            foreach (var record in healthRecords)
            {
                await _healthRecordMongo.UpsertAsync(record);
            }

            // 2. Sync Recovery Analyses
            var analyses = await _dbContext.RecoveryAnalyses
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            foreach (var analysis in analyses)
            {
                await _recoveryAnalysisMongo.UpsertAsync(analysis);
            }

            // 3. Generate and Sync Weekly Reports
            if (healthRecords.Any())
            {
                var minDate = healthRecords.Min(h => h.RecordDate);
                var startOfWeek = GetStartOfWeek(minDate);
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                while (startOfWeek.AddDays(7) <= today) // completed weeks only
                {
                    var endOfWeek = startOfWeek.AddDays(6);

                    var weekHealth = healthRecords
                        .Where(h => h.RecordDate >= startOfWeek && h.RecordDate <= endOfWeek)
                        .ToList();

                    var weekAnalyses = analyses
                        .Where(a => a.AnalysisDate >= startOfWeek && a.AnalysisDate <= endOfWeek)
                        .ToList();

                    var startDateTime = startOfWeek.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                    var endDateTime = endOfWeek.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);

                    var weekWorkouts = await _dbContext.WorkoutSessions
                        .Where(w => w.UserId == user.Id && w.WorkoutDate >= startDateTime && w.WorkoutDate < endDateTime)
                        .ToListAsync();

                    if (weekHealth.Any() || weekAnalyses.Any() || weekWorkouts.Any())
                    {
                        double avgRecovery = weekAnalyses.Any() ? weekAnalyses.Average(a => a.RecoveryScore.Value) : 0;
                        double avgSleep = weekHealth.Any() ? weekHealth.Average(h => h.SleepHours.Value) : 0;
                        int totalDuration = weekWorkouts.Sum(w => w.DurationMinutes);
                        int totalCalories = weekWorkouts.Sum(w => w.CaloriesBurned);
                        int totalSteps = weekHealth.Sum(h => h.Steps.Value);

                        var report = new WeeklyReport(
                            user.Id,
                            startOfWeek,
                            endOfWeek,
                            avgRecovery,
                            avgSleep,
                            totalDuration,
                            totalCalories,
                            totalSteps
                        );

                        await _weeklyReportMongo.UpsertAsync(report);
                    }

                    startOfWeek = startOfWeek.AddDays(7);
                }
            }
        }
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff);
    }
}
