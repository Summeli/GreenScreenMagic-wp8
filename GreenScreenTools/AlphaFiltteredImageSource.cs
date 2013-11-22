using Nokia.Graphics.Imaging;
using Windows.Foundation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.Phone.Media.Capture;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
/**
 * Source class for providing the filttered media stream to the UI
 */

namespace GreenScreenTools
{
    public class AlphaFiltteredImageSource : MediaStreamSource
    {
        private readonly Dictionary<MediaSampleAttributeKeys, string> _emptyAttributes = new Dictionary<MediaSampleAttributeKeys, string>();
        private int _frameBufferSize = 0;
        private byte[] _frameBuffer = null;
        private MediaStreamDescription _videoStreamDescription = null;
        private MemoryStream _frameStream = null;
        private int _frameTime = 0;
        private long _currentTime = 0;
        private static int _frameStreamOffset = 0;
        private Windows.Foundation.Size _frameSize;
        private Semaphore _efectSemaphore = new Semaphore(1, 1);
        private FilterEffect _filterEffect = null;
        private TransparentToBlackFilter _transparentEffect = null;
        private IImageProvider _imageProvider = null;

        public AlphaFiltteredImageSource(IImageProvider imageProvider, Windows.Foundation.Size size)
        {
            _imageProvider = imageProvider;
            _frameSize = size;
        }

        protected override void OpenMediaAsync()
        {
            // General properties

            _frameBufferSize = (int)_frameSize.Width * (int)_frameSize.Height * 4; // RGBA
            _frameBuffer = new byte[_frameBufferSize];
            _frameStream = new MemoryStream(_frameBuffer);

            // Media stream attributes

            var mediaStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();

            mediaStreamAttributes[MediaStreamAttributeKeys.VideoFourCC] = "RGBA";
            mediaStreamAttributes[MediaStreamAttributeKeys.Width] = ((int)_frameSize.Width).ToString();
            mediaStreamAttributes[MediaStreamAttributeKeys.Height] = ((int)_frameSize.Height).ToString();

            _videoStreamDescription = new MediaStreamDescription(MediaStreamType.Video, mediaStreamAttributes);

            // Media stream descriptions

            var mediaStreamDescriptions = new List<MediaStreamDescription>();
            mediaStreamDescriptions.Add(_videoStreamDescription);

            // Media source attributes

            var mediaSourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            mediaSourceAttributes[MediaSourceAttributesKeys.Duration] = TimeSpan.FromSeconds(0).Ticks.ToString(CultureInfo.InvariantCulture);
            mediaSourceAttributes[MediaSourceAttributesKeys.CanSeek] = false.ToString();

            _frameTime = (int)TimeSpan.FromSeconds((double)0).Ticks;

            //_cameraImageSource = new CameraPreviewImageSource(_camera);

            //Create Filter
            var filters = new List<IFilter>();
            Windows.UI.Color green = new Windows.UI.Color();
            green = Windows.UI.Color.FromArgb(255, 0, 255, 0);
            filters.Add(new ChromaKeyFilter(green, 0.5));
            _filterEffect = new FilterEffect(_imageProvider)
            {
                Filters = filters
            };
            _transparentEffect = new TransparentToBlackFilter(_filterEffect);
            // Report that we finished initializing its internal state and can now pass in frame samples

            ReportOpenMediaCompleted(mediaSourceAttributes, mediaStreamDescriptions);
        }

        protected override void CloseMedia()
        {
            if (_frameStream != null)
            {
                _frameStream.Close();
                _frameStream = null;

                _frameTime = 0;
                _frameBufferSize = 0;
                _frameBuffer = null;
                _videoStreamDescription = null;
            }

            if (_filterEffect != null)
            {
                _filterEffect.Dispose();
                _filterEffect = null;
            }

        }

        public void SetFilterSettings(Windows.UI.Color color, double distance)
        {
            var filters = new List<IFilter>();
            filters.Add(new ChromaKeyFilter(color, 0.3, 1, false));
            // filters.Add(new CartoonFilter(false));
            _filterEffect = new FilterEffect(_imageProvider)
            {
                Filters = filters
            };

            _transparentEffect = new TransparentToBlackFilter(_filterEffect);
        }
        /// <summary>
        /// Processes camera frameBuffer using the set effect and provides media element with a filtered frameBuffer.
        /// </summary>
        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            var task = GetNewFrameAndApplyEffect(_frameBuffer.AsBuffer());

            // When asynchroneous call completes, proceed by reporting about the sample completion

            task.ContinueWith((action) =>
            {
                if (_frameStream != null)
                {
                    _frameStream.Position = 0;
                    _currentTime += _frameTime;


                    var sample = new MediaStreamSample(_videoStreamDescription, _frameStream, _frameStreamOffset, _frameBufferSize, _currentTime, _emptyAttributes);

                    ReportGetSampleCompleted(sample);
                }
            });
        }

        private async Task GetNewFrameAndApplyEffect(IBuffer frameBuffer)
        {
            if (_efectSemaphore.WaitOne(500))
            {
                uint scanlineByteSize = (uint)_frameSize.Width * 4; // 4 bytes per pixel in BGRA888 mode
                var bitmap = new Nokia.Graphics.Imaging.Bitmap(_frameSize, ColorMode.Bgra8888, scanlineByteSize, frameBuffer);

                /*
                if (_filterEffect != null)
                {
                    var renderer = new BitmapRenderer(_filterEffect, bitmap);
                    await renderer.RenderAsync();
                }*/

                if (_transparentEffect != null)
                {
                    var renderer = new BitmapRenderer(_transparentEffect, bitmap);
                    await renderer.RenderAsync();
                }
                _efectSemaphore.Release();
            }
        }

        public void pause()
        {
            _efectSemaphore.WaitOne();
        }

        public void resume()
        {
            _efectSemaphore.Release();
        }

        //empty implementations
        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        protected override void SeekAsync(long seekToTime)
        {
            _currentTime = seekToTime;
            ReportSeekCompleted(_currentTime);
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }
    }
}
