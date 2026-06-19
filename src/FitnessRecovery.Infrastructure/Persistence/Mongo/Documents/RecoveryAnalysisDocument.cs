using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;

public class RecoveryAnalysisDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public DateOnly AnalysisDate { get; set; }

    public int RecoveryScore { get; set; }
    
    public string RecoveryStatus { get; set; } = string.Empty;
    
    public int SleepScore { get; set; }
    
    public int HeartRateScore { get; set; }
    
    public int WorkoutLoadScore { get; set; }
    
    public int ActivityScore { get; set; }
    
    public DateTime GeneratedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
