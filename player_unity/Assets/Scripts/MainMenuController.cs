using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject panel_main_menu;
    public GameObject panel_load;
    public GameObject panel_options;
    public GameObject panel_credits;

    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
	    UnityEditor.EditorApplication.isPlaying = false;
	#else
            Application.Quit();
	#endif
    }
    public void ShowMainMenu()
    {
        panel_main_menu.SetActive(true);
	panel_load.SetActive(false);
        panel_options.SetActive(false);
	panel_credits.SetActive(false);
    }
    public void ShowLoadScreen()
    {
        panel_main_menu.SetActive(false);
	panel_load.SetActive(true);
        panel_options.SetActive(false);
	panel_credits.SetActive(false);
    }
    public void ShowOptions()
    {
        panel_main_menu.SetActive(false);
	panel_load.SetActive(false);
        panel_options.SetActive(true);
	panel_credits.SetActive(false);
    }
    public void ShowCredits()
    {
        panel_main_menu.SetActive(false);
	panel_load.SetActive(false);
        panel_options.SetActive(false);
	panel_credits.SetActive(true);
    }

    
}
