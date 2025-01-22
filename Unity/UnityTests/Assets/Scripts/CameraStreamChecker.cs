using UnityEngine;
using UnityEngine.UI;

namespace TankSim.Utils
{
    public class CameraStreamChecker : MonoBehaviour
    {
        private CustomPipelinePlayer pipelinePlayer;
        private RawImage image;
        
        void OnEnable()
        {
            pipelinePlayer = GetComponent<CustomPipelinePlayer>();
            image = GetComponent<RawImage>();
        }

        // Start is called before the first frame update
        void Update()
        {
            if (!pipelinePlayer || !image)
                return;

            if (pipelinePlayer.IsPlaying)
            {
                var deltaTime = Telemetry.Now_Millis - pipelinePlayer.LastFrameMillis;
                image.enabled = deltaTime < 1500;
            } else
            {
                image.enabled = false;
            }
        }
    }
}
