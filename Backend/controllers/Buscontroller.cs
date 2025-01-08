using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;



[ApiController]
[Route("api/[controller]")]

[Authorize]

public class Buscontroller:ControllerBase
{
 private readonly MongoDBService _mongoDBService;

 public Buscontroller(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }


 [HttpGet]
    public ActionResult<List<Bus>> GetAllBuses()
     {
        return _mongoDBService.GetAllBuses();
    }



[HttpGet("stops/{start_stop}")]
   
   public ActionResult<List<string>>GetBusStops(string start_stop){

    var stops=_mongoDBService.GetBusStops(start_stop);
    if (stops == null || stops.Count == 0)
        {
            return NotFound($"No stops found for start stop: {start_stop}");
        }

        return Ok(stops);

     
   }


[HttpPost("bus")]

public   ActionResult  AddBus( Bus newbus){

    _mongoDBService.AddBus(newbus);

    return Ok("New bus route added");

}

[HttpPut("bus/{startstop}")]

public IActionResult UpdateBus(Bus updatebus, string sstop){
    _mongoDBService.UpdateBus(updatebus,sstop);
    return NoContent();
}

}