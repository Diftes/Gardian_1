// CRT Effect for WPF (v6: Полностью независимые эффекты, сканлайны удалены)
// Target: ps_3_0

float Time : register(c0);
float Scale : register(c1);
float2 Resolution : register(c2);
float4 Background : register(c4);

float EnableDistortion : register(c8);
float EnableNoise : register(c9);
float EnableChromatic : register(c10);
float EnableBlur : register(c11);
// register(c12) был EnableScanline - УДАЛЕН
float GlowStrength : register(c13);

float DistortionAmount : register(c14);
float ChromaticSpread : register(c15);
// register(c16) был ScanlineFactor - УДАЛЕН
float NoiseAmount : register(c17);
float LineSpeed : register(c18);
float LineWidth : register(c19);
// register(c20) был ScanlineFrequency - УДАЛЕН
float PhosphorGlow : register(c21);
float BrightnessBoost : register(c22);

sampler2D Input : register(s0);

#define TAU 6.28318530718
#define M_PI 3.14159265f

float rand2dTo1d(float2 value, float2 dotDir)
{
    float2 smallValue = sin(value + Time);
    float random = dot(smallValue, dotDir);
    return frac(sin(random) * 143758.5453f);
}

float4 Aberration(float2 tex_coord)
{
    float texelWidth = 1.0f / Resolution.x;
    
    float2 samplePosR = tex_coord + float2(ChromaticSpread * texelWidth, 0);
    float2 samplePosB = tex_coord - float2(ChromaticSpread * texelWidth, 0);
    
    float4 rColor = tex2D(Input, samplePosR);
    float4 gColor = tex2D(Input, tex_coord);
    float4 bColor = tex2D(Input, samplePosB);
    
    float4 aberrated = float4(rColor.r, gColor.g, bColor.b, 1.0);
    
    if (PhosphorGlow > 0.0)
    {
        float2 samplePosR2 = tex_coord - float2(PhosphorGlow * texelWidth, 0);
        float2 samplePosB2 = tex_coord + float2(PhosphorGlow * texelWidth, 0);
        
        float4 rColor2 = tex2D(Input, samplePosR2);
        float4 bColor2 = tex2D(Input, samplePosB2);
        
        aberrated.r += rColor2.r * 0.5f;
        aberrated.b += bColor2.b * 0.5f;
        
        aberrated.r = min(aberrated.r, 1.0f);
        aberrated.b = min(aberrated.b, 1.0f);
    }
    
    return aberrated;
}

float Gaussian2D(float x, float y, float sigma)
{
    return (1.0f / (sigma * sqrt(2.0f * M_PI))) * exp(-0.5f * (x * x + y * y) / (sigma * sigma));
}

float4 Blur(float2 tex_coord, float sigma)
{
    float texelWidth = 1.0f / Resolution.x;
    float texelHeight = 1.0f / Resolution.y;
    float4 color = float4(0, 0, 0, 0);
    float totalWeight = 0.0f;
    
    for (int x = -2; x <= 2; x++)
    {
        for (int y = -2; y <= 2; y++)
        {
            float2 samplePos = tex_coord + float2(x * texelWidth, y * texelHeight);
            float weight = Gaussian2D(x, y, sigma);
            color += tex2D(Input, samplePos) * weight;
            totalWeight += weight;
        }
    }
    return color / totalWeight;
}

float2 LensDistortion(float2 uv)
{
    float2 centered = uv * 2.0 - 1.0;
    float dist = length(centered);
    centered *= 1.0 + dist * DistortionAmount;
    return centered * 0.5 + 0.5;
}

float4 main(float2 uv : TEXCOORD, uniform float2 pixelSize : PSIZE) : COLOR
{
    float2 tex = uv;
    
    // 1. Искажение (Vignette/Curvature) - Независимо
    if (EnableDistortion > 0.5) tex = LensDistortion(tex);
    if (tex.x < 0 || tex.x > 1 || tex.y < 0 || tex.y > 1) return float4(0, 0, 0, 1);
    
    float4 color = tex2D(Input, tex);
    
    // 2. Хроматическая аберрация и свечение - Независимо
    if (EnableChromatic > 0.5) color = Aberration(tex);
    
    // 3. Размытие (Bloom) - Независимо
    if (EnableBlur > 0.5 && GlowStrength > 0.0)
    {
        float sigma = 1.5f * Scale;
        float4 blurred = Blur(tex, sigma);
        color = lerp(color, blurred, GlowStrength);
    }
    
    // 4. Горизонтальный глитч (Помехи) - Независимо
    float linePosition = frac(Time * LineSpeed);
    float lineWidth = LineWidth / Resolution.y;
    
    if (tex.y > linePosition - lineWidth && tex.y < linePosition)
    {
        float distortionAmount = 0.03f;
        float waveDist = sin(tex.y * TAU * 5.0f + Time * 2.0f) * distortionAmount;
        color = tex2D(Input, float2(tex.x + waveDist, tex.y));
    }

    // 5. Яркость (BrightnessBoost) - Глобально и независимо
    color.rgb += BrightnessBoost;
    color.rgb = min(color.rgb, 1.0f); // Защита от пересвета

    // 6. Шум (Noise) - Независимо
    if (EnableNoise > 0.5)
    {
        float noise = rand2dTo1d(tex, float2(12.9898f, 78.233f)) * NoiseAmount;
        color.rgb *= (1.0f - NoiseAmount + noise);
    }
    
    return color;
}