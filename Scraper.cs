using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Tesseract;

namespace ProcessScreenCaptureOCR
{
    class Scraper
    {
        // Import necessary functions from the Windows API
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public Bitmap CapturE;
        public string Ocr;

        public Scraper()
        {}

        public void Scrape()
        {
            string processName = "AMS2AVX";
            // Find the process by name
            Process process = Process.GetProcessesByName(processName)[0];
            if (process == null)
            {
                Console.WriteLine($"Process {processName} not found.");
                return;
            }

            // Get the main window handle of the process
            IntPtr hWnd = process.MainWindowHandle;

            // Bring the process window to the foreground
            SetForegroundWindow(hWnd);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine($"Window for {processName} not found.");
                return;
            }

            // Get the window rectangle
            if (!GetWindowRect(hWnd, out RECT rect))
            {
                Console.WriteLine("Failed to get the window rectangle.");
                return;
            }

            // Define the rectangle area to capture (x, y, width, height)
            Rectangle captureRect = new Rectangle(530, 940, 510, 450);


            // Capture the specified area of the window
            using (Bitmap bitmap = new Bitmap(captureRect.Width, captureRect.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(captureRect.Location, Point.Empty, captureRect.Size);
                }

                // Save the captured image (optional, for debugging purposes)
                bitmap.Save("CapturedImage.png", System.Drawing.Imaging.ImageFormat.Png);

                // Perform OCR on the captured image
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default, @"./tessdata/tessconfigs/ams2.patterns.config"))
                {
                    using (var img = ConvertBitmapToPix(bitmap))
                    {
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();
                            Console.WriteLine("Extracted Text:");
                            Console.WriteLine(extractedText);
                            this.Ocr = extractedText;
                            this.CapturE = new Bitmap(bitmap);
                        }
                    }
                }
            }
        }
        public static Pix ConvertBitmapToPix(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                return Pix.LoadFromMemory(stream.ToArray());
            }
        }
    }
}

