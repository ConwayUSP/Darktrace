using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BaseWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    public Canvas canvas;
    private Vector2 offset;
    private bool dragging;
    public WindowManager parentRect;

    [SerializeField] private RectTransform header; // cabeçalho janela

    [Header("Program Information")]
    [SerializeField] private string programName = "Application";
    [SerializeField] private Sprite programIcon;

    // Sistema de maximização
    private bool isMaximized = false;
    private Vector2 preMaximizedPosition;
    private Vector2 preMaximizedSize;
    private Vector2 preMaximizedAnchorMin;
    private Vector2 preMaximizedAnchorMax;
    private Vector2 preMaximizedPivot;
    private Vector2 preMaximizedOffsetMin;
    private Vector2 preMaximizedOffsetMax;

    // Sistema de duplo clique
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f; // Tempo máximo entre cliques

    // Sistema de arrasto ao restaurar
    private bool isDraggingMaximized = false;
    private Vector2 dragStartPosition;
    private float normalizedMouseXOnHeader; // Posição normalizada do mouse no header (0 a 1)

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("pointer down");
        
        // Registra foco na janela
        if (parentRect != null)
        {
            parentRect.SetFocusedWindow(this);
        }
        
        if (header != null && RectTransformUtility.RectangleContainsScreenPoint(header, eventData.position, eventData.pressEventCamera))
        {
            // Detecta duplo clique no cabeçalho
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= doubleClickThreshold)
            {
                ToggleMaximize();
                lastClickTime = 0f; // Reset para evitar triplo clique
                return;
            }
            lastClickTime = Time.time;

            // Prepara para arrastar (mesmo se maximizada)
            if (isMaximized)
            {
                isDraggingMaximized = true;
                dragStartPosition = eventData.position;
                
                // Calcula posição normalizada do mouse no header (0 = esquerda, 1 = direita)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    header, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
                float headerWidth = header.rect.width;
                normalizedMouseXOnHeader = (localPoint.x + headerWidth / 2f) / headerWidth;
                normalizedMouseXOnHeader = Mathf.Clamp01(normalizedMouseXOnHeader);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out offset);
                dragging = true;
            }
            
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect == null) return;

        // Se está maximizada e começou a arrastar
        if (isDraggingMaximized)
        {
            // Verifica se moveu o suficiente para desmaximizar
            float dragDistance = Vector2.Distance(dragStartPosition, eventData.position);
            if (dragDistance > 5f) // Threshold para evitar desmaximizar com cliques pequenos
            {
                RestoreWindowWhileDragging(eventData);
                isDraggingMaximized = false;
                dragging = true;
            }
            return;
        }

        // Arrasto normal
        if (!dragging) return;

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
        isDraggingMaximized = false;
    }

    /// <summary>
    /// Restaura a janela durante o arrasto, mantendo o cursor na mesma posição relativa do cabeçalho
    /// </summary>
    private void RestoreWindowWhileDragging(PointerEventData eventData)
    {
        if (!isMaximized) return;

        // Restaura a janela primeiro
        RestoreWindow();

        // Agora calcula onde a janela deve estar para o mouse ficar na mesma posição relativa do header
        // Usa a posição normalizada salva anteriormente
        float restoredHeaderWidth = header.rect.width;
        float offsetXInHeader = (normalizedMouseXOnHeader * restoredHeaderWidth) - (restoredHeaderWidth / 2f);

        // Converte posição do mouse para espaço do canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform, eventData.position, eventData.pressEventCamera, out Vector2 canvasMousePos);

        // Calcula posição da janela para o mouse ficar no ponto correto do header
        // A janela deve estar posicionada de forma que: mousePos = windowPos + offsetXInHeader
        Vector2 targetWindowPos = new Vector2(canvasMousePos.x - offsetXInHeader, canvasMousePos.y);

        // Aplica clamp para não sair da área
        ClampToParent(ref targetWindowPos);

        // Define a nova posição
        rectTransform.anchoredPosition = targetWindowPos;

        // Calcula o offset correto para continuar arrastando
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out offset);
    }

    /// <summary>
    /// Maximiza ou restaura a janela
    /// </summary>
    public void ToggleMaximize()
    {
        if (isMaximized)
        {
            RestoreWindow();
        }
        else
        {
            MaximizeWindow();
        }
    }

    /// <summary>
    /// Maximiza a janela para preencher o container pai (descontando a taskbar)
    /// </summary>
    public void MaximizeWindow()
    {
        if (isMaximized || parentRect == null) return;

        // Salva estado atual
        preMaximizedPosition = rectTransform.anchoredPosition;
        preMaximizedSize = rectTransform.sizeDelta;
        preMaximizedAnchorMin = rectTransform.anchorMin;
        preMaximizedAnchorMax = rectTransform.anchorMax;
        preMaximizedPivot = rectTransform.pivot;
        preMaximizedOffsetMin = rectTransform.offsetMin;
        preMaximizedOffsetMax = rectTransform.offsetMax;

        // Obtém altura da taskbar
        float taskbarHeight = parentRect.GetTaskbarHeight();

        // Maximiza usando âncoras stretch
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;

        // Pivot igual ao do inspector
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Z = 0
        rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);

        // Left = 0, Bottom = 100
        rectTransform.offsetMin = new Vector2(0f, taskbarHeight);

        // Right = 0, Top = 0
        rectTransform.offsetMax = new Vector2(0f, 0f);

        isMaximized = true;
    }

    /// <summary>
    /// Restaura a janela ao tamanho e posição anteriores
    /// </summary>
    public void RestoreWindow()
    {
        if (!isMaximized) return;

        // Restaura estado anterior
        rectTransform.anchorMin = preMaximizedAnchorMin;
        rectTransform.anchorMax = preMaximizedAnchorMax;
        rectTransform.pivot = preMaximizedPivot;
        rectTransform.sizeDelta = preMaximizedSize;
        rectTransform.anchoredPosition = preMaximizedPosition;
        rectTransform.offsetMin = preMaximizedOffsetMin;
        rectTransform.offsetMax = preMaximizedOffsetMax;

        isMaximized = false;
    }

    /// <summary>
    /// Retorna se a janela está maximizada
    /// </summary>
    public bool IsMaximized()
    {
        return isMaximized;
    }

    /// <summary>
    /// Fecha a janela e remove do gerenciador
    /// </summary>
    public void CloseWindow()
    {
        // Notifica o gerenciador antes de destruir
        if (parentRect != null)
        {
            parentRect.Unregister(this);
        }

        // Destroi o GameObject
        Destroy(gameObject);
    }

    /// <summary>
    /// Define o nome do programa
    /// </summary>
    public void SetProgramName(string name)
    {
        programName = name;
        if (parentRect != null)
        {
            parentRect.UpdateTaskbar();
        }
    }

    /// <summary>
    /// Define o ícone do programa
    /// </summary>
    public void SetProgramIcon(Sprite icon)
    {
        programIcon = icon;
        if (parentRect != null)
        {
            parentRect.UpdateTaskbar();
        }
    }

    /// <summary>
    /// Retorna o nome do programa
    /// </summary>
    public string GetProgramName()
    {
        return programName;
    }

    /// <summary>
    /// Retorna o ícone do programa
    /// </summary>
    public Sprite GetProgramIcon()
    {
        return programIcon;
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
