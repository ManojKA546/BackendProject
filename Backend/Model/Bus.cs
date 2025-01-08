using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Bus{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get;set;}= string.Empty;

    [BsonElement("route_number")]
    public string RouteNumber {get;set;}= string.Empty;

    [BsonElement("start_stop")]
    public string Startstop {get;set;}= string.Empty;

    [BsonElement("end_stop")]
    public string Endstop {get;set;}= string.Empty;

    [BsonElement("stops")]
    public List<string> Stops {get;set;}= new List<string>();


}