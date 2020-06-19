using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class ThumbnailsSearchGenerator
    {

        List<Photo> result;
        public List<Photo> Result { get { return result; } }

        public ThumbnailsSearchGenerator(Dictionary<Guid, SearchedThumbnails> legalPhotos, List<Guid> sortedOrderIds, Source source = Source.Cloud)
        {
            if (legalPhotos == null || legalPhotos.Count == 0) // фотки не найдены или их отображение запрещено
            {
                legalPhotos = null;
                sortedOrderIds = null;
                result = null;
            }
            else
            {
                if (sortedOrderIds != null && sortedOrderIds.Count > 0)
                {
                    for (int i = 0; i < sortedOrderIds.Count; i++)
                    {
                        // сюда надо записать извлечение миниатюры и наложение размытости на отписавшиеся лица
                        if (legalPhotos.ContainsKey(sortedOrderIds[i]))
                        {
                            if (legalPhotos[sortedOrderIds[i]].rect != null && legalPhotos[sortedOrderIds[i]].rect.Count > 0) // здесь нужна замутовка лиц
                            {
                                BlurFace blurFace = null;
                                if (source == Source.Local) blurFace = new BlurFace(legalPhotos[sortedOrderIds[i]].thumbnail_local_path, 3, 100, legalPhotos[sortedOrderIds[i]].rect, 90);
                                else blurFace = new BlurFace(legalPhotos[sortedOrderIds[i]].thumbnail_cloud_id, legalPhotos[sortedOrderIds[i]].rect, 90);

                                if (blurFace != null && blurFace.ResultBase64 != null)
                                {
                                    Photo photo = new Photo()
                                    {
                                        photoProcessId = legalPhotos[sortedOrderIds[i]].order_id.ToString(),
                                        dateTime = String.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}", legalPhotos[sortedOrderIds[i]].shooting_datetime),
                                        thumbnailLink = blurFace.ResultBase64
                                    };

                                    if (result == null) result = new List<Photo>();
                                    result.Add(photo);
                                }
                                blurFace = null;
                            }
                            else // замутовка лиц не нужна
                            { 
                                if (source == Source.Cloud && legalPhotos[sortedOrderIds[i]].thumbnail_cloud_id != null && legalPhotos[sortedOrderIds[i]].thumbnail_cloud_id != "")
                                {
                                    Photo photo = new Photo()
                                    {
                                        photoProcessId = legalPhotos[sortedOrderIds[i]].order_id.ToString(),
                                        dateTime = String.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}", legalPhotos[sortedOrderIds[i]].shooting_datetime),
                                        thumbnailLink = String.Format("https://drive.google.com/uc?export=view&id={0}", legalPhotos[sortedOrderIds[i]].thumbnail_cloud_id)
                                    };
                                    if (result == null) result = new List<Photo>();
                                    result.Add(photo);
                                }
                                else if (source == Source.Local && legalPhotos[sortedOrderIds[i]].thumbnail_local_path != null && System.IO.File.Exists(legalPhotos[sortedOrderIds[i]].thumbnail_local_path))
                                {
                                    // загружаем с локального хранилища
                                    int retries = 5;
                                    const int delay = 100;
                                    bool fileIsReady = false;
                                    FileStream input = null;
                                    do //https://stackoverflow.com/questions/21739242/filestream-and-a-filesystemwatcher-in-c-weird-issue-process-cannot-access-the
                                    {
                                        try
                                        {
                                            input = File.OpenRead(legalPhotos[sortedOrderIds[i]].thumbnail_local_path);
                                            fileIsReady = true; // success
                                        }
                                        catch (IOException) { Console.WriteLine("Ooops"); }
                                        retries--;
                                        if (!fileIsReady) Thread.Sleep(delay);
                                    }
                                    while (!fileIsReady && retries > 0);

                                    if (input != null)
                                    {
                                        input.Position = 0;
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            input.CopyTo(ms);
                                            input = null;
                                            Photo photo = new Photo()
                                            {
                                                photoProcessId = legalPhotos[sortedOrderIds[i]].order_id.ToString(),
                                                dateTime = String.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}", legalPhotos[sortedOrderIds[i]].shooting_datetime),
                                                thumbnailLink = "data:image/jpg;base64," + Convert.ToBase64String(ms.ToArray())
                                            };
                                            if (result == null) result = new List<Photo>();
                                            result.Add(photo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    legalPhotos = null;
                    sortedOrderIds = null;
                }
                else
                {
                    legalPhotos = null;
                    sortedOrderIds = null;
                    result = null;
                }
            }
        }

    }
}
