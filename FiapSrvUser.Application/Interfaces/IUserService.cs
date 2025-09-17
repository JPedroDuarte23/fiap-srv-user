using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiapSrvUser.Application.DTOs;
using FiapSrvUser.Domain.Entities;

namespace FiapSrvUser.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);

        Task<IEnumerable<UserDto>> GetAllAsync();

        Task UpdateAsync(Guid id, UpdateUserDto user);

        Task DeleteAsync(Guid id);

        Task<IEnumerable<PlayerDto>> GetPlayersAsync();

        Task<IEnumerable<PublisherDto>> GetPlublishersAsync();
    }
}
