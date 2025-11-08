using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DesktopAppIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static int WIDTH = 150;
    public static int HEIGHT = 170;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
        DesktopGrid.Instance.FreeSlot(rectTransform.anchoredPosition);
    }

    bool dragging = false;
    Vector2 initDrag = Vector2.zero;

    public void OnDrag(PointerEventData eventData)
    {
        initDrag = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging)
        {
            return;
        }
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        GetSlot();
    }

    public void GetSlot()
    {
        Vector2 freeSlot = DesktopGrid.Instance.GetClosestFreeSlot(rectTransform.anchoredPosition);
        rectTransform.anchoredPosition = freeSlot;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(HasDragged);
        if (HasDragged) return;
        Desktop.Instance.OpenApp();
    }

    public bool HasDragged
    {
        get
        {
            return Vector2.Distance(rectTransform.anchoredPosition, initDrag) <= 1;
        }
    }
}
