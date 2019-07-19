using System.Threading.Tasks;

namespace Svrf.Unity.Coroutines
{
    public class WaitFor<T> : WaitFor
    {
        public T Response;

        internal WaitFor(Task<T> task) : base(task)
        {
            task.ContinueWith(x => Response = x.Result);
        }
    }
}
