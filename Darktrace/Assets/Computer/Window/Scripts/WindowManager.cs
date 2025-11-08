using UnityEngine;
using System.Collections.Generic;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance;
    private List<BaseWindow> windows = new();

    [SerializeField]
    private Canvas canvas;

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
    }

    public void Unregister(BaseWindow window)
    {
        if (windows.Contains(window))
            windows.Remove(window);
    }

    public void BringToFront(BaseWindow window)
    {
        window.transform.SetAsLastSibling();
    }
}
