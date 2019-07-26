using Svrf.Editor;
using UnityEditor;
using UnityEngine;

namespace Svrf.Unity.Editor
{
    public class SvrfWindow : EditorWindow
    {
        private static TextureLoader _textureLoader;

        private const int ThumbnailWidth = 128;
        private const int ThumbnailHeight = 96;
        private const int CellWidth = ThumbnailWidth;
        private const int CellSpacing = 5;
        private const int Padding = 5;
        private const int BlockSpacing = 10;

        private Vector2 _scrollPosition;
        private string _searchString = string.Empty;
        public static bool IsFaceFilter = true;

        private Texture _refreshIcon;

        [MenuItem("Window/Svrf")]
        public static void ShowWindow()
        {
            GetWindow<SvrfWindow>("Svrf");
        }

        private void OnDestroy()
        {
            SvrfModelEditor.SelectedSvrfModel = null;
        }

        public async void Awake()
        {
            CreateTextureLoaderInstance();

            _refreshIcon = Resources.Load("refresh_icon") as Texture;

            await _textureLoader.FetchMediaModels();
        }

        private void CreateTextureLoaderInstance()
        {
            _textureLoader = new TextureLoader(Repaint);
        }

        private void OnGUI()
        {
            if (_textureLoader == null)
            {
                CreateTextureLoaderInstance();
                _textureLoader?.LoadMoreModels();
            }
            
            GUILayout.BeginArea(new Rect(Padding, Padding, position.width - 2 * Padding, position.height - 2 * Padding));

            if (Application.isPlaying)
            {
                GUILayout.Space(BlockSpacing);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Warning! Play mode is active.", EditorStyles.wordWrappedLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            DrawSearchArea();
            DrawCellGrid();

            GUILayout.EndArea();
        }

        private void DrawSearchArea()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(Padding);

            DrawWindowLabel();

            IsFaceFilter = GUILayout.Toggle(IsFaceFilter, "Is Face Filter", GUILayout.Width(140));
            
            DrawSearchField();

            GUILayout.Space(Padding);
            GUILayout.EndVertical();
        }

        private void DrawCellGrid()
        {
            if (_textureLoader.ModelsPreviews.Count == 0 && _textureLoader.IsNoResult)
            {
                DrawNoResultMessage();
                return;
            }
            else if (_textureLoader.ModelsPreviews.Count == 0)
            {
                GUILayout.Label("Loading...", EditorStyles.boldLabel);
                return;
            }

            GUILayout.BeginHorizontal();
            var assetsPerRow = GetAssetsCountPerRow();
            var assetsThisRow = 0;

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.Space(Padding);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            SvrfPreview clickedModel = null;

            foreach (var model in _textureLoader.ModelsPreviews)
            {
                if (assetsThisRow >= assetsPerRow)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(BlockSpacing * 2);
                    GUILayout.BeginHorizontal();
                    assetsThisRow = 0;
                }

                if (assetsThisRow > 0) GUILayout.Space(CellSpacing);

                GUILayout.BeginVertical();
                if (GUILayout.Button(model.Texture,
                    GUILayout.Width(ThumbnailWidth), GUILayout.Height(ThumbnailHeight)))
                {
                    clickedModel = model;
                }
                GUILayout.Space(CellSpacing);

                GUILayout.Label(model.Title, EditorStyles.boldLabel, GUILayout.MaxWidth(CellWidth));

                GUILayout.EndVertical();
                assetsThisRow++;
            }

            if (assetsThisRow > 0)
            {
                while (assetsThisRow < assetsPerRow)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(string.Empty, GUILayout.Width(CellWidth), GUILayout.Height(ThumbnailHeight));
                    GUILayout.EndVertical();
                    assetsThisRow++;
                }
            }

            GUILayout.EndHorizontal();

            DrawLoadMoreButton();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            if (clickedModel != null)
            {
                InsertSelectedModel(clickedModel);
            }
        }

        private void DrawSearchField()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Space(Padding);
            _searchString = EditorGUILayout.TextField(_searchString);
            GUILayout.EndVertical();

            var isSearchClicked = GUILayout.Button("Search");

            GUILayout.EndHorizontal();

            if (isSearchClicked)
            {
                _textureLoader.SearchModels(_searchString);
            }
        }

        private void DrawWindowLabel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("You can find any model in Svrf", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            if (string.IsNullOrEmpty(SvrfApiKey.Value))
            {
                var isCreateApiKeyObjectClicked = GUILayout.Button("Create Api Key Game Object", GUILayout.Height(30));

                if (isCreateApiKeyObjectClicked)
                {
                    SvrfObjectsFactory.CreateSvrfApiKey();
                }
            }

            var isRefreshClicked = GUILayout.Button(_refreshIcon, GUILayout.Width(30), GUILayout.Height(30));

            GUILayout.EndHorizontal();

            if (isRefreshClicked)
            {
                _textureLoader.SearchModels(_searchString);
            }
        }

        private static void DrawNoResultMessage()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No result", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void DrawLoadMoreButton()
        {
            GUILayout.Space(BlockSpacing);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var isLoadMoreClicked = false;
            if (!_textureLoader.AreAllModelsLoaded)
            {
                isLoadMoreClicked = GUILayout.Button("Load more");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(Padding);

            if (isLoadMoreClicked)
            {
                _textureLoader.LoadMoreModels();
            }
        }

        private int GetAssetsCountPerRow()
        {
            var flooredToIntWidth = Mathf.FloorToInt((position.width - 40));

            return Mathf.Clamp(flooredToIntWidth / (CellSpacing + CellWidth), 1, int.MaxValue);
        }

        private static void InsertSelectedModel(SvrfPreview clickedModel)
        {
            if (SvrfModelEditor.SelectedSvrfModel != null)
            {
                var svrfModel = SvrfModelEditor.SelectedSvrfModel;

                svrfModel.SvrfModelId = clickedModel.Id;
                SvrfModelEditor.Preview = clickedModel;

                return;
            }

            var svrfGameObject = SvrfObjectsFactory.CreateSvrfModel(clickedModel.Title);
            var svrfComponent = svrfGameObject.GetComponent<SvrfModel>();
            svrfComponent.SvrfModelId = clickedModel.Id;
        }
    }
}
