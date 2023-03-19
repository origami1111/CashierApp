using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierApp
{
    public class UserController
    {
        private CashierDBEntities _objCashierDbEntites;

        public UserController()
        {
            _objCashierDbEntites = new CashierDBEntities();
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await Task.Run(() => _objCashierDbEntites.Users.SingleOrDefault(model => model.Username == username));
        }
    }
}
