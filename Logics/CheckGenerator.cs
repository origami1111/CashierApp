using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashierApp.Logics
{
    public class CheckGenerator
    {
        private readonly List<Product> _products;

        public CheckGenerator(List<Product> products)
        {
            _products = products;
        }

        public Check GenerateCheck(Card card)
        {
            using (CashierDBEntities cashierDbEntities = new CashierDBEntities())
            {
                int? userId = card?.UserId;
                decimal discount = card?.Discount ?? 0.0m;

                var newCheck = new Check()
                {
                    OperationTime = DateTime.Now,
                    UserId = userId,
                    Sum = CalculateSum(),
                    SumWithDiscount = CalculateSumWithDiscount(discount)
                };

                cashierDbEntities.Checks.Add(newCheck);
                cashierDbEntities.SaveChanges();

                return newCheck;
            }
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
