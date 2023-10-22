using System;
using Balancy;
using Balancy.Example;
using Balancy.Models.BalancyShop;
using UnityEngine;

namespace BalancyShop
{
    public class Launcher : MonoBehaviour
    {
        [SerializeField] private string apiGameId;
        [SerializeField] private string publicKey;
        
        [SerializeField] private ShopWindow winStore;
        
        private void Start()
        {
            ExternalEvents.RegisterSmartObjectsListener(new BalancyShopSmartObjectsEvents());
            BalancyShopSmartObjectsEvents.onSmartObjectsInitializedEvent += InitStore;

            Balancy.Main.Init(new AppConfig
            {
                ApiGameId = apiGameId,
                PublicKey = publicKey,
                Environment = GetEnvironment(),                
                PreInitFromResourcesOrCache = true,//It's used for the fast launches
                OnInitProgress = progress =>
                {
                    Debug.Log($"***=> STATUS {progress.Status}");
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
                    winStore.Refresh(LiveOps.Store.DefaultStore);
                },
                OnReadyCallback = response =>
                {
                    Debug.Log($"Balancy Init Complete: {response.Success}, deploy version = {response.DeployVersion}");
                    winStore.Init(LiveOps.Store.DefaultStore);
                }
            });
        }

        private void InitStore()
        {
            BalancyShopSmartObjectsEvents.onSmartObjectsInitializedEvent -= InitStore;
            
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
    }
}
