using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierApp.Logics
{
    public class Printer
    {
        private PrintDocument _printDocument;
        private string _textToPrint;

        public Printer(string textToPrint)
        {
            _textToPrint = textToPrint;
            _printDocument = new PrintDocument();
            _printDocument.DocumentName = DateTime.Now.ToString();
            _printDocument.PrintPage += new PrintPageEventHandler(PrintPageHandler);
        }

        public void Print()
        {
            _printDocument.Print();
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(_textToPrint, new Font("Arial", 14), Brushes.Black, 10, 10);
        }
    }
}
