using UnityEngine;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance;
    private List<BaseWindow> windows = new();

    [SerializeField]
    private Canvas canvas;

    private BaseWindow focusedWindow;

    [Header("Focus Settings")]
    [SerializeField] private float unfocusedAlpha = 0.8f; // Transparência de janelas não focadas
    [SerializeField] private bool enableFocusEffect = true; // Ativa/desativa efeito visual

    [Header("Taskbar")]
    [SerializeField] private Taskbar taskbar;

    private void Awake()
    {
        Instance = this;
    }

    public void Register(BaseWindow window)
    {
        if (!windows.Contains(window))
            windows.Add(window);

        window.transform.parent = transform;
        window.canvas = canvas;
        window.parentRect = this;

        window.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        
        // Adiciona à taskbar
        if (taskbar != null)
        {
            taskbar.AddWindow(window);
        }
        
        // Define primeira janela como focada
        if (focusedWindow == null)
        {
            SetFocusedWindow(window);
        }
    }

    public void Unregister(BaseWindow window)
    {
        if (windows.Contains(window))
            windows.Remove(window);
            
        // Remove da taskbar
        if (taskbar != null)
        {
            taskbar.RemoveWindow(window);
        }
            
        // Se a janela removida estava focada, foca na última janela
        if (focusedWindow == window)
        {
            focusedWindow = null;
            if (windows.Count > 0)
            {
                SetFocusedWindow(windows[windows.Count - 1]);
            }
        }
    }

    public void BringToFront(BaseWindow window)
    {
        window.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Define qual janela está com foco
    /// </summary>
    public void SetFocusedWindow(BaseWindow window)
    {
        if (focusedWindow == window) return;

        // Remove foco da janela anterior
        if (focusedWindow != null && enableFocusEffect)
        {
            SetWindowAlpha(focusedWindow, unfocusedAlpha);
        }

        // Define nova janela focada
        focusedWindow = window;
        BringToFront(window);

        // Aplica efeito de foco
        if (enableFocusEffect)
        {
            SetWindowAlpha(focusedWindow, 1.0f);
        }

        // Atualiza taskbar
        UpdateTaskbar();
    }

    /// <summary>
    /// Retorna a janela atualmente focada
    /// </summary>
    public BaseWindow GetFocusedWindow()
    {
        return focusedWindow;
    }

    /// <summary>
    /// Altera a transparência de uma janela
    /// </summary>
    private void SetWindowAlpha(BaseWindow window, float alpha)
    {
        if (window == null) return;

        CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = window.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = alpha;
    }

    /// <summary>
    /// Retorna lista de todas as janelas registradas
    /// </summary>
    public List<BaseWindow> GetAllWindows()
    {
        return new List<BaseWindow>(windows);
    }

    /// <summary>
    /// Maximiza a janela focada
    /// </summary>
    public void MaximizeFocusedWindow()
    {
        if (focusedWindow != null)
        {
            focusedWindow.ToggleMaximize();
        }
    }

    /// <summary>
    /// Atualiza todos os itens da taskbar
    /// </summary>
    public void UpdateTaskbar()
    {
        if (taskbar != null)
        {
            taskbar.UpdateAllItems();
        }
    }

    /// <summary>
    /// Retorna a altura da taskbar
    /// </summary>
    public float GetTaskbarHeight()
    {
        if (taskbar == null) return 0f;

        RectTransform taskbarRect = taskbar.GetComponent<RectTransform>();
        if (taskbarRect == null) return 0f;

        return taskbarRect.rect.height;
    }
}
