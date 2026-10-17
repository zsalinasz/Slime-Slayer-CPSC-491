using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public AudioSource audioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


        audioSource = gameObject.GetComponent<AudioSource>();
        Debug.Log("Music Manager: Awake() finished executing.");
    }

    void Start()
    {
        //Set the volume to 50% -- other components should be able to override this.
        audioSource.volume = 0.5f;
    }

    public void ChangeVolume(float newVolume)
    {
        //Set volume to provided volume and clamp it between 0.0 and 1.0 for domain safety
        audioSource.volume = Mathf.Clamp01(newVolume);
    }

    public float getVolume()
    {
        //Returns the current volume of the audio source.
        return audioSource.volume;
    }

    

}
