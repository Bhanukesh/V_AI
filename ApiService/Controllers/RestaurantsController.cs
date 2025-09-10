namespace ApiService.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Commands;
using Restaurant.Queries;
using Restaurant.DTO;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController(IMediator mediator) : ControllerBase
{
    [HttpGet(Name = nameof(GetRestaurants))]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants()
    {
        var restaurants = await mediator.Send(new GetRestaurantsQuery());
        return Ok(restaurants);
    }

    [HttpGet("{id}", Name = nameof(GetRestaurantById))]
    public async Task<ActionResult<RestaurantDto>> GetRestaurantById(int id)
    {
        var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));
        
        if (restaurant == null)
        {
            return NotFound($"Restaurant with ID {id} not found.");
        }
        
        return Ok(restaurant);
    }

    [HttpPost(Name = nameof(CreateRestaurant))]
    public async Task<ActionResult<int>> CreateRestaurant(CreateRestaurantDto restaurant)
    {
        try
        {
            var id = await mediator.Send(new CreateRestaurantCommand(restaurant));
            return CreatedAtAction(nameof(GetRestaurantById), new { id }, id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}", Name = nameof(UpdateRestaurant))]
    public async Task<IActionResult> UpdateRestaurant(int id, UpdateRestaurantDto restaurant)
    {
        try
        {
            await mediator.Send(new UpdateRestaurantCommand(id, restaurant));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}", Name = nameof(DeleteRestaurant))]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        try
        {
            await mediator.Send(new DeleteRestaurantCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/kpis", Name = nameof(GetRestaurantKpis))]
    public async Task<ActionResult<RestaurantKpiDto>> GetRestaurantKpis(int id, [FromQuery] string period = "last_30d")
    {
        var kpis = await mediator.Send(new GetRestaurantKpisQuery(id, period));
        
        if (kpis == null)
        {
            return NotFound($"Restaurant with ID {id} not found.");
        }
        
        return Ok(kpis);
    }
}