using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Taskbar : MonoBehaviour
{
    [Header("Taskbar Settings")]
    [SerializeField] private GameObject taskbarItemPrefab;
    [SerializeField] private Transform taskbarItemContainer;
    [SerializeField] private float itemSpacing = 5f;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color focusedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private Dictionary<BaseWindow, TaskbarItem> taskbarItems = new Dictionary<BaseWindow, TaskbarItem>();
    private WindowManager windowManager;

    void Start()
    {
        if (WindowManager.Instance != null)
        {
            windowManager = WindowManager.Instance;
        }
    }

    /// <summary>
    /// Adiciona uma janela à taskbar
    /// </summary>
    public void AddWindow(BaseWindow window)
    {
        if (taskbarItems.ContainsKey(window)) return;

        GameObject itemObj = Instantiate(taskbarItemPrefab, taskbarItemContainer);
        TaskbarItem item = itemObj.GetComponent<TaskbarItem>();

        if (item != null)
        {
            item.Initialize(window, this);
            taskbarItems.Add(window, item);
            UpdateItemVisuals(window);
        }
    }

    /// <summary>
    /// Remove uma janela da taskbar
    /// </summary>
    public void RemoveWindow(BaseWindow window)
    {
        if (!taskbarItems.ContainsKey(window)) return;

        TaskbarItem item = taskbarItems[window];
        taskbarItems.Remove(window);

        if (item != null)
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// Atualiza o visual de um item específico
    /// </summary>
    public void UpdateItemVisuals(BaseWindow window)
    {
        if (!taskbarItems.ContainsKey(window)) return;

        TaskbarItem item = taskbarItems[window];
        bool isFocused = windowManager != null && windowManager.GetFocusedWindow() == window;

        item.SetFocused(isFocused);
    }

    /// <summary>
    /// Atualiza todos os itens da taskbar
    /// </summary>
    public void UpdateAllItems()
    {
        foreach (var window in taskbarItems.Keys)
        {
            UpdateItemVisuals(window);
        }
    }

    /// <summary>
    /// Retorna a cor normal
    /// </summary>
    public Color GetNormalColor()
    {
        return normalColor;
    }

    /// <summary>
    /// Retorna a cor de foco
    /// </summary>
    public Color GetFocusedColor()
    {
        return focusedColor;
    }

    /// <summary>
    /// Retorna a cor de hover
    /// </summary>
    public Color GetHoverColor()
    {
        return hoverColor;
    }
}
