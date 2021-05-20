using NetVips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Util
{
  public static class ImageUtil
  {
    const int VIPS_MAX_COORD = 10000000;
    public static byte[] CreateThumbnail(byte[] image, int? maxwidth = null, int? maxheight = null)
    {
      if (maxwidth == null && maxheight == null)
      {
        throw new ArgumentException("Maximum size for at least one of axis must be specified");
      }

      using (var thumbnailImage = Image.ThumbnailBuffer(image, maxwidth ?? VIPS_MAX_COORD, height: maxheight ?? VIPS_MAX_COORD))
      {
        byte[] thumbnail = thumbnailImage.WriteToBuffer(".jpg");
        return thumbnail;
      }      
    }
  }
}
