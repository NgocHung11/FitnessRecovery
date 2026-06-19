using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitnessRecovery.Features.Auth.Domain;
using FitnessRecovery.Features.Dashboard.Domain;
using FitnessRecovery.Features.Health.Contracts;
using FitnessRecovery.Features.Health.Domain;
using FitnessRecovery.Features.Recovery.Contracts;
using FitnessRecovery.Features.Recovery.Domain;
using FitnessRecovery.Features.Workout.Domain;
using FitnessRecovery.Features.Dashboard.Contracts;
using FitnessRecovery.Infrastructure.Persistence;
using FitnessRecovery.Infrastructure.Persistence.Mongo;
using FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;
using FitnessRecovery.Infrastructure.Repositories;
using FitnessRecovery.SharedKernel.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace FitnessRecovery.UnitTests;

public class MongoIntegrationTests
{
    [Fact]
    public async Task CreateIndexesAsync_ShouldCreateUniqueIndexes()
    {
        // Arrange
        var mockContext = Substitute.For<MongoDbContext>();

        var mockHealthCollection = Substitute.For<IMongoCollection<HealthRecordDocument>>();
        var mockHealthIndexes = Substitute.For<IMongoIndexManager<HealthRecordDocument>>();
        mockHealthCollection.Indexes.Returns(mockHealthIndexes);
        mockContext.GetCollection<HealthRecordDocument>("health_records").Returns(mockHealthCollection);

        var mockRecoveryCollection = Substitute.For<IMongoCollection<RecoveryAnalysisDocument>>();
        var mockRecoveryIndexes = Substitute.For<IMongoIndexManager<RecoveryAnalysisDocument>>();
        mockRecoveryCollection.Indexes.Returns(mockRecoveryIndexes);
        mockContext.GetCollection<RecoveryAnalysisDocument>("recovery_analyses").Returns(mockRecoveryCollection);

        var mockReportCollection = Substitute.For<IMongoCollection<WeeklyReportDocument>>();
        var mockReportIndexes = Substitute.For<IMongoIndexManager<WeeklyReportDocument>>();
        mockReportCollection.Indexes.Returns(mockReportIndexes);
        mockContext.GetCollection<WeeklyReportDocument>("weekly_reports").Returns(mockReportCollection);

        // Act
        await MongoIndexConfig.CreateIndexesAsync(mockContext);

        // Assert
        await mockHealthIndexes.Received(1).CreateOneAsync(
            Arg.Is<CreateIndexModel<HealthRecordDocument>>(m => 
                m.Options.Unique == true && 
                m.Options.Name == "IX_health_records_UserId_RecordDate"),
            Arg.Any<CreateOneIndexOptions>(),
            Arg.Any<CancellationToken>()
        );

        await mockRecoveryIndexes.Received(1).CreateOneAsync(
            Arg.Is<CreateIndexModel<RecoveryAnalysisDocument>>(m => 
                m.Options.Unique == true && 
                m.Options.Name == "IX_recovery_analyses_UserId_AnalysisDate"),
            Arg.Any<CreateOneIndexOptions>(),
            Arg.Any<CancellationToken>()
        );

        await mockReportIndexes.Received(1).CreateOneAsync(
            Arg.Is<CreateIndexModel<WeeklyReportDocument>>(m => 
                m.Options.Unique == true && 
                m.Options.Name == "IX_weekly_reports_UserId_StartDate"),
            Arg.Any<CreateOneIndexOptions>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task HealthRecordMongoRepository_UpsertAsync_ShouldMapAndReplaceDocument()
    {
        // Arrange
        var mockContext = Substitute.For<MongoDbContext>();
        var mockCollection = Substitute.For<IMongoCollection<HealthRecordDocument>>();
        mockContext.GetCollection<HealthRecordDocument>("health_records").Returns(mockCollection);

        var repository = new HealthRecordMongoRepository(mockContext);

        var userId = Guid.NewGuid();
        var record = new HealthRecord(
            userId,
            new DateOnly(2026, 6, 19),
            new SleepHours(7.5),
            SleepQuality.Good,
            new HeartRate(60),
            new HeartRate(75),
            new Steps(10000),
            70.5,
            300
        );

        HealthRecordDocument? capturedDoc = null;
        await mockCollection.ReplaceOneAsync(
            Arg.Any<FilterDefinition<HealthRecordDocument>>(),
            Arg.Do<HealthRecordDocument>(doc => capturedDoc = doc),
            Arg.Any<ReplaceOptions>(),
            Arg.Any<CancellationToken>()
        );

        // Act
        await repository.UpsertAsync(record);

        // Assert
        capturedDoc.Should().NotBeNull();
        capturedDoc!.Id.Should().Be(record.Id);
        capturedDoc.UserId.Should().Be(record.UserId);
        capturedDoc.RecordDate.Should().Be(record.RecordDate);
        capturedDoc.SleepHours.Should().Be(7.5);
        capturedDoc.SleepQuality.Should().Be("Good");
        capturedDoc.RestingHeartRate.Should().Be(60);
        capturedDoc.AverageHeartRate.Should().Be(75);
        capturedDoc.Steps.Should().Be(10000);
        capturedDoc.Weight.Should().Be(70.5);
        capturedDoc.CaloriesBurned.Should().Be(300);
    }

    [Fact]
    public async Task RecoveryAnalysisMongoRepository_UpsertAsync_ShouldMapAndReplaceDocument()
    {
        // Arrange
        var mockContext = Substitute.For<MongoDbContext>();
        var mockCollection = Substitute.For<IMongoCollection<RecoveryAnalysisDocument>>();
        mockContext.GetCollection<RecoveryAnalysisDocument>("recovery_analyses").Returns(mockCollection);

        var repository = new RecoveryAnalysisMongoRepository(mockContext);

        var userId = Guid.NewGuid();
        var analysis = new RecoveryAnalysis(
            userId,
            new DateOnly(2026, 6, 19),
            new RecoveryScore(85),
            RecoveryStatus.Good,
            80,
            85,
            90,
            95
        );

        RecoveryAnalysisDocument? capturedDoc = null;
        await mockCollection.ReplaceOneAsync(
            Arg.Any<FilterDefinition<RecoveryAnalysisDocument>>(),
            Arg.Do<RecoveryAnalysisDocument>(doc => capturedDoc = doc),
            Arg.Any<ReplaceOptions>(),
            Arg.Any<CancellationToken>()
        );

        // Act
        await repository.UpsertAsync(analysis);

        // Assert
        capturedDoc.Should().NotBeNull();
        capturedDoc!.Id.Should().Be(analysis.Id);
        capturedDoc.UserId.Should().Be(analysis.UserId);
        capturedDoc.AnalysisDate.Should().Be(analysis.AnalysisDate);
        capturedDoc.RecoveryScore.Should().Be(85);
        capturedDoc.RecoveryStatus.Should().Be("Good");
        capturedDoc.SleepScore.Should().Be(80);
        capturedDoc.HeartRateScore.Should().Be(85);
        capturedDoc.WorkoutLoadScore.Should().Be(90);
        capturedDoc.ActivityScore.Should().Be(95);
    }

    [Fact]
    public async Task WeeklyReportMongoRepository_UpsertAsync_ShouldMapAndReplaceDocument()
    {
        // Arrange
        var mockContext = Substitute.For<MongoDbContext>();
        var mockCollection = Substitute.For<IMongoCollection<WeeklyReportDocument>>();
        mockContext.GetCollection<WeeklyReportDocument>("weekly_reports").Returns(mockCollection);

        var repository = new WeeklyReportMongoRepository(mockContext);

        var userId = Guid.NewGuid();
        var report = new WeeklyReport(
            userId,
            new DateOnly(2026, 6, 8),
            new DateOnly(2026, 6, 14),
            82.5,
            7.8,
            150,
            1200,
            50000
        );

        WeeklyReportDocument? capturedDoc = null;
        await mockCollection.ReplaceOneAsync(
            Arg.Any<FilterDefinition<WeeklyReportDocument>>(),
            Arg.Do<WeeklyReportDocument>(doc => capturedDoc = doc),
            Arg.Any<ReplaceOptions>(),
            Arg.Any<CancellationToken>()
        );

        // Act
        await repository.UpsertAsync(report);

        // Assert
        capturedDoc.Should().NotBeNull();
        capturedDoc!.Id.Should().Be(report.Id);
        capturedDoc.UserId.Should().Be(report.UserId);
        capturedDoc.StartDate.Should().Be(report.StartDate);
        capturedDoc.EndDate.Should().Be(report.EndDate);
        capturedDoc.AverageRecoveryScore.Should().Be(82.5);
        capturedDoc.AverageSleepHours.Should().Be(7.8);
        capturedDoc.TotalWorkoutDuration.Should().Be(150);
        capturedDoc.TotalCaloriesBurned.Should().Be(1200);
        capturedDoc.TotalSteps.Should().Be(50000);
    }

    [Fact]
    public async Task MongoMigrationService_MigrateAsync_ShouldSyncDataAndGenerateWeeklyReports()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "FitnessRecovery_MigrationTest_" + Guid.NewGuid())
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        var user = new User(
            "migrated@example.com", 
            "hash", 
            "John", 
            "Doe", 
            "Male", 
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
            175, 
            75, 
            "Strength"
        );
        dbContext.Users.Add(user);

        // Seed HealthRecords for a completed week (Monday to Sunday)
        // Let's pick a week. June 8, 2026 is Monday. June 14, 2026 is Sunday.
        var startDate = new DateOnly(2026, 6, 8);
        for (int i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            var record = new HealthRecord(
                user.Id,
                date,
                new SleepHours(8.0),
                SleepQuality.Good,
                new HeartRate(60),
                new HeartRate(70),
                new Steps(10000),
                75.0,
                250
            );
            dbContext.HealthRecords.Add(record);

            var analysis = new RecoveryAnalysis(
                user.Id,
                date,
                new RecoveryScore(80),
                RecoveryStatus.Good,
                80,
                80,
                80,
                80
            );
            dbContext.RecoveryAnalyses.Add(analysis);
        }

        // Seed a Workout Session in that week
        // Tuesday, June 9, 2026
        var workoutDate = new DateTime(2026, 6, 9, 10, 0, 0, DateTimeKind.Utc);
        var workout = new WorkoutSession(
            user.Id,
            WorkoutType.Gym,
            60,
            400,
            WorkoutIntensity.Moderate,
            "Heavy lifting",
            workoutDate
        );
        dbContext.WorkoutSessions.Add(workout);

        await dbContext.SaveChangesAsync();

        // Mock Mongo Repositories
        var mockHealthMongo = Substitute.For<IHealthRecordMongoRepository>();
        var mockRecoveryMongo = Substitute.For<IRecoveryAnalysisMongoRepository>();
        var mockReportMongo = Substitute.For<IWeeklyReportMongoRepository>();

        var migrationService = new MongoMigrationService(
            dbContext,
            mockHealthMongo,
            mockRecoveryMongo,
            mockReportMongo
        );

        // Act
        await migrationService.MigrateAsync();

        // Assert
        // Check that health records and recovery analyses were upserted to Mongo
        await mockHealthMongo.Received(7).UpsertAsync(Arg.Any<HealthRecord>());
        await mockRecoveryMongo.Received(7).UpsertAsync(Arg.Any<RecoveryAnalysis>());

        // Check that a weekly report was generated and upserted
        await mockReportMongo.Received(1).UpsertAsync(Arg.Is<WeeklyReport>(r =>
            r.UserId == user.Id &&
            r.StartDate == startDate &&
            r.EndDate == startDate.AddDays(6) &&
            r.AverageRecoveryScore == 80.0 &&
            r.AverageSleepHours == 8.0 &&
            r.TotalWorkoutDuration == 60 &&
            r.TotalCaloriesBurned == 400 &&
            r.TotalSteps == 70000
        ));
    }
}
