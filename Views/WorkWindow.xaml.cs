using AForge.Video;
using AForge.Video.DirectShow;
using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using ZXing;
using static System.Net.Mime.MediaTypeNames;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for WorkWindow.xaml
    /// </summary>
    
    public partial class WorkWindow : Window
    {
        private readonly ProductController _productController;
        private readonly User _cashier;
        private DispatcherTimer _timer;
        private List<Product> _products;
        private BarcodeReader _barcodeReader;
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;

        public WorkWindow()
        {
            InitializeComponent();
        }

        public WorkWindow(User cashier)
        {
            InitializeComponent();
            InitBarcodeReader();

            _productController = new ProductController();
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

            // video
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_videoDevices.Count > 0)
            {
                _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                _videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                _videoSource.Start();
            }
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

        #region Barcode reader

        delegate void SetStringDelegate(string parameter);
        private void SetResult(string result)
        {
            if (Dispatcher.CheckAccess())
            {
                BarcodeNumberTextBox.Text = result;
                BarcodeNumberTextBox.Focus();
            }
            else
            {
                Dispatcher.Invoke(new SetStringDelegate(SetResult), new object[] { result });
            }
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var frame = (Bitmap)eventArgs.Frame.Clone();
            Result result = _barcodeReader.Decode(frame);

            if (result != null)
            {
                SetResult(result.Text);
            }
        }

        private void InitBarcodeReader()
        {
            _barcodeReader = new BarcodeReader()
            {
                AutoRotate = true
            };

            _barcodeReader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.EAN_13
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_videoSource != null)
            {
                _videoSource.SignalToStop();
                _videoSource.WaitForStop();
            }
        }

        #endregion

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
