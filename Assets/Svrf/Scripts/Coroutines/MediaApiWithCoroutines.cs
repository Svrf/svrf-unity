using Svrf.Api;
using Svrf.Models.Http;

namespace Svrf.Unity.Coroutines
{
    public class MediaApiWithCoroutines
    {
        private readonly MediaApi _mediaApi;

        internal MediaApiWithCoroutines(MediaApi mediaApi)
        {
            _mediaApi = mediaApi;
        }

        public TaskBasedCoroutine<SingleMediaResponse> GetById(int id)
        {
            return new TaskBasedCoroutine<SingleMediaResponse>(_mediaApi.GetByIdAsync(id));
        }

        public TaskBasedCoroutine<SingleMediaResponse> GetById(string id)
        {
            return new TaskBasedCoroutine<SingleMediaResponse>(_mediaApi.GetByIdAsync(id));
        }

        public TaskBasedCoroutine<MultipleMediaResponse> GetTrending(MediaRequestParams requestParams = null)
        {
            return new TaskBasedCoroutine<MultipleMediaResponse>(_mediaApi.GetTrendingAsync(requestParams));
        }

        public TaskBasedCoroutine<MultipleMediaResponse> Search(string query, MediaRequestParams requestParams = null)
        {
            return new TaskBasedCoroutine<MultipleMediaResponse>(_mediaApi.SearchAsync(query, requestParams));
        }
    }
}
