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

        public void Dispose()
        {
            _imageBuffer = null;
            _bgBuffer = null;
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



    }
}
