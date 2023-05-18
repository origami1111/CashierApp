using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for PayWindow.xaml
    /// </summary>
    public partial class PayWindow : Window
    {
        private readonly CashierDBEntities _dbContext;
        private readonly List<Product> _products;
        private readonly CheckGenerator _checkGenerator;
        private readonly BarcodeReader _barcodeReader;
        private Check _createdCheck;
        private Card _card;

        public PayWindow()
        {
            InitializeComponent();
        }

        public PayWindow(List<Product> products)
        {
            InitializeComponent();
            HideErrorMessage();
            _barcodeReader = new BarcodeReader(BarcodeNumberTextBox);
            _dbContext = new CashierDBEntities();
            _products = products;
            _checkGenerator = new CheckGenerator(_dbContext, products);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToPayTextBlock.Text = _checkGenerator.CalculateSum().ToString();
            DiscountTextBlock.Text = ToPayTextBlock.Text;

            _barcodeReader.Start("@device:pnp:\\\\?\\usb#vid_13d3&pid_56a2&mi_00#7&c91c3a9&0&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\\global");
        }

        private void AddDiscountCardButton_Click(object sender, RoutedEventArgs e)
        {
            string cardNumber = BarcodeNumberTextBox.Text;
            HideErrorMessage();

            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 13)
            {
                ShowErrorMessage("Введіть штрих код.");
                return;
            }

            _card = _dbContext.Cards.SingleOrDefault(card => card.CardNumber == cardNumber);

            if (_card == null)
            {
                ShowErrorMessage("Номер картки не знайдений.");
                ClearField();
                return;
            }

            DiscountTextBlock.Text = _checkGenerator.CalculateSumWithDiscount(_card.Discount).ToString();
            HideErrorMessage();
            BarcodeNumberTextBox.Text = string.Empty;
        }

        private async void GenerateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            _createdCheck = _checkGenerator.GenerateCheck(_card);

            CheckStringGenerator checkStringGenerator = new CheckStringGenerator(_products, _createdCheck);
            string textToPrint = checkStringGenerator.GenerateCheck();

            Printer printer = new Printer(textToPrint);

            try
            {
                await Task.Run(() => printer.Print());
            }
            catch (Exception ex)
            {
                // Handle the exception
                ShowErrorMessage($"Failed to print the receipt: {ex.Message}");
                return;
            }

            await Task.Delay(10000);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _barcodeReader.Stop();
        }

        private void ClearField()
        {
            BarcodeNumberTextBox.Text = string.Empty;
        }

        private void ShowErrorMessage(string errorMessage)
        {
            ErrorImage.Visibility = Visibility.Visible;
            ErrorTextBlock.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = errorMessage;
        }

        private void HideErrorMessage()
        {
            ErrorImage.Visibility = Visibility.Hidden;
            ErrorTextBlock.Visibility = Visibility.Hidden;
            ErrorTextBlock.Text = string.Empty;
        }

        private void BarcodeNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Length > 13)
            {
                textBox.Text = textBox.Text.Substring(0, 13);
            }
        }

        private void BarcodeNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, является ли вводимый символ цифрой
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true; // Отменяем ввод символа
            }
        }
    }
}
