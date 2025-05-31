sampler2D input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float hue = uv.x * 300.0;
    float c = 1.0;
    float x = (1.0 - abs(fmod(hue / 60.0, 2.0) - 1.0));
    float3 rgb;

    if (hue < 60.0)
        rgb = float3(c, x, 0);
    else if (hue < 120.0)
        rgb = float3(x, c, 0);
    else if (hue < 180.0)
        rgb = float3(0, c, x);
    else if (hue < 240.0)
        rgb = float3(0, x, c);
    else if (hue < 300.0)
        rgb = float3(x, 0, c);
    else
        rgb = float3(c, 0, x);

    return float4(rgb, 1.0);
}