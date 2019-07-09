using System.Linq;
using Svrf.Models.Http;
using Svrf.Models.Media;
using UnityEngine;

namespace Svrf.Unity.Examples
{
    public class SetApiKeyInCode : MonoBehaviour
    {
        private static SvrfApi _svrfApi;

        async void Start()
        {
            SvrfApiKey.Value = "your key";

            _svrfApi = new SvrfApi();

            MediaRequestParams options = new MediaRequestParams {IsFaceFilter = true};
            MultipleMediaResponse trendingResponse = await _svrfApi.Media.GetTrendingAsync(options);
            MediaModel model = trendingResponse.Media.First();

            GameObject svrfModel = await SvrfModel.GetSvrfModelAsync(model);

            Destroy(GameObject.Find("Loading"));
        }
    }
}
