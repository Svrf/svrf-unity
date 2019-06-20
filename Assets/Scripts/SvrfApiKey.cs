using UnityEngine;
using System;

namespace Assets.Scripts
{
    [Serializable]
    public sealed class SvrfApiKey : MonoBehaviour
    {
        [SerializeField]
        private string _apiKey;

        public static string Value { get; set; }

        public void Awake()
        {
            Value = _apiKey;
        }
    }
}
