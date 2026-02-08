using System;

namespace HighSens.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        string CreateToken(int userId, string userName, string role);
    }
}