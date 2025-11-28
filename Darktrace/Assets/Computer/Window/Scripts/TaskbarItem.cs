using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TaskbarItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Image backgroundImage;

    private BaseWindow window;
    private Taskbar taskbar;
    private bool isFocused = false;
    private bool isHovered = false;

    /// <summary>
    /// Inicializa o item da taskbar
    /// </summary>
    public void Initialize(BaseWindow window, Taskbar taskbar)
    {
        this.window = window;
        this.taskbar = taskbar;

        UpdateVisuals();
    }

    /// <summary>
    /// Atualiza os visuais do item
    /// </summary>
    public void UpdateVisuals()
    {
        if (window == null) return;

        // Atualiza ícone
        if (iconImage != null && window.GetProgramIcon() != null)
        {
            iconImage.sprite = window.GetProgramIcon();
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        // Atualiza cor de fundo
        UpdateBackgroundColor();
    }

    /// <summary>
    /// Define o estado de foco
    /// </summary>
    public void SetFocused(bool focused)
    {
        isFocused = focused;
        UpdateBackgroundColor();
    }

    /// <summary>
    /// Atualiza a cor de fundo baseado no estado
    /// </summary>
    private void UpdateBackgroundColor()
    {
        if (backgroundImage == null || taskbar == null) return;

        if (isFocused)
        {
            backgroundImage.color = taskbar.GetFocusedColor();
        }
        else if (isHovered)
        {
            backgroundImage.color = taskbar.GetHoverColor();
        }
        else
        {
            backgroundImage.color = taskbar.GetNormalColor();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (window == null) return;

        WindowManager manager = window.parentRect;
        if (manager == null) return;

        // Se a janela já está focada, minimiza/restaura
        if (manager.GetFocusedWindow() == window && window.gameObject.activeSelf)
        {
            // TODO: Implementar minimizar quando tiver a funcionalidade
            // Por enquanto, apenas mantém focada
            manager.SetFocusedWindow(window);
        }
        else
        {
            // Ativa a janela se estiver desativada
            if (!window.gameObject.activeSelf)
            {
                window.gameObject.SetActive(true);
            }
            
            // Define foco na janela
            manager.SetFocusedWindow(window);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateBackgroundColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateBackgroundColor();
    }

    /// <summary>
    /// Retorna a janela associada
    /// </summary>
    public BaseWindow GetWindow()
    {
        return window;
    }
}
