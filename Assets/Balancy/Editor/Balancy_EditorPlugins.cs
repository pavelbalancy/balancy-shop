#if UNITY_EDITOR && !BALANCY_SERVER
using UnityEngine;
using UnityEditor;

namespace Balancy.Editor
{
    [ExecuteInEditMode]
    public class Balancy_EditorPlugins : EditorWindow
    {
        [MenuItem("Tools/Balancy/Updates", false, -104001)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(Balancy_EditorPlugins));
            window.titleContent.text = "Balancy Updates";
            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Balancy/Editor/BalancyLogo.png");
        }

        private void Awake()
        {
            minSize = new Vector2(500, 500);
        }
        
        private void OnEnable()
        {
            EditorApplication.update += update;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= update;
        }
        
        private void update()
        {
            // if (_downloading)
            Repaint();
        }

        private Balancy_Plugins plugins;
        
        private Balancy_Plugins Plugins => plugins ?? (plugins = new Balancy_Plugins(this));

        private void OnGUI()
        {
            Plugins.Render();
        }
    }
}
#endif