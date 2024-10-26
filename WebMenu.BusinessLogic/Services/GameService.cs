using WebMenu.BusinessLogic.Interfaces;
using WebMenu.DataAccess.Interfaces;
using Web_Menu.Models;
using FluentValidation;
using FluentValidation.Results;
using WebMenu.BusinessLogic.Validators;

namespace WebMenu.BusinessLogic.Services
{
    public class GameService : IGameService
    {
        private readonly IRepository<Game> _gameRepository;

        public GameService(IRepository<Game> gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _gameRepository.FindAsync(g => true, includeProperties: "Characters");
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            var games = await _gameRepository.FindAsync(g => g.Id == id, includeProperties: "Characters");
            return games.FirstOrDefault();
        }

        public async Task CreateGameAsync(Game game)
        {
            var validator = new GameValidator();
            ValidationResult results = validator.Validate(game);

            if (!results.IsValid)
            {
                throw new ValidationException(results.Errors);
            }

            await _gameRepository.InsertAsync(game);
        }

        public async Task UpdateGameAsync(Game game)
        {
            var validator = new GameValidator();
            ValidationResult results = validator.Validate(game);

            if (!results.IsValid)
            {
                throw new ValidationException(results.Errors);
            }

            await _gameRepository.UpdateAsync(game);
        }

        public async Task DeleteGameAsync(int id)
        {
            await _gameRepository.DeleteAsync(id);
        }
    }
}
