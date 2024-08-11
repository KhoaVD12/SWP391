using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse<string>> LoginAsync(LoginResquestDto loginForm);
        public Task<ServiceResponse<ResetPassDTO>> ResetPass(ResetPassDTO dto);
    }
}
