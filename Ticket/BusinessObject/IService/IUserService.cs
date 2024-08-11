using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> LoginAsync(LoginResquestDto loginForm);
        public Task<ServiceResponse<CreateUserDto>> CreateStaff(CreateUserDto userObject);

    }
}
