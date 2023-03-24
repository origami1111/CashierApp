using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for WorkWindow.xaml
    /// </summary>
    
    public partial class WorkWindow : Window
    {
        private readonly CashierDBEntities _dbContext;
        private readonly ProductController _productController;
        private readonly User _cashier;
        private readonly BarcodeReader _barcodeReader;
        private DispatcherTimer _timer;
        private List<Product> _products;


        public WorkWindow()
        {
            InitializeComponent();
        }

        public WorkWindow(User cashier)
        {
            InitializeComponent();

            _barcodeReader = new BarcodeReader(BarcodeNumberTextBox);

            _dbContext = new CashierDBEntities();
            _productController = new ProductController(_dbContext);
            _cashier = cashier;
            _products = new List<Product>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  DispatcherTimer setup
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();

            CashierNameTextBlock.Text = GetFullCashierName();
            HideErrorMessage();

            _barcodeReader.Start("@device:pnp:\\\\?\\usb#vid_13d3&pid_56a2&mi_00#7&c91c3a9&0&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\\global");
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.Items.Count == 0)
            {
                ShowErrorMessage("Не можливо відкрити чек. Товари відсутні.");
                return;
            }

            // open pay window
            PayWindow payWindow = new PayWindow(_products);
            payWindow.Show();

            DataGrid.ItemsSource = null;
            _products = new List<Product>();
            HideErrorMessage();
        }

        private async void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            string barcode = BarcodeNumberTextBox.Text;
            HideErrorMessage();

            Product objProduct = await _productController.GetProductAsync(barcode);

            if (objProduct == null)
            {
                ShowErrorMessage("Товар не знайдений!");
                ClearField();
                return;
            }

            AddProductToDataGrid(objProduct);
            ClearField();
        }

        private void AddProductToDataGrid(Product objProduct)
        {
            _products.Add(objProduct);
            DataGrid.ItemsSource = null;
            DataGrid.ItemsSource = _products;

            using (var ms = new MemoryStream(objProduct.Image))
            {
                // создаем новый BitmapImage
                var image = new BitmapImage();
                // загружаем изображение из MemoryStream
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                // назначаем BitmapImage источником изображения для элемента Image
                ProductImage.Source = image;
            }

            DataGrid.SelectedIndex = DataGrid.Items.Count;
            SumToPayTextBlock.Text = GetSum().ToString();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();
            var selectedRow = DataGrid.SelectedItem;
            if (selectedRow == null)
            {
                ShowErrorMessage("Виберіть продукт, який потрібно видалити.");
                return;
            }

            var objProduct = (Product)selectedRow;
            _products.Remove(objProduct);
            DataGrid.ItemsSource = null;
            DataGrid.ItemsSource = _products;
            DataGrid.SelectedIndex = DataGrid.Items.Count;
            SumToPayTextBlock.Text = GetSum().ToString();
        }

        private decimal GetSum()
        {
            decimal sum = 0;

            foreach (var item in _products)
            {
                sum += item.Price;
            }

            return sum;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _barcodeReader.Stop();
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

        private void ClearField()
        {
            BarcodeNumberTextBox.Text = string.Empty;
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

            if (text.Length != 0)
            {
                text = text.Remove(text.Length - 1);
                BarcodeNumberTextBox.Text = text;
                BarcodeNumberTextBox.CaretIndex = BarcodeNumberTextBox.Text.Length;
                BarcodeNumberTextBox.Focus();
            }
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
