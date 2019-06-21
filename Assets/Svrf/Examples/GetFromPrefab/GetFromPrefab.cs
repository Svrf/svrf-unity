using UnityEngine;

namespace Svrf.Unity.Examples
{
    public class GetFromPrefab : MonoBehaviour
    {
        [SerializeField]
        private GameObject _svrfModelPrefab;

        void Start()
        {
            var svrfModelPrefab = Instantiate(_svrfModelPrefab);
            svrfModelPrefab.transform.SetParent(transform, false);
        }
    }
}
