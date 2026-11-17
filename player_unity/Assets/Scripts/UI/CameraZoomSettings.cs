using UnityEngine;
using UnityEngine.UI;

public class CameraZoomSettings : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Slider zoomSlider;

    private const string ZoomPref = "CameraZoom";

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        float savedZoom = PlayerPrefs.GetFloat(
            ZoomPref,
            targetCamera.orthographicSize
        );

        zoomSlider.value = savedZoom;
        SetZoom(savedZoom);
    }

    public void SetZoom(float zoom)
    {
        if (targetCamera == null)
            return;

        targetCamera.orthographicSize = zoom;

        PlayerPrefs.SetFloat(ZoomPref, zoom);
        PlayerPrefs.Save();
    }
}