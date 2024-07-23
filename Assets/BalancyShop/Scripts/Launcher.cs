using System;
using System.Collections.Generic;
using Balancy;
using UnityEngine;

namespace BalancyShop
{
    public class Launcher : MonoBehaviour
    {
        private const string GAME_ID = "game_id";
        private const string PUBLIC_KEY = "public_key";
        
        [SerializeField] private string apiGameId;
        [SerializeField] private string publicKey;

        [SerializeField] private DemoUI canvasDemo;
        
        private Dictionary<string, string> _urlParams;
        
        private void Start()
        {
            _urlParams = ParseUrl();
            
            ExternalEvents.RegisterSmartObjectsListener(new BalancyShopSmartObjectsEvents());

            BalancyShopSmartObjectsEvents.onSmartObjectsInitializedEvent += () =>
            {
                canvasDemo.Refresh();
            };

            canvasDemo.Init();
            var t1 = Time.realtimeSinceStartup;
            Balancy.Main.Init(new AppConfig
            {
                ApiGameId = GetGameId(),
                PublicKey = publicKey,
                Environment = GetEnvironment(),                
                OnInitProgress = progress =>
                {
                    var t2 = Time.realtimeSinceStartup;
                    Debug.Log($"***=> STATUS {progress.Status} in {t2-t1} s");
                    switch (progress.Status)
                    {
                        case BalancyInitStatus.PreInitFromResourcesOrCache:
                            //CMS, loaded from resource or cache is ready, invoked only if PreInitFromResourcesOrCache is true
                            break;
                        case BalancyInitStatus.DictionariesReady:
                            //CMS is updated and ready
                            break;
                        case BalancyInitStatus.Finished:
                            //All systems are ready
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                },
                UpdateType = UpdateType.FullUpdate,
                UpdatePeriod = 5,
                OnContentUpdateCallback = updateResponse =>
                {
                    Debug.Log("Content Updated " + updateResponse.AffectedDictionaries.Length);
                    canvasDemo.Refresh();
                },
                OnReadyCallback = response =>
                {
                    Debug.Log($"Balancy Init Complete: {response.Success}, deploy version = {response.DeployVersion} user={Auth.GetUserId()}");
                    if (!response.Success)
                        Debug.LogError($"ERR {response.Error.Code} - {response.Error.Message}");
                    // canvasDemo.Refresh();
                }
            });
        }
        
        Constants.Environment GetEnvironment()
        {
#if GAME_SERVER
            return Constants.Environment.Production;
#elif GAME_STAGE
            return Constants.Environment.Stage;
#else
            return Constants.Environment.Development;
#endif
        }
        
        private string GetGameId()
        {
            return _urlParams.TryGetValue(GAME_ID, out var gameId) ? gameId : apiGameId;
        }
        
        private Dictionary<string, string> ParseUrl()
        {
            var paramsDict = new Dictionary<string, string>();

            var url = Application.absoluteURL;
            var splitUrl = url.Split('?');
            if (splitUrl.Length == 2)
            {
                var prms = splitUrl[1];
                var splitParams = prms.Split('&');
                foreach (var kvp in splitParams)
                {
                    var splitKvp = kvp.Split('=');
                    if (splitKvp.Length == 2)
                    {
                        if (!paramsDict.ContainsKey(splitKvp[0]))
                            paramsDict.Add(splitKvp[0], splitKvp[1]);
                    }
                }
            }

            return paramsDict;
        }
    }
}
