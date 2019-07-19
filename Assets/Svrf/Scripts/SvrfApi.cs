using Svrf.Unity.Coroutines;

namespace Svrf.Unity
{
    public class SvrfApi : SvrfClient
    {
        public MediaApiForCoroutine MediaForCoroutine { get; }

        public SvrfApi() : base(SvrfApiKey.Value)
        {
            MediaForCoroutine = new MediaApiForCoroutine(Media);
        }
    }
}
