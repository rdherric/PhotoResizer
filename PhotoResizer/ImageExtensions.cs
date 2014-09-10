using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PhotoResizer
{
    /// <summary>
    /// ImageExtensions extends the System.Drawing.Image class
    /// to provide easy resizing methods.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Resize takes an Image and resizes it to the desired
        /// size with the specified side length.
        /// </summary>
        /// <param name="img">The Image to resize</param>
        /// <param name="longSideLength">The length of the long side of the Image</param>
        /// <returns>Resized Image</returns>
        public static Image Resize(this Image img, Int32 longSideLength)
        {
            //Figure out which side is the long side
            Int32 imgLongSide = img.Size.Width;
            if (img.Size.Height > img.Size.Width)
                imgLongSide = img.Size.Height;
            
            //Calculate the percent change in the size
            //of the Image
            Double percentChange = Convert.ToDouble(longSideLength) / Convert.ToDouble(imgLongSide);

            //Now calculate the sizes of the two sides
            Int32 newWidth = Convert.ToInt32(img.Size.Width * percentChange);
            Int32 newHeight = Convert.ToInt32(img.Size.Height * percentChange);

            //Create the return Image
            Bitmap bmp = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            //Get a Graphics from the Bitmap and draw the Image
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //Set the way that the Graphics will scale the image
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                //Draw the Image into the Bitmap
                g.DrawImage(img,
                    new Rectangle(0, 0, newWidth, newHeight),
                    new Rectangle(0, 0, img.Size.Width, img.Size.Height),
                    GraphicsUnit.Pixel);
            }

            //Return the result
            return bmp;
        }
    }
}