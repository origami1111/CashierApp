using CashierApp.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CashierApp
{
    public class UserController
    {
        private readonly CashierDBEntities _dbContext;

        public UserController(CashierDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(model => model.Username == username);
        }
    }
}
