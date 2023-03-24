using CashierApp.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CashierApp.Logics
{
    public class ProductController
    {
        private CashierDBEntities _dbContext;

        public ProductController(CashierDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> GetProductAsync(string barcode)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(model => model.Barcode == barcode);
        }
    }
}
