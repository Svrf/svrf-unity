using Svrf.Api;
using Svrf.Models.Http;

namespace Svrf.Unity.Coroutines
{
    public class MediaApiForCoroutine
    {
        private readonly MediaApi _mediaApi;

        internal MediaApiForCoroutine(MediaApi mediaApi)
        {
            _mediaApi = mediaApi;
        }

        public WaitFor<SingleMediaResponse> GetById(int id)
        {
            return new WaitFor<SingleMediaResponse>(_mediaApi.GetByIdAsync(id));
        }

        public WaitFor<SingleMediaResponse> GetById(string id)
        {
            return new WaitFor<SingleMediaResponse>(_mediaApi.GetByIdAsync(id));
        }

        public WaitFor<MultipleMediaResponse> GetTrending(MediaRequestParams requestParams = null)
        {
            return new WaitFor<MultipleMediaResponse>(_mediaApi.GetTrendingAsync(requestParams));
        }

        public WaitFor<MultipleMediaResponse> Search(string query, MediaRequestParams requestParams = null)
        {
            return new WaitFor<MultipleMediaResponse>(_mediaApi.SearchAsync(query, requestParams));
        }
    }
}
