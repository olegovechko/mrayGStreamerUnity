using System.Runtime.InteropServices;
using TankSim;
using TankSim.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class mainUI : MonoBehaviour
{
	//[DllImport("native")]
	//private static extern float add(float x, float y);

	public const string DllName = "GStreamerUnityPlugin";
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private int mray_gstreamer_test();

    [SerializeField]
    private VideoPlayerUI videoPlayer;

    [SerializeField]
    private Image imageProgress;

    [SerializeField]
    private TMP_Text textProgress;

    public void ToggleVideo()
    {
        videoPlayer.gameObject.SetActive(!videoPlayer.gameObject.activeSelf);
    }

    public void StreamVideo(int camID)
    {
        if (videoPlayer.cameraId != camID)
        {
            videoPlayer.cameraId = camID;
            if (!videoPlayer.gameObject.activeSelf)
                videoPlayer.gameObject.SetActive(true);
            else
                videoPlayer.play();
        }
    }

    void Start()
    {
        videoPlayer.cameraId = -1;
        //Debug.Log($"mray_gstreamer_test() ---> {mray_gstreamer_test()}");
        //Debug.Log($"NativeLIB: {add(222, 111)}");
    }

    bool screenPressed = false;
    long screenPressStart = 0;

    float progress = -1; 
    float progressDeltaTime = 1500;
    int screenSelected = -1;


    private int decodeViewPos(Vector3 pos)
    {
        if (pos.x < 0.5)
        {
            return (pos.y < 0.5) ? 2 : 0;
        } else
        {
            return (pos.y < 0.5) ? 3 : 1;
        }
    }

    private string getLabelText(int id)
    {
        switch (id)
        {
            case 0:
                return "L";
            case 1:
                return "R";
            case 2:
                return "B";
            case 3:
                return "D";
        }
        return "-";
    }

    void Update ()
    {
        bool bPressed = Input.GetMouseButton(0);
        
        if (!screenPressed)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Input.mousePosition;
                Vector3 posView = Camera.main.ScreenToViewportPoint(pos);
                int newCamera = decodeViewPos(posView);
                
                if (newCamera != screenSelected)
                {
                    screenPressStart = Telemetry.Now_Millis;
                    screenPressed = true;
                    Debug.Log($"view pos: {newCamera}");
                    textProgress.text = getLabelText(newCamera);
                }
            }
        } else
        {
            if (Input.GetMouseButton(0))
            {
                progress = (Telemetry.Now_Millis - screenPressStart) / progressDeltaTime;
                if (progress >= 1)
                {
                    Vector3 pos = Input.mousePosition;
                    Vector3 posView = Camera.main.ScreenToViewportPoint(pos);
                    int newCamera = decodeViewPos(posView);
                    
                    Debug.Log($"Select camera: {newCamera}");

                    screenSelected = newCamera;
                    progress = 0;
                    screenPressed = false;
                    StreamVideo(newCamera);
                }
            } else
            {
                screenPressed = false;
            }
        }
        imageProgress.fillAmount = screenPressed ? progress : 0;
        textProgress.enabled = screenPressed ? progress > 0 : false;

    }
}
