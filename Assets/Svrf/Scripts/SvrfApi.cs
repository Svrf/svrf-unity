using Svrf.Unity.Coroutines;

namespace Svrf.Unity
{
    public class SvrfApi : SvrfClient
    {
        public MediaApiWithCoroutines MediaCoroutines { get; }

        public SvrfApi() : base(SvrfApiKey.Value)
        {
            MediaCoroutines = new MediaApiWithCoroutines(Media);
        }
    }
}
