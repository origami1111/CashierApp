using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace CashierApp.Logics
{
    public class CheckGenerator
    {
        private readonly List<Product> _products;

        public CheckGenerator(List<Product> products)
        {
            _products = products;
        }

        public Check CheckGenerate(Card card)
        {
            using (CashierDBEntities cashierDbEntities = new CashierDBEntities())
            {
                int? userId = card == null ? null : card.UserId;
                decimal discount = card == null ? 0.0m : card.Discount;

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
            return _products.Sum(product => product.Price);
        }

        public decimal CalculateSumWithDiscount(decimal discount)
        {
            decimal sum = CalculateSum();
            return sum * (1 - discount);
        }
    }
}
