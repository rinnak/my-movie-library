using TVSeriesLibrary.Models;
using TVSeriesLibrary.Data;

namespace TVSeriesLibrary.Service;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}