using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashierApp.Logics
{
    public class CheckGenerator
    {
        private readonly CashierDBEntities _objCashierDbEntities;
        private Check _check { get; set; }

        public CheckGenerator(List<Product> products)
        {
            _objCashierDbEntities = new CashierDBEntities();
            _check = new Check();
            _check.Products = products;
        }

        public void GenerateCheck()
        {
            _check.OperationTime = DateTime.Now;
            // add to db
            _objCashierDbEntities.Checks.Add(_check);
            _objCashierDbEntities.SaveChanges();
        }

        public decimal GetSum()
        {
            _check.Sum = CalculateTheSum();
            return _check.Sum;
        }

        public decimal GetDiscount()
        {
            return _check.Discount;
        }

        public bool AddCustomerToCheck(string cardNumber)
        {
            var objCard = _objCashierDbEntities.Cards.SingleOrDefault(model => model.CardNumber == cardNumber);

            if (objCard == null)
            {
                return false;
            }

            var objUser = _objCashierDbEntities.Users.Single(model => model.Id == objCard.UserId);
            _check.UserId = objUser.Id;
            _check.Discount = objCard.Discount;

            return true;
        }

        private decimal CalculateTheSum()
        {
            decimal sum = 0;

            if (_check.User == null || _check.Discount == 00.0m)
            {
                foreach (var item in _check.Products)
                {
                    sum += item.Price;
                }
            }
            else
            {
                foreach (var item in _check.Products)
                {
                    sum += item.Price;
                }

                sum = (sum / 100) * _check.Discount;
            }

            _check.Sum = sum;
            return sum;
        }
    }
}
