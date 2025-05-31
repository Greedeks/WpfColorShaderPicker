using System;
using System.Windows.Media.Effects;

namespace WpfColorShaderPicker.HueColorPicker
{
    public class HueBarEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/WpfColorShaderPicker;component/HueColorPicker/HueBarEffect.ps")

        };

        public HueBarEffect() => PixelShader = _pixelShader;
    }
}
