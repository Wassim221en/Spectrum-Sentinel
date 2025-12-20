using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Country.Queries.GetAll;

public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery.Request, OperationResponse<GetAllCountriesQuery.Response>>
{
    private readonly IRepository _repository;

    public GetAllCountriesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllCountriesQuery.Response>> Handle(GetAllCountriesQuery.Request request, CancellationToken cancellationToken)
    {
        var specification = new GetAllCountriesSpecification(request);
        var countries = await _repository.Query<Domain.Entities.Settings.Country>()
            .Where(specification.Criteria)
            .Select(GetAllCountriesQuery.Response.CountryRes.Selector())
            .ToListAsync(cancellationToken);

        return new GetAllCountriesQuery.Response
        {
            Count = countries.Count,
            Countries = countries
        };
    }
}

