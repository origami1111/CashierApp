using AForge.Video;
using AForge.Video.DirectShow;
using CashierApp.Logics;
using CashierApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZXing;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for PayWindow.xaml
    /// </summary>
    public partial class PayWindow : Window
    {
        private readonly List<Product> _products;
        private readonly CheckGenerator _checkGenerator;
        private Check _createdCheck;
        private Card _card;

        private BarcodeReader _barcodeReader;
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;

        public PayWindow()
        {
            InitializeComponent();
        }

        public PayWindow(List<Product> products)
        {
            InitializeComponent();
            InitBarcodeReader();
            _products = products;
            _checkGenerator = new CheckGenerator(products);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToPayTextBlock.Text = _checkGenerator.CalculateSum().ToString();
            DiscountTextBlock.Text = ToPayTextBlock.Text;

            // video
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_videoDevices.Count > 0)
            {
                _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                _videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                _videoSource.Start();
            }
        }

        private void AddDiscountCardButton_Click(object sender, RoutedEventArgs e)
        {
            string cardNumber = BarcodeNumberTextBox.Text;

            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 13)
            {
                ShowErrorMessage("Введіть штрих код.");
                return;
            }

            using (CashierDBEntities cashierDBEntities = new CashierDBEntities())
            {
                _card = cashierDBEntities.Cards.SingleOrDefault(card => card.CardNumber == cardNumber);

                if (_card != null)
                {
                    DiscountTextBlock.Text = _checkGenerator.CalculateSumWithDiscount(_card.Discount).ToString();
                }
            }
        }

        private void GenerateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            _createdCheck = _checkGenerator.GenerateCheck(_card);

            CheckStringGenerator checkStringGenerator = new CheckStringGenerator(_products, _createdCheck);
            string textToPrint = checkStringGenerator.GenerateCheck();

            Printer printer = new Printer(textToPrint);
            printer.Print();
        }

        #region BarcodeReader

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
