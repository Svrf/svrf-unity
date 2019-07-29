using System;
using System.Threading.Tasks;
using Svrf.Exceptions;
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

        private const string NoApiKeyMessage = "Please set api key in the Svrf Api Key game object";
        private const string InvalidMediaTypeMessage = "Only 3D models are supported.";
        private const string ModelIdNotFoundMessage = "The model Id was not found.";

        public static SvrfModel SelectedSvrfModel { get; set; }
        public static SvrfPreview Preview { get; set; }

        private string _errorMessage;
        private bool _isLoading;

        public async void Awake()
        {
            _svrfModel = (SvrfModel) target;

            if (_svrfApi == null)
            {
                _svrfApi = new SvrfApi();
            }

            if (string.IsNullOrEmpty(_svrfModel.SvrfModelId)) return;

            _isLoading = true;
            await InsertThumbnailImage();
            _isLoading = false;
        }

        private async Task InsertThumbnailImage()
        {
            MediaModel model;

            try
            {
                model = (await _svrfApi.Media.GetByIdAsync(_svrfModel.SvrfModelId)).Media;
                if (model.Type != MediaType.Model3D)
                {
                    SetErrorMessage(InvalidMediaTypeMessage);
                    return;
                }
            }
            catch (ArgumentException)
            {
                SetErrorMessage(NoApiKeyMessage);
                return;
            }
            catch (ApiKeyNotFoundException)
            {
                SetErrorMessage(NoApiKeyMessage);
                return;
            }
            catch
            {
                SetErrorMessage(ModelIdNotFoundMessage);
                return;
            }
            
            var textureRequest = UnityWebRequestTexture.GetTexture(model.Files.Images.Width136);

            await textureRequest.SendWebRequest();

            Preview = new SvrfPreview
            {
                Texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture,
                Title = model.Title,
                Id = model.Id,
            };

            Repaint();
        }

        private void SetErrorMessage(string message)
        {
            _errorMessage = message;
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();

            if (string.IsNullOrEmpty(_svrfModel.SvrfModelId))
            {
                _errorMessage = string.Empty;
                GUILayout.FlexibleSpace();
                GUILayout.Label("Model isn't selected");
            }
            else if (Preview == null || Preview.Id != _svrfModel.SvrfModelId)
            {
                if (_svrfModel.IsChanged && !_isLoading)
                {
                    Awake();
                    _svrfModel.IsChanged = false;
                }
            } 
            else
            {
                _errorMessage = string.Empty;
                DrawModelPreview();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                DrawErrorMessage();
            }

            var isOpenWindowClicked = GUILayout.Button("Open Svrf Window");

            if (!isOpenWindowClicked) return;

            SelectedSvrfModel = _svrfModel;
            SvrfWindow.ShowWindow();
        }

        private void DrawErrorMessage()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_errorMessage);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawModelPreview()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label($"Model: {Preview.Title}", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Preview.Texture, GUILayout.Width(136));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}
