using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BalancyShop
{
    public class MyButton : Button
    {
        private const float ScaleAmount = 0.95f; // Change this to the desired scale amount when the button is pressed
        private Vector3 originalScale;

        protected override void Awake()
        {
            base.Awake();
            originalScale = transform.localScale;
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (this.interactable)
                transform.localScale = originalScale * ScaleAmount;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            transform.localScale = originalScale;
        }
    }
}