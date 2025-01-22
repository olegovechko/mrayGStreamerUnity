using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.UIElements;
using System.Text;
using System;
using UnityEngine.Video;
using System.Diagnostics.Tracing;
//using TankSim.Network;
using System.Runtime.InteropServices;

namespace TankSim
{
    public static class Telemetry
    {
        static public string Now_FolderName
        {
            get
            {
                var now = System.DateTime.Now;
                return $"{now.Year}{now.Month:00}{now.Day:00}_{now.Hour:00}{now.Minute:00}{now.Second:00}";
            }
        }
        static public string Now_Timestamp
        {
            get
            {
                var now = System.DateTime.Now;
                return $"{now.Year}{now.Month:00}{now.Day:00}-{now.Hour:00}:{now.Minute:00}:{now.Second:00}.{now.Millisecond:000}";
            }
        }

        static public string Now_Time_Colored
        {
            get
            {
                var now = System.DateTime.Now;
                return $"<color=white>{now.Hour:00}:{now.Minute:00}:{now.Second:00}<color=grey>.{now.Millisecond:000}</color>";
            }
        }

        static public long Now_Millis
        {
            get
            {
                return System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
        }
 
        private static bool recordingStarted = false;
        private static int framesRecorded = 0;
        private static int numFramesToFlush = 10;
        //private static System.Diagnostics.Process videoRecorder;
        private static string recordingFolder;
        
        private static FileStream telemetryFile;

        public static void StartRecording(string loginName)
        {
            try{
                recordingFolder = Path.Combine(Application.persistentDataPath, Now_FolderName);

                Directory.CreateDirectory(recordingFolder);
                // Text
                telemetryFile = File.Create(Path.Combine(recordingFolder, "telemetry.log"));
                // Video
                /*
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = Config.Instance.telemetry.screenRecordingProcess;
                info.Arguments = Config.Instance.telemetry.screenRecordingOptions;
                info.WorkingDirectory = recordingFolder;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardInput = true;
                videoRecorder = System.Diagnostics.Process.Start(info);
                */

                framesRecorded = 0;
                recordingStarted = true;
                AddStartFrame(loginName);
            } catch (Exception e)
            {
                Debug.LogError($"Can't start telemetry recording! {e}");
                recordingStarted = false;
            }
        }

        public static void StopRecording()
        {

            try{
                AddStopFrame();
                
                recordingStarted = false;

                if (telemetryFile != null)
                {
                    telemetryFile.Flush();
                    telemetryFile.Close();
                    telemetryFile = null;
                }
            } catch (Exception)
            {
            }
        }

        public static void AddStartFrame(string login)
        {
            if (!recordingStarted)
                return;
            framesRecorded++;
            var loginBytes = System.Text.Encoding.Unicode.GetBytes(login);
            string fixName = "";
            for (int i=0; i<loginBytes.Length; i++)
            {
                fixName += $"{loginBytes[i]:x2}";
            }

            telemetryFile.Write(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{Now_Timestamp},START,{fixName}\n"));

            if (framesRecorded % numFramesToFlush == 0)
                telemetryFile.Flush();
        }

        public static void AddStopFrame()
        {
            if (!recordingStarted)
                return;
            framesRecorded++;

            telemetryFile.Write(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{Now_Timestamp},STOP,0\n"));

            telemetryFile.Flush();
        }



        public static void AddRXFrame(byte[] frame)
        {
            if (!recordingStarted)
                return;
            framesRecorded++;
            telemetryFile.Write(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{Now_Timestamp},RX,{BytesToString(frame)}\n"));

            if (framesRecorded % numFramesToFlush == 0)
                telemetryFile.Flush();
        }

        public static void AddTXFrame(byte[] frame)
        {
            if (!recordingStarted)
                return;
            framesRecorded++;
            telemetryFile.Write(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{Now_Timestamp},TX,{BytesToString(frame)}\n"));
            if (framesRecorded % numFramesToFlush == 0)
                telemetryFile.Flush();
        }

     /*   public static void AddGPSFrame(byte[] frame, GPSProtocol gPSProtocol)
        {
            if (!recordingStarted)
                return;
            framesRecorded++;
            telemetryFile.Write(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{Now_Timestamp},{gPSProtocol},{Encoding.ASCII.GetString(frame)}\n"));
            if (framesRecorded % numFramesToFlush == 0)
                telemetryFile.Flush();
        }
*/
        public static void AddPingFrame(int ping, string dest)
        {
            if (!recordingStarted)
                return;
            framesRecorded++;
            telemetryFile.Write(
                System.Text.Encoding.ASCII.GetBytes(
                    $"{Now_Timestamp},PING,{dest}:{ping}\n"));
            if (framesRecorded % numFramesToFlush == 0)
                telemetryFile.Flush();
        }

        private static string BytesToString(byte[] frame)
        {
            return BitConverter.ToString(frame).Replace("-", "");
            /*
            StringBuilder sb = new StringBuilder(frame.Length *2);
            foreach(byte b in frame)
            {
                sb.Append($"{b:X}");
            }
            return sb.ToString();
            */
        } 

    }
}