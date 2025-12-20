using Template.Dashboard.Core.Response;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.Dashboard.Interfaces;

public interface IAuthService<TUser>
    where TUser:User
{
    Task<string>GenerateAccessToken(TUser user);

    Task<OperationResponse<string>> GenerateRefreshToken(Guid userId, string? oldRefreshToken = null,
        CancellationToken cancellationToken = default);

    Task<OperationResponse<string>> ForgetPasswordAsync(TUser user);
    string GenerateResetToken();
    string HashToken(string token);
}

