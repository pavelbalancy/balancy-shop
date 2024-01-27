using System;
using System.IO;
using Balancy.Data;
using Balancy.Models;
using Newtonsoft.Json;

namespace Balancy
{
    public partial class DataEditor
    {
        static partial void PrepareGeneratedData();
        static partial void MoveAllData(string userId);
        static partial void TransferAllSmartObjectsFromLocalToCloud(string userId);
        static partial void PreloadAllSmartObjects(string userId, bool skipServerLoading);
        static partial void PreloadAllSmartObjects(string userId);
        static partial void ResetAllSmartObjects(string userId);

        public static void Init()
        {
            Storage.OnPrepareModelsAndData = PrepareModelsAndData;
            Storage.OnUserChange = CompleteUserChange;
            Storage.OnResetAllProfiles = ResetAllProfiles;
            Storage.OnPreloadAllSmartObjects = Preload;
        }

        private static void PrepareModelsAndData()
        {
            DataManager.Init();
            PrepareGeneratedData();
            CheckProfilesForTheMigration();
        }

        protected static DataManager.ParseWrapper<T> ParseDictionary<T>() where T : BaseModel
        {
            return DataManager.ParseDictionary<T>();
        }
        
        public static T GetModelByUnnyId<T>(string unnyId) where T : BaseModel
        {
            return DataManager.GetModelByUnnyId<T>(unnyId);
        }

        private static void CheckProfilesForTheMigration()
        {
            var path = GetOfflineProfileOldPath();
            if (File.Exists(path))
            {
                var streamReader = new StreamReader(path, true);
                var data = streamReader.ReadToEnd();
                streamReader.Close();
                
                var systemData = JsonConvert.DeserializeObject<UnnySystemData>(data);
                var userId = string.IsNullOrEmpty(systemData?.ProfileId) ? Constants.LOCAL_USER : systemData.ProfileId;

                MoveAllData(userId);
            }
        }

        private static string GetOfflineProfileOldPath()
        {
            return $"{UnityEngine.Application.persistentDataPath}/UnnyProfile/UnnySystem";
        }

        internal static void CompleteUserChange(string userId)
        {
            MigrateOfflineDataToUser(userId);
            TransferAllSmartObjectsFromLocalToCloud(userId);
        }
        
        public static void ResetAllProfiles(string gameId, Constants.Environment environment)
        {
#if !BALANCY_SERVER
            var userId = Auth.GetUserId(gameId, environment);
            Storage.SetGameId(gameId);
            ResetAllProfiles(userId, null);
#endif
        }

        public static void ResetAllProfiles(string userId, Action callback)
        {
            ResetAllSmartObjects(userId);
            
            InvokeCallbackWhenResetsAreComplete(() =>
            {
                TransferAllSmartObjectsFromLocalToCloud(userId);
                callback?.Invoke();
            });
        }
        
        internal static void Preload(string userId, bool skipServerLoading)
        {
            PreloadAllSmartObjects(userId, skipServerLoading);
        }
        
        private static void InvokeCallbackWhenResetsAreComplete(Action callback)
        {
            if (_resetsInProgress == 0)
                callback.Invoke();
            else
                _resetCompletedCallback += callback;
        }

        public static void MigrateSmartObject(string userId, string dataName)
        {
#if !BALANCY_SERVER
            var folder = UnityEngine.Application.persistentDataPath + "/";
            var oldPath = $"{folder}{dataName}";
            var newPath =  $"{Storage.GetSmartObjectPath(userId)}/{dataName}";

            if (Directory.Exists(oldPath))
            {
                if (Directory.Exists(newPath))
                {
                    UnityEngine.Debug.Log("already exists");
                    Directory.Delete(newPath, true);
                }

                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                Directory.Move(oldPath, newPath);
            }
#endif
        }

        public static void TransferSmartObjectFromLocalToCloud<T>(string userId) where T : ParentBaseData, new()
        {
#if !BALANCY_SERVER
            SmartStorage.LoadSmartObjectFromLocal<T>(userId);
#endif
        }
        
        private static int _resetsInProgress = 0;
        private static Action _resetCompletedCallback;

        public static void ResetSmartObject<T>(string userId) where T : ParentBaseData, new()
        {
            _resetsInProgress++;
            SmartStorage.ClearSmartObject<T>(userId, (repsonse)=>
            {
                _resetsInProgress--;
                
                if (_resetsInProgress == 0 && _resetCompletedCallback != null)
                {
                    var m = _resetCompletedCallback;
                    _resetCompletedCallback = null;
                    m.Invoke();
                }
            });
        }

        private static void MigrateOfflineDataToUser(string userId)
        {
#if !BALANCY_SERVER
            var oldPath = Storage.GetSmartObjectPath(Constants.LOCAL_USER);
            var newPath = Storage.GetSmartObjectPath(userId);

            if (Directory.Exists(oldPath))
            {
                if (Directory.Exists(newPath))
                {
                    Directory.Delete(newPath, true);
                }

                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                Directory.Move(oldPath, newPath);
            }
#endif
        }
    }
}
