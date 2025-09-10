namespace ApiService.Restaurant.Commands;

using MediatR;
using Data;
using Restaurant.DTO;
using Microsoft.EntityFrameworkCore;

public record UpdateRestaurantCommand(int Id, UpdateRestaurantDto Restaurant) : IRequest;

public class UpdateRestaurantHandler(RestaurantDbContext context) : IRequestHandler<UpdateRestaurantCommand>
{
    public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        
        if (restaurant == null)
        {
            throw new KeyNotFoundException($"Restaurant with ID {request.Id} not found.");
        }

        var dto = request.Restaurant;
        
        restaurant.Name = dto.Name;
        restaurant.Address = dto.Address;
        restaurant.Phone = dto.Phone;
        restaurant.Email = dto.Email;
        restaurant.TimeZoneId = dto.TimeZoneId;
        restaurant.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}