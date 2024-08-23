using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface IAuthenticationService
    {
        Task<TokenResponse<string>> LoginAsync(LoginResquestDto loginForm);
        public Task<TokenResponse<ResetPassDTO>> ResetPass(ResetPassDTO dto);
    }
}
