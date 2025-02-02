using UnityEngine;
using System.Collections;
using TMPro;

namespace TankSim.UI
{
    public class VideoPlayerUI : MonoBehaviour
    {
        #region Editor
        
        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        public int cameraId = 0;

        [SerializeField]
        private Transform parentAttach;

        [SerializeField]
        private GameObject[] pathTrail;

        [SerializeField]
        private TMP_Text fpsLabel;

        #endregion

        private CustomPipelinePlayer player = null;
        private GstCustomTexture playerTexture = null;

        public int FPS => player != null ? player.FPS : 0;
        public TMP_Text FPSLabel => fpsLabel;

        private string pipeline => 
            (cameraId == 0 ? Config.Instance.cameras.camera1 + Config.Instance.cameras.optionsCamera1 :
            cameraId == 1 ? Config.Instance.cameras.camera2 + Config.Instance.cameras.optionsCamera2:
            cameraId == 2 ? Config.Instance.cameras.camera3 + Config.Instance.cameras.optionsCamera3:
                Config.Instance.cameras.camera4) + Config.Instance.cameras.optionsSufix;

        private void OnEnable()
        {
            play();
        }

        private void OnDisable()
        {
            if (player != null)
            {
                GameObject.Destroy(player.gameObject);
                player = null;
            }
        }

        IEnumerator startPlay()
        {
            if (player != null)
            {
                GameObject.Destroy(player.gameObject);
                player = null;
                yield return new WaitForSecondsRealtime(0.05f);
            }

            GameObject go = GameObject.Instantiate<GameObject>(playerPrefab, parentAttach);
            if (go)
            {
                player = go.GetComponent<CustomPipelinePlayer>();
                playerTexture = go.GetComponent<GstCustomTexture>();

                if (player == null || playerTexture == null)
                {
                    GameObject.Destroy(go);
                }
                else
                {
                    player.pipeline = pipeline;
                    go.SetActive(true);
                }
            }
            yield return null;
        }

        public void play()
        {
            StartCoroutine(startPlay());
        }

        public void ShowPathTrail(int pathTrailIDX)
        {
            if (pathTrail == null && pathTrail.Length > 0)
                return;

            int idx = (pathTrailIDX % (pathTrail.Length + 1)) - 1;

            for (int i=0; i<pathTrail.Length; i++)
            {
                pathTrail[i].SetActive(i==idx);    
            }
        }

        void Update()
        {
            if (!player)
                return;

            bool needRestart = false;

            var deltaTime = Telemetry.Now_Millis - player.LastFrameMillis;
            if (deltaTime > 30000) // no frames for 30 sec. need restart
            {
                needRestart = true;
            } else if (player.IsPlaying && deltaTime > 3000) // no frames for 3 sec. need restart after some frames received
            {
                //needRestart = true;
            }

            if (needRestart)
            {
                Debug.Log("Restarting video stream... " + deltaTime);
                // Restarting stream...
                play();
            }

            if (FPSLabel != null)
            {
                if (Config.Instance.showFPS)
                {
                    FPSLabel.text = $"{FPS}";
                } else
                {
                    FPSLabel.text = "";
                }
            }
        }
    }
}