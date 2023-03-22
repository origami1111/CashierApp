using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierApp.Logics
{
    public class CheckStringGenerator
    {
        private readonly List<Product> products;
        private readonly Check check;

        public CheckStringGenerator(List<Product> products, Check check)
        {
            this.products = products;
            this.check = check;
        }

        public string GenerateCheck()
        {
            string receipt = "";

            // Add the header
            receipt += new string('-', 120) + "\n";

            // Add the products
            foreach (var product in products)
            {
                receipt += $"{product.Price,10} х {product.Amount,10}\n";
                receipt += $"{product.Nomitation, -20}\n";
                receipt += $"\n";
            }

            receipt += new string('-', 120) + "\n";

            // Add the sum
            receipt += new string('-', 120) + "\n";
            receipt += $"{"Сума:", 10} {check.Sum, 10} грн\n";
            receipt += $"{"Сума зі знижкою:", 10} {check.SumWithDiscount, 10} грн\n";

            receipt += new string('-', 120) + "\n";

            // Add the date
            receipt += new string('-', 120) + "\n";
            receipt += $"{"Дата:", 20} {check.OperationTime}\n";

            return receipt;
        }
    }
}
