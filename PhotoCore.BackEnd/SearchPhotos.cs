using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class SearchPhotos : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchFaces));

        bool disposed = false;
        /// <summary> список дат поиска с гарантированно корректным форматом </summary>
        List<string> dates = null;
        string awsAccessKeyId;
        string awsSecretAccessKey;
        Amazon.RegionEndpoint awsRegionEndpoint;
        string awsCollectionId = "lebyazhiy";
        float awsFaceMatchThreshold;

        SearchPhotosResponseBackEnd response = null;
        public SearchPhotosResponseBackEnd Response { get { return response; } }

        public SearchPhotos(SearchPhotosRequestFrontEnd request, string sessionInfo, string faceDir, ProjectConfigData configData)
        {
            response = new SearchPhotosResponseBackEnd();

            List<Guid>[] searchedFaceIds = null;

            this.awsAccessKeyId = configData.awsAccessKeyId;
            this.awsSecretAccessKey = configData.awsSecretAccessKey;
            this.awsRegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(configData.awsEndpoint);//("eu-west-1");
            this.awsCollectionId = configData.awsCollectionId;
            this.awsFaceMatchThreshold = configData.awsFaceMatchThreshold;

            if (request != null)
            {
                if (request.requestId == null) response.requestId = Guid.NewGuid().ToString(); else response.requestId = request.requestId;

                if (request.faces == null || request.faces.Count == 0)
                {
                    if (sessionInfo != null) log.Debug(String.Format("SearchPhotos. Request. Id={0}. Null faces. Session info: {1}.", response.requestId, sessionInfo.Replace("\n", "; ")));
                    else log.Debug(String.Format("SearchPhotos. Request. Id={0}. Null faces. Session info: NULL.", response.requestId));
                    response.photos = null;
                    log.Debug(String.Format("SearchPhotos. Response. Id={0}. Null result. Reason = \"No faces\".", response.requestId));
                }
                else if (request.searchDates == null || request.searchDates.Count == 0) // пустой список дат
                {
                    if (sessionInfo != null) log.Debug(String.Format("SearchPhotos. Request. Id={0}. Null dates. Session info: {1}.", response.requestId, sessionInfo.Replace("\n", "; ")));
                    else log.Debug(String.Format("SearchPhotos. Request. Id={0}. Null dates. Session info: NULL.", response.requestId));
                    response.photos = null;
                    log.Debug(String.Format("SearchPhotos. Response. Id={0}. Null result. Reason = \"No dates\".", response.requestId));
                }
                else
                {
                    dates = new List<string>();
                    for (int i = 0; i < request.searchDates.Count; i++)
                    {
                        if (request.searchDates[i] != null && request.searchDates[i].Length == 8)
                        {
                            bool isCorrect = true;
                            for (int c = 0; c < 8; c++) { if (request.searchDates[i][c] < 0x30 || request.searchDates[i][c] > 0x39) { isCorrect = false; break; } }
                            if (isCorrect) dates.Add(request.searchDates[i]);
                        }
                    }
                    if (dates.Count == 0)
                    {
                        if (sessionInfo != null) log.Debug(String.Format("SearchPhotos. Request. Id={0}. Null dates. Session info: {1}.", response.requestId, sessionInfo.Replace("\n", "; ")));
                        else log.Debug(String.Format("SearchPhotos. Request. Id={0}. Null dates. Session info: NULL.", response.requestId));
                        response.photos = null;
                        log.Debug(String.Format("SearchPhotos. Response. Id={0}. Null result. Reason = \"No dates\".", response.requestId));
                    }
                    else
                    {
                        response.photos = null;
                        List<string> faceFileNames = new List<string>();
                        for (int i = 0; i < request.faces.Count; i++)
                        {
                            string faceFile = String.Format("{0}{1}.jpg", faceDir, request.faces[i]);
                            if (System.IO.File.Exists(faceFile)) faceFileNames.Add(faceFile);
                        }

                        if (faceFileNames != null && faceFileNames.Count > 0 && dates != null && dates.Count > 0)
                        {
                            if (sessionInfo != null) log.Debug(String.Format("SearchPhotos. Request. Id={0}. Total faces={1}. Total dates={2}. Session info: {3}.", response.requestId, sessionInfo.Replace("\n", "; "), faceFileNames.Count, dates.Count));
                            else log.Debug(String.Format("SearchPhotos. Request. Id={0}. Total faces={1}. Total dates={2}. Session info: NULL.", response.requestId, faceFileNames.Count, dates.Count));
                            response.photos = null;
                            AmazonSearcher amazonSearcher = new AmazonSearcher(faceFileNames, dates, configData);
                            searchedFaceIds = amazonSearcher.SearchedFaceIds;
                            amazonSearcher = null;

                            if (searchedFaceIds != null && searchedFaceIds.Length > 0)
                            {
                                MsSqlDbExplorer mySqlDbExplorer = new MsSqlDbExplorer(new ProjectConfigData());
                                response.photos = mySqlDbExplorer.SelectFromContent(searchedFaceIds, request.locationId, dates);

                                //log.Debug(String.Format("SearchPhotos. Response. Id={0}. Null result. Reason = \"No dates\".", response.requestId));
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            // дописать логирование
                        }
                    }
                }
            }
            else
            {
                log.Debug("SearchPhotos. Null request.");
                response.requestId = Guid.NewGuid().ToString();
                response.photos = null;
            }
        }


        //private Dictionary<string, FaceMatch> SearchOneFace(string faceFileName)
        //{
        //    Dictionary<string, FaceMatch> facesData = null;
        //    try
        //    {
        //        AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(awsAccessKeyId, awsSecretAccessKey, awsRegionEndpoint);
        //        Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image() { Bytes = new MemoryStream(System.IO.File.ReadAllBytes(faceFileName)) };
        //        SearchFacesByImageRequest searchFacesByImageRequest = new SearchFacesByImageRequest() { CollectionId = awsCollectionId, Image = image, FaceMatchThreshold = awsFaceMatchThreshold, MaxFaces = 1000 };
        //        SearchFacesByImageResponse searchFacesByImageResponse = rekognitionClient.SearchFacesByImageAsync(searchFacesByImageRequest).Result;
        //        if (searchFacesByImageResponse != null && searchFacesByImageResponse.FaceMatches.Count > 0)
        //        {
        //            facesData = new Dictionary<string, FaceMatch>();
        //            for (int f = 0; f < searchFacesByImageResponse.FaceMatches.Count; f++)
        //            {
        //                string dateMask = searchFacesByImageResponse.FaceMatches[f].Face.ExternalImageId;
        //                if (dateMask.Length > 7)
        //                {
        //                    dateMask = dateMask.Substring(0, 8);
        //                    if (dates != null && dates.Contains(dateMask))
        //                    {
        //                        if (!facesData.ContainsKey(searchFacesByImageResponse.FaceMatches[f].Face.FaceId))
        //                            facesData.Add(searchFacesByImageResponse.FaceMatches[f].Face.FaceId, searchFacesByImageResponse.FaceMatches[f]);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {

        //    }
        //    if (facesData != null && facesData.Count == 0) facesData = null;
        //    return facesData;
        //}

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                try
                {
                    dates = null;
                }
                catch (Exception exc)
                {
                }
            }
            disposed = true;
        }

        ~SearchPhotos() { Dispose(false); }
    }

}
