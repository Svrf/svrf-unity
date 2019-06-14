using Assets.Scripts.Models;
using Svrf.Models.Media;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF;

namespace Assets.Scripts.Utilities
{
    public static class SvrfModelUtility
    {
        public static async Task AddSvrfModel(GameObject gameObject, MediaModel model, SvrfModelOptions options)
        {
            var gltfComponent = gameObject.AddComponent<GLTFComponent>();

            gltfComponent.GLTFUri = model.GetMainGltfFile();

            var gltfShader = gltfComponent.GetType().GetField("shaderOverride", BindingFlags.NonPublic | BindingFlags.Instance);

            var shader = options.OverrideShader ?? Shader.Find("Custom/FaceFilterShader");
            gltfShader.SetValue(gltfComponent, shader);

            await gltfComponent.Load();

            var occluder = gltfComponent.gameObject.transform.GetChild(0).GetChild(0).Find("Occluder");

            if (occluder == null)
            {
                return;
            }

            if (options.WithOccluder)
            {
                var meshRenderer = occluder.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
                meshRenderer.sharedMaterials[0].shader = Shader.Find("Custom/OccluderShader");
            }
            else
            {
                occluder.gameObject.SetActive(false);
            }
        }
    }
}
