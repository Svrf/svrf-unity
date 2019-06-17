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

        public Shader OverrideShader;

        private static SvrfApi _svrf;

        public async void Start()
        {
            CreateSvrfInstance();

            var model = (await _svrf.Media.GetByIdAsync(Id)).Media;
            var options = new SvrfModelOptions
            {
                OverrideShader = OverrideShader,
                WithOccluder = WithOccluder
            };

            await SvrfModelUtility.AddSvrfModel(gameObject, model, options);
        }

        public static async Task<GameObject> GetSvrfGameObject(MediaModel model, SvrfModelOptions options)
        {
            var svrfGameObject = new GameObject();
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
