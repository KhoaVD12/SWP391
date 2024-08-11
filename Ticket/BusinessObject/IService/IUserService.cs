using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService;

public interface IUserService
{
    Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsers(int page, int pageSize, string search, string sort);
    Task<ServiceResponse<CreateUserDto>> CreateUserAsync(CreateUserDto userObject);
    Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsersByStaff(int page, int pageSize, string search, string sort);
    Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsersBySponsor(int page, int pageSize, string search, string sort);
    Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsersByOrganizer(int page, int pageSize, string search, string sort);
    Task<ServiceResponse<UserDTO>> GetUserById(int id);
    Task<ServiceResponse<string>> UpdateUser(UserUpdateDTO userUpdate);
    Task<ServiceResponse<UserDTO>> ChangeStatusCollection(int userId,
        UserStatusDTO statusReq);

}