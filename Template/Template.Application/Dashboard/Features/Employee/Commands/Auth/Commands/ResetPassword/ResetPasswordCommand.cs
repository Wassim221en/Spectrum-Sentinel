using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand()
{
    public record Request : IRequest<OperationResponse>
    {
        public string Email { get; set; }
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

