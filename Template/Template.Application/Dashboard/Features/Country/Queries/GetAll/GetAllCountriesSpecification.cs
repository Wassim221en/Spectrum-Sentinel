using Template.Domain.Specifications;

namespace Template.Dashboard.Country.Queries.GetAll;

public class GetAllCountriesSpecification : Specification<Domain.Entities.Settings.Country>
{
    public GetAllCountriesSpecification(GetAllCountriesQuery.Request request)
    {
        ApplyFilters(c =>
            (request.Search == null || request.Search == "" || c.Name.Contains(request.Search))
        );
    }
}

