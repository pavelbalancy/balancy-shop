using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Balancy.Addressables;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Balancy.Editor
{
    [InitializeOnLoad]
    public static class AddressablesHelper
    {
        private class FullInfo
        {
            public AddressablesGroup[] groups;
        }

        private class AddressablesGroup
        {
            public string guid;
            public string name;
            public FileInfo[] entries;

            public AddressablesGroup(int size)
            {
                entries = new FileInfo[size];
            }
        }

        private class FileInfo
        {
            [JsonIgnore]
            public Object link;
            [JsonIgnore]
            public string path;
            [JsonIgnore]
            public string texturePath;

            public string guid;
            public string name;
            public string hash;
            [JsonIgnore]
            public string group;
        }
        
        private class SynchAddressablesResponse
        {
            public string[] assets;
            public Size size;
            
            public class Size
            {
                public int x;
                public int y;
            }
        }

        private class GameInfo
        {
            public string GameId;
            public string Token;
            public Constants.Environment Environment;
            public Action<string, float> OnProgress;
            public Action<string> OnComplete;
            public Action OnStart;
        }

        static AddressablesHelper()
        {
            Balancy_Editor.SynchAddressablesEvent += SynchAddressables;
        }

        internal static void SynchAddressables()
        {
            ProcessData();
        }

        private static AddressableAssetSettings _settings;
        private static GameInfo _gameInfo;

        private static void SynchAddressables(string gameId, string token, Constants.Environment environment, Action<string, float> onProgress, Action onStart, Action<string> onComplete)
        {
            _settings = AddressableAssetSettingsDefaultObject.Settings;
            if (_settings == null)
            {
                Debug.LogError("No Addressables Found in the project");
                onComplete?.Invoke(null);
                return;
            }

            if (_settings.groups == null)
            {
                Debug.LogError("Addressables groups is null");
                onComplete?.Invoke(null);
                return;
            }

            _gameInfo = new GameInfo
            {
                GameId = gameId,
                Token = token,
                Environment = environment,
                OnProgress = onProgress,
                OnComplete = onComplete,
                OnStart =  onStart
            };

            Addressables_Editor.ShowWindow(_settings, gameId);
        }

        private static void ProcessData()
        {
            _gameInfo.OnStart();
            var info = ReadData(_settings);
            CalculateHashes(info);
            SendInfoToServer(info, _gameInfo);
        }

        private static FullInfo ReadData(AddressableAssetSettings settings)
        {
            var selectedGroups = Addressables_Editor.GetSelectedGroups(_gameInfo.GameId);
            var groupsCount = GetGroupsCount(settings, selectedGroups);
            var info = new FullInfo {groups = new AddressablesGroup[groupsCount]};

            int i = 0;
            foreach (var group in settings.groups)
            {
                if (group.ReadOnly || !selectedGroups.Contains(group.Guid))
                    continue;

                var entries = group.entries;

                var infoGroup = info.groups[i] = new AddressablesGroup(entries.Count);
                infoGroup.name = group.Name;
                infoGroup.guid = group.Guid;
                var files = infoGroup.entries;

                var j = 0;
                foreach (var entry in entries)
                {
                    files[j++] = new FileInfo
                    {
                        link = entry.MainAsset,
                        guid = entry.guid,
                        name = entry.address,
                        path = entry.AssetPath,
                        group = group.Name
                    };
                }

                i++;
            }

            return info;
        }

        private static int GetGroupsCount(AddressableAssetSettings settings, List<string> selectedGroups)
        {
            return settings.groups.Count(group => group != null && !group.ReadOnly && selectedGroups.Contains(group.Guid));
        }

        private static void CalculateHashes(FullInfo info)
        {
            foreach (var group in info.groups)
            {
                foreach (var entry in group.entries)
                {
                    string filePath = null;
                    switch (entry.link)
                    {
                        case Texture2D _texture2D:
                            filePath = entry.texturePath = entry.path;
                            break;
                        case GameObject _gameObject:
                        {
                            var script = _gameObject.GetComponentInChildren<IUnnyAsset>();
                            filePath = entry.texturePath = script?.GetPreviewImagePath();
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        var md5 = MD5.Create();
                        var stream = File.OpenRead(filePath);
                        var checkSum = md5.ComputeHash(stream);
                        var hash = BitConverter.ToString(checkSum).Replace("-", string.Empty);
                        entry.hash = hash;
                    }
                }
            }
        }

        private static void SendInfoToServer(FullInfo info, GameInfo gameInfo)
        {
            var helper = EditorCoroutineHelper.Create();
            var req = new EditorUtils.ServerRequest($"/v1.2/check_assets/{gameInfo.GameId}/{(int)gameInfo.Environment}");
            req.SetHeader("Content-Type", "application/json")
                .SetHeader("Authorization", "Bearer " + gameInfo.Token)
                .AddBody("groups", info.groups);
            
            var cor = UnityUtils.SendRequest(req, request =>
            {
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    gameInfo.OnComplete?.Invoke(request.error);
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<SynchAddressablesResponse>(request.downloadHandler.text);
                    SendImagesToServer(gameInfo, info, response.assets, new Vector2Int(response.size.x, response.size.y));
                }
            });
            helper.LaunchCoroutine(cor);
        }
        
        private static void SendImagesToServer(GameInfo gameInfo, FullInfo info, string[] guids, Vector2Int maxSize)
        {
            var helper = EditorCoroutineHelper.Create();

            var cor = SendImagesToServerCoroutine(gameInfo, info, guids, maxSize);

            helper.LaunchCoroutine(cor);
        }
        
        private static IEnumerator SendImagesToServerCoroutine(GameInfo gameInfo, FullInfo info, string[] guids, Vector2Int maxSize)
        {
            var dict = MapFiles(info);
            int uploadedFiles = 0;
            string currentError = null;
            foreach (var guid in guids)
            {
                if (!string.IsNullOrEmpty(currentError))
                    break;
                
                if (!dict.TryGetValue(guid, out var fileInfo))
                {
                    Debug.LogError("File with guid " + guid + " wasn't found");
                    continue;
                }

                var newTexture = GetCompressedTexture(fileInfo, maxSize);

                if (newTexture == null)
                    continue;

                gameInfo.OnProgress?.Invoke(fileInfo.name, (float)uploadedFiles / guids.Length);
                uploadedFiles++;
                var req = new EditorUtils.ServerRequest("/v1.1/upload_img");
                req.SetHeader("Authorization", "Bearer " + gameInfo.Token)
                    .SetHeader("game-id", gameInfo.GameId)
                    .AddBody("guid", guid)
                    .AddBody("hash", fileInfo.hash)
                    .AddBody("group", fileInfo.group)
                    .AddBody("name", fileInfo.name)
                    .AddBody("env", (int) gameInfo.Environment)
                    .SetMultipart()
                    .AddTexture(newTexture, fileInfo.texturePath, fileInfo.name);

                yield return UnityUtils.SendRequest(req, request =>
                {
#if UNITY_2020_1_OR_NEWER
                    if (request.result != UnityWebRequest.Result.Success)
#else
                    if (request.isNetworkError || request.isHttpError)
#endif
                    {
                        currentError = request.error;
                        Debug.LogWarning("Image upload error: " + request.error + " : " + request.url);
                    }
                });
            }
            
            gameInfo.OnComplete?.Invoke(currentError);
        }

        private static Texture2D GetCompressedTexture(FileInfo fileInfo, Vector2Int maxSize)
        {
            if (string.IsNullOrEmpty(fileInfo.texturePath))
            {
                Debug.LogError("No image found for guid " + fileInfo.guid);
                return null;
            }

            var tImporter = AssetImporter.GetAtPath(fileInfo.texturePath) as TextureImporter;
            if (tImporter == null)
                return null;

            tImporter.isReadable = true;
            AssetDatabase.ImportAsset(fileInfo.texturePath);
            AssetDatabase.Refresh();
        
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(fileInfo.texturePath);

            Texture2D newTexture = null;
            if (texture.width > maxSize.x || texture.height > maxSize.y)
            {
                var scaleX = (float) maxSize.x / texture.width;
                var scaleY = (float) maxSize.y / texture.height;
                var scale = Mathf.Min(scaleX, scaleY);

                int newWidth = Mathf.Max(Mathf.RoundToInt(scale * texture.width), 1);
                int newHeight = Mathf.Max(Mathf.RoundToInt(scale * texture.height), 1);

                newTexture = ScaleTexture(texture, newWidth, newHeight);
            }
            else
            {
                newTexture = CopyTexture(texture);
            }

            tImporter.isReadable = false;
            AssetDatabase.ImportAsset(fileInfo.texturePath);
            AssetDatabase.Refresh();

            return newTexture;
        }

        private static Texture2D CopyTexture(Texture2D source)
        {
            Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            texture2D.SetPixels32(source.GetPixels32(), 0);
            texture2D.Apply();

            return texture2D;
        }

        private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D texture2D = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
            Color[] pixels = texture2D.GetPixels(0);
            Color32[] sourcePixels = source.GetPixels32(0);
            
            float num1 = 1f / (float) targetWidth;
            float num2 = 1f / (float) targetHeight;
            for (int index = 0; index < pixels.Length; ++index)
            {
                var tx = (index % targetWidth) * num1;
                var ty = (index / targetWidth) * num2;
                var sx = Mathf.RoundToInt(tx * source.width);
                var sy = Mathf.RoundToInt(ty * source.height);
                pixels[index] = sourcePixels[sx + sy * source.width];
            }

            texture2D.SetPixels(pixels, 0);
            texture2D.Apply();
            return texture2D;
        }

        private static Dictionary<string, FileInfo> MapFiles(FullInfo info)
        {
            return info.groups.SelectMany(group => group.entries).ToDictionary(entry => entry.guid);
        }

        private static string ConvertToJson(FullInfo info)
        {
            return JsonConvert.SerializeObject(info);
        }
    }

    [ExecuteInEditMode]
    public class Addressables_Editor : EditorWindow
    {
        private class GroupSetting
        {
            public string Guid;
            public bool Selected;
        }

        private static string _gameId;
        private static AddressableAssetSettings _settings;
        private static List<GroupSetting> _groups;
        
        private Vector2 _scrollPos;

        internal static List<string> GetSelectedGroups(string gameId)
        {
            var groups = new List<string>();
            
            var set = PlayerPrefs.GetString("Balancy_addressables_settings_" + gameId);
            if (!string.IsNullOrEmpty(set))
            {
                var gs = set.Split(',');
                foreach (var group in gs)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

        internal static void ShowWindow(AddressableAssetSettings settings, string gameId)
        {
            _gameId = gameId;
            _settings = settings;
            var window = GetWindow(typeof(Addressables_Editor));
            window.titleContent.text = "Addressables Config";

            _groups = new List<GroupSetting>();

            var groups = GetSelectedGroups(gameId);
            foreach (var group in groups)
            {
                _groups.Add(new GroupSetting { Guid = group, Selected = true });
            }

            window.Show();
        }

        private void Save()
        {
            PlayerPrefs.SetString("Balancy_addressables_settings_" + _gameId, String.Join(",", _groups.FindAll(g => g.Selected).Select(g => g.Guid)));
            PlayerPrefs.Save();
        }

        private void OnGUI()
        {
            if (_settings == null || _settings.groups == null)
            {
                Close();
                return;
            }

            var select = _groups.Count == 0 || _groups.Any(g => !g.Selected);
            if (GUILayout.Button(select ? "Select all" : "Unselect all"))
            {
                foreach (var group in _settings.groups)
                {
                    if (group.ReadOnly)
                        continue;

                    var curGroup = _groups.Find(g => g.Guid == group.Guid);

                    if (curGroup == null)
                    {
                        curGroup = new GroupSetting { Guid = group.Guid, Selected = select };
                        _groups.Add(curGroup);
                    }
                    else
                    {
                        curGroup.Selected = select;
                    }

                    Save();
                }
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(300));

            foreach (var group in _settings.groups)
            {
                if (group.ReadOnly)
                    continue;
                var setGroup = _groups.Find(g => g.Guid == group.Guid);
                if (setGroup == null)
                {
                    setGroup = new GroupSetting { Guid = group.Guid, Selected = false };
                    _groups.Add(setGroup);
                }

                var newValue = EditorGUILayout.Toggle(group.Name, setGroup.Selected);
                if (setGroup.Selected != newValue)
                {
                    setGroup.Selected = newValue;
                    Save();
                }

                setGroup.Selected = newValue;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);



            if (GUILayout.Button("Synch Addressables"))
                AddressablesHelper.SynchAddressables();
        }
    }
}