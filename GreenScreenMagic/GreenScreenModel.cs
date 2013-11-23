using GreenScreenTools;
using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;

namespace GreenScreenMagic
{
    public class GreenScreenModel : IDisposable
    {
        private IBuffer _imageBuffer = null;
        private IBuffer _bgBuffer = null;
        private double _currentDelta;
        private Windows.UI.Color _currentColor = new Windows.UI.Color();

        public IBuffer ImageBuffer
        {
            get
            {
                using (BufferImageSource source = new BufferImageSource(_imageBuffer))
                using (JpegRenderer renderer = new JpegRenderer(source))
                {
                    IBuffer buffer = null;

                    Task.Run(async () => { buffer = await renderer.RenderAsync(); }).Wait();

                    return buffer;
                }
            }

            set
            {
                if (_imageBuffer != value)
                {
                    _imageBuffer = value;
                }
            }
        }

        public IBuffer BackgroundBuffer
        {
            get
            {
                using (BufferImageSource source = new BufferImageSource(_bgBuffer))
                using (JpegRenderer renderer = new JpegRenderer(source))
                {
                    IBuffer buffer = null;

                    Task.Run(async () => { buffer = await renderer.RenderAsync(); }).Wait();

                    return buffer;
                }
            }

            set
            {
                if (_bgBuffer != value)
                {
                    _bgBuffer = value;
                }
            }
        }

        public void Dispose()
        {
            _imageBuffer = null;
            _bgBuffer = null;
        }

        //if both buffers are set, then we're ready to go
        public Boolean isReady()
        {
            if (_imageBuffer != null && _imageBuffer.Length > 0 && _bgBuffer != null &&_bgBuffer.Length > 0)
                return true;
            else
                return false;
        }

        public async Task RenderBitmapTransparentAndBlackAsync(WriteableBitmap bitmap, Windows.UI.Color color, double distance)
        {
            _currentDelta = distance;
            _currentColor = color;
            var filters = new List<IFilter>();
            filters.Add(new ChromaKeyFilter(_currentColor, _currentDelta, 1, false));

            using (BufferImageSource source = new BufferImageSource(_imageBuffer))
            using (FilterEffect effect = new FilterEffect(source) { Filters = filters })
            using (TransparentToBlackFilter blackEffect = new TransparentToBlackFilter(effect))
            using (WriteableBitmapRenderer renderer = new WriteableBitmapRenderer(blackEffect, bitmap))
            {
                await renderer.RenderAsync();

                bitmap.Invalidate();
            }
        }

        public async Task RenderResultBitmap(WriteableBitmap bitmap)
        {
     
            var chomafilters = new List<IFilter>();
            chomafilters.Add(new ChromaKeyFilter(_currentColor, _currentDelta, 1, false));

             // First remove the chroma key from the fron image, and then combine images 1, and 2
            using (BufferImageSource frontSource = new BufferImageSource(_imageBuffer))
            using (FilterEffect front = new FilterEffect(frontSource) { Filters = chomafilters })
            //combine images
            using (BufferImageSource background = new BufferImageSource(_bgBuffer))
            using (FilterEffect filters = new FilterEffect(background))
            using (WriteableBitmapRenderer renderer = new WriteableBitmapRenderer(filters, bitmap))
            {
                filters.Filters = new IFilter[] { new BlendFilter(front, BlendFunction.Normal) };
                await renderer.RenderAsync();

                bitmap.Invalidate();
            }
        }


    }
}
