using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankerMade.Application.Common.Exceptions;
using TankerMade.Contracts.DTOs.Users;
using TankerMade.Contracts.Services;

namespace TankerMade.Application.Services
{
    public class UserService : IUserService
    {
        // TODO: Add constructor with repository dependencies when ready
        // private readonly IUserRepository _userRepository;
        // public UserService(IUserRepository userRepository) => _userRepository = userRepository;

        // Create
        public async Task<UserDto> CreateAsync(CreateUserDto createDto)
        {
            // TODO: Hash password, validate input, save to database
            throw new NotImplementedException("CreateAsync not implemented yet");
        }

        // Read methods
        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            // TODO: Get from database, map to DTO
            throw new NotImplementedException("GetByIdAsync not implemented yet");
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            // TODO: Get from database by username, map to DTO
            throw new NotImplementedException("GetByUsernameAsync not implemented yet");
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            // TODO: Get from database by email, map to DTO
            throw new NotImplementedException("GetByEmailAsync not implemented yet");
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            // TODO: Get all users from database, map to DTOs
            throw new NotImplementedException("GetAllAsync not implemented yet");
        }

        // Update with role protection - SIGNATURE MATCHES INTERFACE
        public async Task<UserDto?> UpdateAsync(UpdateUserDto updateDto, Guid currentUserId, string currentUserRole)
        {
            // Get existing user
            var existingUser = await GetByIdAsync(updateDto.Id);

            if (existingUser == null)
                return null;

            // SECURITY: Only admins can change roles
            if (updateDto.Role != existingUser.Role && currentUserRole != "Admin")
            {
                throw new UnauthorizedException("Only administrators can modify user roles");
            }

            // SECURITY: Users can only update their own profile (unless admin)
            if (updateDto.Id != currentUserId && currentUserRole != "Admin")
            {
                throw new UnauthorizedException("Users can only modify their own profile");
            }

            // TODO: Hash password if changed, update database, map to DTO
            throw new NotImplementedException("UpdateAsync database implementation not complete yet");
        }

        // Delete
        public async Task<bool> DeleteAsync(Guid id)
        {
            // TODO: Delete from database, return success/failure
            throw new NotImplementedException("DeleteAsync not implemented yet");
        }

        // Business operations
        public async Task<bool> ExistsAsync(Guid id)
        {
            // TODO: Check if user exists in database
            throw new NotImplementedException("ExistsAsync not implemented yet");
        }

        public async Task<IEnumerable<UserDto>> SearchAsync(string searchTerm)
        {
            // TODO: Search users by username/email containing searchTerm
            throw new NotImplementedException("SearchAsync not implemented yet");
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            // TODO: Check if username already exists
            throw new NotImplementedException("UsernameExistsAsync not implemented yet");
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            // TODO: Check if email already exists
            throw new NotImplementedException("EmailExistsAsync not implemented yet");
        }
    }
}