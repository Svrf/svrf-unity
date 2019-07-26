using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Svrf.Models.Enums;
using Svrf.Models.Http;
using Svrf.Models.Media;
using Svrf.Unity.Editor.Extensions;
using UnityEngine;
using UnityEngine.Networking;

// Async function are run without awaiting response because we need to load preview images
// in different threads. That's why we need to disable the pragma warning.
#pragma warning disable CS4014

namespace Svrf.Unity.Editor
{
    public class TextureLoader
    {
        public Action OnTextureLoaded { get; set; }
        public readonly List<SvrfPreview> ModelsPreviews = new List<SvrfPreview>();

        public bool AreAllModelsLoaded;
        public bool IsNoResult;

        private readonly SvrfApi _api;

        private int _pageNum = 0;
        private const int Size = 10;

        public TextureLoader(Action callback)
        {
            _api = new SvrfApi();

            OnTextureLoaded = callback;
        }

        public async Task FetchMediaModels(string searchString = null)
        {
            var options = new MediaRequestParams
            {
                PageNum = _pageNum,
                Size = Size,
                Type = new[] { MediaType.Model3D },
                IsFaceFilter = SvrfWindow.IsFaceFilter,
            };

            var multipleResponse = string.IsNullOrEmpty(searchString) ?
                await _api.Media.GetTrendingAsync(options) :
                await _api.Media.SearchAsync(searchString, options);

            _pageNum = multipleResponse.NextPageNum;

            var media = multipleResponse.Media.ToList();

            AreAllModelsLoaded = media.Count < Size;

            IsNoResult = media.Count == 0;

            foreach (var model in media)
            {
                LoadThumbnailImage(model);
            }
        }

        public void LoadMoreModels()
        {
            FetchMediaModels();
        }

        public void SearchModels(string searchString = null)
        {
            Clear();
            FetchMediaModels(searchString);
        }

        private void Clear()
        {
            AreAllModelsLoaded = false;
            _pageNum = 0;
            ModelsPreviews.Clear();
        }

        private async Task LoadThumbnailImage(MediaModel model)
        {
            var request = UnityWebRequestTexture.GetTexture(model.Files.Images.Size720x720);

            await request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                return;
            }

            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            ModelsPreviews.Add(new SvrfPreview { Texture = texture, Title = model.Title, Id = model.Id});
            OnTextureLoaded();
        }
    }
}
