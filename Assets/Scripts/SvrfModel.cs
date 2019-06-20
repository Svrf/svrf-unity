﻿using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Svrf.Models.Media;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SvrfModel : MonoBehaviour
    {
        public string Id;

        public bool WithOccluder = DefaultOptions.WithOccluder;
        public Shader ShaderOverride = DefaultOptions.ShaderOverride;

        private static SvrfApi _svrf;
        private static readonly SvrfModelOptions DefaultOptions = new SvrfModelOptions()
        {
            ShaderOverride = null,
            WithOccluder = true,
        };

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

        public static async Task<GameObject> GetSvrfModel(MediaModel model, SvrfModelOptions options = null, GameObject gameObject = null)
        {
            // It's impossible to use null coalescing operator with Unity objects.
            gameObject = gameObject == null ? new GameObject("SvrfModel") : gameObject;
            options = options ?? DefaultOptions;

            await SvrfModelUtility.AddSvrfModel(gameObject, model, options);
            return gameObject;
        }

        private static void CreateSvrfInstance()
        {
            if (_svrf == null)
            {
                _svrf = new SvrfApi();
            }
        }
    }
}
