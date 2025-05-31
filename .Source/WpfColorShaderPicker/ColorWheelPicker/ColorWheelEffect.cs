using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WpfColorShaderPicker.ColorWheelPicker
{
    public class ColorWheelEffect : ShaderEffect
    {
        private static readonly PixelShader _shader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/WpfColorShaderPicker;component/ColorWheelPicker/ColorWheelEffect.ps")
        };

        public ColorWheelEffect()
        {
            PixelShader = _shader;
            UpdateShaderValue(InputProperty);
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorWheelEffect), 0);

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
    }
}
