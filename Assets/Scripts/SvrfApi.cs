using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SVRF.Client.Api;
using SVRF.Client.Client;
using SVRF.Client.Model;

namespace UnitySvrf
{
    public class SvrfApi
    {
        private readonly AuthenticateApi _authenticateApi = new AuthenticateApi();
        private readonly MediaApi _mediaApi = new MediaApi();
        private const string ApiKey = "key";

        private static DateTime _expirationTime;
        private static string Token { get; set; }

        public async Task Authenticate()
        {
            var body = new Body(ApiKey);

            var result = await _authenticateApi.AuthenticateAsync(body);
            _expirationTime = DateTime.Now.AddSeconds(result.ExpiresIn != null ? (double)result.ExpiresIn : 0);
            Token = result.Token;
        }

        public async Task<Media> GetModelById(string id)
        {
            await PrepareAppToken();

            var response = await _mediaApi.GetByIdAsync(id);

            return response.Media;
        }

        public async Task<TrendingResponse> GetTrendingModels(int pageNum = 0, int size = 10)
        {
            await PrepareAppToken();

            var response = await _mediaApi.GetTrendingAsync(
                new List<MediaType> {MediaType._3d}, null, null, size, null, pageNum, null, null, null
            );

            return response;
        }

        public async Task<TrendingResponse> GetTrendingFaceFilters(int pageNum = 0, int size = 10)
        {
            await PrepareAppToken();

            var response = await _mediaApi.GetTrendingAsync(
                null, null, "Face Filters", size, null, pageNum, null, null, null
            );

            return response;
        }

        public async Task<SearchMediaResponse> SearchModels(string query, int pageNum = 0, int size = 10)
        {
            await PrepareAppToken();

            var response = await _mediaApi.SearchAsync(
                query, new List<MediaType> {MediaType._3d}, null, null, size, null, pageNum, null, null, null
            );

            return response;
        }

        public async Task<SearchMediaResponse> SearchFaceFilters(string query, int pageNum = 0, int size = 10)
        {
            await PrepareAppToken();

            var response = await _mediaApi.SearchAsync(
                query, null, null, null, size, null, pageNum, true, null, false
            );

            return response;
        }

        private async Task PrepareAppToken()
        {
            var isTokenValid = !string.IsNullOrEmpty(Token)
                               && (DateTime.Compare(_expirationTime, DateTime.Now) > 0);

            if (!isTokenValid)
            {
                await Authenticate();
            }

            Configuration.Default.AddApiKey("x-app-token", Token);
        }
    }
}
