using Balancy.Example;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;

namespace BalancyShop
{
    public class ShopWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private RectTransform content;
        
        [SerializeField] private GameObject pagePrefab;
        [SerializeField] private GameObject pageOffersPrefab;

        private GameStoreBase _smartConfig;

        public void Init(SmartConfig smartConfig)
        {
            Refresh(smartConfig);
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (_smartConfig != null)
            {
                _smartConfig.OnStoreUpdatedEvent -= Refresh;
                _smartConfig = null;
            }
        }

        public void Refresh(GameStoreBase smartConfig)
        {
            if (_smartConfig != smartConfig)
            {
                CleanUp();
                _smartConfig = smartConfig;
                _smartConfig.OnStoreUpdatedEvent += Refresh;
            }
            
            content.RemoveChildren();

            AddOffersSection();
            
            var pages = smartConfig.ActivePages;
            foreach (var page in pages)
            {
                var newButton = Instantiate(pagePrefab, content);
                var storeTabButton = newButton.GetComponent<PageView>();
                storeTabButton.Init(smartConfig, page);
            }
        }

        private void AddOffersSection()
        {
            var newButton = Instantiate(pageOffersPrefab, content);
            var storeTabButton = newButton.GetComponent<PageViewOffers>();
            storeTabButton.Init();
        }
    }
}
