using AForge.Video;
using ZXing;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;

namespace CashierApp.Logics
{
    public class BarcodeReader
    {
        private VideoCaptureDevice _barcodeDevice;
        private readonly ZXing.BarcodeReader _reader;
        private readonly TextBox _textBox;

        public BarcodeReader(TextBox textBox)
        {
            _reader = new ZXing.BarcodeReader();

            _reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.EAN_13
            };

            this._textBox = textBox;
        }

        public void Start(string barcodeDeviceName)
        {
            _barcodeDevice = new VideoCaptureDevice(barcodeDeviceName);
            _barcodeDevice.NewFrame += BarcodeDevice_NewFrame;
            _barcodeDevice.Start();
        }

        public void Stop()
        {
            _barcodeDevice.SignalToStop();
            _barcodeDevice.WaitForStop();
        }

        private void BarcodeDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Convert the video frame to a bitmap
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            // Decode the barcode
            Result result = _reader.Decode(bitmap);

            if (result != null)
            {
                string barcode = result.Text;
                SetResult(barcode);
            }
        }

        delegate void SetStringDelegate(string parameter);
        private void SetResult(string result)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _textBox.Text = result;
                _textBox.Focus();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new SetStringDelegate(SetResult), new object[] { result });
            }
        }
    }
}
