using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.City.Queries.GetById;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.City.Commands.Modify;

public class ModifyCityHandler : IRequestHandler<ModifyCityCommand.Request, OperationResponse<GetCityByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public ModifyCityHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetCityByIdQuery.Response>> Handle(ModifyCityCommand.Request request, CancellationToken cancellationToken)
    {
        var city = await _repository.TrackingQuery<Domain.Entities.Settings.City>()
            .FirstOrDefaultAsync(c => c.Id == request.CityId, cancellationToken);

        if (city is null)
            return new HttpMessage("City not found", HttpStatusCode.NotFound);

        // Verify country exists
        var countryExists = await _repository.Query<Domain.Entities.Settings.Country>()
            .AnyAsync(c => c.Id == request.CountryId, cancellationToken);

        if (!countryExists)
            return new HttpMessage("Country not found", HttpStatusCode.NotFound);

        city.Name = request.Name;
        city.CountryId = request.CountryId;

        _repository.Update(city);
        await _repository.SaveChangesAsync(cancellationToken);

        return await _repository.Query<Domain.Entities.Settings.City>()
            .Include(c => c.Country)
            .Where(c => c.Id == request.CityId)
            .Select(GetCityByIdQuery.Response.Selector())
            .FirstAsync(cancellationToken);
    }
}

