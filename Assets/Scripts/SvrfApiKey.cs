using UnityEngine;
using System;

[Serializable]
public sealed class SvrfApiKey : MonoBehaviour
{
    [SerializeField]
    private string _apiKey;

    public static string ApiKey { get; set; }

    void Start()
    {
        if (!Application.isEditor)
            ApiKey = "";
    }

    void OnValidate()
    {
        ApiKey = _apiKey;
    }
}
