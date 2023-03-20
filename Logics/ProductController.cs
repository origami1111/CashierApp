using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierApp.Logics
{
    public class ProductController
    {
        private CashierDBEntities _objCashierDbEntites;

        public ProductController()
        {
            _objCashierDbEntites= new CashierDBEntities();
        }

        public async Task<Product> GetProductAsync(string barcode)
        {
            return await Task.Run(() => _objCashierDbEntites.Products.SingleOrDefault(model => model.Barcode == barcode));
        }
    }
}
