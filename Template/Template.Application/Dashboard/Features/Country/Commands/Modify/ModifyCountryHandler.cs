using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Country.Queries.GetById;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Country.Commands.Modify;

public class ModifyCountryHandler : IRequestHandler<ModifyCountryCommand.Request, OperationResponse<GetCountryByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public ModifyCountryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetCountryByIdQuery.Response>> Handle(ModifyCountryCommand.Request request, CancellationToken cancellationToken)
    {
        var country = await _repository.TrackingQuery<Domain.Entities.Settings.Country>()
            .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

        if (country is null)
            return new HttpMessage("Country not found", HttpStatusCode.NotFound);

        country.Name = request.Name;

        _repository.Update(country);
        await _repository.SaveChangesAsync(cancellationToken);

        return await _repository.Query<Domain.Entities.Settings.Country>()
            .Where(c => c.Id == request.CountryId)
            .Select(GetCountryByIdQuery.Response.Selector())
            .FirstAsync(cancellationToken);
    }
}

