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

        private SmartConfig _smartConfig;

        public void Init(SmartConfig smartConfig)
        {
            _smartConfig = smartConfig;
            _smartConfig.OnStoreUpdatedEvent += Refresh;
            Refresh(_smartConfig);
        }

        private void OnDestroy()
        {
            if (_smartConfig != null)
            {
                _smartConfig.OnStoreUpdatedEvent -= Refresh;
                _smartConfig = null;
            }
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
    }
}
