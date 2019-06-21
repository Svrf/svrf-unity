using System.Linq;
using Svrf.Models.Http;
using UnityEngine;

namespace Svrf.Unity.Examples
{
    public class GetFromMethod : MonoBehaviour
    {
        private static SvrfApi _svrfApi;

        async void Start()
        {
            _svrfApi = new SvrfApi();

            var trendingResponse = await _svrfApi.Media.GetTrendingAsync(new MediaRequestParams { IsFaceFilter = true });
            var model = trendingResponse.Media.First();

            var svrfModel = await SvrfModel.GetSvrfModel(model);
            svrfModel.transform.SetParent(transform);
        }
    }
}
