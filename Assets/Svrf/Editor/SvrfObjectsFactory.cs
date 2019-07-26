using Svrf.Unity;
using UnityEditor;
using UnityEngine;

namespace Svrf.Editor
{
    public static class SvrfObjectsFactory
    {
        public static GameObject CreateSvrfModel(string name = null)
        {
            name = name ?? "Svrf Model";

            var gameObject = new GameObject(name);
            gameObject.AddComponent<SvrfModel>();

            HandleObjectCreating(gameObject);

            return gameObject;
        }

        public static GameObject CreateSvrfApiKey()
        {
            var gameObject = new GameObject("Svrf Api Key");
            gameObject.AddComponent<SvrfApiKey>();

            HandleObjectCreating(gameObject);

            return gameObject;
        }

        private static void HandleObjectCreating(GameObject gameObject)
        {
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
        }
    }
}
