using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using Svrf.Models.Media;
using System.Threading.Tasks;
using UnityEngine;
using UnitySvrf;

public class SvrfModel : MonoBehaviour
{
    public string Id;
    public bool WithOccluder;
    public Shader OverrideShader;

    private Shader _shader;
    private static SvrfApi _svrfApi = new SvrfApi();

    public async void Start()
    {
        var model = (await _svrfApi.Media.GetByIdAsync(Id)).Media;
        var options = new SvrfModelOptions
        {
            OverrideShader = OverrideShader,
            WithOccluder = WithOccluder
        };

        await SvrfModelUtility.AddSvrfModel(gameObject, model, options);
    }

    public static async Task<GameObject> GetSvrfGameObject(MediaModel model, SvrfModelOptions options)
    {
        var svrfGameObject = new GameObject();
        await SvrfModelUtility.AddSvrfModel(svrfGameObject, model, options);
        return svrfGameObject;
    }
}
