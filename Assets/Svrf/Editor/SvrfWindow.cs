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
                GUILayout.Label("Warning! This window opened in play mode.", EditorStyles.wordWrappedLabel);
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

            GUILayout.BeginHorizontal();
            GUILayout.Label("You can find any model in Svrf", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            var isRefreshChecked = GUILayout.Button(_refreshIcon, GUILayout.Width(30), GUILayout.Height(30));

            GUILayout.EndHorizontal();

            IsFaceFilter = GUILayout.Toggle(IsFaceFilter, "Is Face Filter", GUILayout.Width(140));

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Space(Padding);
            _searchString = EditorGUILayout.TextField(_searchString);
            GUILayout.EndVertical();


            var isSearchChecked = GUILayout.Button("Search");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(Padding);
            GUILayout.EndVertical();

            if (isRefreshChecked)
            { 
                _textureLoader.SearchModels(_searchString);
            }

            if (isSearchChecked)
            {
                _textureLoader.SearchModels(_searchString);
            }

        }

        private void DrawCellGrid()
        {
            if (_textureLoader.ModelsPreviews.Count == 0 && _textureLoader.IsNoResult)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("No result", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return;
            }
            else if (_textureLoader.ModelsPreviews.Count == 0)
            {
                GUILayout.Label("Loading...", EditorStyles.boldLabel);
                return;
            }


            GUILayout.BeginHorizontal();
            var assetsPerRow = Mathf.Clamp(
                Mathf.FloorToInt((position.width - 40)) / (CellSpacing + CellWidth), 1, int.MaxValue);
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
            GUILayout.Space(BlockSpacing);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var isLoadMoreChecked = false;
            if (!_textureLoader.IsAllPossibleModels)
            {
                isLoadMoreChecked = GUILayout.Button("Load more");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(Padding);

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            if (isLoadMoreChecked)
            {
                _textureLoader.LoadMoreModels();
            }

            if (clickedModel != null)
            {
                InsertSelectedModel(clickedModel);
            }
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

            var svrfGameObject = new GameObject(clickedModel.Title);
            var svrfComponent = svrfGameObject.AddComponent<SvrfModel>();
            svrfComponent.SvrfModelId = clickedModel.Id;
        }
    }
}
