using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using Microsoft.Kinect;



namespace Microsoft.Kinect.Wrapper
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class KinectWrapper
    {

        readonly int Bgr32BytesPerPixel = 4;


        #region field

        public delegate void CallBackFunction();
        public delegate void CallBackFunction2([MarshalAs(UnmanagedType.SafeArray)] ref byte[] colorPixel);
        private CallBackFunction _colorframe_callback;
        private CallBackFunction _depthframe_callback;

        //private CallBackFunction2 _callback2;


        private KinectSensor _kinect = null;
        private byte[] _colorPixel;
        private byte[] _depthPixel;


        private int _width;
        private int _height;


        #endregion

        #region コンストラクタ & デストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KinectWrapper()
        {

            if (KinectSensor.KinectSensors.Count == 0)
            {
                throw new Exception("Kinectが接続されていません");
            }

            // Kinectインスタンスを取得する
            _kinect = KinectSensor.KinectSensors[0];
        }


        ~KinectWrapper()
        {
            if (_kinect == null) return;
            if (_kinect.IsRunning == true)
            {
                _kinect.Stop();
                _kinect.Dispose();
            }
            _kinect = null;
        }

        #endregion


        #region プロパティ


        /// <summary>
        /// KinectSensorの数
        /// </summary>
        public int Count
        {
            get { return KinectSensor.KinectSensors.Count; }
        }

        
        /// <summary>
        /// キネクトデバイスの接続ID
        /// </summary>
        public string DeviceConnectionId
        {
            get { return _kinect != null ? _kinect.DeviceConnectionId : string.Empty; }
        }
 

        /// <summary>
        /// KinectSensorの角度
        /// </summary>
        public int ElevationAngle
        {
            get { return _kinect != null ? _kinect.ElevationAngle : 0; }
            set {
                if (_kinect == null) return;
                _kinect.ElevationAngle = value; 
            }
        }


        /// <summary>
        /// KinectSensorの動作状態
        /// true: running
        /// false:not running
        /// </summary>
        public bool IsRunning
        {
            get { return  _kinect != null ? _kinect.IsRunning : false; }
        }

        /// <summary>
        /// KinectSensorの最大角度
        /// </summary>
        public int MaxElevationAngle
        {
            get { return _kinect != null ? _kinect.MaxElevationAngle : 0; }
        }


        /// <summary>
        /// KinectSensorの最少角度
        /// </summary>
        public int MinElevationAngle
        {
            get { return _kinect != null ? _kinect.MinElevationAngle : 0; }
        }


        /// <summary>
        /// KinectSensorの接続状態
        /// </summary>
        public Microsoft.Kinect.Wrapper.KinectStatus Status
        {
            get { return _kinect != null ? (Microsoft.Kinect.Wrapper.KinectStatus)_kinect.Status   : 0; }
        }



        #endregion


        #region pixel data

        /// <summary>
        /// RGB pixcel data
        /// </summary>
        public byte[] ColorPixel
        {
            get { return _colorPixel; }
        }


        /// <summary>
        /// Depth pixcel data
        /// </summary>
        public byte[] DepthPixel
        {
            get { return _depthPixel; }
        }


        /// <summary>
        /// image width
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// image width
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        #endregion



        #region Stop & Go

        /// <summary>
        /// KinectSensor Start
        /// </summary>
        public void Start()
        {
            if (_kinect == null) return;
            _kinect.Start();
        }

        /// <summary>
        /// KinectSensor Stop
        /// </summary>
        public void Stop()
        {
            if (_kinect == null) return;
            _kinect.Stop();
        }

        /// <summary>
        /// KinectSensor Dispose
        /// </summary>
        public void Dispose()
        {
            if (_kinect == null) return;
            _kinect.Dispose();
        }


        /// <summary>
        /// KinectSensor Stop & Dispose
        /// </summary>
        public void StopAndDispose()
        {
            if (_kinect == null) return;
            _kinect.Stop();
            _kinect.Dispose();
        }

        #endregion


        #region Get RGB 

        public string ColorFrame_OneFrame(
            [MarshalAs(UnmanagedType.FunctionPtr)] 
            ref CallBackFunction colorframe_callback
            , ColorImageFormat format = ColorImageFormat.RgbResolution640x480Fps30
            )
        {
            try
            {
                _colorframe_callback = colorframe_callback;
                ColorStream_Enable(format);
                _kinect.ColorFrameReady += ColorFrame_OneFrame;


                return "ready";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// RGBカメラのフレームを１フレーム分だけ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ColorFrame_OneFrame(object sender, ColorImageFrameReadyEventArgs e)
        {
            try
            {
                // RGBカメラのフレームデータを取得する
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        _kinect.ColorFrameReady -= ColorFrame_OneFrame;

                        // RGBカメラのピクセルデータを取得する
                        _colorPixel = new byte[colorFrame.PixelDataLength];
                        colorFrame.CopyPixelDataTo(_colorPixel);

                        if (_colorframe_callback != null) _colorframe_callback();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion


        /// <summary>
        /// ColorStream.Enable
        /// </summary>
        /// <param name="format"></param>
        public void ColorStream_Enable(ColorImageFormat format = ColorImageFormat.RgbResolution640x480Fps30)
        {
            _kinect.ColorStream.Enable((Microsoft.Kinect.ColorImageFormat)format);

            //画像サイズ設定
            switch (format)
            {
                case ColorImageFormat.RgbResolution640x480Fps30:
                    _width = 640;
                    _height = 480;
                    break;
                case ColorImageFormat.RgbResolution1280x960Fps12:
                    _width = 1280;
                    _height = 960;
                    break;
                case ColorImageFormat.RawYuvResolution640x480Fps15:
                    _width = 640;
                    _height = 480;
                    break;
                case ColorImageFormat.YuvResolution640x480Fps15:
                    _width = 640;
                    _height = 480;
                    break;
            }

        }


        /// <summary>
        /// RGBカメラのピクセルデータを取得し、_colorPixelへコピーする
        /// </summary>
        public bool GetColorFramePixcel()
        {
            using (ColorImageFrame colorFrame = _kinect.ColorStream.OpenNextFrame(100))
            {
                if (colorFrame == null) return false;

                // RGBカメラのピクセルデータを取得する
                _colorPixel = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(_colorPixel);
                return true;
            }
        }



        #region Get Depth

        public string DepthFrame_OneFrame(
            [MarshalAs(UnmanagedType.FunctionPtr)] 
            ref CallBackFunction depthframe_callback
            , DepthImageFormat format = DepthImageFormat.Resolution80x60Fps30
            )
        {
            try
            {
                _depthframe_callback = depthframe_callback;
                _kinect.ColorStream.Enable(Microsoft.Kinect.ColorImageFormat.RgbResolution640x480Fps30);
                _kinect.DepthStream.Enable((Microsoft.Kinect.DepthImageFormat)format);
                _kinect.DepthFrameReady += DepthFrameReady_OneFrame;

                //画像サイズ設定
                switch (format)
                {
                    case DepthImageFormat.Resolution80x60Fps30:
                        _width = 80;
                        _height = 60;
                        break;
                    case DepthImageFormat.Resolution320x240Fps30:
                        _width = 320;
                        _height = 240;
                        break;
                    case DepthImageFormat.Resolution640x480Fps30:
                        _width = 320;
                        _height = 240;
                        break;
                }

                return "ready";
            }
            catch (Exception ex)
            {
                _kinect.Stop();
                _kinect.Dispose();
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// RGBカメラのフレームを１フレーム分だけ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DepthFrameReady_OneFrame(object sender, DepthImageFrameReadyEventArgs e)
        {
            try
            {
                // RGBカメラのフレームデータを取得する
                using (var depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame != null)
                    {
                        _kinect.DepthFrameReady -= DepthFrameReady_OneFrame;

                        // RGBカメラのピクセルデータを取得する
                        //_depthPixel = new short[depthFrame.PixelDataLength];
                        //depthFrame.CopyPixelDataTo(_depthPixel);

                        _depthPixel = ConvertDepthColor(_kinect, depthFrame);

                        if (_depthframe_callback != null) _depthframe_callback();
                    }
                }
            }
            catch (Exception ex)
            {
                _kinect.Stop();
                _kinect.Dispose();
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// 距離データをカラー画像に変換する
        /// </summary>
        /// <param name="kinect"></param>
        /// <param name="depthFrame"></param>
        /// <returns></returns>
        private byte[] ConvertDepthColor(KinectSensor kinect, DepthImageFrame depthFrame)
        {
            ColorImageStream colorStream = kinect.ColorStream;
            if (colorStream == null) return null;

            DepthImageStream depthStream = kinect.DepthStream;
            if (depthStream == null) return null;

            // 距離カメラのピクセルごとのデータを取得する
            short[] depthPixel = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(depthPixel);

            // 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
            ColorImagePoint[] colorPoint =
              new ColorImagePoint[depthFrame.PixelDataLength];
            kinect.MapDepthFrameToColorFrame(depthStream.Format, depthPixel,
              colorStream.Format, colorPoint);

            byte[] depthColor = new byte[depthFrame.PixelDataLength * Bgr32BytesPerPixel];
            for (int index = 0; index < depthPixel.Length; index++)
            {
                // 距離カメラのデータから、距離を取得する
                int distance = depthPixel[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
                int x = Math.Min(colorPoint[index].X, colorStream.FrameWidth - 1);
                int y = Math.Min(colorPoint[index].Y, colorStream.FrameHeight - 1);

                int colorIndex = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel;

                // サポート外 0-40cm
                if (distance == depthStream.UnknownDepth)
                {
                    depthColor[colorIndex] = 0;
                    depthColor[colorIndex + 1] = 0;
                    depthColor[colorIndex + 2] = 255;
                }
                // 近すぎ 40cm-80cm(default mode)
                else if (distance == depthStream.TooNearDepth)
                {
                    depthColor[colorIndex] = 0;
                    depthColor[colorIndex + 1] = 255;
                    depthColor[colorIndex + 2] = 0;
                }
                // 遠すぎ 3m(Near),4m(Default)-8m
                else if (distance == depthStream.TooFarDepth)
                {
                    depthColor[colorIndex] = 255;
                    depthColor[colorIndex + 1] = 0;
                    depthColor[colorIndex + 2] = 0;
                }
                // 有効な距離データ
                else
                {
                    depthColor[colorIndex] = 0;
                    depthColor[colorIndex + 1] = 255;
                    depthColor[colorIndex + 2] = 255;
                }
            }

            return depthColor;
        }



        #endregion





        //public KinectAudioSource AudioSource { get; }
        //public ColorImageStream ColorStream { get; }
        //public DepthImageStream DepthStream { get; }
        //public static KinectSensorCollection KinectSensors { get; }
        //public SkeletonStream SkeletonStream { get; }
        //public string UniqueKinectId { get; }

        //public event EventHandler<AllFramesReadyEventArgs> AllFramesReady;
        //public event EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady;
        //public event EventHandler<DepthImageFrameReadyEventArgs> DepthFrameReady;
        //public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        //public void Dispose();
        //public void MapDepthFrameToColorFrame(DepthImageFormat depthImageFormat, short[] depthPixelData, ColorImageFormat colorImageFormat, ColorImagePoint[] colorCoordinates);
        //public ColorImagePoint MapDepthToColorImagePoint(DepthImageFormat depthImageFormat, int depthX, int depthY, short depthPixelValue, ColorImageFormat colorImageFormat);
        //public SkeletonPoint MapDepthToSkeletonPoint(DepthImageFormat depthImageFormat, int depthX, int depthY, short depthPixelValue);
        //public ColorImagePoint MapSkeletonPointToColor(SkeletonPoint skeletonPoint, ColorImageFormat colorImageFormat);
        //public DepthImagePoint MapSkeletonPointToDepth(SkeletonPoint skeletonPoint, DepthImageFormat depthImageFormat);

    }
}
