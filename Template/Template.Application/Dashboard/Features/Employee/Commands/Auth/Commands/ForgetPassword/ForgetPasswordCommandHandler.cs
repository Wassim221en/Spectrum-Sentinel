using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Template.API.Dashboard.Events;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Events;
using Template.Dashboard.Interfaces;
using Template.Domain.Events.Employee;
using Template.Domain.Exceptions.Http;

namespace Template.API.Employee.Commands.Auth.Commands.ForgetPassword;

public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand.Request,OperationResponse>
{
    private readonly IAuthService<Domain.Entities.Security.Employee> _authService;
    private readonly UserManager<Domain.Entities.Security.Employee> _userManager;
    private readonly IEventBus _eventBus;
    private readonly IWhatsAppService _whatsAppService;

    public ForgetPasswordCommandHandler(IAuthService<Domain.Entities.Security.Employee> authService, UserManager<Domain.Entities.Security.Employee> userManager, IEventBus eventBus, IWhatsAppService whatsAppService)
    {
        _authService = authService;
        _userManager = userManager;
        _eventBus = eventBus;
        _whatsAppService = whatsAppService;
    }

    public async Task<OperationResponse> Handle(ForgetPasswordCommand.Request request, CancellationToken cancellationToken)
    {
        var employee=await _userManager.FindByEmailAsync(request.Email);
        if (employee is null)
            return new HttpMessage("User Not Found!", HttpStatusCode.NotFound);
        var resetToken =  _authService.GenerateResetToken();
        employee.SetResetPasswordToken( _authService.HashToken(resetToken),DateTime.UtcNow.AddHours(1));
        await _userManager.UpdateAsync(employee);
        var forgetPasswordIntegrationEvent=new ForgetPasswordIntegrationEvent(employee.FullName,resetToken,employee.Email!);
        await _whatsAppService.SendMessageAsync(employee.PhoneNumber??"", resetToken);
        await _eventBus.PublishAsync(forgetPasswordIntegrationEvent, cancellationToken);
        return OperationResponse.Ok();
    }
}

