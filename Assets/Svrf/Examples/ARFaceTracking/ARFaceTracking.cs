using System.Linq;
using Svrf.Models.Http;
using Svrf.Models.Media;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Svrf.Unity.Examples
{
    public class ARFaceTracking : MonoBehaviour
    {
        [SerializeField] private string apiKey = "your api key";
        async void Start()
        {
            SvrfApiKey.Value = apiKey;

            SvrfApi api = new SvrfApi();

            MediaRequestParams options = new MediaRequestParams {IsFaceFilter = true};
            MultipleMediaResponse trendingResponse = await api.Media.GetTrendingAsync(options);
            MediaModel model = trendingResponse.Media.First();

            GameObject svrfModel = await SvrfModel.GetSvrfModelAsync(model);

            ARFaceManager faceManager = GameObject.FindObjectOfType<ARFaceManager>();
            faceManager.facePrefab = svrfModel;
       
            Destroy(GameObject.Find("Loading"));
        }
    }
}
