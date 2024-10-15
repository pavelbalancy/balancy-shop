using System;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;

namespace Balancy.Example
{
    public class WinStore : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private RectTransform content;
        
        [SerializeField] private GameObject pagePrefab;

#if !BALANCY_SERVER

        public void Init(GameStoreBase smartConfig)
        {
            GameStoreBase.OnStoreUpdatedEvent += Refresh;
            Refresh(smartConfig);
        }

        private void OnDestroy()
        {
            GameStoreBase.OnStoreUpdatedEvent -= Refresh;
        }

        private void Refresh(GameStoreBase smartConfig)
        {
            content.RemoveChildren();

            var pages = smartConfig.ActivePages;
        
            foreach (var page in pages)
            {
                var newButton = Instantiate(pagePrefab, content);
                var storeTabButton = newButton.GetComponent<PageView>();
                storeTabButton.Init(smartConfig, page);
            }
        }
#endif
    }
}