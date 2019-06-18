using Assets.Scripts.Models;
using Svrf.Models.Media;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityGLTF;

namespace Assets.Scripts.Utilities
{
    internal static class SvrfModelUtility
    {
        internal static async Task AddSvrfModel(GameObject gameObject, MediaModel model, SvrfModelOptions options)
        {
            var gltfComponent = gameObject.AddComponent<GLTFComponent>();

            // Do not load the model automatically: let us call .Load() method manually and await it.
            SetGltfComponentField(gltfComponent, "loadOnStart", false);
            gltfComponent.GLTFUri = model.GetMainGltfFile();

            // Use the Unity Standard shader if no override is provided.
            var shader = options.ShaderOverride == null ? Shader.Find("Standard") : options.ShaderOverride;
            SetGltfComponentField(gltfComponent, "shaderOverride", shader);

            await gltfComponent.Load();

            var gltfRoot = gameObject.transform.Find("Root Scene");
            var rootNode = gltfRoot.Find("RootNode");
            var occluder = rootNode.Find("Occluder");

            // GLTF models are right-handed, but the Unity coordinates are left-handed,
            // so rotating the model around Y axis.
            gltfRoot.transform.Rotate(Vector3.up, 180);

            if (occluder == null)
            {
                return;
            }

            if (options.WithOccluder)
            {
                var meshRenderer = occluder.transform.Find("Primitive").GetComponent<SkinnedMeshRenderer>();
                // If we need to handle occlusion, apply our custom shader to handle it.
                meshRenderer.sharedMaterials[0].shader = Shader.Find("Svrf/OccluderShader");
            }
            else
            {
                // If we don't need to handle occlusion, hide the occluder.
                occluder.gameObject.SetActive(false);
            }
        }

        internal static void SetGltfComponentField(GLTFComponent component, string name, object value)
        {
            var field = typeof(GLTFComponent).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(component, value);
        }
    }
}
