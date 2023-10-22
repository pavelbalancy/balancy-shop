using UnityEngine;
using UnityEngine.UI;

namespace Balancy.Example
{
    public class ForceTransformUpdater : MonoBehaviour
    {
        private RectTransform _transform;

        private void FindTransform()
        {
            if (_transform == null)
                _transform = transform as RectTransform;
        }

        public void ForceUpdate()
        {
            FindTransform();
            Calculate();
        }

        private void Calculate()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);
        }
    }
}
