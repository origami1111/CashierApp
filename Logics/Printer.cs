using System;
using System.Drawing;
using System.Drawing.Printing;

namespace CashierApp.Logics
{
    public class Printer
    {
        private readonly PrintDocument _printDocument;
        private readonly string _textToPrint;

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
