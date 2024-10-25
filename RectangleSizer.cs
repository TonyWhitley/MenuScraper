using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MenuScraper.RectangleSizer;

public partial class CaptureForm : Form
{
    // Import necessary functions from the Windows API
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private Bitmap screenCapture;
    private Point startPoint;
    private Rectangle selectionRect;
    private bool isDragging = false;

    public CaptureForm()
    {
        //InitializeComponent();
        this.DoubleBuffered = true;
        this.MouseDown += CaptureForm_MouseDown;
        this.MouseMove += CaptureForm_MouseMove;
        this.MouseUp += CaptureForm_MouseUp;
        this.Paint += CaptureForm_Paint;
        CaptureWindowImage("AMS2AVX");
    }

    private void CaptureWindowImage(string processName)
    {
        //IntPtr hWnd = FindWindow(null, windowTitle);
        //if (hWnd == IntPtr.Zero)
        //{
        //    MessageBox.Show($"Window with title '{windowTitle}' not found.");
        //    this.Close();
        //    return;
        //}
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
        // Get window rectangle
        if (!GetWindowRect(hWnd, out RECT rect))
        {
            MessageBox.Show("Failed to get the window rectangle.");
            this.Close();
            return;
        }

        // Define capture area based on the window size
        Rectangle captureRect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

        // Capture the window image
        screenCapture = new Bitmap(captureRect.Width, captureRect.Height);
        using (Graphics g = Graphics.FromImage(screenCapture))
        {
            g.CopyFromScreen(captureRect.Location, Point.Empty, captureRect.Size);
        }

        // Set the form size to display the capture
        this.ClientSize = screenCapture.Size;
        this.BackgroundImage = screenCapture;
        this.BackgroundImageLayout = ImageLayout.Stretch;

        SetForegroundWindow(this.Handle);

    }

    private void CaptureForm_MouseDown(object sender, MouseEventArgs e)
    {
        startPoint = e.Location;
        isDragging = true;
    }

    private void CaptureForm_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            var width = e.X - startPoint.X;
            var height = e.Y - startPoint.Y;
            selectionRect = new Rectangle(startPoint.X, startPoint.Y, width, height);
            Invalidate(); // Trigger repaint
        }
    }

    private void CaptureForm_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        Console.WriteLine($"Selected RECT: {selectionRect}");
        ShowSelectedRectangle(selectionRect);

        MessageBox.Show($"Selected Rectangle: {selectionRect}");
    }

    private void CaptureForm_Paint(object sender, PaintEventArgs e)
    {
        if (selectionRect != Rectangle.Empty)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, selectionRect);
            }
        }
    }
    private void ShowSelectedRectangle(Rectangle rect)
    {
        // Create a bitmap of the selected rectangle
        Bitmap selectedArea = new Bitmap(rect.Width, rect.Height);
        using (Graphics g = Graphics.FromImage(selectedArea))
        {
            g.DrawImage(screenCapture, 0, 0, rect, GraphicsUnit.Pixel);
        }

        // Display the selected area in a new form
        Form previewForm = new Form
        {
            Text = "Selected Area Preview",
            ClientSize = new Size(rect.Width, rect.Height),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        };

        PictureBox pictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            Image = selectedArea,
            SizeMode = PictureBoxSizeMode.StretchImage
        };
        previewForm.Controls.Add(pictureBox);

        previewForm.ShowDialog();
    }

}
