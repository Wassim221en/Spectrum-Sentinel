using Template.Domain.Specifications;

namespace Template.Dashboard.Neighborhood.Queries.GetAll;

public class GetAllNeighborhoodsSpecification : Specification<Domain.Entities.Settings.Neighborhood>
{
    public GetAllNeighborhoodsSpecification(GetAllNeighborhoodsQuery.Request request)
    {
        ApplyFilters(n =>
            (request.Search == null || request.Search == "" || n.Name.Contains(request.Search))
            &&
            (!request.CityId.HasValue || n.CityId == request.CityId.Value)
        );
    }
}

