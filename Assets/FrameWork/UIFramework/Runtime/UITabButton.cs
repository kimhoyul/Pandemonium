using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TOONIPLAY
{
    public class UITabButton : UIToggle
    {
        [SerializeField] private UIComponent tabContentPanel;

        private List<Graphic> allChildsGraphic = new();

        public UIComponent ContentPanel => tabContentPanel;

        protected override void Awake()
        {
            onValueChanged.AddListener((bool value) =>
            {
                if (group == null)
                    return;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return;
#endif

                var tabControlComponent = group.gameObject.GetComponent<UIComponent>();
                if (tabControlComponent != null)
                {
                    if (tabContentPanel == null)
                        return;

                    if (value && tabContentPanel.gameObject.activeSelf)
                        return;

                    // NOTE(jaeil) : 모든 버튼에 대해 동작해서 서로 on/off 할려고 하기 때문에 showOnly를 false로 세팅합니다.
                    tabControlComponent.ShowUIComponent(tabContentPanel, value, false);
                }

                OnValueChangedTabButton(value);
            });
        }

        protected override void Start()
        {
            CircuitChilds(gameObject);
            onValueChanged.AddListener(ChangeGraphicColors);
            onValueChanged.Invoke(isOn);
            StartCoroutine(DelayOneFrame());
        }

        protected virtual void OnValueChangedTabButton(bool value) { }

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
                allChildsGraphic.Add(childs[i].GetComponent<Graphic>());
            }
        }

        private void ChangeGraphicColors(bool isOn)
        {
            if (allChildsGraphic.Count == 0)
                return;

            if (isOn)
            {
                for (int i = 0; i < allChildsGraphic.Count; i++)
                {
                    allChildsGraphic[i].CrossFadeColor(colors.selectedColor, colors.fadeDuration, true, true);
                }
            }
            else
            {
                for (int i = 0; i < allChildsGraphic.Count; i++)
                {
                    allChildsGraphic[i].CrossFadeColor(colors.normalColor, colors.fadeDuration, true, true);
                }

                if (graphic == null)
                    return;

                graphic.CrossFadeAlpha(isOn ? 1f : 0f, colors.fadeDuration, true);
            }
        }

        private IEnumerator DelayOneFrame()
        {
            yield return null;
            ChangeGraphicColors(isOn);
        }
    }
}
