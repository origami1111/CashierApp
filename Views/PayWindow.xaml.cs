using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Product> _products;

        public PayWindow()
        {
            InitializeComponent();
        }

        public PayWindow(List<Product> products)
        {
            InitializeComponent();
            _products = products;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddDiscountCardButton_Click(object sender, RoutedEventArgs e)
        {
            string cardNumber = BarcodeNumberTextBox.Text;

            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 13)
            {
                ShowErrorMessage("Введіть штрих код.");
                return;
            }

            throw new NotImplementedException();
        }

        private void GenerateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PrintCheckButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
