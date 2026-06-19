using System;

namespace FitnessRecovery.Features.Dashboard.DTOs;

public record WeeklyReportDto(
    Guid Id,
    Guid UserId,
    DateOnly StartDate,
    DateOnly EndDate,
    double AverageRecoveryScore,
    double AverageSleepHours,
    int TotalWorkoutDuration,
    int TotalCaloriesBurned,
    int TotalSteps,
    DateTime GeneratedAt);
