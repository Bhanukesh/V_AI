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
        // Temporary mock data for development
        var mockRestaurants = new List<RestaurantDto>
        {
            new(1, "The Golden Spoon", "123 Main St, Downtown", "+1-555-0123", "contact@goldenspoon.com", "America/New_York", DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, 1),
            new(2, "Sunset Bistro", "456 Oak Ave, Midtown", "+1-555-0456", "info@sunsetbistro.com", "America/New_York", DateTime.UtcNow.AddDays(-20), DateTime.UtcNow, 1),
            new(3, "Corner Cafe", "789 Pine Rd, Uptown", "+1-555-0789", "hello@cornercafe.com", "America/New_York", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, 1)
        };
        
        return Ok(mockRestaurants);
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