using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
        private readonly User _cashier;
        private DispatcherTimer _timer;

        public WorkWindow()
        {
            InitializeComponent();
        }

        public WorkWindow(User cashier)
        {
            InitializeComponent();
            _cashier = cashier;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  DispatcherTimer setup
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();

            CashierNameTextBlock.Text = GetFullCashierName();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTextBlock.Text = DateTime.Now.ToString();
            CommandManager.InvalidateRequerySuggested();
        }

        private string GetFullCashierName()
        {
            return $"Касир: {_cashier.LastName} {_cashier.FirstName} {_cashier.Patronymic}";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            BarcodeNumberTextBox.Text += button.Content;
            BarcodeNumberTextBox.CaretIndex = BarcodeNumberTextBox.Text.Length;
            BarcodeNumberTextBox.Focus();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            string text = BarcodeNumberTextBox.Text;
            text = text.Remove(text.Length - 1);
            BarcodeNumberTextBox.Text = text;
            BarcodeNumberTextBox.CaretIndex = BarcodeNumberTextBox.Text.Length;
            BarcodeNumberTextBox.Focus();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            BarcodeNumberTextBox.Text = string.Empty;
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
