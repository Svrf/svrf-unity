using UnityEngine;
using System;

namespace Assets.Scripts
{
    [Serializable]
    public sealed class SvrfApiKey : MonoBehaviour
    {
        [SerializeField]
        private string _apiKey;

        public static string ApiKey { get; set; }

        public void Awake()
        {
            ApiKey = _apiKey;
        }
    }
}
