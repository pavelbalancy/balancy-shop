using Balancy.Models.GameShop;
using UnityEngine;

namespace BalancyShop
{
    public class BaseWindow : MonoBehaviour
    {
        [SerializeField] public WindowType winType;

        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            if (isActive)
                Refresh();
        }

        public virtual void Refresh()
        {
            
        }
    }
}
