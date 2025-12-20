using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.City.Queries.GetAll;

public class GetAllCitiesHandler : IRequestHandler<GetAllCitiesQuery.Request, OperationResponse<GetAllCitiesQuery.Response>>
{
    private readonly IRepository _repository;

    public GetAllCitiesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllCitiesQuery.Response>> Handle(GetAllCitiesQuery.Request request, CancellationToken cancellationToken)
    {
        var specification = new GetAllCitiesSpecification(request);
        var cities = await _repository.Query<Domain.Entities.Settings.City>()
            .Include(c => c.Country)
            .Where(specification.Criteria)
            .Select(GetAllCitiesQuery.Response.CityRes.Selector())
            .ToListAsync(cancellationToken);

        return new GetAllCitiesQuery.Response
        {
            Count = cities.Count,
            Cities = cities
        };
    }
}

