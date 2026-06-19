using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessRecovery.Infrastructure.Persistence.Mongo.Documents;

public class HealthRecordDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public DateOnly RecordDate { get; set; }

    public double SleepHours { get; set; }
    
    public string SleepQuality { get; set; } = string.Empty;
    
    public int RestingHeartRate { get; set; }
    
    public int AverageHeartRate { get; set; }
    
    public int Steps { get; set; }
    
    public double Weight { get; set; }
    
    public int CaloriesBurned { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
