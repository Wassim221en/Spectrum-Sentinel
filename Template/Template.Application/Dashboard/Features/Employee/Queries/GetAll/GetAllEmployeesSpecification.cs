using Template.Domain.Specifications;

namespace Template.Dashboard.Employee.Queries.GetAll;

public class GetAllEmployeesSpecification : Specification<Domain.Entities.Security.Employee>
{
    public GetAllEmployeesSpecification(GetAllEmployeesQuery.Request request)
    {
        ApplyFilters(e =>
            ((request.Search == null || request.Search == "") || 
             (e.FirstName.Contains(request.Search) || 
              e.LastName.Contains(request.Search) || 
              (e.Email ?? "").Contains(request.Search)))
            &&
            (!request.IsActive.HasValue || (request.IsActive.Value ? !e.DateDeleted.HasValue : e.DateDeleted.HasValue))
        );
    }
}

