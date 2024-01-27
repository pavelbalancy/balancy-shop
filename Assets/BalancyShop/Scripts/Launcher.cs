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

        [SerializeField] private DemoUI canvasDemo;
        
        private void Start()
        {
            ExternalEvents.RegisterSmartObjectsListener(new BalancyShopSmartObjectsEvents());

            canvasDemo.Init();
            Balancy.Main.Init(new AppConfig
            {
                ApiGameId = apiGameId,
                PublicKey = publicKey,
                Environment = GetEnvironment(),                
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
                    canvasDemo.Refresh();
                },
                OnReadyCallback = response =>
                {
                    Debug.Log($"Balancy Init Complete: {response.Success}, deploy version = {response.DeployVersion} user={Auth.GetUserId()}");
                    if (!response.Success)
                        Debug.LogError($"ERR {response.Error.Code} - {response.Error.Message}");
                    canvasDemo.Refresh();
                }
            });
        }
        
        Constants.Environment GetEnvironment()
        {
            return Constants.Environment.Production;
        }
    }
}
