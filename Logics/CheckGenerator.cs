using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierApp.Logics
{
    public class CheckGenerator
    {
        private CashierDBEntities _objCashierDbEntities;
        private Check _check { get; set; }

        public CheckGenerator(List<Product> products) 
        {
            _objCashierDbEntities = new CashierDBEntities();
            _check.Products = products;
        }

        public void GenerateCheck()
        {
            _check.OperationTime = DateTime.Now;
            // add to db
            _objCashierDbEntities.Checks.Add(_check);
            _objCashierDbEntities.SaveChanges();
        }

        public void AddCustomerNameToCheck()
        {
            throw new NotImplementedException();
        }

        public decimal CalculateTheSum()
        {
            throw new NotImplementedException();
        }
    }
}
