using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOONIPLAY
{
	public class ToggleObjectActiveChange : UIBehaviour
	{
        private Toggle _toggle;
        private UIToggle _uiToggle;

        [Tooltip("자식 객체에 변화를 적용할지 여부")]
        [SerializeField] private bool _isChangeApplyChild = true;

        [Tooltip("토글 여부에 따라 활성화할 객체")]
        [SerializeField] private GameObject _graphics;

        private List<Graphic> _allChildsGraphic = new List<Graphic>();

		public ToggleObjectActiveChange()
		{
		}

        protected override void Start()
        {
            base.Start();

            if (_isChangeApplyChild == false)
                return;

            _toggle = GetComponent<Toggle>();
            _uiToggle = GetComponent<UIToggle>();
            
            CircuitChilds(_graphics);

            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(ChangeChildsActive);
                ChangeChildsActive(_toggle.isOn, true);
            }
            else if (_uiToggle != null)
            {
                _uiToggle.onValueChanged.AddListener(ChangeChildsActive);
                ChangeChildsActive(_uiToggle.isOn, true);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_isChangeApplyChild == false)
                return;

            if (_toggle != null)
            {
                ChangeChildsActive(_toggle.isOn, true);
            }
            else if (_uiToggle != null)
            {
                ChangeChildsActive(_uiToggle.isOn, true);
            }
        }

        /// <summary>
        /// 오브젝트를 순회하며 하위에 존재하는 모든 자식 오브젝트의 그래픽을 찾는다.
        /// </summary>
        /// <param name="targetObject">이번에 탐색할 오브젝트</param>
        private void CircuitChilds(GameObject targetObject)
        {
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
        /// 모든 하위 객체를 활성화 혹은 비활성화한다.
        /// </summary>
        /// <param name="active">활성화 여부</param>
        /// <param name="instant">fade 효과 여부</param>
        private void ChangeChildsActive(bool active, bool instant)
        {
            for (int i = 0; i < _allChildsGraphic.Count; i++)
            {
                var graphic = _allChildsGraphic[i];
                if (graphic != null)
                {
                    _allChildsGraphic[i].CrossFadeAlpha(active ? 1f : 0f, instant ? 0f : 0.1f, true);
                }
            }
        }

        private void ChangeChildsActive(bool active)
        {
            bool instant = false;

            if (_toggle != null)
                instant = _toggle.toggleTransition == Toggle.ToggleTransition.None;
            else if (_uiToggle != null)
                instant = _uiToggle.toggleTransition == UIToggle.ToggleTransition.None;

            ChangeChildsActive(active, instant);
        }
    }
}
