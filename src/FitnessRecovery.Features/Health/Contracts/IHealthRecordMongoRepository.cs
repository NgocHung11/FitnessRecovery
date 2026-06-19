using System;
using System.Threading.Tasks;
using FitnessRecovery.Features.Health.Domain;

namespace FitnessRecovery.Features.Health.Contracts;

public interface IHealthRecordMongoRepository
{
    Task UpsertAsync(HealthRecord record);
    Task DeleteAsync(Guid userId, DateOnly date);
}
