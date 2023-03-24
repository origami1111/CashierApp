using System.Threading;
using System.Collections.Concurrent;
using AForge.Video;
using ZXing;
using AForge.Video.DirectShow;
using System.Drawing;

namespace CashierApp.Logics
{
    public class BarcodeReader
    {
        private ConcurrentQueue<string> barcodeQueue;
        private Thread barcodeThread;
        private VideoCaptureDevice barcodeDevice;
        private ZXing.BarcodeReader reader;

        public BarcodeReader()
        {
            barcodeQueue = new ConcurrentQueue<string>();
            barcodeThread = new Thread(ReadBarcodes);
            barcodeThread.Start();
            reader = new ZXing.BarcodeReader();
        }

        public void Start(string barcodeDeviceName)
        {
            barcodeDevice = new VideoCaptureDevice(barcodeDeviceName);
            barcodeDevice.NewFrame += BarcodeDevice_NewFrame;
            barcodeDevice.Start();
        }

        public void Stop()
        {
            barcodeDevice.SignalToStop();
            barcodeDevice.WaitForStop();
            barcodeThread.Abort();
        }

        public bool TryDequeueBarcode(out string barcode)
        {
            return barcodeQueue.TryDequeue(out barcode);
        }

        private void ReadBarcodes()
        {
            while (true)
            {
                string barcode = "";
                if (barcodeQueue.TryDequeue(out barcode))
                {
                    // Handle barcode data here
                }
                Thread.Sleep(100);
            }
        }

        private void BarcodeDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Convert the video frame to a bitmap
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            // Decode the barcode
            Result result = reader.Decode(bitmap);

            if (result != null)
            {
                string barcode = result.Text;

                // Enqueue the barcode data
                barcodeQueue.Enqueue(barcode);
            }
        }
    }
}
