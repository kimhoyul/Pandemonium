using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOONIPLAY
{
    [RequireComponent(typeof(Selectable))]
    public class SelectableObjectColorChange : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        private Selectable _selectable;

        [SerializeField]
        [Tooltip("자식 객체에 색상 변화를 적용할지 여부")]
        private bool _isChangeApplyChild = true;
        private readonly List<Graphic> _allChildsGraphic = new();

        private bool _isSelect;

        protected SelectableObjectColorChange()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_selectable.interactable)
                return;

            ChangeChildsColor(_selectable.colors.pressedColor, false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_selectable.interactable)
                return;

            ChangeChildsColor(_selectable.colors.selectedColor, false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelect || !_selectable.interactable)
                return;

            ChangeChildsColor(_selectable.colors.highlightedColor, false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelect || !_selectable.interactable)
                return;

            ChangeChildsColor(_selectable.colors.normalColor, false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!_selectable.interactable)
                return;

            ChangeChildsColor(_selectable.colors.selectedColor, false);
            _isSelect = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!_selectable.interactable)
                return;

            ChangeChildsColor(_selectable.colors.normalColor, false);
            _isSelect = false;
        }

        protected override void Start()
        {
            base.Start();

            _selectable = GetComponent<Selectable>();

            CircuitChilds(gameObject);
            ChangeChildsColor(_selectable.interactable ? _selectable.colors.normalColor : _selectable.colors.disabledColor, false);
        }

        /// <summary>
        /// 오브젝트를 순회하며 하위에 존재하는 모든 자식 오브젝트의 그래픽을 찾는다.
        /// </summary>
        /// <param name="targetObject">이번에 탐색할 오브젝트</param>
        private void CircuitChilds(GameObject targetObject)
        {
            if (_isChangeApplyChild == false)
                return;

            int childCount = targetObject.transform.childCount;
            if (childCount == 0)
                return;

            GameObject[] childs = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                childs[i] = targetObject.transform.GetChild(i).gameObject;
                CircuitChilds(childs[i]);
                _allChildsGraphic.Add(childs[i].GetComponent<Graphic>());
            }
        }

        /// <summary>
        /// 지정한 색상으로 모든 하위 객체의 색을 변경한다.
        /// </summary>
        /// <param name="color">변경할 색상</param>
        /// <param name="instant">즉시 변경 여부</param>
        private void ChangeChildsColor(Color color, bool instant)
        {
            if (_isChangeApplyChild == false)
                return;

            for (int i = 0; i < _allChildsGraphic.Count; i++)
            {
                var graphic = _allChildsGraphic[i];
                if (graphic != null)
                {
                    graphic.CrossFadeColor(color, instant ? 0f : _selectable.colors.fadeDuration, true, true);
                }
            }
        }
    }
}
