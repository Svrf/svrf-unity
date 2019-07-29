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
    internal class TextureLoader
    {
        internal Action OnTextureLoaded { get; set; }

        internal bool AreAllModelsLoaded;
        internal bool IsNoResult;
        internal bool IsLoading;

        internal List<string> ModelIds { get; set; } = new List<string>();

        private readonly SvrfApi _api;

        private int _pageNum = 0;
        private const int Size = 10;

        internal TextureLoader(Action onTextureLoaded)
        {
            _api = new SvrfApi();

            OnTextureLoaded = onTextureLoaded;
        }

        internal async Task FetchMediaModels(string searchString = null)
        {
            IsLoading = true;
            OnTextureLoaded();

            var options = new MediaRequestParams
            {
                PageNum = _pageNum,
                Size = Size,
                Type = new[] { MediaType.Model3D },
                IsFaceFilter = SvrfWindow.IsFaceFilter ? SvrfWindow.IsFaceFilter : (bool?) null,
            };

            var multipleResponse = string.IsNullOrEmpty(searchString) ?
                await _api.Media.GetTrendingAsync(options) :
                await _api.Media.SearchAsync(searchString, options);

            _pageNum = multipleResponse.NextPageNum;

            var media = multipleResponse.Media.ToList();

            OnFinishLoading(media.Count);

            foreach (var model in media)
            {
                ModelIds.Add(model.Id);

                if (!ModelPreviewsStorage.Previews.ContainsKey(model.Id))
                {
                    LoadThumbnailImage(model);
                }
            }
        }

        internal void LoadMoreModels()
        {
            FetchMediaModels();
        }

        internal void SearchModels(string searchString = null)
        {
            Clear();
            FetchMediaModels(searchString);
        }

        private void OnFinishLoading(int mediaCount)
        {
            AreAllModelsLoaded = mediaCount < Size;

            IsNoResult = mediaCount == 0;

            IsLoading = false;

            OnTextureLoaded();
        }

        private void Clear()
        {
            AreAllModelsLoaded = false;
            _pageNum = 0;
            ModelIds.Clear();
        }

        private async Task LoadThumbnailImage(MediaModel model)
        {
            var request = UnityWebRequestTexture.GetTexture(model.Files.Images.Size720x720);

            await request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                ModelIds.RemoveAt(ModelIds.FindIndex(id => id == model.Id));
                return;
            }

            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

            ModelPreviewsStorage.Previews.Add(model.Id, new SvrfPreview
            {
                Id = model.Id,
                Texture = texture,
                Title = model.Title
            });

            OnTextureLoaded();
        }
    }
}
