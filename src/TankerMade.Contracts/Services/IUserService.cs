using TankerMade.Contracts.DTOs.Users;

namespace TankerMade.Contracts.Services
{
    public interface IUserService
    {

        // Create
        Task<UserDto> CreateAsync(CreateUserDto createDto);

        // Read
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllAsync();

        // Update
        Task<UserDto?> UpdateAsync(UpdateUserDto updateDto, Guid currentUserId, string currentUserRole);

        // Delete
        Task<bool> DeleteAsync(Guid id);

        // Additional business operations
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<UserDto>> SearchAsync(string searchTerm);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);       
    }
}