using TMPro;
using UnityEngine;

namespace BalancyShop
{
    public class BattleWindow : BaseWindow
    {
        [SerializeField] private TMP_Text userId;
        [SerializeField] private MyButton copyButton;
        
        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            if (isActive)
                userId.SetText(Balancy.Auth.GetUserId());   
        }

        private void Awake()
        {
            copyButton.onClick.AddListener(OnCopyClicked);
        }

        private void OnCopyClicked()
        {
            GUIUtility.systemCopyBuffer = Balancy.Auth.GetUserId();
        }
    }
}
