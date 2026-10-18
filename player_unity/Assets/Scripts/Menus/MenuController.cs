using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            toggle_menu();
        }
    }

    public void toggle_menu()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
    }
}
