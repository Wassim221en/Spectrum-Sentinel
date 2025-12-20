using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Common.Interfaces;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Queries.GetById;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Employee.Commands.Modify;

public class ModifyEmployeeHandler : IRequestHandler<ModifyEmployeeCommand.Request, OperationResponse<GetEmployeeByIdQuery.Response>>
{
    private readonly UserManager<Domain.Entities.Security.Employee> _userManager;
    private readonly IRepository _repository;
    private readonly IFileService _fileService;
    public ModifyEmployeeHandler(UserManager<Domain.Entities.Security.Employee> userManager, IRepository repository, IFileService fileService)
    {
        _userManager = userManager;
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<OperationResponse<GetEmployeeByIdQuery.Response>> Handle(ModifyEmployeeCommand.Request request, CancellationToken cancellationToken)
    {
        var employee = await _repository.TrackingQuery<Domain.Entities.Security.Employee>()
            .Include(e => e.UserRoles)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee is null)
            return new HttpMessage("Employee not found", HttpStatusCode.NotFound);
        string? imageUrl=null;
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.UserName = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        
        var identityResult = await _userManager.UpdateAsync(employee);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return new HttpMessage($"Updating employee failed: {errors}", HttpStatusCode.BadRequest);
        }
        
        var currentRoles = await _userManager.GetRolesAsync(employee);
        var rolesToRemove = currentRoles.ToList();
        
        if (rolesToRemove.Any())
        {
            identityResult = await _userManager.RemoveFromRolesAsync(employee, rolesToRemove);
            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                return new HttpMessage($"Removing roles failed: {errors}", HttpStatusCode.InternalServerError);
            }
        }
        if (request.RoleIds.Any())
        {
            var roles = await _repository.Query<Domain.Primitives.Entity.Identity.Role>()
                .Where(r => request.RoleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            foreach (var role in roles)
            {
                identityResult = await _userManager.AddToRoleAsync(employee, role.Name!);
                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return new HttpMessage($"Adding role failed: {errors}", HttpStatusCode.InternalServerError);
                }
            }
        }
        
        return await _repository.Query<Domain.Entities.Security.Employee>()
            .Include(e => e.UserRoles)
            .Where(e => e.Id == request.EmployeeId)
            .Select(GetEmployeeByIdQuery.Response.Selector())
            .FirstAsync(cancellationToken);
    }
}

