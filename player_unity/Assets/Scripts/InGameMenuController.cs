using UnityEngine;
using UnityEngine.InputSystem;

public class InGameMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject pauseBackGround;
    public GameObject img_under_construction;

    private GameObject playerObject; 
    private PlayerInput playerController;

    private bool isPaused = false;
    private bool sign_shown = false;

    void Start()
    {
	playerObject = GameObject.FindGameObjectWithTag("Player");
	if (playerObject != null)
 	    playerController = playerObject.GetComponent<PlayerInput>();

	InGameMenuUI ui = pauseMenuUI.GetComponent<InGameMenuUI>();

	ui.btn_resume.onClick.AddListener(Resume);
	ui.btn_inventory.onClick.AddListener(UnderConstruction);
	ui.btn_stats.onClick.AddListener(UnderConstruction);
	ui.btn_settings.onClick.AddListener(UnderConstruction);
	ui.btn_save_exit.onClick.AddListener(UnderConstruction);
        pauseMenuUI.SetActive(false);
	pauseBackGround.SetActive(false);
	img_under_construction.SetActive(false);
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
	img_under_construction.SetActive(false);
	sign_shown = false;
    }
    public void UnderConstruction()
    {
        if (sign_shown)
	{
	    img_under_construction.SetActive(false);
	}
	else
	{
	    img_under_construction.SetActive(true);
	}
	sign_shown = !sign_shown;
    }




}
