
using Template.Domain.Specifications;

namespace Template.Dashboard.Role.Queries.GetAll;

public class GetAllRolesSpecification:Specification<Domain.Primitives.Entity.Identity.Role>
{
    public GetAllRolesSpecification(GetAllRolesQuery.Request request)
    {
        ApplyFilters(r=>
            ((request.Search==null ||request.Search=="")||(r.Name??"").Contains(request.Search))
        &&
        (!request.IsActive.HasValue||r.IsActive == request.IsActive)
        );
    }
}