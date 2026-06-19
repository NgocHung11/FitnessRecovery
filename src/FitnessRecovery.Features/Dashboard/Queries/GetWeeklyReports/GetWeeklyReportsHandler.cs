using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitnessRecovery.Features.Dashboard.Contracts;
using FitnessRecovery.Features.Dashboard.DTOs;
using FitnessRecovery.SharedKernel.Models;

namespace FitnessRecovery.Features.Dashboard.Queries.GetWeeklyReports;

public class GetWeeklyReportsHandler
{
    private readonly IWeeklyReportMongoRepository _weeklyReportRepository;

    public GetWeeklyReportsHandler(IWeeklyReportMongoRepository weeklyReportRepository)
    {
        _weeklyReportRepository = weeklyReportRepository;
    }

    public async Task<Result<List<WeeklyReportDto>>> HandleAsync(GetWeeklyReportsQuery query, CancellationToken cancellationToken = default)
    {
        var reports = await _weeklyReportRepository.GetByUserIdAsync(query.UserId);

        var dtos = reports.Select(r => new WeeklyReportDto(
            r.Id,
            r.UserId,
            r.StartDate,
            r.EndDate,
            r.AverageRecoveryScore,
            r.AverageSleepHours,
            r.TotalWorkoutDuration,
            r.TotalCaloriesBurned,
            r.TotalSteps,
            r.GeneratedAt
        )).ToList();

        return Result.Success(dtos);
    }
}
