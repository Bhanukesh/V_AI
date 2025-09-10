namespace ApiService.Restaurant.Commands;

using MediatR;
using Data;
using Restaurant.DTO;

public record CreateRestaurantCommand(CreateRestaurantDto Restaurant) : IRequest<int>;

public class CreateRestaurantHandler(RestaurantDbContext context) : IRequestHandler<CreateRestaurantCommand, int>
{
    public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Restaurant;
        
        var entity = new Data.Restaurant
        {
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            TimeZoneId = dto.TimeZoneId,
            OrganizationId = dto.OrganizationId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Restaurants.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}