using CashierApp.Models;
using System.Data.Entity.Migrations;
using System.Linq;

namespace CashierApp.Logics
{
    public class Discount
    {
        private readonly CashierDBEntities _dbContext;
        private readonly Card _card;

        public Discount(CashierDBEntities dbContext, Card card)
        {
            _dbContext = dbContext;
            _card = card;
        }

        public void CalculateDiscount()
        {
            decimal sumOfChecks = _dbContext.Checks.Where(model => model.UserId == _card.UserId)
                .Sum(model => model.Sum);

            var currentDiscount = _card.Discount;

            if (sumOfChecks >= 5000 && sumOfChecks <= 4999)
            {
                currentDiscount = 0.05m;
            }
            else if (sumOfChecks >= 10000 && sumOfChecks <= 9999)
            {
                currentDiscount = 0.1m;
            }
            else if (sumOfChecks >= 15000 && sumOfChecks <= 14999)
            {
                currentDiscount = 0.15m;
            }

            _card.Discount = currentDiscount;
            _dbContext.Cards.AddOrUpdate(_card);
            _dbContext.SaveChanges();
        }
    }
}
