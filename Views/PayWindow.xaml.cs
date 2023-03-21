using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for PayWindow.xaml
    /// </summary>
    public partial class PayWindow : Window
    {
        private CheckGenerator _checkGenerator;

        public PayWindow()
        {
            InitializeComponent();
        }

        public PayWindow(List<Product> products)
        {
            _checkGenerator = new CheckGenerator(products);
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToPayTextBlock.Text = _checkGenerator.GetSum().ToString();
        }

        private void AddDiscountCardButton_Click(object sender, RoutedEventArgs e)
        {
            string cardNumber = BarcodeNumberTextBox.Text;

            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 13)
            {
                ShowErrorMessage("Введіть штрих код.");
                return;
            }

            if (!_checkGenerator.AddCustomerToCheck(cardNumber))
            {
                ShowErrorMessage("Не знайдено користувача.");
                return;
            }

            // update the sum
            ToPayTextBlock.Text = _checkGenerator.GetSum().ToString();
            DiscountTextBlock.Text = _checkGenerator.GetDiscount().ToString();
        }

        private void GenerateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            _checkGenerator.GenerateCheck();
        }

        private void ShowErrorMessage(string errorMessage)
        {
            ErrorImage.Visibility = Visibility.Visible;
            ErrorTextBlock.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = errorMessage;
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
