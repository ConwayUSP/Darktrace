using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageApp : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject senderMessagePrefab;
    [SerializeField] private GameObject receiverMessagePrefab;
    [SerializeField] private GameObject typingIndicatorPrefab;

    [Header("Container")]
    [SerializeField] private Transform messagesContainer;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Auto Scroll Settings")]
    [SerializeField] private float scrollThreshold = 0.01f; // Quão perto do fundo precisa estar (0 = fundo exato, 1 = topo)

    private GameObject currentTypingIndicator;
    private bool shouldAutoScroll = true;

    void Start()
    {
        // Detecta quando usuário rola manualmente
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
    }

    /// <summary>
    /// Detecta se usuário está rolando manualmente
    /// </summary>
    private void OnScrollChanged(Vector2 scrollPosition)
    {
        // Verifica se está perto do fundo
        shouldAutoScroll = scrollPosition.y <= scrollThreshold;
    }

    public void AddSenderMessage(string text)
    {
        AddMessage(text, true);
    }

    public void AddReceiverMessage(string text)
    {
        AddMessage(text, false);
    }

    private void AddMessage(string text, bool isSender)
    {
        if (messagesContainer == null)
        {
            return;
        }

        GameObject prefab = isSender ? senderMessagePrefab : receiverMessagePrefab;

        if (prefab == null)
        {
            return;
        }
        GameObject messageObj = Instantiate(prefab, messagesContainer);

        // Configura o texto
        MessageItem messageItem = messageObj.GetComponent<MessageItem>();
        if (messageItem != null)
        {
            messageItem.SetText(text);
        }

        Debug.Log(shouldAutoScroll);

        // Scroll para baixo se necessário
        if (shouldAutoScroll)
        {
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    /// <summary>
    /// Mostra o indicador de digitação
    /// </summary>
    public void StartTyping()
    {
        // Se já existe, não cria outro
        if (currentTypingIndicator != null)
        {
            return;
        }

        if (typingIndicatorPrefab == null || messagesContainer == null)
        {
            return;
        }

        // Instancia o prefab de digitação
        currentTypingIndicator = Instantiate(typingIndicatorPrefab, messagesContainer);



        // Scroll para baixo se necessário
        if (shouldAutoScroll)
        {
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    /// <summary>
    /// Remove o indicador de digitação
    /// </summary>
    public void StopTyping()
    {
        if (currentTypingIndicator != null)
        {
            Destroy(currentTypingIndicator);
            currentTypingIndicator = null;
        }
    }

    /// <summary>
    /// Verifica se está digitando
    /// </summary>
    public bool IsTyping()
    {
        return currentTypingIndicator != null;
    }

    public void ClearAllMessages()
    {
        if (messagesContainer == null) return;

        foreach (Transform child in messagesContainer)
        {
            Destroy(child.gameObject);
        }

        currentTypingIndicator = null;
    }

    /// <summary>
    /// Rola para o fundo no próximo frame
    /// </summary>
    private IEnumerator ScrollToBottomNextFrame()
    {
        // Aguarda 2 frames para garantir que o layout foi atualizado
        yield return null;
        yield return null;

        Debug.Log("Scroll To Bottom");

        ScrollToBottom();
        LayoutRebuilder.ForceRebuildLayoutImmediate(messagesContainer.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Força scroll para o fundo
    /// </summary>
    public void ScrollToBottom()
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// Força scroll para o topo
    /// </summary>
    public void ScrollToTop()
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    /// <summary>
    /// Ativa/desativa auto-scroll
    /// </summary>
    public void SetAutoScroll(bool enabled)
    {
        shouldAutoScroll = enabled;
    }

    void OnDestroy()
    {
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }
    }
}
