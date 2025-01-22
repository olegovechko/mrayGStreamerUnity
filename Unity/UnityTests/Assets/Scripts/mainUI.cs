using System.Runtime.InteropServices;
using TankSim.UI;
using UnityEngine;

public class mainUI : MonoBehaviour
{
	//[DllImport("native")]
	//private static extern float add(float x, float y);

	public const string DllName = "GStreamerUnityPlugin";
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private int mray_gstreamer_test();

    [SerializeField]
    private VideoPlayerUI videoPlayer;

    public void ToggleVideo()
    {
        videoPlayer.gameObject.SetActive(!videoPlayer.gameObject.activeSelf);
    }

    void Start()
    {
        //Debug.Log($"mray_gstreamer_test() ---> {mray_gstreamer_test()}");
        //Debug.Log($"NativeLIB: {add(222, 111)}");
    }
}
