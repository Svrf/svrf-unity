using Svrf.Unity;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    void Update()
    {
        if (!SvrfModel.IsLoading)
        {
            Destroy(gameObject);
        }
    }
}
