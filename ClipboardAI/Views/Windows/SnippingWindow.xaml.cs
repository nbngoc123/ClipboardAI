using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ClipboardAI.Views.Windows;

public partial class SnippingWindow : Window
{
    private System.Windows.Point _startPoint;
    private bool _isDragging;

    public BitmapSource? CapturedImage { get; private set; }

    public SnippingWindow()
    {
        InitializeComponent();
        
        // Cover all screens
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.VirtualScreenWidth;
        Height = SystemParameters.VirtualScreenHeight;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(OverlayCanvas);
            SelectionRectangle.Visibility = Visibility.Visible;
            System.Windows.Controls.Canvas.SetLeft(SelectionRectangle, _startPoint.X);
            System.Windows.Controls.Canvas.SetTop(SelectionRectangle, _startPoint.Y);
            SelectionRectangle.Width = 0;
            SelectionRectangle.Height = 0;
            OverlayCanvas.CaptureMouse();
        }
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging)
        {
            var currentPoint = e.GetPosition(OverlayCanvas);
            var x = Math.Min(currentPoint.X, _startPoint.X);
            var y = Math.Min(currentPoint.Y, _startPoint.Y);
            var width = Math.Abs(currentPoint.X - _startPoint.X);
            var height = Math.Abs(currentPoint.Y - _startPoint.Y);

            System.Windows.Controls.Canvas.SetLeft(SelectionRectangle, x);
            System.Windows.Controls.Canvas.SetTop(SelectionRectangle, y);
            SelectionRectangle.Width = width;
            SelectionRectangle.Height = height;
        }
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            OverlayCanvas.ReleaseMouseCapture();
            SelectionRectangle.Visibility = Visibility.Collapsed;

            var x = System.Windows.Controls.Canvas.GetLeft(SelectionRectangle);
            var y = System.Windows.Controls.Canvas.GetTop(SelectionRectangle);
            var width = SelectionRectangle.Width;
            var height = SelectionRectangle.Height;

            if (width > 0 && height > 0)
            {
                CaptureScreenRegion((int)x, (int)y, (int)width, (int)height);
            }

            DialogResult = true;
            Close();
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            DialogResult = false;
            Close();
        }
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    internal static extern bool DeleteObject(IntPtr hObject);

    private void CaptureScreenRegion(int x, int y, int width, int height)
    {
        // Adjust for VirtualScreen offset
        int screenX = (int)SystemParameters.VirtualScreenLeft + x;
        int screenY = (int)SystemParameters.VirtualScreenTop + y;

        using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screenX, screenY, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
            }

            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                CapturedImage = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }
    }
}
