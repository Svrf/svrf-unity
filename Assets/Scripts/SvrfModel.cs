using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Svrf.Models.Media;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SvrfModel : MonoBehaviour
    {
        public string Id;
        public bool WithOccluder = true;

        public Shader ShaderOverride;

        private static SvrfApi _svrf;

        public async void Start()
        {
            CreateSvrfInstance();

            var model = (await _svrf.Media.GetByIdAsync(Id)).Media;
            var options = new SvrfModelOptions
            {
                ShaderOverride = ShaderOverride,
                WithOccluder = WithOccluder
            };

            await SvrfModelUtility.AddSvrfModel(gameObject, model, options);
        }

        public static async Task<GameObject> GetSvrfModel(MediaModel model, SvrfModelOptions options, GameObject gameObject = null)
        {
            var svrfGameObject = gameObject == null ? new GameObject() : gameObject;

            await SvrfModelUtility.AddSvrfModel(svrfGameObject, model, options);
            return svrfGameObject;
        }

        private void CreateSvrfInstance()
        {
            if (_svrf == null)
            {
                _svrf = new SvrfApi();
            }
        }
    }
}
