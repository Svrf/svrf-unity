using Svrf;

namespace Assets.Scripts
{
    public class SvrfApi : SvrfClient
    {
        public SvrfApi() : base(SvrfApiKey.Value) { }
    }
}
