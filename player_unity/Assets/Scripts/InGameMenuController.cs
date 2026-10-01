using UnityEngine;
using UnityEngine.InputSystem;

public class InGameMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject pauseBackGround;

    private GameObject playerObject; 
    private PlayerInput playerController;

    private bool isPaused = false;

    void Start()
    {
	playerObject = GameObject.FindGameObjectWithTag("Player");
	if (playerObject != null)
 	    playerController = playerObject.GetComponent<PlayerInput>();

	InGameMenuUI ui = pauseMenuUI.GetComponent<InGameMenuUI>();

	ui.btn_resume.onClick.AddListener(Resume);
        pauseMenuUI.SetActive(false);
	pauseBackGround.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
	    Debug.Log("Escape detected");
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
	pauseBackGround.SetActive(true);
	if (playerController != null)
            playerController.enabled = false;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
	pauseBackGround.SetActive(false);
	if (playerController != null)
	    playerController.enabled = true;
        Time.timeScale = 1f;
        isPaused = false;
    }
}
