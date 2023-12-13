using TOONIPLAY;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITouchStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private float range = 120.0f;

    private Vector2 originPosition;
    private Vector2 startPosition;
    private Vector2 delta;

    private RectTransform rectTransform;

    public Vector2 Delta => delta;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        originPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var dragPosition = eventData.position - startPosition;
        delta = eventData.delta;

        var length = dragPosition.magnitude;
        if (length > 1E-05f)
            dragPosition /= length;
        else
            dragPosition = Vector2.zero;

        rectTransform.anchoredPosition = dragPosition * Mathf.Clamp(length, 0.0f, range);

        SendValue(dragPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = eventData.position;
        delta = eventData.delta;

        SendValue(Vector2.zero);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        startPosition = Vector2.zero;
        delta = eventData.delta;

        rectTransform.anchoredPosition = originPosition;

        SendValue(Vector2.zero);
    }

    private void SendValue(Vector2 value)
    {
        // TODO(jaeil) : 추후 원하는 기능 입력 받게 만든다.
        // var player = CharacterManager.Instance.GetMainCharacterController();
        // if (player != null)
        //     player.SetMovementInput(value);
    }
}
