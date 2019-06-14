using Svrf.Models.Media;
using System.Linq;

namespace Assets.Scripts.Utilities
{
    public static class SvrfModelExtensions
    {
        public static string GetMainGltfFile(this MediaModel model)
        {
            var gltfFiles = model.Files.Gltf;
            var fileName = gltfFiles.Keys.First(k => k.EndsWith(".gltf"));
            return gltfFiles[fileName];
        }
    }
}
