using HighSens.Application.DTOs.Auth;

namespace HighSens.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginRequest request);
    }
}