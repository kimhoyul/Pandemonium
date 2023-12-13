using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOONIPLAY
{
    public class UIScrollRect : ScrollRect
    {
        [SerializeField] private Image _leftEdge;
        [SerializeField] private Image _rightEdge;
        [SerializeField] private Image _topEdge;
        [SerializeField] private Image _bottomEdge;

        private bool _isRouteToParent = false;

        private void Update()
        {
            if (horizontal &&  _leftEdge != null && _rightEdge != null)
            {
                ShowEdge(_leftEdge, horizontalNormalizedPosition > 0f + float.Epsilon);
                ShowEdge(_rightEdge, horizontalNormalizedPosition < 1f - float.Epsilon);
            }

            if (vertical && _topEdge != null && _bottomEdge != null)
            {
                ShowEdge(_topEdge, verticalNormalizedPosition < 1f - float.Epsilon);
                ShowEdge(_bottomEdge, verticalNormalizedPosition > 0f + float.Epsilon);
            }
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            DoForParents<IInitializePotentialDragHandler>(parent => { parent.OnInitializePotentialDrag(eventData); });
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_isRouteToParent)
                DoForParents<IDragHandler>(parent => { parent.OnDrag(eventData); });
            else
                base.OnDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                _isRouteToParent = true;
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                _isRouteToParent = true;
            else
                _isRouteToParent = false;

            if (_isRouteToParent)
                DoForParents<IBeginDragHandler>(parent => { parent.OnBeginDrag(eventData); });
            else
                base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_isRouteToParent)
                DoForParents<IEndDragHandler>(parent => { parent.OnEndDrag(eventData); });
            else
                base.OnEndDrag(eventData);
            _isRouteToParent = false;
        }

        private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                foreach (var component in parent.GetComponents<Component>())
                {
                    if (component is T)
                        action((T)(IEventSystemHandler)component);
                }
                parent = parent.parent;
            }
        }

        private void ShowEdge(Image image, bool isShowShadow)
        {
            if (image.gameObject.activeSelf != isShowShadow)
                image.gameObject.SetActive(isShowShadow);
        }
    }
}