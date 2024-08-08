using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> LoginAsync(LoginResquestDto loginform);
        public Task<ServiceResponse<CreateUserDto>> CreateStaff(CreateUserDto userObject);

    }
}
