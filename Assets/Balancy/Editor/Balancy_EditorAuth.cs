#if UNITY_EDITOR && !BALANCY_SERVER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Balancy.Dictionaries;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Balancy.Editor
{
    public class Balancy_EditorAuth
    {
        private string _userEmail;
        private string _userPassword;
        
        private static readonly string USER_INFO_FOLDER = Application.persistentDataPath + "/Balancy/Editor";
        private static readonly string USER_INFO_PATH = USER_INFO_FOLDER + "/user.info";
        
        private static readonly GUILayoutOption LAYOUT_BUTTON_LOGOUT = GUILayout.Width(30);
        
        public class GameInfo
        {
            [JsonProperty("id")]
            public string GameId;
            [JsonProperty("name")]
            public string GameName;
            [JsonProperty("publicKey")]
            public string PublicKey;
            
            private int SelectedBranchIndex;
            [JsonIgnore] public string[] AllBranches;
            [JsonIgnore] private BranchesFile _branchesFile;
            
            private UserInfo _userInfo;

            internal void InitBranches(UserInfo userInfo, string data)
            {
                _userInfo = userInfo;
                _branchesFile = JsonConvert.DeserializeObject<BranchesFile>(data);
                AllBranches = new string[_branchesFile.Branches.Length];
                for (int i = 0;i<_branchesFile.Branches.Length;i++)
                    AllBranches[i] = _branchesFile.Branches[i].Name;

                SelectedBranchIndex = GetBranchIndex(userInfo.GetSelectedBranchId(GameId));
            }

            public int GetSelectedBranchId()
            {
                return GetSelectedBranch()?.Id ?? -1;
            }

            public BranchesFile.BranchInfo GetSelectedBranch()
            {
                if (SelectedBranchIndex >= 0 && _branchesFile?.Branches != null)
                    return _branchesFile.Branches[SelectedBranchIndex];
                return null;
            }
            
            private int GetBranchIndex(int branchId)
            {
                for (int i = 0; i < _branchesFile.Branches.Length; i++)
                {
                    if (_branchesFile.Branches[i].Id == branchId)
                        return i;
                }

                return -1;
            }
            
            public void RenderBranches()
            {
                EditorGUI.BeginChangeCheck();
                var color = GUI.color;
                if (SelectedBranchIndex < 0)
                    GUI.color = Color.red;
                SelectedBranchIndex = EditorGUILayout.Popup("Selected Branch: ", SelectedBranchIndex, AllBranches);
                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log(AllBranches[SelectedBranchIndex]);
                    _userInfo.SetServerBranch(GameId, _branchesFile.Branches[SelectedBranchIndex].Id);
                }

                GUI.color = color;
            }
        }

        private class GamesInfo
        {
            [JsonProperty("games")]
            public GameInfo[] Games;

            private string[] _gamesList;
            private int _selectedGame;
            private Vector2 _scrollPos;
            private UserInfo _userInfo;

            public void Init(UserInfo userInfo)
            {
                int count = Games.Length;
                _gamesList = new string[count];
                for (int i = 0; i < count; i++)
                    _gamesList[i] = Games[i].GameName;

                _userInfo = userInfo;
                _selectedGame = GetGameIndex(userInfo.SelectedGameId);
            }
            
            private int GetGameIndex(string gameId)
            {
                for (int i = 0; i < Games.Length; i++)
                {
                    if (Games[i].GameId == gameId)
                        return i;
                }

                return -1;
            }

            public void Render()
            {
                EditorGUI.BeginChangeCheck();
                _selectedGame = EditorGUILayout.Popup("Selected Game: ", _selectedGame, _gamesList);

                if (EditorGUI.EndChangeCheck())
                    if (_selectedGame != -1)
                        _userInfo.SetSelectedGameId(Games[_selectedGame].GameId);
            }

            public GameInfo GetSelectedGameInfo()
            {
                if (_selectedGame < 0 || _selectedGame >= Games.Length)
                    return null;

                return Games[_selectedGame];
            }

            public bool HasSelectedGame()
            {
                return _selectedGame >= 0;
            }
        }

        internal class AuthResponse
        {
            [JsonProperty("accessToken")]
            public string AccessToken;
            [JsonProperty("email")]
            public string Email;
        }
        
        internal class UserInfo : AuthResponse
        {
            [JsonProperty("selectedGameId")]
            public string SelectedGameId { get; private set; }

            [JsonProperty("selectedBranches")]
            public Dictionary<string, int> SelectedBranches { get; private set; }
            
            [JsonProperty("privateKey")]
            public string PrivateKey;
            
            public void Save()
            {
                Balancy.Dictionaries.FileHelper.CheckFolder(USER_INFO_FOLDER);
                var text = JsonConvert.SerializeObject(this);
                File.WriteAllText(USER_INFO_PATH, text);
            }

            public static UserInfo InitDefault()
            {
                var info = new UserInfo();
                info.Save();

                return info;
            }

            public void SetSelectedGameId(string gameId)
            {
                SelectedGameId = gameId;
                Save();
            }
            
            public void SetServerBranch(string gameId, int branchId)
            {
                if (SelectedBranches == null)
                    SelectedBranches = new Dictionary<string, int>();
                SelectedBranches[gameId] = branchId;
                Save();
            }

            public int GetSelectedBranchId(string gameId)
            {
                if (SelectedBranches == null)
                    SelectedBranches = new Dictionary<string, int>();
                if (SelectedBranches.TryGetValue(gameId, out var id))
                    return id;
                return -1;
            }
        }

        private UserInfo _userInfo;

        private bool _loadingGames;
        private bool _loadingBranches;
        private GamesInfo _gamesInfo;
        private EditorWindow _parent;

        public Balancy_EditorAuth(EditorWindow parent)
        {
            if (File.Exists(USER_INFO_PATH))
            {
                var allText = File.ReadAllText(USER_INFO_PATH);
                _userInfo = JsonConvert.DeserializeObject<UserInfo>(allText);
                if (_userInfo == null)
                    _userInfo = UserInfo.InitDefault();
                _userEmail = _userInfo?.Email;
            }
            else
            {
                _userInfo = UserInfo.InitDefault();
            }

            _parent = parent;
        }

        private void Repaint()
        {
            // _parent.Repaint();
        }

        public void Render()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            RenderAuth();
            RenderGames();
            RenderBranches();
            GUILayout.EndVertical();
        }

        private void RenderGames()
        {
            if (!IsAuthorized())
                return;

            if (!_loadingGames)
            {
                if (_gamesInfo != null)
                {
                    _gamesInfo.Render();
                }
                else
                {
                    _loadingGames = true;
                    SendGamesRequest();
                }
            }
        }
        
        private void RenderBranches()
        {
            if (!IsAuthorized() || _loadingGames || _gamesInfo == null)
                return;

            if (!_loadingBranches)
            {
                var gameInfo = _gamesInfo.GetSelectedGameInfo();
                if(gameInfo == null)
                    return;

                if (gameInfo.AllBranches != null)
                {
                    gameInfo.RenderBranches();
                }
                else
                {
                    _loadingBranches = true;
                    SendBranchesRequest(gameInfo);
                }
            }
        }

        private void RenderAuth()
        {
            if (IsAuthorized())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Balancy User: ");

                var color = GUI.color;
                GUI.color = Color.green;
                GUILayout.Label(_userInfo.Email);
                GUI.color = color;
                
                if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), LAYOUT_BUTTON_LOGOUT))
                    LogOut();
                
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Balancy User");
                _userEmail = EditorGUILayout.TextField("Email", _userEmail);
                _userPassword = EditorGUILayout.PasswordField("Password", _userPassword);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Click to create a new Account"))
                {
                    Application.OpenURL("https://balancy.dev/auth");
                }

                GUI.enabled = !string.IsNullOrEmpty(_userEmail) && !string.IsNullOrEmpty(_userPassword);
                if (GUILayout.Button("Authorize"))
                {
                    SendAuthRequest(_userEmail, _userPassword);
                }

                GUI.enabled = true;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
        
        public bool HasSelectedGame()
        {
            return IsAuthorized() && (_gamesInfo?.HasSelectedGame() ?? false);
        }
        
        public bool HasSelectedBranch()
        {
            return HasSelectedGame() && (_gamesInfo.GetSelectedGameInfo().GetSelectedBranchId() >= 0);
        }

        public bool IsAuthorized()
        {
            return !string.IsNullOrEmpty(GetAccessToken());
        }

        public string GetAccessToken()
        {
            return _userInfo?.AccessToken;
        }
        
        private void SendGamesRequest()
        {
            SendDefaultRequest("/v1/games", "GET", (data, error) =>
            {
                if (!string.IsNullOrEmpty(data))
                {
                    _gamesInfo = JsonConvert.DeserializeObject<GamesInfo>(data);
                    _gamesInfo.Init(_userInfo);
                }
                else
                    LogOut();
                _loadingGames = false;
            });
        }
        
        private void SendBranchesRequest(GameInfo gameInfo)
        {
            SendDefaultRequest($"/v1/games/{gameInfo.GameId}/branches", "GET", (data, error) =>
            {
                gameInfo.InitBranches(_userInfo, data);
                _loadingBranches = false;
            });
        }

        private void LogOut()
        {
            _userInfo = UserInfo.InitDefault();
            _gamesInfo = null;
        }

        public void GenerateCode()
        {
        }
        
        public void SynchAddressables()
        {
        }

        private void SendDefaultRequest(string url, string method, Action<string, string> callback)
        {
            var helper = EditorCoroutineHelper.Create();
            var req = new EditorUtils.ServerRequest(url, method);
            req.SetHeader("Content-Type", "application/json;charset=UTF-8")
                // .SetHeader("Accept", "application/json")
                .SetHeader("authorization", "Bearer " + _userInfo.AccessToken);
            
            var cor = UnityUtils.SendRequest(req, request =>
            {
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    callback?.Invoke(null, request.error);
                    EditorUtility.DisplayDialog("Error", "Failed " + url + " with error " + request.error, "Ok");
                }
                else
                {
                    callback?.Invoke(request.downloadHandler.text, null);
                    Repaint();
                }
            });
            helper.LaunchCoroutine(cor);
        }

        private void SendAuthRequest(string email, string password)
        {
            var helper = EditorCoroutineHelper.Create();
            var req = new EditorUtils.ServerRequest("/v1/sign", "POST");
            req.SetHeader("Content-Type", "application/json;charset=UTF-8")
                .SetHeader("Accept", "application/json")
                .AddBody("email", email)
                .AddBody("password", password);

            var cor = UnityUtils.SendRequest(req, request =>
            {
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    EditorUtility.DisplayDialog("Error", "Failed to authorize " + request.error, "Ok");
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<UserInfo>(request.downloadHandler.text);
                    if (string.IsNullOrEmpty(response.AccessToken))
                        EditorUtility.DisplayDialog("Error", "Authorized, but had received a bad token " + request.downloadHandler.text, "Ok");
                    else
                    {
                        _userInfo = response;
                        _userInfo.Save();
                    }
                }
            });
            helper.LaunchCoroutine(cor);
        }

        public string GetPrivateKey()
        {
            return _userInfo?.PrivateKey;
        }
        
        public void SetPrivateKey(string privateKey)
        {
            if (_userInfo != null)
            {
                _userInfo.PrivateKey = privateKey;
                _userInfo.Save();
            }
        }
        
        public GameInfo GetSelectedGameInfo()
        {
            return _gamesInfo.GetSelectedGameInfo();
        }

        internal UserInfo GetUserInfo()
        {
            return _userInfo;
        }
    }
}
#endif