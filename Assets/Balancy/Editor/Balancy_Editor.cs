#if UNITY_EDITOR && !BALANCY_SERVER
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Balancy.Editor
{
    [ExecuteInEditMode]
    public class Balancy_Editor : EditorWindow
    {
        public delegate void SynchAddressablesDelegate(string gameId, string token, int branchId, Action<string, float> onProgress, Action onStart, Action<string> onComplete);
        public delegate void SynchAddressablesDelegateNew(Balancy_EditorAuth editorAuth, string gameId, string token, int branchId, string branchName, Action<string, float> onProgress, Action onStart, Action<string> onComplete);
        public static event SynchAddressablesDelegate SynchAddressablesEvent;
        public static event SynchAddressablesDelegateNew SynchAddressablesEventNew;
        
        private string CACHE_PATH => Application.persistentDataPath + "/Balancy/Models";

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
            GUI.enabled = !_downloading && AuthHelper.HasSelectedBranch() && !EditorApplication.isCompiling;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Content Management");
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
                
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Addressables (NEW)"))
                    StartSynchingAddressablesNew();
                
                if (GUILayout.Button("Addressables (Legacy)"))
                    StartSynchingAddressables();
                
                GUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                var directoryExists = Directory.Exists(CACHE_PATH);
                GUI.enabled = directoryExists;
                if (GUILayout.Button("Clear locally cached Content"))
                {
                    ClearCache();
                }

                GUI.enabled = !_downloading && AuthHelper.HasSelectedBranch() && !EditorApplication.isCompiling;
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                RenderSmartObjects();
                GUI.enabled = true;
            }

            GUILayout.EndVertical();
            GUI.enabled = true;
        }
        
        private void RenderSmartObjects()
        {
            GUILayout.Label("Clear the latest user profile completely");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Dev"))
                ResetAll(Constants.Environment.Development);
            if (GUILayout.Button("Stage"))
                ResetAll(Constants.Environment.Stage);
            if (GUILayout.Button("Prod"))
                ResetAll(Constants.Environment.Production);
            GUILayout.EndHorizontal();
        }

        private void StartCodeGeneration()
        {
            _downloading = true;
            _downloadingProgress = 0.5f;
            _downloadingFileName = "Generating the code...";

            var gameInfo = _authHelper.GetSelectedGameInfo();
            var branchId = gameInfo.GetSelectedBranchId();
            var token = _authHelper.GetAccessToken();
            Balancy_CodeGeneration.StartGeneration(
                gameInfo.GameId,
                token,
                branchId,
                () => { _downloading = false; },
                PluginUtils.CODE_GENERATION_PATH
            );
        }
        
        private void StartSynchingAddressablesNew()
        {
            if (SynchAddressablesEventNew == null)
            {
                EditorUtility.DisplayDialog("Warning", "Addressables (NEW) Plugin is not installed. Please install it below and don't forget to import Unity's Addressables from Package Manager", "Got it");
            }
            else
            {
                var gameInfo = _authHelper.GetSelectedGameInfo();
                var branchId = gameInfo.GetSelectedBranchId();
                var branchName = gameInfo.GetSelectedBranch().Name;
                var token = _authHelper.GetAccessToken();
                SynchAddressablesEventNew(
                    AuthHelper,
                    gameInfo.GameId,
                    token,
                    branchId,
                    branchName,
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
                            EditorUtility.DisplayDialog("Success", "Addressables are now synched. Please ensure to DEPLOY the game in Balancy dashboard to release the new bundles.", "Ok");
                    }
                );
            }
        }

        private void StartSynchingAddressables()
        {
            if (SynchAddressablesEvent == null)
            {
                EditorUtility.DisplayDialog("Warning", "Addressables (Legacy) Plugin is not installed. Please install it below and don't forget to import Unity's Addressables from Package Manager", "Got it");
            }
            else
            {
                var gameInfo = _authHelper.GetSelectedGameInfo();
                var branchId = gameInfo.GetSelectedBranchId();
                var token = _authHelper.GetAccessToken();
                SynchAddressablesEvent(
                    gameInfo.GameId,
                    token,
                    branchId,
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

            var gameInfo = _authHelper.GetSelectedGameInfo();
            var branchName = gameInfo.GetSelectedBranch()?.Name;
            var appConfig = new AppConfig
            {
                ApiGameId = gameInfo.GameId,
                PublicKey = gameInfo.PublicKey,
                BranchName = branchName
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
        
        private void ResetAll(Constants.Environment environment)
        {
            var gameInfo = _authHelper.GetSelectedGameInfo();
            DataEditor.ResetAllProfiles(gameInfo.GameId, environment);
            EditorUtility.DisplayDialog("Success", $"The profile was erased on {environment}", "Thanks");
        }
        
        private void ClearCache()
        {
            Directory.Delete(CACHE_PATH, true);
        }
    }
}
#endif