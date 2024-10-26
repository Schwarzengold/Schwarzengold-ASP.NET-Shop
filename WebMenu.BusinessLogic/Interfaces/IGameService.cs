using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web_Menu.Models;

namespace WebMenu.BusinessLogic.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<Game> GetGameByIdAsync(int id);
        Task CreateGameAsync(Game game);
        Task UpdateGameAsync(Game game);
        Task DeleteGameAsync(int id);
    }
}
