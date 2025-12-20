using Template.Domain.Specifications;

namespace Template.Dashboard.City.Queries.GetAll;

public class GetAllCitiesSpecification : Specification<Domain.Entities.Settings.City>
{
    public GetAllCitiesSpecification(GetAllCitiesQuery.Request request)
    {
        ApplyFilters(c =>
            (request.Search == null || request.Search == "" || c.Name.Contains(request.Search))
            &&
            (!request.CountryId.HasValue || c.CountryId == request.CountryId.Value)
        );
    }
}

