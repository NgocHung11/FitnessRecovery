using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;

public class WeeklyReportDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }

    public double AverageRecoveryScore { get; set; }
    
    public double AverageSleepHours { get; set; }
    
    public int TotalWorkoutDuration { get; set; }
    
    public int TotalCaloriesBurned { get; set; }
    
    public int TotalSteps { get; set; }
    
    public DateTime GeneratedAt { get; set; }
}
