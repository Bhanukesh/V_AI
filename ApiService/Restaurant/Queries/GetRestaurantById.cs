namespace ApiService.Restaurant.Queries;

using MediatR;
using Data;
using Restaurant.DTO;
using Microsoft.EntityFrameworkCore;

public record GetRestaurantByIdQuery(int Id) : IRequest<RestaurantDto?>;

public class GetRestaurantByIdHandler(RestaurantDbContext context) : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto?>
{
    public async Task<RestaurantDto?> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Restaurants
            .Where(r => r.Id == request.Id)
            .Select(r => new RestaurantDto(
                r.Id,
                r.Name,
                r.Address,
                r.Phone,
                r.Email,
                r.TimeZoneId,
                r.CreatedAt,
                r.UpdatedAt,
                r.OrganizationId
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}