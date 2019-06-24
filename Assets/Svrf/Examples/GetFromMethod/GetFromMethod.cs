using System.Linq;
using Svrf.Models.Http;
using UnityEngine;

namespace Svrf.Unity.Examples
{
    public class GetFromMethod : MonoBehaviour
    {
        async void Start()
        {
            var api = new SvrfApi();

            var trendingResponse = await api.Media.GetTrendingAsync(new MediaRequestParams { IsFaceFilter = true });
            var model = trendingResponse.Media.First();

            var svrfModel = await SvrfModel.GetSvrfModel(model);
            svrfModel.transform.SetParent(transform);
        }
    }
}
