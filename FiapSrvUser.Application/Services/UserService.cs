using FiapSrvUser.Application.DTOs;
using FiapSrvUser.Application.Exceptions;
using FiapSrvUser.Application.Interfaces;
using FiapSrvUser.Application.Interfaces.Repositories;
using FiapSrvUser.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FiapSrvUser.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository repo, ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Buscando usuário com ID {UserId}", id);

            var existingUser = await _repo.GetByIdAsync(id);

            if (existingUser == null)
            {
                _logger.LogWarning("Usuário com ID {UserId} não encontrado", id);
                throw new NotFoundException("Usuário não encontrado");
            }

            UserDto dto = null;

            if (existingUser is Player player)
            {
                dto = new UserDto(player);
            }
            else if (existingUser is Publisher publisher)
            {
                dto = new UserDto(publisher);
            }

            _logger.LogInformation("Usuário com ID {UserId} retornado com sucesso", id);

            return dto;
        }

        public async Task UpdateAsync(Guid id, UpdateUserDto dto)
        {
            _logger.LogInformation("Iniciando atualização do usuário {UserId}", id);

            var existingUser = await _repo.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("Usuário {UserId} não encontrado para atualização", id);
                throw new NotFoundException("Usuário não encontrado");
            }

            existingUser.Name = dto.Name;
            existingUser.Username = dto.UserName;

            try
            {
                await _repo.UpdateAsync(existingUser);
                _logger.LogInformation("Usuário {UserId} atualizado com sucesso", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário {UserId}", id);
                throw new ModifyDatabaseException(ex.Message);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Iniciando exclusão do usuário {UserId}", id);

            var existingUser = await _repo.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning("Usuário {UserId} não encontrado para exclusão", id);
                throw new NotFoundException("Usuário não encontrado");
            }

            try
            {
                await _repo.DeleteAsync(id);
                _logger.LogInformation("Usuário {UserId} excluído com sucesso", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário {UserId}", id);
                throw new ModifyDatabaseException(ex.Message);
            }
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersAsync()
        {
            _logger.LogInformation("Buscando todos os jogadores");

            var auxList = (await _repo.GetPlayersAsync()).ToList();
            var playersList = auxList.Select(p => new PlayerDto(p)).ToList();

            _logger.LogInformation("{Count} jogadores encontrados", playersList.Count);

            return playersList;
        }

        public async Task<IEnumerable<PublisherDto>> GetPlublishersAsync()
        {
            _logger.LogInformation("Buscando todos os publicadores");

            var auxList = (await _repo.GetPublishersAsync()).ToList();
            var publishersList = auxList.Select(p => new PublisherDto(p)).ToList();

            _logger.LogInformation("{Count} publicadores encontrados", publishersList.Count);

            return publishersList;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todos os usuários");

            var auxList = (await _repo.GetAllAsync()).ToList();
            var userList = new List<UserDto>();

            foreach (var user in auxList)
            {
                if (user is Player player)
                    userList.Add(new UserDto(player));
                else if (user is Publisher publisher)
                    userList.Add(new UserDto(publisher));
            }

            _logger.LogInformation("{Count} usuários encontrados", userList.Count);

            return userList;
        }
    }
}
