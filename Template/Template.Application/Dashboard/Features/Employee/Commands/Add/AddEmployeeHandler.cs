using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.API.Dashboard.Events;
using Template.Dashboard.Common.Interfaces;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Queries.GetAll;
using Template.Dashboard.Events;
using Template.Domain.Events.Employee;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Employee.Commands.Add;

public class AddEmployeeHandler : IRequestHandler<AddEmployeeCommand.Request, OperationResponse<GetAllEmployeesQuery.Response.EmployeeRes>>
{
    private readonly UserManager<Domain.Entities.Security.Employee> _userManager;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IFileService _fileService;

    public AddEmployeeHandler(
        UserManager<Domain.Entities.Security.Employee> userManager,
        IRepository repository,
        IEventBus eventBus, IFileService fileService)
    {
        _userManager = userManager;
        _repository = repository;
        _eventBus = eventBus;
        _fileService = fileService;
    }

    public async Task<OperationResponse<GetAllEmployeesQuery.Response.EmployeeRes>> Handle(AddEmployeeCommand.Request request, CancellationToken cancellationToken)
    {
        var isUserNameExists = await _repository.Query<Domain.Entities.Security.Employee>()
            .AnyAsync(e => e.UserName == request.UserName, cancellationToken: cancellationToken);
        if(isUserNameExists)
            return new HttpMessage("UserName Already Exists", HttpStatusCode.BadRequest);
        if(request.Email is not null && await _repository.Query<Domain.Entities.Security.Employee>()
            .AnyAsync(e => e.Email == request.Email, cancellationToken: cancellationToken))
            return new HttpMessage("Email Already Exists", HttpStatusCode.BadRequest);
        if (await _repository.Query<Domain.Entities.Security.Employee>()
                .AnyAsync(e => e.PhoneNumber == request.PhoneNumber, cancellationToken: cancellationToken))
            return new HttpMessage("PhoneNumber Already Exists", HttpStatusCode.BadRequest);
        var employee = new Domain.Entities.Security.Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
        };
        var identityResult = await _userManager.CreateAsync(employee, request.Password);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return new HttpMessage($"Adding employee failed: {errors}", HttpStatusCode.BadRequest);
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
        
        var integrationEvent = new EmployeeCreatedIntegrationEvent(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email!,
            employee.PhoneNumber);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return new GetAllEmployeesQuery.Response.EmployeeRes
        {
            EmployeeId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = employee.FullName,
            UserName = employee.UserName,
            Email = employee.Email ?? "",
            PhoneNumber = employee.PhoneNumber ?? "",
            IsActive = true
        };
    }
}

