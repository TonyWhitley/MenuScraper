using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;


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



        private static Pix BitmapToPix(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png); // Convert Bitmap to a PNG byte stream
                ms.Position = 0; // Reset stream position
                return Pix.LoadFromMemory(ms.ToArray()); // Load Pix from byte array
            }
        }

        private static Bitmap ConvertToBlackAndWhite(Bitmap original)
        {
            Bitmap bwImage = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grayScale = (int)((pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11));
                    Color bwColor = grayScale > 128 ? Color.White : Color.Black; // Threshold
                    bwImage.SetPixel(x, y, bwColor);
                }
            }
            return bwImage;
        }

        public static string PerformOCR(Bitmap bitmap)
        {
            string extractedText;

            // Set up the Tesseract engine to use custom pattern and words files
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("user_words_file", @"./tessdata/tessconfigs/ams2_words.txt");
                engine.SetVariable("user_patterns_file", @"./tessdata/tessconfigs/ams2_patterns.txt");
                engine.SetVariable("tessedit_pageseg_mode", "3"); // SINGLE_BLOCK mode
                engine.SetVariable("classify_bln_numeric_mode", "1");

                using (var img = BitmapToPix(ConvertToBlackAndWhite(bitmap)))
                //using (var img = BitmapToPix(bitmap))
                {
                    using (var page = engine.Process(img))
                    {
                        extractedText = page.GetText();
                    }
                }
            }

            return extractedText;
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

                string ocrText = PerformOCR(bitmap);
                Console.WriteLine("Extracted Text:");
                Console.WriteLine(ocrText);
                this.Ocr = ocrText;
                this.CapturE = new Bitmap(bitmap);
                // Perform OCR on the captured image
                //using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default, @"./tessdata/tessconfigs/ams2.patterns.config"))
                //{
                //    using (var img = ConvertBitmapToPix(bitmap))
                //    {
                //        using (var page = engine.Process(img))
                //        {
                //            string extractedText = page.GetText();
                //            Console.WriteLine("Extracted Text:");
                //            Console.WriteLine(extractedText);
                //            this.Ocr = extractedText;
                //            this.CapturE = new Bitmap(bitmap);
                //        }
                //    }
                //}
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

