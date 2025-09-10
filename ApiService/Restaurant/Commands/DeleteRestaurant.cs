namespace ApiService.Restaurant.Commands;

using MediatR;
using Data;
using Microsoft.EntityFrameworkCore;

public record DeleteRestaurantCommand(int Id) : IRequest;

public class DeleteRestaurantHandler(RestaurantDbContext context) : IRequestHandler<DeleteRestaurantCommand>
{
    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        
        if (restaurant == null)
        {
            throw new KeyNotFoundException($"Restaurant with ID {request.Id} not found.");
        }

        context.Restaurants.Remove(restaurant);
        await context.SaveChangesAsync(cancellationToken);
    }
}