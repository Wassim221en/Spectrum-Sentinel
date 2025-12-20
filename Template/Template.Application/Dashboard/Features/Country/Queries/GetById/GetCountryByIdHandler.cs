using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Country.Queries.GetById;

public class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery.Request, OperationResponse<GetCountryByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public GetCountryByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetCountryByIdQuery.Response>> Handle(GetCountryByIdQuery.Request request, CancellationToken cancellationToken)
    {
        var country = await _repository.Query<Domain.Entities.Settings.Country>()
            .Where(c => c.Id == request.Id)
            .Select(GetCountryByIdQuery.Response.Selector())
            .FirstOrDefaultAsync(cancellationToken);

        if (country is null)
            return new HttpMessage("Country not found", HttpStatusCode.NotFound);

        return country;
    }
}

