using UnityEngine;

namespace BalancyShop
{
    public class TakeSizeFromContent : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private bool isHorizontal;
        [SerializeField] private bool isVertical;
        private RectTransform myTransform;

        private void Awake()
        {
            myTransform = transform as RectTransform;
        }

        public void SetContent(RectTransform child)
        {
            content = child;
        }

        private void Update()
        {
            if (content != null)
            {
                if (isVertical && Mathf.Abs(content.sizeDelta.y - myTransform.sizeDelta.y) > float.Epsilon)
                {
                    var newSize = myTransform.sizeDelta;
                    newSize.y = content.sizeDelta.y;
                    myTransform.sizeDelta = newSize;
                }
                
                if (isHorizontal && Mathf.Abs(content.sizeDelta.x - myTransform.sizeDelta.x) > float.Epsilon)
                {
                    var newSize = myTransform.sizeDelta;
                    newSize.x = content.sizeDelta.x;
                    myTransform.sizeDelta = newSize;
                }
            }
        }
    }
}