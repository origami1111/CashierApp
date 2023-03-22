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
                receipt += $"{product.Price} х {product.Amount}\n";
                receipt += $"{product.Nomitation}\n";
                receipt += $"\n";
            }

            receipt += new string('-', 120) + "\n";

            // Add the sum
            receipt += new string('-', 120) + "\n";
            receipt += $"{"Сума:"} {check.Sum} грн\n";
            receipt += $"{"Сума зі знижкою:"} {check.SumWithDiscount} грн\n";

            receipt += new string('-', 120) + "\n";

            // Add the date
            receipt += new string('-', 120) + "\n";
            receipt += $"{"Дата:"} {check.OperationTime}\n";

            return receipt;
        }
    }
}
