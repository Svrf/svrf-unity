using System.Collections;
using System.Linq;
using Svrf.Models.Http;
using Svrf.Models.Media;
using Svrf.Unity.Coroutines;
using Svrf.Unity.Models;
using UnityEngine;

namespace Svrf.Unity.Examples
{
    public class UsingCoroutines : MonoBehaviour
    {
        public IEnumerator Start()
        {
            SvrfApi api = new SvrfApi();

            TaskBasedCoroutine<MultipleMediaResponse> requestCoroutine = api.MediaCoroutines.GetTrending();
            yield return requestCoroutine;

            MediaModel model = requestCoroutine.Result.Media.First();
            SvrfModelOptions options = new SvrfModelOptions { WithOccluder = false };
            yield return SvrfModel.GetSvrfModelCoroutine(model, options);

            Destroy(GameObject.Find("Loading"));
        }
    }
}
