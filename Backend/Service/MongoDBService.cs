using MongoDB.Driver;

public class MongoDBService{

    private readonly IMongoCollection<Bus> _busCollection; 
    private readonly IMongoCollection<User> _userCollection;

    public MongoDBService(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("mongodb"));
        var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        _busCollection = database.GetCollection<Bus>(configuration["DatabaseSettings:BusCollectionName"]);
        _userCollection = database.GetCollection<User>(configuration["DatabaseSettings:UserCollectionName"]);
    }

    public List<Bus> GetAllBuses() => _busCollection.Find(bus => true).ToList();

    public List<string> GetBusStops(string start_stop){

        var stops=_busCollection
                 .Find(bus => bus.Startstop == start_stop)
                 // Filter by start stop
                 .Project(bus => bus.Stops) 
                 .ToList()                                // Fetch the matched documents
                 .SelectMany(stops => stops)         // Flatten all stop lists
                .Distinct()                            // Avoid duplicate stop names
                .ToList();                               // Convert to List<string>

        return stops;
    }

    public User? GetUserByUsername(string username){

        var filter = Builders<User>.Filter.Eq(u => u.Username, username);
        return _userCollection.Find(filter).FirstOrDefault();
 
    }

   public void AddUser(User newUser) => _userCollection.InsertOne(newUser);

   public void AddBus(Bus newbus)=> _busCollection.InsertOne(newbus);

   public void UpdateBus(Bus updatebus,string sstop)=>_busCollection.ReplaceOne(bus=>bus.Endstop==sstop, updatebus);
     
   


}