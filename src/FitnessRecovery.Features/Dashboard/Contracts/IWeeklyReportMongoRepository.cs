using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessRecovery.Features.Dashboard.Domain;

namespace FitnessRecovery.Features.Dashboard.Contracts;

public interface IWeeklyReportMongoRepository
{
    Task UpsertAsync(WeeklyReport report);
    Task<List<WeeklyReport>> GetByUserIdAsync(Guid userId);
}
