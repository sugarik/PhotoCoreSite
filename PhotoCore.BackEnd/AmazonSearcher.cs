using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class AmazonSearcher
    {
        List<string> dates = null;
        string awsAccessKeyId;
        string awsSecretAccessKey;
        Amazon.RegionEndpoint awsRegionEndpoint;
        string awsCollectionId;
        float awsFaceMatchThreshold;
        float awsSimilarityLevel;

        object locker = new object();

        List<Guid>[] searchedFaceIds = null;
        public List<Guid>[] SearchedFaceIds { get { return searchedFaceIds; } }

        public AmazonSearcher(List<string> faceFileNames, List<string> dates, ProjectConfigData configData)
        {
            this.dates = dates;
            this.awsAccessKeyId = configData.awsAccessKeyId;
            this.awsSecretAccessKey = configData.awsSecretAccessKey;
            this.awsRegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(configData.awsEndpoint);//("eu-west-1");
            this.awsCollectionId = configData.awsCollectionId;
            this.awsFaceMatchThreshold = configData.awsFaceMatchThreshold;
            this.awsSimilarityLevel = configData.awsSimilarityLevel;

            if (faceFileNames.Count > 1)
            {
                var tasks = new List<Task<List<Guid>>>();

                for (int i = 0; i < faceFileNames.Count; i++)
                    tasks.Add(Task.Run(async () => SearchOneFace(faceFileNames[i])));

                searchedFaceIds = Task.WhenAll(tasks).Result; // количество списков равное кол-ву поисковых лиц

            }
            else searchedFaceIds = new List<Guid>[] { SearchOneFace(faceFileNames[0])};
        }

        private List<Guid> SearchOneFace(string faceFileName)
        {
            List<Guid> facesData = null;
            try
            {
                Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image() { Bytes = new MemoryStream(System.IO.File.ReadAllBytes(faceFileName)) };
                SearchFacesByImageRequest searchFacesByImageRequest = new SearchFacesByImageRequest() { CollectionId = awsCollectionId, Image = image, FaceMatchThreshold = awsFaceMatchThreshold, MaxFaces = 1000 };
                using (AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(awsAccessKeyId, awsSecretAccessKey, awsRegionEndpoint))
                {
                    SearchFacesByImageResponse searchFacesByImageResponse = rekognitionClient.SearchFacesByImageAsync(searchFacesByImageRequest).Result;
                    if (searchFacesByImageResponse != null && searchFacesByImageResponse.FaceMatches.Count > 0)
                    {
                        facesData = new List<Guid>();
                        for (int f = 0; f < searchFacesByImageResponse.FaceMatches.Count; f++)
                        {
                            string dateMask = searchFacesByImageResponse.FaceMatches[f].Face.ExternalImageId;
                            if (dateMask.Length > 7)
                            {
                                dateMask = dateMask.Substring(0, 8);
                                if (dates != null && dates.Contains(dateMask))
                                {
                                    if (searchFacesByImageResponse.FaceMatches[f].Similarity >= awsSimilarityLevel)
                                    {
                                        Guid faceId;
                                        if (Guid.TryParse(searchFacesByImageResponse.FaceMatches[f].Face.FaceId, out faceId))
                                        {
                                            if (!facesData.Contains(faceId)) facesData.Add(faceId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {

            }

            if (facesData != null && facesData.Count == 0) facesData = null;
            return facesData;
        }

        //public class FaceMatchResult
        //{
        //    public float Similarity { get; set; }
        //    public float Confidence { get; set; }
        //    public string ExternalImageId { get; set; }
        //}

        //public AmazonSearcher(List<string> faceFileNames, List<string> dates, ProjectConfigData configData)
        //{
        //    this.dates = dates;
        //    this.awsAccessKeyId = configData.awsAccessKeyId;
        //    this.awsSecretAccessKey = configData.awsSecretAccessKey;
        //    this.awsRegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(configData.awsEndpoint);//("eu-west-1");
        //    this.awsCollectionId = configData.awsCollectionId;
        //    this.awsFaceMatchThreshold = configData.awsFaceMatchThreshold;
        //    this.awsSimilarityLevel = configData.awsSimilarityLevel;

        //    if (faceFileNames.Count > 1)
        //    {
        //        searchedFacesData = new Dictionary<string, FaceMatchResult>();
        //        var tasks = new List<Task<Dictionary<string, FaceMatchResult>>>();

        //        for (int i = 0; i < faceFileNames.Count; i++)
        //            tasks.Add(Task.Run(async () => SearchOneFace(faceFileNames[i])));

        //        var results = Task.WhenAll(tasks).Result;

        //        for (int i = 0; i < results.Length; i++) // по таскам
        //        {
        //            if (results != null && results.Length > 0)
        //            {
        //                foreach (var faceMatch in results[i])
        //                {
        //                    if (!searchedFacesData.ContainsKey(faceMatch.Key) && faceMatch.Value != null)
        //                        searchedFacesData.Add(faceMatch.Key, faceMatch.Value);
        //                }
        //            }
        //        }
        //    }
        //    else searchedFacesData = SearchOneFace(faceFileNames[0]);
        //}

        //private Dictionary<string, FaceMatchResult> SearchOneFace(string faceFileName)
        //{
        //    Dictionary<string, FaceMatchResult> facesData = null;
        //    try
        //    {
        //        Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image() { Bytes = new MemoryStream(System.IO.File.ReadAllBytes(faceFileName)) };
        //        SearchFacesByImageRequest searchFacesByImageRequest = new SearchFacesByImageRequest() { CollectionId = awsCollectionId, Image = image, FaceMatchThreshold = awsFaceMatchThreshold, MaxFaces = 1000 };
        //        using (AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(awsAccessKeyId, awsSecretAccessKey, awsRegionEndpoint))
        //        {
        //            SearchFacesByImageResponse searchFacesByImageResponse = rekognitionClient.SearchFacesByImageAsync(searchFacesByImageRequest).Result;
        //            if (searchFacesByImageResponse != null && searchFacesByImageResponse.FaceMatches.Count > 0)
        //            {
        //                facesData = new Dictionary<string, FaceMatchResult>();
        //                for (int f = 0; f < searchFacesByImageResponse.FaceMatches.Count; f++)
        //                {
        //                    string dateMask = searchFacesByImageResponse.FaceMatches[f].Face.ExternalImageId;
        //                    if (dateMask.Length > 7)
        //                    {
        //                        dateMask = dateMask.Substring(0, 8);
        //                        if (dates != null && dates.Contains(dateMask))
        //                        {
        //                            if (searchFacesByImageResponse.FaceMatches[f].Similarity >= awsSimilarityLevel && !facesData.ContainsKey(searchFacesByImageResponse.FaceMatches[f].Face.FaceId))
        //                                facesData.Add(searchFacesByImageResponse.FaceMatches[f].Face.FaceId, new FaceMatchResult()
        //                                {
        //                                    Confidence = searchFacesByImageResponse.FaceMatches[f].Face.Confidence,
        //                                    ExternalImageId = searchFacesByImageResponse.FaceMatches[f].Face.ExternalImageId,
        //                                    Similarity = searchFacesByImageResponse.FaceMatches[f].Similarity
        //                                });
        //                        }
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

    }
}
