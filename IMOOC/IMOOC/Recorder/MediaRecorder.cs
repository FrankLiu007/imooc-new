using System;
using System.Runtime.InteropServices;

namespace IMOOC.Recorder
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DeviceInfo
    {
        public int idx;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct EncoderInfo
    {
        public int cid;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string name;
    }

   public class MediaRecorder
    {
        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr MR_GetVersion();

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_Init();

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void MR_Quit();

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetCameraDeviceList(IntPtr devs, ref int size);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetSoundDeviceList(IntPtr devs, ref int size);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetVideoEncoderList(IntPtr devs, ref int size);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetSoundEncoderList(IntPtr devs, ref int size);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_StartMediaServer(int port);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_StopMediaServer();

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_OpenRecorder(int cameraIdx, int soundIdx);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void MR_CloseRecorder(int handle);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetResolution(int handle, ref int width, ref int height);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetResolution(int handle, int width, int height);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetFrameRate(int handle, ref int fps);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetFrameRate(int handle, int fps);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetAudioProfile(int handle, ref int channels, ref int sampleRate, ref int sampleBits);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetAudioProfile(int handle, int channels, int sampleRate, int sampleBits);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetVideoEncoder(int handle, int codec);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetAudioEncoder(int handle, int codec);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetVideoBitrate(int handle, ref int bitrate);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetVideoBitrate(int handle, int bitrate);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetOutputFile(int handle, [MarshalAs(UnmanagedType.LPStr)]string filepath);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_Start(int handle);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_Pause(int handle);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_Stop(int handle);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_GetState(int handle, ref int state);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_StartPublish(int handle, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_StopPublish(int handle);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_SetVideoWindow(int handle, int hwnd);

        [DllImport("MediaRecorder.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MR_ShowVideoWindow(int handle, int visable);



        public static int MAX_PLANE = 8;
        public static int MAX_PATH = 260;
        public static int MAX_DEVICE = 16;
        public static int MAX_ENCODER = 16;
        public static int RTSP_PORT = 554;    /// 默认RTSP端口



        public static String GetVersion()
        {
            IntPtr intPtr = MR_GetVersion();
            string str = Marshal.PtrToStringAnsi(intPtr);
            return str;
        }

        public static int Init()
        {
            return MR_Init();
        }

        public static void Quit()
        {
            MR_Quit();
        }

        public static DeviceInfo[] GetCameraDeviceList()
        {
            int size = Marshal.SizeOf(typeof(DeviceInfo)) * MAX_DEVICE;
            byte[] bytes = new byte[size];
            IntPtr pBuff = Marshal.AllocHGlobal(size);
            int count = MAX_DEVICE;
            int rc = MR_GetCameraDeviceList(pBuff, ref count);
            if (rc != 0)
            {
                return new DeviceInfo[0];
            }

            DeviceInfo[] devList = new DeviceInfo[count];

            for (int i = 0; i < count; i++)
            {
                IntPtr pPonitor = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(DeviceInfo)) * i);
                devList[i] = (DeviceInfo)Marshal.PtrToStructure(pPonitor, typeof(DeviceInfo));

                //System.Console.WriteLine(devList[i].idx);
                //System.Console.WriteLine(devList[i].name);
            }
            Marshal.FreeHGlobal(pBuff);

            return devList;
        }

        public static DeviceInfo[] GetSoundDeviceList()
        {
            int size = Marshal.SizeOf(typeof(DeviceInfo)) * MAX_DEVICE;
            byte[] bytes = new byte[size];
            IntPtr pBuff = Marshal.AllocHGlobal(size);
            int count = MAX_DEVICE;
            int rc = MR_GetSoundDeviceList(pBuff, ref count);
            if (rc != 0)
            {
                return new DeviceInfo[0];
            }

            DeviceInfo[] devList = new DeviceInfo[count];

            for (int i = 0; i < count; i++)
            {
                IntPtr pPonitor = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(DeviceInfo)) * i);
                devList[i] = (DeviceInfo)Marshal.PtrToStructure(pPonitor, typeof(DeviceInfo));

                //System.Console.WriteLine(devList[i].idx);
                //System.Console.WriteLine(devList[i].name);
            }
            Marshal.FreeHGlobal(pBuff);

            return devList;
        }

        public static EncoderInfo[] GetVideoEncoderList()
        {
            int size = Marshal.SizeOf(typeof(EncoderInfo)) * MAX_DEVICE;
            byte[] bytes = new byte[size];
            IntPtr pBuff = Marshal.AllocHGlobal(size);
            int count = MAX_ENCODER;
            int rc = MR_GetVideoEncoderList(pBuff, ref count);
            if (rc != 0)
            {
                return new EncoderInfo[0];
            }

            EncoderInfo[] encoderList = new EncoderInfo[count];

            for (int i = 0; i < count; i++)
            {
                IntPtr pPonitor = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(EncoderInfo)) * i);
                encoderList[i] = (EncoderInfo)Marshal.PtrToStructure(pPonitor, typeof(EncoderInfo));
            }
            Marshal.FreeHGlobal(pBuff);

            return encoderList;
        }

        public static EncoderInfo[] GetSoundEncoderList()
        {
            int size = Marshal.SizeOf(typeof(EncoderInfo)) * MAX_DEVICE;
            byte[] bytes = new byte[size];
            IntPtr pBuff = Marshal.AllocHGlobal(size);
            int count = MAX_ENCODER;
            int rc = MR_GetSoundEncoderList(pBuff, ref count);
            if (rc != 0)
            {
                return new EncoderInfo[0];
            }

            EncoderInfo[] encoderList = new EncoderInfo[count];

            for (int i = 0; i < count; i++)
            {
                IntPtr pPonitor = new IntPtr(pBuff.ToInt64() + Marshal.SizeOf(typeof(EncoderInfo)) * i);
                encoderList[i] = (EncoderInfo)Marshal.PtrToStructure(pPonitor, typeof(EncoderInfo));
            }
            Marshal.FreeHGlobal(pBuff);

            return encoderList;
        }

        public static int StartMediaServer(int port)
        {
            return MR_StartMediaServer(port);
        }

        public static void StopMediaServer()
        {
            MR_StopMediaServer();
        }


        private int m_handle = 0;

        public static MediaRecorder Open(int camera, int sound)
        {
            int handle = MR_OpenRecorder(camera, sound);
            if (handle <= 0)
            {
                return null;
            }
            return new MediaRecorder(handle);
        }

        public MediaRecorder(int handle)
        {
            m_handle = handle;            
        }

        public MediaRecorder()
        {

        }

        public bool IsOpen()
        {
            return (m_handle > 0);
        }

        public void Close()
        {
            if (m_handle > 0)
            {
                MR_CloseRecorder(m_handle);
                m_handle = 0;
            }
        }

        public int GetResolution(ref int width, ref int height)
        {
            return MR_GetResolution(m_handle, ref width, ref height);
        }

        public int SetResolution(int width, int height)
        {
            return MR_SetResolution(m_handle, width, height);
        }

        public int GetFrameRate(ref int fps)
        {
            return MR_GetFrameRate(m_handle, ref fps);
        }

        public int SetFrameRate(int fps)
        {
            return MR_SetFrameRate(m_handle, fps);
        }

        public int GetAudioProfile(ref int channels, ref int sampleRate, ref int sampleBits)
        {
            return MR_GetAudioProfile(m_handle, ref channels, ref sampleRate, ref sampleBits);
        }

        public int SetAudioProfile(int channels, int sampleRate, int sampleBits)
        {
            return MR_SetAudioProfile(m_handle, channels, sampleRate, sampleBits);
        }


        public int SetVideoEncoder(int codec)
        {
            return MR_SetVideoEncoder(m_handle, codec);
        }

        public int SetAudioEncoder(int codec)
        {
            return MR_SetAudioEncoder(m_handle, codec);
        }

        public int GetVideoBitrate(ref int bitrate)
        {
            return MR_GetVideoBitrate(m_handle, ref bitrate);
        }

        public int SetVideoBitrate(int bitrate)
        {
            return MR_SetVideoBitrate(m_handle, bitrate);
        }


        public int SetOutputFile(string filepath)
        {
            return MR_SetOutputFile(m_handle, filepath);
        }


        public int Start()
        {
            return MR_Start(m_handle);
        }

        public int Pause()
        {
            return MR_Pause(m_handle);
        }


        public int Stop()
        {
            return MR_Stop(m_handle);
        }


        public int GetState(ref int state)
        {
            return MR_GetState(m_handle, ref state);
        }


        public int StartPublish(string name)
        {
            return MR_StartPublish(m_handle, name);
        }

        public int StopPublish()
        {
            return MR_StopPublish(m_handle);
        }

        public int SetVideoWindow(int hwnd)
        {
            return MR_SetVideoWindow(m_handle, hwnd);
        }

        public int ShowVideoWindow(int visable)
        {
            return MR_ShowVideoWindow(m_handle, visable);
        }

        public void InitMediaRecorder()
        {
            Init();
            Console.WriteLine(GetVersion());
            StartMediaServer(RTSP_PORT);
        }

        public void UninitMediaRecorder()
        {
            StopMediaServer(); 
            Quit();
            Console.WriteLine("quit");
        }
    }

}
