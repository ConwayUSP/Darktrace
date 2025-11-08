using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Desktop : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
{
    public static Desktop Instance { private set; get; }

    [SerializeField] private GameObject BaseWindow;
    [SerializeField] private RectTransform selectVisual;
    [SerializeField] private ContextMenu contextMenu;

    private Vector2? startSelect = null;
    private Canvas canvas;
    private RectTransform rectTransform;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        selectVisual.gameObject.SetActive(false);
    }

    public void OpenApp()
    {
        GameObject go = Instantiate(BaseWindow, transform);
        WindowManager.Instance.Register(go.GetComponent<BaseWindow>());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Começa a seleção
            startSelect = eventData.position;
            selectVisual.gameObject.SetActive(true);
            UpdateSelectionVisual(eventData);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (startSelect != null)
            UpdateSelectionVisual(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (startSelect == null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 currentLocal);
            contextMenu.ShowContextMenu(currentLocal);
            return;
        }

        // Finaliza a seleção
        selectVisual.gameObject.SetActive(false);
        startSelect = null;
    }

    

    private void UpdateSelectionVisual(PointerEventData eventData)
    {
        if (canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, startSelect.Value, eventData.pressEventCamera, out Vector2 startLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 currentLocal);

        Vector2 size = currentLocal - startLocal;

        selectVisual.anchoredPosition = startLocal + size * 0.5f;
        selectVisual.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
    }
}
