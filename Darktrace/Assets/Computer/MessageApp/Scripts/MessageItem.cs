using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class MessageItem : MonoBehaviour
{
    [SerializeField] private RectTransform bubble;
    [SerializeField] private RectTransform statusText;
    [SerializeField] private TextMeshProUGUI messageText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetText(messageText.text);
    }


    public void SetText(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
        StartCoroutine(UpdateHeightNextFrame());
    }

    private IEnumerator UpdateHeightNextFrame()
    {
        yield return null; // Aguarda um frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubble.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(bubble);
        UpdateHeight();
    }

    private void UpdateHeight()
    {
        if (bubble == null) return;

        float totalHeight = bubble.sizeDelta.y;

        if (statusText != null && statusText.gameObject.activeSelf)
        {
            totalHeight += 10f + statusText.sizeDelta.y;
        }

        totalHeight += 20f;

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, totalHeight);
    }
}
