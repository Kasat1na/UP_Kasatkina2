using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteErasee.Converter
{
    public static class CaptchaGenerator
    {
        private static readonly Random _random = new Random();

        // Генерация случайного текста CAPTCHA
        public static string GenerateCaptcha(int length = 4)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Range(0, length)
                                        .Select(_ => chars[_random.Next(chars.Length)])
                                        .ToArray());
        }

        // Генерация CAPTCHA изображения в формате Avalonia Bitmap
        public static Bitmap GenerateCaptchaImage(string captchaText)
        {
            const int width = 200;
            const int height = 80;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            // Фон
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 40,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            };

            // Рисуем текст CAPTCHA с небольшим сдвигом по координатам
            for (int i = 0; i < captchaText.Length; i++)
            {
                float x = 20 + i * 40;
                float y = _random.Next(40, 60);
                canvas.DrawText(captchaText[i].ToString(), x, y, paint);
            }

            // Добавляем линии шума
            using var linePaint = new SKPaint
            {
                Color = SKColors.Gray,
                StrokeWidth = 2
            };

            for (int i = 0; i < 5; i++)
            {
                float x1 = _random.Next(0, width);
                float y1 = _random.Next(0, height);
                float x2 = _random.Next(0, width);
                float y2 = _random.Next(0, height);
                canvas.DrawLine(x1, y1, x2, y2, linePaint);
            }

            // Получаем изображение
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream(data.ToArray());

            return new Bitmap(stream);
        }
    }
}
