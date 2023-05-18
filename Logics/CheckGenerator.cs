using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashierApp.Logics
{
    public class CheckGenerator
    {
        private readonly List<Product> _products;
        private readonly CashierDBEntities _dbContext;

        public CheckGenerator(CashierDBEntities dbContext, List<Product> products)
        {
            _products = products;
            _dbContext = dbContext;
        }

        public Check GenerateCheck(Card card)
        {
            var newCheck = new Check();
            decimal discountPercentage = 0.0m;

            if (card != null)
            {
                int? userId = card.UserId;
                discountPercentage = card.Discount;

                newCheck = new Check()
                {
                    OperationTime = DateTime.Now,
                    UserId = userId,
                    Sum = CalculateSum(),
                    SumWithDiscount = CalculateSumWithDiscount(discountPercentage)
                };

                Discount discount = new Discount(_dbContext, card);
                discount.CalculateDiscount();

                _dbContext.Checks.Add(newCheck);
                _dbContext.SaveChanges();
            }
            else
            {
                newCheck = new Check()
                {
                    OperationTime = DateTime.Now,
                    Sum = CalculateSum(),
                    SumWithDiscount = CalculateSumWithDiscount(discountPercentage)
                };

                _dbContext.Checks.Add(newCheck);
                _dbContext.SaveChanges();
            }

            return newCheck;
        }

        public decimal CalculateSum()
        {
            return Math.Round(_products.Sum(product => product.Price), 2);
        }

        public decimal CalculateSumWithDiscount(decimal discount)
        {
            decimal sum = CalculateSum();
            return Math.Round(sum * (1 - discount), 2);
        }
    }
}
