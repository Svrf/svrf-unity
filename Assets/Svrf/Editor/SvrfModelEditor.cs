using System.Threading.Tasks;
using Svrf.Models.Enums;
using Svrf.Models.Media;
using Svrf.Unity.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS4014

namespace Svrf.Unity.Editor
{
    [ExecuteAlways]
    [CustomEditor(typeof(SvrfModel))]
    public class SvrfModelEditor : UnityEditor.Editor
    {
        private SvrfModel _svrfModel;
        private SvrfApi _svrfApi;

        public static SvrfModel SelectedSvrfModel;
        public static SvrfPreview Preview;

        private bool _isBadRequest;
        
        private async void Awake()
        {
            _svrfModel = (SvrfModel) target;

            if (_svrfApi == null)
            {
                _svrfApi = new SvrfApi();
            }

            if (!string.IsNullOrEmpty(_svrfModel.SvrfModelId))
            {
                await InsertThumbnailImage();
            }
        }

        private async Task InsertThumbnailImage()
        {
            MediaModel model;

            try
            {
                model = (await _svrfApi.Media.GetByIdAsync(_svrfModel.SvrfModelId)).Media;

                if (model.Type != MediaType.Model3D)
                {
                    _isBadRequest = true;
                    Repaint();
                    return;
                }
            }
            catch
            {
                _isBadRequest = true;
                Repaint();
                return;
            }
            
            var textureRequest = UnityWebRequestTexture.GetTexture(model.Files.Images.Max);

            await textureRequest.SendWebRequest();

            Preview = new SvrfPreview
            {
                Texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture,
                Title = model.Title,
                Id = model.Id,
            };

            Repaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();

            if (string.IsNullOrEmpty(_svrfModel.SvrfModelId))
            {
                _isBadRequest = false;
                GUILayout.FlexibleSpace();
                GUILayout.Label("Model didn't select");
            } else if (Preview == null || Preview.Id != _svrfModel.SvrfModelId)
            {
                Awake();
            } 
            else
            {
                _isBadRequest = false;
                GUILayout.Label($"Model : {Preview.Title}");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Preview.Texture, GUILayout.Width(128), GUILayout.Height(96));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (_isBadRequest)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Bad request ID");
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            var isOpenWindowClicked = GUILayout.Button("Open Svrf Window");

            if (isOpenWindowClicked)
            {
                SelectedSvrfModel = _svrfModel;
                SvrfWindow.ShowWindow();
            }

            if (string.IsNullOrEmpty(SvrfApiKey.Value))
            {
                if (GUILayout.Button("Create Api key Game Object"))
                {
                    var gameObject = new GameObject("Svrf Api Key");
                    gameObject.AddComponent<SvrfApiKey>();

                    Selection.activeObject = gameObject;
                }
            }
        }
    }

}
