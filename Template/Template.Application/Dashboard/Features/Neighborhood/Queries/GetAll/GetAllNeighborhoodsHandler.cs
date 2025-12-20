using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Neighborhood.Queries.GetAll;

public class GetAllNeighborhoodsHandler : IRequestHandler<GetAllNeighborhoodsQuery.Request, OperationResponse<GetAllNeighborhoodsQuery.Response>>
{
    private readonly IRepository _repository;

    public GetAllNeighborhoodsHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllNeighborhoodsQuery.Response>> Handle(GetAllNeighborhoodsQuery.Request request, CancellationToken cancellationToken)
    {
        var specification = new GetAllNeighborhoodsSpecification(request);
        var neighborhoods = await _repository.Query<Domain.Entities.Settings.Neighborhood>()
            .Include(n => n.City)
            .Where(specification.Criteria)
            .Select(GetAllNeighborhoodsQuery.Response.NeighborhoodRes.Selector())
            .ToListAsync(cancellationToken);

        return new GetAllNeighborhoodsQuery.Response
        {
            Count = neighborhoods.Count,
            Neighborhoods = neighborhoods
        };
    }
}

