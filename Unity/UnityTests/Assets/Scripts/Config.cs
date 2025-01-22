using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System;

namespace TankSim
{
    public class CamerasConfigData 
    {
        public string camera1 = "rtspsrc location=rtsp://192.168.15.184/live/0";
        public string camera2 = "rtspsrc location=rtsp://192.168.15.109/live/1";
        public string camera3 = "rtspsrc location=rtsp://192.168.15.109/live/1";
        public string camera4 = "rtspsrc location=rtsp://192.168.15.109/live/1";

        public string optionsCamera1 = " latency=0 ! decodebin ! videoconvert ! video/x-raw,format=I420 ! ";
        public string optionsCamera2 = " latency=0 ! decodebin ! videoconvert ! video/x-raw,format=I420 ! ";
        public string optionsCamera3 = " latency=0 ! decodebin ! videoconvert ! video/x-raw,format=I420 ! ";
        public string optionsCamera4 = " latency=0 ! decodebin ! videoconvert ! video/x-raw,format=I420 ! ";

        public string optionsSufix = "appsink name=videoSink sync=false";
    }

    public class MapConfigData
    {
        public string pathWindows = "D:/Projects/MinNET/Build/MapUI/MapUI.exe";
        public string pathLinux = "MapUI.x86_64";
        public string options = "-monitor 2";

        public string path => 
                        (Application.platform == RuntimePlatform.WindowsEditor || 
                         Application.platform == RuntimePlatform.WindowsPlayer ||
                         Application.platform == RuntimePlatform.WindowsServer) ? pathWindows : pathLinux;
    }

    public class NetworkConfigData
    {
        public string address = "127.0.0.1";
        public ushort port = 9999;

        public string addressNMEA = "127.0.0.1";
        public ushort portNMEA = 9999;

        public string addressJ1 = "127.0.0.1";
        public string addressJ2 = "127.0.0.1";
        public string addressWorld = "8.8.8.8";

        public string addressInternalGPS = "127.0.0.1";
        public ushort portInternalGPS = 9907;
    }



    public class TelemetryConfigData
    {
        public string path = "C:\\Telemetry\\";
        public string gstreamerProcessWindows = "C:/gstreamer/1.0/msvc_x86_64/bin/gst-launch-1.0.exe";
        public string screenRecordingOptions = "-e d3d11screencapturesrc ! queue! videoconvert ! x264enc ! h264parse ! mpegtsmux ! filesink location=screen.mp4";
        public string pathLinux = "C:\\Telemetry\\";
        public string gstreamerProcessLinux = "gst-launch-1.0";
        public string screenRecordingOptionsLinux = "-e d3d11screencapturesrc ! queue! videoconvert ! x264enc ! h264parse ! mpegtsmux ! filesink location=screen.mp4";
        public string tsConvertOptions = "filesrc location=INFILENAME ! tsdemux ! \"video/x-h265,profile=main\" ! h265parse ! mp4mux ! filesink location=OUTFILENAME";
        public string RecordingOptions => 
                        (Application.platform == RuntimePlatform.WindowsEditor || 
                         Application.platform == RuntimePlatform.WindowsPlayer ||
                         Application.platform == RuntimePlatform.WindowsServer) ? screenRecordingOptions : screenRecordingOptionsLinux;

        public string TelemetyPath => 
                        (Application.platform == RuntimePlatform.WindowsEditor || 
                         Application.platform == RuntimePlatform.WindowsPlayer ||
                         Application.platform == RuntimePlatform.WindowsServer) ? path : pathLinux;

        public string GStreamerProcess => 
                        (Application.platform == RuntimePlatform.WindowsEditor || 
                         Application.platform == RuntimePlatform.WindowsPlayer ||
                         Application.platform == RuntimePlatform.WindowsServer) ? gstreamerProcessWindows : gstreamerProcessLinux;
    }

    public class Config
    {
        protected const string fileName = "Config.xml";
        public bool release = false;
        public bool showFPS = true;
        [NonSerialized]
        public bool useCustomInput = true;
        public bool controllerConnectionCheck = true;

        public MapConfigData map = new MapConfigData(); 
        public TelemetryConfigData telemetry = new TelemetryConfigData();
        public NetworkConfigData network = new NetworkConfigData();
        public CamerasConfigData cameras = new CamerasConfigData();
        
        private static Config instance;
        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    string path = Path.Combine(Application.persistentDataPath, fileName);
#else
                    string path = Path.Combine(Application.streamingAssetsPath, fileName);
#endif
                    if (System.IO.File.Exists(path))
                    {
                        instance = Config.Deserialize<Config>(path);
                    }
                    else
                    {
                        instance = new Config();
                        Config.Save();
                    }
                }
                return instance;
            }
        }

        public static void Save()
        {
            if (instance == null)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, fileName);
#else
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
#endif
            Serialize(instance, path);
        }

        private static void Serialize(object item, string path)
        {
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            StreamWriter writer = new StreamWriter(path);
            serializer.Serialize(writer.BaseStream, item);
            writer.Close();
        }

        private static T Deserialize<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(path);
            T deserialized = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        }

        private static T Deserialize<T>(byte[] data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(data);
            T deserialized = (T)serializer.Deserialize(stream);
            stream.Close();
            return deserialized;
        }
    }
}