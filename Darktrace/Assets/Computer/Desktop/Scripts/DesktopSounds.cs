using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopSounds : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip clickSound, declickSound, pcStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource.PlayOneShot(pcStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null) return; // segurança

        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            _audioSource.PlayOneShot(clickSound);

        if (Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.rightButton.wasReleasedThisFrame)
            _audioSource.PlayOneShot(declickSound);
    }
}
