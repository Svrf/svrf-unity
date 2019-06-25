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

            var options = new MediaRequestParams {IsFaceFilter = true};
            var trendingResponse = await api.Media.GetTrendingAsync(options);
            var model = trendingResponse.Media.First();

            await SvrfModel.GetSvrfModelAsync(model);
        }
    }
}
