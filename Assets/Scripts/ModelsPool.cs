using System.Collections;
using System.Collections.Generic;
using SVRF.Client.Model;

namespace Assets.Scripts
{
    internal static class ModelsPool
    {
        private static readonly IDictionary<string, Media> _models = new Dictionary<string, Media>();

        internal static void Add(IEnumerable<Media> mediaCollection)
        {
            foreach (var media in mediaCollection)
            {
                Add(media);
            }
        }

        internal static void Add(Media media)
        {
            if (!_models.ContainsKey(media.Id))
            {
                _models.Add(media.Id, media);
            }
        }

        internal static Media Get(string id)
        {
            return _models[id];
        }
    }
}
