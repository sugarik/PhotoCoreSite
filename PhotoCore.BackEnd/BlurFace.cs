using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PhotoCore

// "https://docs.microsoft.com/ru-ru/xamarin/xamarin-forms/user-interface/graphics/skiasharp/transforms/rotate
// "https://drive.google.com/uc?export=view&id=1JX_tTVV-_7xBVIdy3cfwF4jwE6-IKENt"
{
    public class BlurFace
    {
        DateTime dtStart;
        DateTime dtStop;
        bool trace = true;
        int jpegQuality;
        byte[] bytes;
        public byte[] ResultArray { get { return bytes; } }
        public string ResultBase64 { get { if (bytes != null) { return "data:image/jpg;base64," + Convert.ToBase64String(bytes); } else { return null; } } }
        public int ResultProcessTime { get { return (int)(dtStop - dtStart).TotalMilliseconds; } }

        public BlurFace(string storeId, List<RestrictedFaceArea> hiddenRects, int jpegQuality)
        {
            dtStart = DateTime.Now;
            if (String.IsNullOrEmpty(storeId)) bytes = null;
            else
            {
                bytes = null;
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    try
                    {
                        byte[] result = httpClient.GetByteArrayAsync(String.Format("https://drive.google.com/uc?export=view&id={0}", storeId)).Result;
                        using (MemoryStream stream = new MemoryStream(result))
                        using (SKImage image = SKImage.FromEncodedData(stream))
                            Process(image, hiddenRects, jpegQuality);
                        result = null;
                    }
                    catch (Exception exc1)
                    {
                        try
                        {
                            Thread.Sleep(100);
                            byte[] result = httpClient.GetByteArrayAsync(String.Format("https://drive.google.com/uc?export=view&id={0}", storeId)).Result;
                            using (MemoryStream stream = new MemoryStream(result))
                            using (SKImage image = SKImage.FromEncodedData(stream))
                                Process(image, hiddenRects, jpegQuality);
                            result = null;
                        }
                        catch (Exception exc2)
                        {
                            try
                            {
                                Thread.Sleep(100);
                                byte[] result = httpClient.GetByteArrayAsync(String.Format("https://drive.google.com/uc?export=view&id={0}", storeId)).Result;
                                using (MemoryStream stream = new MemoryStream(result))
                                using (SKImage image = SKImage.FromEncodedData(stream))
                                    Process(image, hiddenRects, jpegQuality);
                                result = null;
                            }
                            catch (Exception exc3)
                            {

                            }
                        }
                    }
                }
            }
        }

        public BlurFace(string file, int retryAttempts, int nextRetryDelay, List<RestrictedFaceArea> hiddenRects, int jpegQuality)
        {
            dtStart = DateTime.Now;
            if (retryAttempts < 1) retryAttempts = 1;
            if (nextRetryDelay < 100) nextRetryDelay = 100;
            if (String.IsNullOrEmpty(file)) bytes = null;
            else
            {
                bytes = null;

                if (File.Exists(file))
                {
                    bool fileIsReady = false;
                    FileStream fileStream = null;
                    try
                    {
                        do
                        {
                            try { fileStream = File.OpenRead(file); fileIsReady = true; }
                            catch (IOException exc) { Console.WriteLine("Ooops"); }
                            retryAttempts--;
                            if (!fileIsReady) Thread.Sleep(nextRetryDelay);
                        }
                        while (!fileIsReady && retryAttempts > 0);

                        if (fileStream != null)
                        {
                            fileStream.Position = 0;
                            using (SKData data = SKData.Create(fileStream))
                            using (SKImage image = SKImage.FromEncodedData(data))
                            {
                                if (image != null) Process(image, hiddenRects, jpegQuality);
                            }
                        }
                    }
                    catch (Exception exc)
                    {

                    }
                    fileStream = null;
                }
            }
        }

        void Process(SKImage image, List<RestrictedFaceArea> hiddenRects, int jpegQuality)
        {
            this.jpegQuality = jpegQuality;
            bytes = null;
            using (SKBitmap bitmap = SKBitmap.FromImage(image))
            {
                for (int f = 0; f < hiddenRects.Count; f++)
                {
                    SKBitmap faceBlurBitmap = null;
                    SKBitmap ovalMask = null;
                    // кварат зоны наложения маски                  
                    SKRect target = new SKRect(hiddenRects[f].x, hiddenRects[f].y, hiddenRects[f].x + hiddenRects[f].w, hiddenRects[f].y + hiddenRects[f].h);
                    float rotateAngle = hiddenRects[f].a;

                    float blurSideSize = (float)(Math.Max(target.Width, target.Height) * 1.5);
                    float sigma = blurSideSize / 25;
                    // квадрат с запасом для равномерного размыливания изображения
                    SKRect targetPlus = new SKRect(target.MidX - blurSideSize / 2, target.MidY - blurSideSize / 2, target.MidX + blurSideSize / 2, target.MidY + blurSideSize / 2);

                    using (SKSurface surfaceBlur = SKSurface.Create(new SKImageInfo((int)targetPlus.Width, (int)targetPlus.Height)))
                    using (SKCanvas canvasBlur = surfaceBlur.Canvas)
                    {
                        using (SKImageFilter imageFilterBlur = SKImageFilter.CreateBlur(sigma, sigma))
                        using (SKPaint paintBlur = new SKPaint { ImageFilter = imageFilterBlur, IsAntialias = false, })
                        {
                            canvasBlur.DrawImage(image, targetPlus, new SKRect(0, 0, targetPlus.Width, targetPlus.Height), paintBlur);
                        }
                        using (SKImage imageBlur = surfaceBlur.Snapshot())
                        {
                            faceBlurBitmap = SKBitmap.FromImage(imageBlur);
                            //if (trace) SaveJpegImage(imageBlur, "FaceBlur.jpg");
                        }
                    }

                    // формируем эллипс-маску с учётом требуемого наклона
                    float width = Math.Max(target.Width, target.Height);
                    using (SKSurface surfaceOval = SKSurface.Create(new SKImageInfo((int)width, (int)width)))
                    using (SKCanvas canvasOval = surfaceOval.Canvas)
                    {
                        using (SKRoundRect roundRectOval = new SKRoundRect(new SKRect((width - target.Width) / 2, (width - target.Height) / 2, (width - target.Width) / 2 + target.Width, (width - target.Height) / 2 + target.Height), (float)(target.Width / 2.2), (float)(target.Height / 2.2)))
                        using (SKPaint paintOval = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = SKColors.White })
                        {
                            canvasOval.Save();
                            canvasOval.RotateDegrees(rotateAngle, (float)(width / 2), (float)(width / 2));
                            canvasOval.DrawRoundRect(roundRectOval, paintOval); // на прежнем канвасе
                            canvasOval.Restore();
                        }
                        using (SKImage imageOval = surfaceOval.Snapshot())
                        {
                            ovalMask = SKBitmap.FromImage(imageOval);
                            //if (trace) SaveJpegImage(imageOval, "OvalMask.jpg");
                        }
                    }

                    bytes = ovalMask.Bytes;

                    int imageX = 0, imageY = 0, blurX = 0, blurY = 0;

                    int deltaBlurXImage = (int)(target.MidX - (float)ovalMask.Width / 2);
                    int deltaBlurYImage = (int)(target.MidY - (float)ovalMask.Height / 2);

                    int deltaMaskBlurX = (int)Math.Round((float)faceBlurBitmap.Width / 2 - (float)ovalMask.Width / 2);
                    int deltaMaskBlurY = (int)Math.Round((float)faceBlurBitmap.Height / 2 - (float)ovalMask.Height / 2);

                    int col = 0; int row = 0;
                    try
                    {
                        for (int i = 3; i < bytes.Length; i = i + 4)
                        {
                            if (bytes[i] > 0)
                            {
                                imageX = col + deltaBlurXImage;
                                imageY = row + deltaBlurYImage;
                                blurX = col + deltaMaskBlurX;
                                blurY = row + deltaMaskBlurY;
                                if (imageX < bitmap.Width && imageY < bitmap.Height && imageX < bitmap.Width && imageY < bitmap.Height)
                                    bitmap.SetPixel(imageX, imageY, faceBlurBitmap.GetPixel(blurX, blurY));
                            }
                            col++;
                            if (col >= ovalMask.RowBytes >> 2) { col = 0; row++; }
                        }
                    }
                    catch (Exception exc)
                    {

                    }
                    using (SKImage sKImage = SKImage.FromBitmap(bitmap))
                    using (SKData tmpData = sKImage.Encode(SKEncodedImageFormat.Jpeg, jpegQuality))
                    {
                        bytes = tmpData.ToArray();
                        if (trace) { using (FileStream tmpStream = new FileStream("Blur result.jpg", FileMode.Create)) tmpData.SaveTo(tmpStream); }
                    }
                    faceBlurBitmap = null;
                    ovalMask = null;
                }
            }
            dtStop = DateTime.Now;
        }

        void SaveJpegImage(SKImage image, string fileName)
        {
            SKData data = image.Encode(SKEncodedImageFormat.Jpeg, jpegQuality);
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                data.SaveTo(stream);
            }
        }

        void SavePngImage(SKImage image, string fileName)
        {
            SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                data.SaveTo(stream);
            }
        }

    }
}
