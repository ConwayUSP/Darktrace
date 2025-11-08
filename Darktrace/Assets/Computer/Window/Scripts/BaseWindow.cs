using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class BaseWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    public Canvas canvas;
    private Vector2 offset;
    private bool dragging;
    public WindowManager parentRect;

    [SerializeField] private RectTransform header; // cabeçalho janela

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("pointer down");
        if (header != null && RectTransformUtility.RectangleContainsScreenPoint(header, eventData.position, eventData.pressEventCamera))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out offset);
            dragging = true;
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || parentRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, eventData.position, eventData.pressEventCamera, out Vector2 globalMousePos))
        {
            Vector2 targetPos = globalMousePos - offset;

            ClampToParent(ref targetPos);

            rectTransform.anchoredPosition = targetPos;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    private void ClampToParent(ref Vector2 targetPos)
    {
        // Pega tamanho da janela e do container
        Vector2 windowSize = rectTransform.rect.size;
        Vector2 parentSize = parentRect.GetComponent<RectTransform>().rect.size;

        // Calcula limites em coordenadas locais
        float minX = -parentSize.x / 2 + windowSize.x / 2;
        float maxX = parentSize.x / 2 - windowSize.x / 2;
        float minY = -parentSize.y / 2 + windowSize.y / 2;
        float maxY = parentSize.y / 2 - windowSize.y / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
    }
}
