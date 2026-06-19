using System;
using System.Threading.Tasks;
using FitnessRecovery.Features.Recovery.Domain;

namespace FitnessRecovery.Features.Recovery.Contracts;

public interface IRecoveryAnalysisMongoRepository
{
    Task UpsertAsync(RecoveryAnalysis analysis);
}
