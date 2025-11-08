using UnityEngine;

public class Desktop : MonoBehaviour
{
    public static Desktop Instance { private set; get; }
    [SerializeField]
    private GameObject BaseWindow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenApp()
    {
        GameObject go = Instantiate(BaseWindow);
        WindowManager.Instance.Register(go.GetComponent<BaseWindow>());
    }
}
