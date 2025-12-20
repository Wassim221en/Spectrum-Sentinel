using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Neighborhood.Queries.GetAll;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Neighborhood.Commands.Add;

public class AddNeighborhoodHandler : IRequestHandler<AddNeighborhoodCommand.Request, OperationResponse<GetAllNeighborhoodsQuery.Response.NeighborhoodRes>>
{
    private readonly IRepository _repository;

    public AddNeighborhoodHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllNeighborhoodsQuery.Response.NeighborhoodRes>> Handle(AddNeighborhoodCommand.Request request, CancellationToken cancellationToken)
    {
        // Verify city exists
        var cityExists = await _repository.Query<Domain.Entities.Settings.City>()
            .AnyAsync(c => c.Id == request.CityId, cancellationToken);

        if (!cityExists)
            return new HttpMessage("City not found", HttpStatusCode.NotFound);

        var neighborhood = new Domain.Entities.Settings.Neighborhood
        {
            Name = request.Name,
            CityId = request.CityId
        };

        await _repository.AddAsync(neighborhood);
        await _repository.SaveChangesAsync(cancellationToken);

        // Get city name for response
        var city = await _repository.Query<Domain.Entities.Settings.City>()
            .FirstAsync(c => c.Id == request.CityId, cancellationToken);

        return new GetAllNeighborhoodsQuery.Response.NeighborhoodRes
        {
            NeighborhoodId = neighborhood.Id,
            Name = neighborhood.Name,
            CityId = neighborhood.CityId,
            CityName = city.Name
        };
    }
}

