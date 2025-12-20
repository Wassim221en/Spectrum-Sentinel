using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Template.API.Dashboard.Events;
using Template.Application.Events;
using Template.Application.Features.Auth.Commands.ResetPassword;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Events;
using Template.Dashboard.Interfaces;
using Template.Domain.Events.Employee;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Employee.Commands.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand.Request, OperationResponse>
{
    private readonly IAuthService<Domain.Entities.Security.Employee> _authService;
    private readonly UserManager<Domain.Entities.Security.Employee> _userManager;
    private readonly IEventBus _eventBus;

    public ResetPasswordCommandHandler(
        IAuthService<Domain.Entities.Security.Employee> authService,
        UserManager<Domain.Entities.Security.Employee> userManager,
        IEventBus eventBus)
    {
        _authService = authService;
        _userManager = userManager;
        _eventBus = eventBus;
    }

    public async Task<OperationResponse> Handle(ResetPasswordCommand.Request request, CancellationToken cancellationToken)
    {
        // Validate passwords match
        if (request.NewPassword != request.ConfirmPassword)
            return new HttpMessage("Passwords do not match!", HttpStatusCode.BadRequest);

        // Find user by email
        var employee = await _userManager.FindByEmailAsync(request.Email);
        if (employee is null)
            return new HttpMessage("User not found!", HttpStatusCode.NotFound);

        // Hash the provided token and validate it
        var hashedToken = _authService.HashToken(request.ResetToken);
        if (!employee.ValidateResetPasswordToken(hashedToken))
            return new HttpMessage("Invalid or expired reset token!", HttpStatusCode.BadRequest);

        // Remove old password and set new one
        var removePasswordResult = await _userManager.RemovePasswordAsync(employee);
        if (!removePasswordResult.Succeeded)
            return new HttpMessage("Failed to reset password!", HttpStatusCode.InternalServerError);

        var addPasswordResult = await _userManager.AddPasswordAsync(employee, request.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
            return new HttpMessage($"Failed to set new password: {errors}", HttpStatusCode.BadRequest);
        }

        // Clear reset token
        employee.ClearResetPasswordToken();
        await _userManager.UpdateAsync(employee);

        // Publish integration event
        var integrationEvent = new PasswordResetSuccessIntegrationEvent(
            employee.FullName,
            employee.Email!);
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return OperationResponse.Ok();
    }
}

