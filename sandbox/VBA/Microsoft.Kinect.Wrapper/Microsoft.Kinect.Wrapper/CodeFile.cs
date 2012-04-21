
namespace Microsoft.Kinect.Wrapper
{

    public enum KinectStatus
    {
        Undefined = 0,
        Disconnected = 1,
        Connected = 2,
        Initializing = 3,
        Error = 4,
        NotPowered = 5,
        NotReady = 6,
        DeviceNotGenuine = 7,
        DeviceNotSupported = 8,
        InsufficientBandwidth = 9,
    }


    public enum DepthImageFormat
    {
        Undefined = 0,
        Resolution640x480Fps30 = 1,
        Resolution320x240Fps30 = 2,
        Resolution80x60Fps30 = 3,
    }


    
    public enum ColorImageFormat
    {
        Undefined = 0,
        RgbResolution640x480Fps30 = 1,
        RgbResolution1280x960Fps12 = 2,
        YuvResolution640x480Fps15 = 3,
        RawYuvResolution640x480Fps15 = 4,
    }
}
