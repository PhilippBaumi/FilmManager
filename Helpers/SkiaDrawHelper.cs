using SkiaSharp;

namespace FilmManager.Helpers
{
    public class SKiaDrawHelper
    {
        public static void DrawHeader(SKCanvas canvas, SKImageInfo info, string text)
        {
            text = text.Trim();
            using SKPaint backgroundPaint = new()
            {
                IsAntialias = true,
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(info.Width, info.Height),
                    new[] { SKColor.Parse("#0066CC"), SKColor.Parse("#00CCFF") },
                    null,
                    SKShaderTileMode.Clamp)
            };

            using SKPaint reelPaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 8
            };

            using SKPaint dotPaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            using SKPaint titlePaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            using SKFont font = new()
            {
                Size = Math.Max(30, info.Height * 0.23f),
                Typeface = SKTypeface.FromFamilyName("Open Sans", SKFontStyle.BoldItalic)
            };

            SKRoundRect card = new(new SKRect(0, 0, info.Width, info.Height), 34, 34);
            canvas.DrawRoundRect(card, backgroundPaint);
            float centerY = info.Height * 0.5f;
            canvas.DrawText(text, info.Width * 0.40f, centerY + font.Size * 0.35f, SKTextAlign.Center, font, titlePaint);
        }
    }
}
