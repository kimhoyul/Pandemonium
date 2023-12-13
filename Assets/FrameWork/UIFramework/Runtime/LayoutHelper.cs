using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TOONIPLAY.Utilities
{
    static public class LayoutHelper
    {
        static public void ForceUpdateLayout(GameObject rootObject, int count) => UIManager.Instance.StartCoroutine(CoSequentialForceUdateLayout(rootObject, count));

        static public IEnumerator CoSequentialForceUdateLayout(GameObject rootObject, int count)
        {
            for (var i = 0; i < count; i++)
                yield return UIManager.Instance.StartCoroutine(LayoutHelper.CoForceUpdateLayout(rootObject));
        }

        // NOTE(jaeil) : ContentSizeFitter가 LayoutGroup 타입 아래 있으면 Update가 두 번 일어나야 제대로 계산이 된다.
        // 그래서 EndOfFrame에 강제로 한 번 더 Update를 돌려주는 루틴 (https://forum.unity.com/threads/the-correct-way-to-use-content-size-fitter-inside-scrollarea.1327944/#post-8393082)
        static public IEnumerator CoForceUpdateLayout(GameObject rootObject)
        {
            yield return new WaitForEndOfFrame();

            RebuildLayout<ContentSizeFitter>(rootObject);
            RebuildLayout<UIContentSizeFitter>(rootObject);
            RebuildLayout<RenderedTextSizeFitter>(rootObject);
            RebuildLayout<PreferredTextSizeFitter>(rootObject);
        }

        static private void RebuildLayout<T>(GameObject rootObject) where T : Behaviour
        {
            var layoutgroup = rootObject.GetComponentsInChildren<T>();
            foreach (var layout in layoutgroup)
            {
                if (layout != null)
                {
                    layout.enabled = false;
                    layout.enabled = true;
                }
            }
        }
    }
}
