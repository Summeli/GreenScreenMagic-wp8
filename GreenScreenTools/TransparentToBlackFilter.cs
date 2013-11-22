using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace GreenScreenTools
{
    class TransparentToBlackFilter : CustomEffectBase
    {
        public TransparentToBlackFilter(IImageProvider imageprovider)
            : base(imageprovider)
        {

        }


        protected override void OnProcess(PixelRegion source, PixelRegion target)
        {
            source.ForEachRow((index, width, pos) =>
            {
                for (int x = 0; x < width; ++x, ++index)
                {
                    Color c = ToColor(source.ImagePixels[index]);
                    //if alpha channel, then convert color to black
                    if (c.A == 0)
                    {
                        c.A = 255;
                        c.B = 0;
                        c.G = 0;
                        c.R = 0;
                    }

                    target.ImagePixels[index] = FromColor(c);
                }
            });
        }
    }
}
