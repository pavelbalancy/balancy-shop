#if UNITY_EDITOR && !BALANCY_SERVER
using System;
using UnityEngine;
using UnityEditor;

namespace Balancy.Editor
{
    [ExecuteInEditMode]
    public class Balancy_Editor : EditorWindow
    {
        public delegate void SynchAddressablesDelegate(string gameId, string token, Constants.Environment environment, Action<string, float> onProgress, Action onStart, Action<string> onComplete);
        public static event SynchAddressablesDelegate SynchAddressablesEvent;

        [MenuItem("Tools/Balancy/Config", false, -104002)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(Balancy_Editor));
            window.titleContent.text = "Balancy Config";
            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Balancy/Editor/BalancyLogo.png");
        }
        
        [MenuItem("Tools/Balancy/Open PersistentDataPath ", false, -103000)]
        public static void OpenPersistentDataPath()
        {
            var path = Application.persistentDataPath;
            EditorUtility.RevealInFinder(path);
        }

        private void Awake()
        {
            minSize = new Vector2(500, 500);
        }

        readonly string[] SERVER_TYPE = {"Development", "Stage", "Production"};
        private Balancy_EditorAuth _authHelper;
        
        private bool _downloading;
        private int _versionNumber;
        private float _downloadingProgress;
        private string _downloadingFileName;
        
        private Balancy_EditorAuth AuthHelper => _authHelper ?? (_authHelper = new Balancy_EditorAuth(this));

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
        
        private void OnGUI()
        {
            GUI.enabled = !_downloading;
            
            RenderSettings();
            EditorGUILayout.Space();
            RenderLoader();
        }
        
        private void RenderSettings()
        {
            AuthHelper.Render();
        }

        private void RenderLoader()
        {
            GUI.enabled = !_downloading && AuthHelper.HasSelectedGame() && !EditorApplication.isCompiling;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Data Editor");
            var curServer = AuthHelper.GetUserInfo().SelectedServer;

            var newServer = GUILayout.SelectionGrid(curServer, SERVER_TYPE, SERVER_TYPE.Length, EditorStyles.radioButton);
            if (newServer != curServer)
                AuthHelper.GetUserInfo().SetServerType(newServer);

            if (_downloading)
            {
                GUI.enabled = true;
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.ProgressBar(rect, _downloadingProgress, _downloadingFileName);
                GUI.enabled = false;
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Generate Code"))
                    StartCodeGeneration();

                if (GUILayout.Button("Download Data"))
                    StartDownloading();
                
                if (GUILayout.Button("Synch Addressables"))
                    StartSynchingAddressables();
                
                GUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                RenderVersionLoader();
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                RenderSmartObjects();
            }

            GUILayout.EndVertical();
            GUI.enabled = true;
        }
        
        private void RenderVersionLoader()
        {
            GUILayout.Label("Download a Specific Balance Version");
            GUILayout.BeginHorizontal();
            var stringNumber = EditorGUILayout.TextField("Version Number: ", _versionNumber.ToString());
            int.TryParse(stringNumber, out _versionNumber);
            
            if (GUILayout.Button("Download Data"))
                StartDownloading(_versionNumber);
            GUILayout.EndHorizontal();
        }
        
        private void RenderSmartObjects()
        {
            GUILayout.Label("Clear the latest user profile completely");
            if (GUILayout.Button("Reset"))
            {
                ResetAll();
            }
        }

        private void StartCodeGeneration()
        {
            _downloading = true;
            _downloadingProgress = 0.5f;
            _downloadingFileName = "Generating the code...";

            var gameInfo = AuthHelper.GetSelectedGameInfo();
            var token = AuthHelper.GetAccessToken();
            Balancy_CodeGeneration.StartGeneration(
                gameInfo.GameId,
                token,
                (Constants.Environment) AuthHelper.GetUserInfo().SelectedServer,
                () => { _downloading = false; },
                PluginUtils.CODE_GENERATION_PATH
            );
        }

        private void StartSynchingAddressables()
        {
            if (SynchAddressablesEvent == null)
            {
                EditorUtility.DisplayDialog("Warning", "Addressables Plugin is not installed. Please install it below and don't forget to import Unity's Addressables from Package Manager", "Got it");
            }
            else
            {
                var gameInfo = AuthHelper.GetSelectedGameInfo();
                var token = AuthHelper.GetAccessToken();
                SynchAddressablesEvent(
                    gameInfo.GameId,
                    token,
                    (Constants.Environment) AuthHelper.GetUserInfo().SelectedServer,
                    (fileName, progress) =>
                    {
                        _downloadingFileName = fileName;
                        _downloadingProgress = progress;
                    },
                    () =>
                    {
                        _downloading = true;
                        _downloadingProgress = 0f;
                        _downloadingFileName = "Synchronizing addressables...";  
                    },
                    (error) =>
                    {
                        _downloading = false;
                        if (!string.IsNullOrEmpty(error))
                            EditorUtility.DisplayDialog("Error", error, "Ok");
                        else
                            EditorUtility.DisplayDialog("Success", "Addressables are now synched. Please reload Balancy web page", "Ok");
                    }
                );
            }
        }

        private void StartDownloading(int versionNumber = 0)
        {
            _downloading = true;
            _downloadingProgress = 0;

            var gameInfo = AuthHelper.GetSelectedGameInfo();
            var appConfig = new AppConfig
            {
                ApiGameId = gameInfo.GameId,
                PublicKey = gameInfo.PublicKey,
                Environment = (Constants.Environment) AuthHelper.GetUserInfo().SelectedServer
            };
            
            DicsHelper.LoadDocs(appConfig, responseData =>
            {
                _downloading = false;
                if (!responseData.Success)
                    EditorUtility.DisplayDialog("Error", responseData.Error.Message, "Ok");
            }, (fileName, progress) =>
            {
                _downloadingFileName = fileName;
                _downloadingProgress = progress;
            }, versionNumber);
        }
        
        private void ResetAll()
        {
            var gameInfo = AuthHelper.GetSelectedGameInfo();
            DataEditor.ResetAllProfiles(gameInfo.GameId, (Constants.Environment) AuthHelper.GetUserInfo().SelectedServer);
            EditorUtility.DisplayDialog("Success", "The profile was erased.", "Thanks");
        }
    }
}
#endif