using CashierApp.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CashierApp.Logics
{
    public class CheckStringGenerator
    {
        private readonly List<Product> _products;
        private readonly Check _check;

        private const string SumLabel = "Сума:";
        private const string DiscountedSumLabel = "Сума зі знижкою:";
        private const string DateLabel = "Дата:";

        public CheckStringGenerator(List<Product> products, Check check)
        {
            _products = products;
            _check = check;
        }

        public string GenerateCheck()
        {
            var receipt = new StringBuilder();

            // Add the header
            receipt.AppendLine(new string('-', 120));

            // Add the products
            foreach (var product in _products)
            {
                receipt.AppendLine($"{product.Price} х {product.Amount}");
                receipt.AppendLine($"{product.Nomitation}");
                receipt.AppendLine();
            }

            receipt.AppendLine(new string('-', 120));

            // Add the sum
            receipt.AppendLine(new string('-', 120));
            receipt.AppendLine($"{SumLabel} {FormatCurrency(_check.Sum)}");
            receipt.AppendLine($"{DiscountedSumLabel} {FormatCurrency((decimal)_check.SumWithDiscount)}");

            receipt.AppendLine(new string('-', 120));

            // Add the date
            receipt.AppendLine(new string('-', 120));
            receipt.AppendLine($"{DateLabel} {_check.OperationTime}");

            return receipt.ToString();
        }

        private string FormatCurrency(decimal value)
        {
            RegionInfo region = new RegionInfo("UA");
            return $"{value} {region.CurrencySymbol}";
        }
    }
}
