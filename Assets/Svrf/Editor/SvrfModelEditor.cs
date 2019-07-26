using System.Threading.Tasks;
using Svrf.Models.Enums;
using Svrf.Models.Media;
using Svrf.Unity.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Svrf.Unity.Editor
{
    [ExecuteAlways]
    [CustomEditor(typeof(SvrfModel))]
    public class SvrfModelEditor : UnityEditor.Editor
    {
        private SvrfModel _svrfModel;
        private SvrfApi _svrfApi;

        public static SvrfModel SelectedSvrfModel { get; set; }
        public static SvrfPreview Preview { get; set; }

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
                    SetUpBadRequest();
                    return;
                }
            }
            catch
            {
                SetUpBadRequest();
                return;
            }
            
            var textureRequest = UnityWebRequestTexture.GetTexture(model.Files.Images.Size720x720);

            await textureRequest.SendWebRequest();

            Preview = new SvrfPreview
            {
                Texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture,
                Title = model.Title,
                Id = model.Id,
            };

            Repaint();
        }

        private void SetUpBadRequest()
        {
            _isBadRequest = true;
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
                GUILayout.Label("Model isn't selected");
            }
            else if (Preview == null || Preview.Id != _svrfModel.SvrfModelId)
            {
                Awake();
            } 
            else
            {
                _isBadRequest = false;
                DrawModelPreview();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (_isBadRequest)
            {
                DrawBadRequest();
            }

            var isOpenWindowClicked = GUILayout.Button("Open Svrf Window");

            if (!isOpenWindowClicked) return;

            SelectedSvrfModel = _svrfModel;
            SvrfWindow.ShowWindow();
        }

        private static void DrawBadRequest()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Invalid model ID. Only 3D models are supported");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawModelPreview()
        {
            GUILayout.Label($"Model: {Preview.Title}");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Preview.Texture, GUILayout.Width(128), GUILayout.Height(96));
        }
    }
}
