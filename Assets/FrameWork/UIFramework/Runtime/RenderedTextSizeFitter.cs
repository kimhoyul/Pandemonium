using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RenderedTextSizeFitter : UIBehaviour, ILayoutController
{
    [SerializeField] TMP_Text targetTMP;
    [SerializeField] Vector2 padding;

    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }
#endif

    private void ResizeByText()
    {
        if (targetTMP == null)
            return;

        targetTMP.GetComponent<RectTransform>().sizeDelta = CalculateTextSize(targetTMP);
        targetTMP.enableWordWrapping = true;
        targetTMP.overflowMode = TextOverflowModes.Overflow;
        rectTransform.sizeDelta = CalculateTextSize(targetTMP) + padding;
    }

    private void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    private Vector2 CalculateTextSize(TMP_Text TMP)
    {
        return new Vector2(TMP.rectTransform.sizeDelta.x, TMP.renderedHeight);
    }

    public void SetLayoutHorizontal()
    {
        ResizeByText();
    }

    public void SetLayoutVertical()
    {
        ResizeByText();
    }
}