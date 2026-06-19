using System;

namespace FitnessRecovery.Features.Dashboard.Domain;

public class WeeklyReport
{
    public WeeklyReport(
        Guid userId,
        DateOnly startDate,
        DateOnly endDate,
        double averageRecoveryScore,
        double averageSleepHours,
        int totalWorkoutDuration,
        int totalCaloriesBurned,
        int totalSteps)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        AverageRecoveryScore = averageRecoveryScore;
        AverageSleepHours = averageSleepHours;
        TotalWorkoutDuration = totalWorkoutDuration;
        TotalCaloriesBurned = totalCaloriesBurned;
        TotalSteps = totalSteps;
        GeneratedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public double AverageRecoveryScore { get; private set; }
    public double AverageSleepHours { get; private set; }
    public int TotalWorkoutDuration { get; private set; }
    public int TotalCaloriesBurned { get; private set; }
    public int TotalSteps { get; private set; }
    public DateTime GeneratedAt { get; private set; }
}
