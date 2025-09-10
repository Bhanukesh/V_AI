namespace ApiService.Restaurant.Queries;

using MediatR;
using Data;
using Restaurant.DTO;
using Microsoft.EntityFrameworkCore;

public record GetRestaurantsQuery : IRequest<IEnumerable<RestaurantDto>>;

public class GetRestaurantsHandler(RestaurantDbContext context) : IRequestHandler<GetRestaurantsQuery, IEnumerable<RestaurantDto>>
{
    public async Task<IEnumerable<RestaurantDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
    {
        return await context.Restaurants
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
            .ToListAsync(cancellationToken);
    }
}