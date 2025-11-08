using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour
{
    private RectTransform m_RectTransform;
    [SerializeField] private Canvas canvas;

    public void ShowContextMenu(Vector2 position)
    {
        if (m_RectTransform == null)
            m_RectTransform = GetComponent<RectTransform>();

        m_RectTransform.anchoredPosition = position;
        gameObject.SetActive(true);
    }

    public void HideContextMenu()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();

            Vector2 mousePos = Mouse.current.position.ReadValue();

            bool inside = RectTransformUtility.RectangleContainsScreenPoint(
                m_RectTransform,
                mousePos,
                canvas ? canvas.worldCamera : null
            );

            if (!inside)
                HideContextMenu();
        }
    }
}
