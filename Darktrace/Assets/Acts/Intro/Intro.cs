using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MessageApp messageApp;

    [Header("Timing Settings")]
    [SerializeField] private float delayBetweenMessages = 1.5f;
    [SerializeField] private float typingDuration = 2f;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Aguarda um momento antes de começar
        yield return new WaitForSeconds(1f);

        // Início da conversa
        yield return SendReceiverMessage("Ei Carlos");
        yield return SendReceiverMessage("Bem-vindo de volta. Estamos felizes em te ter novamente.");
        yield return SendReceiverMessage("Como foi com a família? Aproveitou bem o descanso?");

        // Resposta do jogador
        yield return SendSenderMessage("Foi tranquilo. Sempre bom descansar a cabeça…");

        // Continuação
        yield return SendReceiverMessage("haha. voce tava precisando cara!");
        yield return SendSenderMessage("Já não aguentava mais kkk");
        yield return SendReceiverMessage("o pessoal sentiu sua falta po");

        yield return SendReceiverMessage("Olha, sei que pode ser meio anti climático mas…");
        yield return SendReceiverMessage("Assim que você chegar precisamos conversar. Tenho algo grande pra tu, cara.");

        // Resposta do jogador
        yield return SendSenderMessage("Me conta mais");

        // Explicação sobre HexCore
        yield return SendReceiverMessage("Você conhece o grupo HexCore né?");
        yield return SendReceiverMessage("Aquele grupo da investigação de crimes cibernéticos");
        yield return SendReceiverMessage("Cara, eles não param.");
        yield return SendReceiverMessage("Mas agora pegamos eles de jeito!");

        yield return SendReceiverMessage("Acreditamos ter pegado o chefao, se chama Suárez.");
        yield return SendReceiverMessage("Ele nao resistiu a prisão mas não abriu a boca até agora.");
        yield return SendReceiverMessage("Até tentamos oferecer reduzir a pena dele, caso ele entregasse mais alguém do grupo. Mas o cara é cabeça dura.");

        yield return SendReceiverMessage("Bom, a verdade é que preciso da sua ajuda.");
        yield return SendReceiverMessage("A única coisa que conseguimos encontrar na casa do cara é o computador.");
        yield return SendReceiverMessage("Computador novo. Desbloqueado, sem senha. Invadir não foi o problema.");

        yield return SendReceiverMessage("O ponto é que tá tudo muito suspeito…");
        yield return SendReceiverMessage("O computador parece limpo, mas alguns logs apontam para a residência do suspeito.");
        yield return SendReceiverMessage("Ele nem abriu a boca. Certeza que tem algo estranho.");

        yield return SendReceiverMessage("Posso contar com você?");

        // Resposta final do jogador
        yield return SendSenderMessage("Só garante de pagar o meu almoço.");

        yield return SendReceiverMessage("kkkkkk, cê nunca muda");
        yield return SendReceiverMessage("engracadao");

        yield return SendSenderMessage("Já to chegando…");

        // Fim da intro
        Debug.Log("Intro finalizada!");
    }

    /// <summary>
    /// Envia uma mensagem do receiver com efeito de digitação
    /// </summary>
    private IEnumerator SendReceiverMessage(string text)
    {
        // Mostra "digitando..."
        messageApp.StartTyping();
        
        // Aguarda duração da digitação
        yield return new WaitForSeconds(typingDuration);
        
        // Remove "digitando..."
        messageApp.StopTyping();
        
        // Adiciona mensagem
        messageApp.AddReceiverMessage(text);
        
        // Aguarda antes da próxima mensagem
        yield return new WaitForSeconds(delayBetweenMessages);
    }

    /// <summary>
    /// Envia uma mensagem do sender (jogador)
    /// </summary>
    private IEnumerator SendSenderMessage(string text)
    {
        messageApp.AddSenderMessage(text);
        
        // Aguarda antes da próxima mensagem
        yield return new WaitForSeconds(delayBetweenMessages);
        
        yield return null;
    }

    /// <summary>
    /// Para a intro (se necessário)
    /// </summary>
    public void StopIntro()
    {
        StopAllCoroutines();
        if (messageApp != null)
        {
            messageApp.StopTyping();
        }
    }
}
