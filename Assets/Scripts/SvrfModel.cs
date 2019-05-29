using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGLTF;

public class SvrfModel : MonoBehaviour
{
    public string Id;

    public async void Start()
    {
        gameObject.transform.SetParent(gameObject.transform);

        // make API request
        // var request = await api.GetById(Id);

        var gltfComponent = gameObject.AddComponent<GLTFComponent>();
        gltfComponent.GLTFUri = "https://www.svrf.com/storage/svrf-models/730861/gltf/StarGlassesBlender2.gltf"; // request.uri

        await gltfComponent.Load();
    }
}
