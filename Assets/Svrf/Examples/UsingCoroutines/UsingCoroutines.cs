using System.Collections;
using System.Linq;
using Svrf.Models.Media;
using Svrf.Unity.Models;
using UnityEngine;

namespace Svrf.Unity.Examples
{
    public class UsingCoroutines : MonoBehaviour
    {
        public void Start()
        {
            StartCoroutine(LoadModel());
        }

        public IEnumerator LoadModel()
        {
            SvrfApi api = new SvrfApi();

            var multipleResponse = api.MediaForCoroutine.GetTrending();
            yield return multipleResponse;

            MediaModel model = multipleResponse.Response.Media.First();
            SvrfModelOptions options = new SvrfModelOptions {WithOccluder = false};
            yield return SvrfModel.GetSvrfModelYieldInstruction(model, options);

            Destroy(GameObject.Find("Loading"));
        }
    }
}
