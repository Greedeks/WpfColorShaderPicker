using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfColorShaderPicker.HueColorPicker
{
    public partial class HueBar
    {
        internal static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(HueBar), new PropertyMetadata(Colors.White, OnSelectedColorChanged));

        internal Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HueBar bar = (HueBar)d;
            Color color = (Color)e.NewValue;
            ColorToHSV(color, out double h, out _, out double v);
            bar.Hue = h;
            bar.Value = v;
            bar.UpdateAll();
        }

        internal double Hue { get; private set; } = 0;
        internal double Value { get; private set; } = 1;
        internal Brush HueMarkerFill { get; set; } = Brushes.White;
        internal Brush ValueMarkerFill { get; set; } = Brushes.White;
        internal Brush ValueBarFill { get; set; }

        private bool _hueDrag = false;
        private bool _valueDrag = false;

        public HueBar()
        {
            InitializeComponent();

            PART_HueCanvas.MouseDown += HueCanvas_MouseDown;
            PART_HueCanvas.MouseMove += HueCanvas_MouseMove;
            PART_HueCanvas.MouseUp += HueCanvas_MouseUp;

            PART_ValueCanvas.MouseDown += ValueCanvas_MouseDown;
            PART_ValueCanvas.MouseMove += ValueCanvas_MouseMove;
            PART_ValueCanvas.MouseUp += ValueCanvas_MouseUp;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => UpdateAll();

        public static void ColorToHSV(Color color, out double hue, out double sat, out double val)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            val = max;
            double delta = max - min;
            sat = (max == 0) ? 0 : delta / max;
            if (delta == 0) hue = 0;
            else if (max == r) hue = 60 * (((g - b) / delta) % 6);
            else if (max == g) hue = 60 * (((b - r) / delta) + 2);
            else hue = 60 * (((r - g) / delta) + 4);
            if (hue < 0) hue += 360;
        }

        private void HueCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _hueDrag = true;
            PART_HueCanvas.CaptureMouse();
            SetHueFromPoint(e.GetPosition(PART_HueCanvas).X);
        }

        private void HueCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_hueDrag)
                SetHueFromPoint(e.GetPosition(PART_HueCanvas).X);
        }

        private void HueCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_hueDrag)
            {
                _hueDrag = false;
                PART_HueCanvas.ReleaseMouseCapture();
            }
        }

        private void SetHueFromPoint(double x)
        {
            double w = PART_HueBar.Width;
            double markerW = PART_HueMarker.Width;
            x = Math.Max(markerW / 2, Math.Min(w - markerW / 2, x));
            Canvas.SetLeft(PART_HueMarker, x - markerW / 2);
            Canvas.SetTop(PART_HueMarker, (PART_HueBar.Height - markerW) / 2);
            Hue = 300.0 * (x - markerW / 2) / (w - markerW);
            HueMarkerFill = new SolidColorBrush(ColorFromHSV(Hue, 1, Value));
            PART_HueMarker.Fill = HueMarkerFill;
            UpdateValueBar();
            UpdateValueMarker();
        }

        private void ValueCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _valueDrag = true;
            PART_ValueCanvas.CaptureMouse();
            SetValueFromPoint(e.GetPosition(PART_ValueCanvas).X);
        }

        private void ValueCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_valueDrag)
                SetValueFromPoint(e.GetPosition(PART_ValueCanvas).X);
        }

        private void ValueCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_valueDrag)
            {
                _valueDrag = false;
                PART_ValueCanvas.ReleaseMouseCapture();
            }
        }

        private void SetValueFromPoint(double x)
        {
            double w = PART_ValueBar.Width;
            double markerW = PART_ValueMarker.Width;
            x = Math.Max(markerW / 2, Math.Min(w - markerW / 2, x));
            Canvas.SetLeft(PART_ValueMarker, x - markerW / 2);
            Canvas.SetTop(PART_ValueMarker, (PART_ValueBar.Height - markerW) / 2);
            Value = (x - markerW / 2) / (w - markerW);
            ValueMarkerFill = Brushes.Transparent;
            PART_ValueMarker.Fill = ValueMarkerFill;
            UpdateHueMarker();
        }

        private void UpdateAll()
        {
            UpdateHueMarker();
            UpdateValueBar();
            UpdateValueMarker();
        }

        private void UpdateHueMarker()
        {
            double w = PART_HueBar.Width;
            double markerW = PART_HueMarker.Width;
            double x = Hue / 300.0 * (w - markerW) + markerW / 2;
            Canvas.SetLeft(PART_HueMarker, x - markerW / 2);
            Canvas.SetTop(PART_HueMarker, (PART_HueBar.Height - markerW) / 2);
            HueMarkerFill = new SolidColorBrush(ColorFromHSV(Hue, 1, Value));
            PART_HueMarker.Fill = HueMarkerFill;
        }

        private void UpdateValueBar()
        {
            ValueBarFill = new LinearGradientBrush(Colors.Black, ColorFromHSV(Hue, 1, 1), new Point(0, 0.5), new Point(1, 0.5));
            PART_ValueBar.Fill = ValueBarFill;
        }

        private void UpdateValueMarker()
        {
            double w = PART_ValueBar.Width;
            double markerW = PART_ValueMarker.Width;
            double x = Value * (w - markerW) + markerW / 2;
            Canvas.SetLeft(PART_ValueMarker, x - markerW / 2);
            Canvas.SetTop(PART_ValueMarker, (PART_ValueBar.Height - markerW) / 2);
            ValueMarkerFill = Brushes.Transparent;
            PART_ValueMarker.Fill = ValueMarkerFill;
        }

        internal static Color ColorFromHSV(double h, double s, double v)
        {
            int i;
            double f, p, q, t;
            if (s == 0)
            {
                int val = (int)(v * 255);
                return Color.FromRgb((byte)val, (byte)val, (byte)val);
            }
            h /= 60;
            i = (int)Math.Floor(h);
            f = h - i;
            p = v * (1.0 - s);
            q = v * (1.0 - s * f);
            t = v * (1.0 - s * (1.0 - f));
            double r, g, b;
            switch (i)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }
            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
