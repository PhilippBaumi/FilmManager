using SkiaSharp;

namespace FilmManager.Helpers
{
    public class SKiaDrawHelper
    {
        public static void DrawHeader(SKCanvas canvas, SKImageInfo info, string text)
        {
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

            SKRoundRect card = new(new SKRect(0, 0, info.Width, info.Height), 34, 34);
            canvas.DrawRoundRect(card, backgroundPaint);

            using SKPaint reelPaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 8
            };

            float centerY = info.Height * 0.5f;
            float reelRadius = Math.Min(info.Height, info.Width) * 0.22f;
            float reelX = info.Width * 0.22f;
            canvas.DrawCircle(reelX, centerY, reelRadius, reelPaint);

            using SKPaint dotPaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            for (int i = 0; i < 6; i++)
            {
                double angle = i * Math.PI / 3;
                float x = reelX + (float)Math.Cos(angle) * reelRadius * 0.55f;
                float y = centerY + (float)Math.Sin(angle) * reelRadius * 0.55f;
                canvas.DrawCircle(x, y, reelRadius * 0.13f, dotPaint);
            }

            using SKPaint titlePaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true
            };
            using SKFont font = new()
            {
                Size = Math.Max(30, info.Height * 0.23f),
                Typeface = SKTypeface.FromFamilyName("Open Sans", SKFontStyle.Bold)
            };
            canvas.DrawText(text, info.Width * 0.40f, centerY + font.Size * 0.35f, SKTextAlign.Center, font, titlePaint);
        }
    }
}
