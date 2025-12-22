using Template.Domain.Specifications;

namespace Template.Dashboard.Employee.Queries.GetAll;

public class GetAllEmployeesSpecification : Specification<Domain.Entities.Security.Employee>
{
    public GetAllEmployeesSpecification(GetAllEmployeesQuery.Request request)
    {
        ApplyFilters(e =>
            !e.DateDeleted.HasValue&&
            ((request.Search == null || request.Search == "") || 
             (e.FirstName.Contains(request.Search) || 
              e.LastName.Contains(request.Search) || 
              (e.Email ?? "").Contains(request.Search)))
            &&
            (!request.Status.HasValue || (request.Status.Value ==e.Status))
        );
    }
}

