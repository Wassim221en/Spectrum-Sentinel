using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.City.Queries.GetAll;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.City.Commands.Add;

public class AddCityHandler : IRequestHandler<AddCityCommand.Request, OperationResponse<GetAllCitiesQuery.Response.CityRes>>
{
    private readonly IRepository _repository;

    public AddCityHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllCitiesQuery.Response.CityRes>> Handle(AddCityCommand.Request request, CancellationToken cancellationToken)
    {
        // Verify country exists
        var countryExists = await _repository.Query<Domain.Entities.Settings.Country>()
            .AnyAsync(c => c.Id == request.CountryId, cancellationToken);

        if (!countryExists)
            return new HttpMessage("Country not found", HttpStatusCode.NotFound);

        var city = new Domain.Entities.Settings.City
        {
            Name = request.Name,
            CountryId = request.CountryId
        };

        await _repository.AddAsync(city);
        await _repository.SaveChangesAsync(cancellationToken);

        // Get country name for response
        var country = await _repository.Query<Domain.Entities.Settings.Country>()
            .FirstAsync(c => c.Id == request.CountryId, cancellationToken);

        return new GetAllCitiesQuery.Response.CityRes
        {
            CityId = city.Id,
            Name = city.Name,
            CountryId = city.CountryId,
            CountryName = country.Name
        };
    }
}

