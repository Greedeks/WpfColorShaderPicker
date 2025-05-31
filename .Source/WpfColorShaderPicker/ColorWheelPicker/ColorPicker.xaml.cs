using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfColorShaderPicker.ColorWheelPicker
{
    public partial class ColorPicker
    {
        private bool _isDragging = false;
        private double _selectedX, _selectedY;
        private double _brightness = 1.0;

        public ColorPicker()
        {
            InitializeComponent();
            PART_Wheel.Loaded += (s, e) => CenterMarker();
            PART_Wheel.SizeChanged += (s, e) => CenterMarker();
            PART_BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _brightness = PART_BrightnessSlider.Value;
            UpdateColorDisplay();
        }

        private void CenterMarker()
        {
            _selectedX = 100;
            _selectedY = 100;
            UpdateMarkerPosition(_selectedX, _selectedY);
            UpdateColorDisplay();
        }

        private void Wheel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PART_Wheel.CaptureMouse();
            _isDragging = true;
            UpdateColorByMousePosition(e.GetPosition(PART_Wheel));
        }

        private void Wheel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
                UpdateColorByMousePosition(e.GetPosition(PART_Wheel));
        }

        private void Wheel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                PART_Wheel.ReleaseMouseCapture();
            }
        }

        private void UpdateColorByMousePosition(Point pos)
        {
            double centerX = 100, centerY = 100, radius = 100;
            double dx = pos.X - centerX, dy = pos.Y - centerY;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance > radius)
            {
                dx *= radius / distance;
                dy *= radius / distance;
            }

            _selectedX = centerX + dx;
            _selectedY = centerY + dy;

            UpdateMarkerPosition(_selectedX, _selectedY);

            Color color = GetColorFromWheel(_selectedX, _selectedY, 200, 200, _brightness);

            if (color.A < 128) return;

            UpdateColorDisplay(color);
        }

        private void UpdateMarkerPosition(double x, double y)
        {
            Canvas.SetLeft(PART_Marker, x - PART_Marker.Width / 2);
            Canvas.SetTop(PART_Marker, y - PART_Marker.Height / 2);
        }

        private void UpdateColorDisplay()
        {
            Color color = GetColorFromWheel(_selectedX, _selectedY, 200, 200, _brightness);
            UpdateColorDisplay(color);
        }

        private void UpdateColorDisplay(Color color)
        {
            PART_ColorTextBox.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            PART_ColorPreview.Fill = new SolidColorBrush(color);
        }

        internal static Color GetColorFromWheel(double x, double y, double width, double height, double value)
        {
            double centerX = width / 2, centerY = height / 2;
            double normX = (x - centerX) / (width / 2);
            double normY = (y - centerY) / (height / 2);
            double saturation = Math.Sqrt(normX * normX + normY * normY);

            if (saturation > 1) return Colors.Transparent;

            double hue = 3 * (Math.PI - Math.Atan2(normY, -normX)) / Math.PI;
            double chroma = value * saturation;
            double second = chroma * (1 - Math.Abs(hue % 2.0 - 1));
            double m = value - chroma;
            double r, g, b;
            if (hue < 1)
                (r, g, b) = (chroma, second, 0);
            else if (hue < 2)
                (r, g, b) = (second, chroma, 0);
            else if (hue < 3)
                (r, g, b) = (0, chroma, second);
            else if (hue < 4)
                (r, g, b) = (0, second, chroma);
            else if (hue < 5)
                (r, g, b) = (second, 0, chroma);
            else
                (r, g, b) = (chroma, 0, second);

            byte R = (byte)Math.Round((r + m) * 255);
            byte G = (byte)Math.Round((g + m) * 255);
            byte B = (byte)Math.Round((b + m) * 255);

            return Color.FromRgb(R, G, B);
        }
    }
}