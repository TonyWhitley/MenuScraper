using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Tesseract;

namespace ProcessScreenCaptureOCR
{
    class Program2
    {
        // Import necessary functions from the Windows API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

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

        static public void Scraper()
        {
            // Specify the process window title or class name (use "AMS2AVX" if that's the exact window title)
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
            //string windowTitle = "AMS2AVX";
            //IntPtr hWnd = FindWindow(null, windowTitle);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine($"Window with title  not found.");
                return;
            }

            // Get the window rectangle
            if (!GetWindowRect(hWnd, out RECT rect))
            {
                Console.WriteLine("Failed to get the window rectangle.");
                return;
            }

            // Define the rectangle area to capture based on the window position
            //Rectangle captureRect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            // Define the rectangle area to capture (x, y, width, height)
            Rectangle captureRect = new Rectangle(500, 940, 540, 500);


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
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = ConvertBitmapToPix(bitmap))
                    {
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();
                            Console.WriteLine("Extracted Text:");
                            Console.WriteLine(extractedText);
                        }
                    }
                }
            }

            Console.WriteLine("OCR process completed.");
            Console.ReadLine();
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

    class Program
    {
        // Importing necessary Windows API functions
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        static public void Scraper()
        {
            // Name of the target process
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

            // Get the dimensions of the process window
            RECT rect;
            GetWindowRect(hWnd, out rect);

            // Calculate width and height of the window
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            // Capture the screen area of the specific process window
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, new Size(width, height));
                }

                // Save the captured image (optional, for debugging purposes)
                bitmap.Save("CapturedProcessImage.png", System.Drawing.Imaging.ImageFormat.Png);

                // Perform OCR on the captured image
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = ConvertBitmapToPix(bitmap))
                    {
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();
                            Console.WriteLine("Extracted Text:");
                            Console.WriteLine(extractedText);
                        }
                    }
                }
            }

            Console.WriteLine("OCR process completed.");
            Console.ReadLine();
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

