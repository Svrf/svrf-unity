using System.Threading.Tasks;
using UnityEngine;

namespace Svrf.Unity.Coroutines
{
    public class WaitFor : CustomYieldInstruction
    {
        public override bool keepWaiting => !IsCompleted;

        protected bool IsCompleted;

        internal WaitFor(Task task)
        {
            task.ContinueWith(t => IsCompleted = t.IsCompleted);
        }
    }
}
