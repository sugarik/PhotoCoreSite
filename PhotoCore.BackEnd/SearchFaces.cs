using DlibDotNet;
using ExifLib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotoCore
{

    // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/cropping
    public class SearchFaces : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchFaces));

        string cameraSerialNumber = null;
        public string CameraSerialNumber { get { return cameraSerialNumber; } }

        DateTime photoDateTime = DateTime.MinValue;
        public DateTime PhotoDateTime { get { return photoDateTime; } }

        bool disposed = false;
        //SKColor maskColor = new SKColor(77, 168, 151, 255);
        SKColor maskColor = new SKColor(255, 255, 255, 255);

        List<byte[]> facesByteArrays = null;
        public List<byte[]> DetectedFaces { get { return facesByteArrays; } }

        bool isSuccess;
        public bool IsSuccess { get { return isSuccess; } }

        int processTime;
        public int ProcessTime { get { return processTime; } }

        class FaceLocation
        {
            /// <summary> Количество пикселей до левого края квадрата лица от левого края фотографии </summary>
            public int Left { get; set; }
            /// <summary> Количество пикселей до правого края квадрата лица от левого края фотографии </summary>
            public int Right { get; set; }
            /// <summary> Количество пикселей до верхнего края квадрата лица от верха фотографии </summary>
            public int Top { get; set; }
            /// <summary> Количество пикселей до нижнего края квадрата лица от верха фотографии </summary>
            public int Bottom { get; set; }
            /// <summary> Угол бокового наклона лица </summary>
            public double Angle { get; set; }
        }

        SearchFacesResponseBackEnd response = null;
        public SearchFacesResponseBackEnd Response { get { return response; } }

        public SearchFaces(SearchFacesRequestFrontEnd request, string sessionInfo, string faceDir, int[] additionalRotateAngles = null, int photoJpegQuality = 95, double faceClippingRatio = 1.2)
        {
            response = new SearchFacesResponseBackEnd();

            if (request != null)
            {
                if (request.requestId == null) response.requestId = Guid.NewGuid().ToString(); else response.requestId = request.requestId;

                if (request.selfieImage == null || request.selfieImage == "")
                {
                    if (sessionInfo != null) log.Debug(String.Format("SearchFaces. Request. Id={0}. Null image. Session info: {1}", response.requestId, sessionInfo.Replace("\n", "; ")));
                    else log.Debug(String.Format("SearchFaces. Request. Id={0}. Null image. Session info: NULL", response.requestId));
                    response.faces = null;
                    log.Debug(String.Format("SearchFaces. Response. Id={0}. Null result.", response.requestId));
                }
                else
                {
                    byte[] selphiByteArray = null;
                    try
                    {
                        var match = Regex.Match(request.selfieImage, @"data:(?<type>.+?);base64,(?<data>.+)"); // расчленяем на составные части
                        var base64Data = match.Groups["data"].Value;
                        var contentType = match.Groups["type"].Value; if (contentType == null) contentType = "NULL";
                        selphiByteArray = Convert.FromBase64String(base64Data);

                        string contentInfo = String.Format("Content type: {0}. Base64 length: {1} bytes. Content length: {2}", contentType, base64Data.Length, selphiByteArray.Length);
                        if (sessionInfo != null) log.Debug(String.Format("SearchFaces. Request. Id={0}. {1}. Session info: {2}", response.requestId, contentInfo, sessionInfo.Replace("\n", "; ")));
                        else log.Debug(String.Format("SearchFaces. Request. Id={0}. {1}. Session info: NULL", response.requestId, contentInfo));
                    }
                    catch (Exception exc)
                    {

                    }

                    if (selphiByteArray != null)
                    {
                        response.faces = null;
                        SearchFaces sf = new SearchFaces(selphiByteArray, null, 90, 1.5);
                        if (sf.DetectedFaces != null && sf.DetectedFaces.Count > 0)
                        {
                            if (!Directory.Exists(faceDir)) { try { Directory.CreateDirectory(faceDir); } catch (Exception exc) { } }

                            for (int i = 0; i < sf.DetectedFaces.Count; i++)
                            {
                                try
                                {
                                    var faceId = Guid.NewGuid().ToString();
                                    System.IO.File.WriteAllBytes(String.Format("{0}{1}.jpg", faceDir, faceId), sf.DetectedFaces[i]);
                                    if (response.faces == null) response.faces = new List<SearchFacesResponseBackEnd.Face>();
                                    SearchFacesResponseBackEnd.Face face = new SearchFacesResponseBackEnd.Face() { faceImage = "data:image/jpg;base64," + Convert.ToBase64String(sf.DetectedFaces[i]), faceId = faceId };
                                    response.faces.Add(face);
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                    else
                    {
                        if (sessionInfo != null) log.Debug(String.Format("SearchFaces. Request. Id={0}. Bad image. Session info: {1}.", response.requestId, sessionInfo.Replace("\n", "; ")));
                        else log.Debug(String.Format("SearchFaces. Request. Id={0}. Bad image. Session info: NULL.", response.requestId));
                        response.faces = null;
                        log.Debug(String.Format("SearchFaces. Response. Id={0}. Null result.", response.requestId));
                    }
                }
            }
            else
            {
                log.Debug("SearchFaces. Null request.");
                response.requestId = Guid.NewGuid().ToString();
                response.faces = null;
            }
        }


        public SearchFaces(byte[] photoArray, int[] additionalRotateAngles = null, int photoJpegQuality = 95, double faceClippingRatio = 1.2)
        {
            if (photoArray != null)
            {
                try
                {
                    using (Stream input = new MemoryStream(photoArray))
                    {
                        photoArray = null;
                        using (var inputStream = new SKManagedStream(input))
                        using (var codec = SKCodec.Create(inputStream))
                        using (var original = SKBitmap.Decode(codec))
                        {
                            // ЧИТАЕМ EXIF-ИНФОРМАЦИЮ
                            input.Position = 0;
                            try
                            {
                                using (ExifReader reader = new ExifReader(input))
                                {
                                    reader.GetTagValue(ExifTags.DateTime, out photoDateTime);
                                    reader.GetTagValue(ExifTags.BodySerialNumber, out cameraSerialNumber);
                                }
                            }
                            catch (Exception exc) { }

                            // НОРМАЛИЗУЕМ ИЗОБРАЖЕНИE ПО ВРАЩЕНИЮ
                            SKBitmap normalized = AdjustOrientation(original, codec.EncodedOrigin);
                            double scaleFactor = 2;

                            // ПОЛУЧАЕМ ДЕТЕКТИРУЕМОЕ НА ЛИЦА ИЗОБРАЖЕНИЕ
                            using (var scanned = normalized.Resize(new SKImageInfo((int)Math.Round((double)normalized.Width / scaleFactor), (int)Math.Round((double)normalized.Height / scaleFactor)), SKFilterQuality.High))
                            {
                                if (scanned == null) return;

                                int additionalFacesCounter = 0;

                                List<FaceLocation> faceLocationList = new List<FaceLocation>();

                                using (var fd = Dlib.GetFrontalFaceDetector())
                                {
                                    DlibDotNet.Rectangle[] faces = null;
                                    using (var array2D = Dlib.LoadImageData<RgbPixel>(ImagePixelFormat.Rgba, scanned.Bytes, (uint)scanned.Height, (uint)scanned.Width, (uint)(scanned.Bytes.Length / scanned.Height)))
                                        faces = fd.Operator(array2D);

                                    if (faces != null && faces.Length > 0)
                                    {
                                        for (int f = 0; f < faces.Length; f++)
                                        {
                                            #region обрезаем лицо до квадрата
                                            Point center = faces[f].Center;
                                            int radius = 0;
                                            if (faces[f].Width < faces[f].Height) radius = (int)faces[f].Width / 2; else radius = (int)faces[f].Height / 2;
                                            faces[f].Left = center.X - radius;
                                            faces[f].Right = center.X + radius;
                                            faces[f].Top = center.Y - radius;
                                            faces[f].Bottom = center.Y + radius;
                                            #endregion обрезаем лицо до квадрата
                                            FaceLocation faceLocation = CalculateNormalFaceLocation(faces[f], normalized.Width, normalized.Height, scaleFactor, faceClippingRatio);
                                            faceLocationList.Add(faceLocation);
                                        }
                                    }

                                    if (additionalRotateAngles != null && additionalRotateAngles.Length > 0)
                                    {
                                        for (int r = 0; r < additionalRotateAngles.Length; r++)
                                        {
                                            if (additionalRotateAngles[r] != 0)
                                            {
                                                DlibDotNet.Rectangle[] addFaces = null;
                                                SKBitmap rotatedScanned = Rotate(scanned, additionalRotateAngles[r]);
                                                using (var array2D = Dlib.LoadImageData<RgbPixel>(ImagePixelFormat.Rgba, rotatedScanned.Bytes, (uint)rotatedScanned.Height, (uint)rotatedScanned.Width, (uint)(rotatedScanned.Bytes.Length / rotatedScanned.Height)))
                                                    addFaces = fd.Operator(array2D);

                                                if (addFaces != null && addFaces.Length > 0)
                                                {
                                                    for (int i = 0; i < addFaces.Length; i++)
                                                    {
                                                        #region обрезаем лицо до квадрата
                                                        Point center = addFaces[i].Center;
                                                        int radius = 0;
                                                        if (addFaces[i].Width < addFaces[i].Height) radius = (int)addFaces[i].Width / 2; else radius = (int)addFaces[i].Height / 2;
                                                        addFaces[i].Left = center.X - radius;
                                                        addFaces[i].Right = center.X + radius;
                                                        addFaces[i].Top = center.Y - radius;
                                                        addFaces[i].Bottom = center.Y + radius;
                                                        #endregion обрезаем лицо до квадрата
                                                        FaceLocation faceLocation = CalculateRotatedFaceLocation((double)rotatedScanned.Width / 2, (double)rotatedScanned.Height / 2, addFaces[i], -additionalRotateAngles[r], normalized.Width, normalized.Height, scaleFactor, faceClippingRatio);
                                                        additionalFacesCounter++;
                                                        faceLocationList.Add(faceLocation);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (faceLocationList.Count > 0)
                                {
                                    List<FaceLocation> correlatedFaceList = GetCorrelatedFaceList(faceLocationList, additionalFacesCounter); // пропускаем через коррелятор лиц для избавления от дублей и уменьшения бокового наклона

                                    if (correlatedFaceList != null && correlatedFaceList.Count > 0)
                                    {
                                        for (int i = 0; i < correlatedFaceList.Count; i++)
                                        {
                                            //var cropRect = new SKRectI { Left = correlatedFaceList[i].Left, Top = correlatedFaceList[i].Top, Right = correlatedFaceList[i].Right, Bottom = correlatedFaceList[i].Bottom };
                                            var cropRect = new SKRectI();
                                            int w = correlatedFaceList[i].Right - correlatedFaceList[i].Left;
                                            int h = correlatedFaceList[i].Bottom - correlatedFaceList[i].Top;
                                            int centerX = correlatedFaceList[i].Left + w / 2;
                                            int centerY = correlatedFaceList[i].Top + h / 2;

                                            if (w > h)
                                            {
                                                cropRect.Left = centerX - h / 2;
                                                cropRect.Right = centerX + h / 2;
                                                cropRect.Top = centerY - h / 2;
                                                cropRect.Bottom = centerY + h / 2;
                                            }
                                            else if (w < h)
                                            {
                                                cropRect.Left = centerX - w / 2;
                                                cropRect.Right = centerX + w / 2;
                                                cropRect.Top = centerY - w / 2;
                                                cropRect.Bottom = centerY + w / 2;
                                            }
                                            else
                                            {
                                                cropRect.Left = correlatedFaceList[i].Left;
                                                cropRect.Top = correlatedFaceList[i].Top;
                                                cropRect.Right = correlatedFaceList[i].Right;
                                                cropRect.Bottom = correlatedFaceList[i].Bottom;
                                            }

                                            var faceBitmap = new SKBitmap(cropRect.Width, cropRect.Height);
                                            normalized.ExtractSubset(faceBitmap, cropRect);

                                            //// ТЕПЕРЬ БУДЕМ ПОВОРАЧИВАТЬ
                                            SKBitmap rotated = Rotate(faceBitmap, -correlatedFaceList[i].Angle);

                                            // ТЕПЕРЬ НАКЛАДЫВАЕМ МАСКУ НА ЛИЦО В ВИДЕ КРУГА
                                            double radius = 0;
                                            if (cropRect.Width < cropRect.Height) radius = (double)cropRect.Width / 2 * (1 + 0.5 / 2);
                                            else radius = (double)cropRect.Height / 2 * (1 + 0.5 / 2);

                                            using (SKCanvas canvas = new SKCanvas(rotated))
                                            {
                                                canvas.DrawBitmap(rotated, 0, 0);
                                                SKPaint paint = new SKPaint();
                                                paint.Color = maskColor;
                                                paint.Style = SKPaintStyle.Stroke;
                                                paint.StrokeWidth = (float)(radius * 0.4);

                                                canvas.DrawCircle((float)rotated.Width / 2, (float)rotated.Height / 2, (float)radius, paint);
                                                canvas.Flush();
                                            }

                                            // ВЫРЕЗАЕМ ИТОГ
                                            double x = (double)rotated.Width / 2;
                                            double y = (double)rotated.Height / 2;
                                            var finalCropRect = new SKRectI { Left = (int)(x - (double)faceBitmap.Width / 2), Top = (int)(y - (double)faceBitmap.Height / 2), Right = (int)(x + (double)faceBitmap.Width / 2), Bottom = (int)(y + (double)faceBitmap.Height / 2) };
                                            faceBitmap.Dispose();
                                            using (SKBitmap face = new SKBitmap(finalCropRect.Width, finalCropRect.Height))
                                            {
                                                rotated.ExtractSubset(face, finalCropRect);
                                                try
                                                {
                                                    if (face.Width > 600 * scaleFactor)
                                                    {
                                                        using (var scaled = face.Resize(new SKImageInfo((int)Math.Round(400 * scaleFactor), (int)Math.Round(400 * scaleFactor)), SKFilterQuality.High))
                                                        {
                                                            if (scaled != null)
                                                            {
                                                                using (var image = SKImage.FromBitmap(scaled))
                                                                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 90))
                                                                {
                                                                    if (facesByteArrays == null) facesByteArrays = new List<byte[]>();
                                                                    facesByteArrays.Add(data.ToArray());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        using (var image = SKImage.FromBitmap(face))
                                                        using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 90))
                                                        {
                                                            if (facesByteArrays == null) facesByteArrays = new List<byte[]>();
                                                            facesByteArrays.Add(data.ToArray());
                                                        }
                                                    }
                                                }
                                                catch (Exception exc) { };
                                            }
                                        }
                                        normalized.Dispose();
                                        correlatedFaceList = null;
                                    }
                                    faceLocationList = null;
                                }
                            }
                        }
                        isSuccess = true;
                    }
                }
                catch (Exception exc)
                {
                    try
                    {
                        isSuccess = false;
                        if (exc.StackTrace != null && exc.StackTrace != "") log.Debug(String.Format("SearchFaces. Exception: {0}", exc.StackTrace));
                        else if (exc.Message != null && exc.Message != "") log.Debug(String.Format("SearchFaces. Exception: {0}", exc.Message));
                    }
                    catch (Exception ex) { }
                }
            }
            else
            {
                log.Debug("SearchFaces. Null request.");
            }
        }

        FaceLocation CalculateRotatedFaceLocation(double baseX, double baseY, Rectangle faceRect, double angle, int origWidth, int origHeight, double scaleFactor, double faceClippingRatio)
        {
            FaceLocation faceLocation = new FaceLocation();
            double resultX = (faceRect.Center.X - baseX) * Math.Cos(angle * Math.PI / 180) - (faceRect.Center.Y - baseY) * Math.Sin(angle * Math.PI / 180) + baseX;
            double resultY = (faceRect.Center.X - baseX) * Math.Sin(angle * Math.PI / 180) + (faceRect.Center.Y - baseY) * Math.Cos(angle * Math.PI / 180) + baseY;
            double distanceToCenterX = (baseX - resultX) * scaleFactor;
            double distanceToCenterY = (baseY - resultY) * scaleFactor;
            double halhOfWidth = Math.Round((faceRect.Right - faceRect.Left) * scaleFactor * faceClippingRatio / 2);
            double halfOfHeight = Math.Round((faceRect.Bottom - faceRect.Top) * scaleFactor * faceClippingRatio / 2);
            double delta = 0; if (halhOfWidth > halfOfHeight) delta = halhOfWidth; else delta = halfOfHeight;
            faceLocation.Angle = angle;
            faceLocation.Left = (int)Math.Round((double)origWidth / 2 - distanceToCenterX - delta); if (faceLocation.Left < 0) faceLocation.Left = 0;
            faceLocation.Right = (int)Math.Round((double)origWidth / 2 - distanceToCenterX + delta); if (faceLocation.Right >= origWidth) faceLocation.Right = origWidth - 1;
            faceLocation.Top = (int)Math.Round((double)origHeight / 2 - distanceToCenterY - delta); if (faceLocation.Top < 0) faceLocation.Top = 0;
            faceLocation.Bottom = (int)Math.Round((double)origHeight / 2 - distanceToCenterY + delta); if (faceLocation.Bottom >= origHeight) faceLocation.Bottom = origHeight - 1;
            return faceLocation;
        }

        FaceLocation CalculateNormalFaceLocation(Rectangle faceRect, int origWidth, int origHeight, double scaleFactor, double faceClippingRatio)
        {
            FaceLocation faceLocation = new FaceLocation();
            double halhOfWidth = Math.Round((faceRect.Right - faceRect.Left) * scaleFactor * faceClippingRatio / 2);
            double halfOfHeight = Math.Round((faceRect.Bottom - faceRect.Top) * scaleFactor * faceClippingRatio / 2);
            double delta = 0; if (halhOfWidth > halfOfHeight) delta = halhOfWidth; else delta = halfOfHeight;
            faceLocation.Angle = 0;
            faceLocation.Left = (int)Math.Round(faceRect.Center.X * scaleFactor - delta); if (faceLocation.Left < 0) faceLocation.Left = 0;
            faceLocation.Right = (int)Math.Round(faceRect.Center.X * scaleFactor + delta); if (faceLocation.Right >= origWidth) faceLocation.Right = origWidth - 1;
            faceLocation.Top = (int)Math.Round(faceRect.Center.Y * scaleFactor - delta); if (faceLocation.Top < 0) faceLocation.Top = 0;
            faceLocation.Bottom = (int)Math.Round(faceRect.Center.Y * scaleFactor + delta); if (faceLocation.Bottom >= origHeight) faceLocation.Bottom = origHeight - 1;
            return faceLocation;
        }

        int GetRectanglesIntersectionSquare(FaceLocation a, FaceLocation b)
        {
            int xIntersection = GetSegmentsIntersectionLength(a.Left, a.Right, b.Left, b.Right);
            int yIntersection = GetSegmentsIntersectionLength(a.Top, a.Bottom, b.Top, b.Bottom);

            return xIntersection * yIntersection;
        }

        int GetSegmentsIntersectionLength(int aLeft, int aRight, int bLeft, int bRight)
        {
            int left = Math.Max(aLeft, bLeft);
            int right = Math.Min(aRight, bRight);

            return Math.Max(right - left, 0);
        }

        List<FaceLocation> GetCorrelatedFaceList(List<FaceLocation> faceLocationList, int additionalFacesCounter)
        {
            if (faceLocationList == null || faceLocationList.Count == 0) return null;

            if (additionalFacesCounter > 0) // дополнительные лица, найденные в процессе дополнительного вращения. Есть смысл коррелировать
            {
                if (faceLocationList.Count == 1) return faceLocationList; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! надо обязательно повернуть на нужный угол !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                else
                {
                    List<List<FaceLocation>> preCorrelatedList = new List<List<FaceLocation>>();
                    List<FaceLocation> correlatedList = new List<FaceLocation>();

                    for (int i = 0; i < faceLocationList.Count; i++)
                    {
                        if (preCorrelatedList.Count == 0)
                        {
                            List<FaceLocation> list = new List<FaceLocation>();
                            list.Add(faceLocationList[i]);
                            preCorrelatedList.Add(list);
                        }
                        else
                        {
                            bool isCorrelated = false;
                            for (int listId = 0; listId < preCorrelatedList.Count; listId++)
                            {
                                int square = GetRectanglesIntersectionSquare(preCorrelatedList[listId][0], faceLocationList[i]);
                                if (square > 0) // пересекаются
                                {
                                    if (square > ((preCorrelatedList[listId][0].Right - preCorrelatedList[listId][0].Left) * (preCorrelatedList[listId][0].Bottom - preCorrelatedList[listId][0].Top)) * 0.4)
                                    {
                                        preCorrelatedList[listId].Add(faceLocationList[i]);
                                        isCorrelated = true;
                                        break;
                                    }

                                }
                            }
                            if (!isCorrelated)
                            {
                                List<FaceLocation> list = new List<FaceLocation>();
                                list.Add(faceLocationList[i]);
                                preCorrelatedList.Add(list);
                            }
                        }
                    }

                    if (preCorrelatedList.Count > 0)
                    {
                        for (int preList = 0; preList < preCorrelatedList.Count; preList++)
                        {
                            if (preCorrelatedList[preList] != null && preCorrelatedList[preList].Count > 0)
                            {
                                if (preCorrelatedList[preList].Count == 1) correlatedList.Add(preCorrelatedList[preList][0]);
                                else
                                {
                                    double angelValue = 0;
                                    int left = 0;
                                    int right = 0;
                                    int top = 0;
                                    int bottom = 0;

                                    for (int t = 0; t < preCorrelatedList[preList].Count; t++)
                                    {
                                        angelValue = angelValue + preCorrelatedList[preList][t].Angle;
                                        left = left + preCorrelatedList[preList][t].Left;
                                        right = right + preCorrelatedList[preList][t].Right;
                                        top = top + preCorrelatedList[preList][t].Top;
                                        bottom = bottom + preCorrelatedList[preList][t].Bottom;
                                    }
                                    FaceLocation avg = new FaceLocation()
                                    {
                                        Angle = angelValue / preCorrelatedList[preList].Count,
                                        Left = (int)((double)left / preCorrelatedList[preList].Count),
                                        Right = (int)((double)right / preCorrelatedList[preList].Count),
                                        Top = (int)((double)top / preCorrelatedList[preList].Count),
                                        Bottom = (int)((double)bottom / preCorrelatedList[preList].Count)
                                    };
                                    correlatedList.Add(avg);
                                }
                            }
                        }
                    }
                    return correlatedList;
                }
            }
            else // нет смысла коррелировать лица - бесполезно. Все лица из фотографии без вращения
            {
                return faceLocationList;
            }
        }

        /// <summary> Метод восстановления корректной ориентации фотографии. </summary>
        /// <param name="bitmap"> Корректируемое по вращению изображение. </param>
        /// <param name="orientation"> Величина вращения изображения. </param>
        /// <returns></returns>
        SKBitmap AdjustOrientation(SKBitmap bitmap, SKEncodedOrigin orientation)
        {
            SKBitmap rotated;
            switch (orientation)
            {
                case SKEncodedOrigin.BottomRight:
                    using (var surface = new SKCanvas(bitmap))
                    {
                        surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        surface.DrawBitmap(bitmap.Copy(), 0, 0);
                    }
                    return bitmap;
                case SKEncodedOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(rotated.Width, 0);
                        surface.RotateDegrees(90);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                case SKEncodedOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(0, rotated.Height);
                        surface.RotateDegrees(270);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                default:
                    return bitmap;
            }
        }

        SKBitmap Rotate(SKBitmap bitmap, double angle)
        {
            double radians = Math.PI * angle / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees((float)angle);
                surface.Translate(-originalWidth / 2, -originalHeight / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            }
            return rotatedBitmap;
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                try
                {

                }
                catch (Exception exc)
                {
                }
            }
            disposed = true;
        }

        ~SearchFaces() { Dispose(false); }
    }
}
