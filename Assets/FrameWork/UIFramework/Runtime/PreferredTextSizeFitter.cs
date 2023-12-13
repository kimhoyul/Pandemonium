using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PreferredTextSizeFitter : UIBehaviour, ILayoutController
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
        targetTMP.enableWordWrapping = false;
        rectTransform.sizeDelta = targetTMP.GetPreferredValues() + padding;
    }

    private Vector2 CalculateTextSize(TMP_Text TMP)
    {
        return TMP.GetPreferredValues();
    }

    public void SetLayoutHorizontal()
    {
        ResizeByText();
    }

    public void SetLayoutVertical()
    {
        ResizeByText();
    }

    private void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }
}
