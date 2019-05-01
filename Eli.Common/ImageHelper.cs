using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Eli.Common
{
    public static class ImageHelper
    {
        public static string GetImageUrl(object fileName)
        {
            return ConfigValues.UPLOAD_DIRECTORY + fileName;
        }

        /// <summary>
        /// Getting a thumbnail for an image
        /// </summary>
        /// <param name="image">image path or filename</param>
        /// <param name="width">Thumbnail width</param>
        /// <param name="height">Thumbnail height</param>
        /// <param name="thumbnailType">The type of return thumbnail image</param>
        /// <returns></returns>
        public static string GetThumb(object image, int width, int height, ThumbnailType thumbnailType)
        {
            return String.Format("{0}Thumbnail.aspx?Image={1}&w={2}&h={3}&t={4}", ConfigValues.SITE_ROOT, image, width, height, (int)thumbnailType);
        }
        /// <summary>
        /// Get thumbnail image by percent
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thumbnailType"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static string GetThumb(object image, int width, int height, ThumbnailType thumbnailType, int percent)
        {
            return String.Format("{0}Thumbnail.aspx?Image={1}&w={2}&h={3}&t={4}&p={5}", ConfigValues.SITE_ROOT, image, width, height, (int)thumbnailType, percent);
        }
        /// <summary>
        /// Get thumbnail image with Constrain Proportions
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thumbnailType"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public static string GetThumb(object image, int width, int height, ThumbnailType thumbnailType, Dimensions dimensions)
        {
            return String.Format("{0}Thumbnail.aspx?Image={1}&w={2}&h={3}&t={4}&d={5}", ConfigValues.SITE_ROOT, image, width, height, (int)thumbnailType, (int)dimensions);
        }
        /// <summary>
        /// Get thumbnail image by cropping
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thumbnailType"></param>
        /// <param name="anchorPosition"></param>
        /// <returns></returns>
        public static string GetThumb(object image, int width, int height, ThumbnailType thumbnailType, AnchorPosition anchorPosition)
        {
            return String.Format("{0}Thumbnail.aspx?Image={1}&w={2}&h={3}&t={4}&a={5}", ConfigValues.SITE_ROOT, image, width, height, (int)thumbnailType, (int)anchorPosition);
        }

        /// Creates a resized bitmap from an existing image on disk.
        /// Call Dispose on the returned Bitmap object
        /// Bitmap or null

        public static Bitmap CreateThumbNail(string lcFilename, int lnWidth, int lnHeight)
        {
            Bitmap bmpOut;
            try
            {
                var loBMP = new Bitmap(HttpContext.Current.Server.MapPath(string.Format("~{0}{1}",ConfigValues.UPLOAD_DIRECTORY, lcFilename)));
                var loFormat = loBMP.RawFormat;
                decimal lnRatio;
                int lnNewWidth;
                int lnNewHeight;

                //*** If the image is smaller than a thumbnail just return it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;
                if (loBMP.Width > loBMP.Height)
                {
                    lnRatio = (decimal)lnWidth / loBMP.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBMP.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBMP.Height;

                    lnNewHeight = lnHeight;

                    decimal lnTemp = loBMP.Width * lnRatio;

                    lnNewWidth = (int)lnTemp;
                }

                // System.Drawing.Image imgOut =

                //      loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,

                //                              null,IntPtr.Zero);



                // *** This code creates cleaner (though bigger) thumbnails and properly

                // *** and handles GIF files better by generating a white background for

                // *** transparent images (as opposed to black)

                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);

                var g = Graphics.FromImage(bmpOut);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);

                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                loBMP.Dispose();
            }
            catch
            {
                return null;
            }
            return bmpOut;

        }

        public static Image Crop(Image imgPhoto, int width, int height, AnchorPosition anchor)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;

            float nPercent;

            var nPercentW = ((float)width / (float)sourceWidth);
            var nPercentH = ((float)height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int)
                                (height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = (int)
                                ((height - (sourceHeight * nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = (int)
                                (width - (sourceWidth * nPercent));
                        break;
                    default:
                        destX = (int)
                                ((width - (sourceWidth * nPercent)) / 2);
                        break;
                }
            }

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            using (var ia = new ImageAttributes())
            {
                ia.SetWrapMode(WrapMode.TileFlipXY);

                var bmPhoto = new Bitmap(width,
                                         height, imgPhoto.PixelFormat);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                      imgPhoto.VerticalResolution);

                var grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;
                grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

                //grPhoto.DrawImage(imgPhoto,
                //    new Rectangle(destX, sourceHeight > sourceWidth ? 0 : destY, destWidth, destHeight),
                //    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                //    GraphicsUnit.Pixel);
                grPhoto.DrawImage(imgPhoto, new Rectangle(new Point(destX, sourceHeight > sourceWidth ? 0 : destY), new Size(destWidth, destHeight)), sourceX, sourceY, sourceWidth, sourceHeight, GraphicsUnit.Pixel, ia);

                grPhoto.Dispose();
                return bmPhoto;
            }
        }

        public static Image ScaleByPercent(Image imgPhoto, int percent)
        {
            var nPercent = ((float)percent / 100);

            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;

            var destX = 0;
            var destY = 0;
            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            var grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth, destHeight),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image ConstrainProportions(Image imgPhoto, int size, Dimensions dimension)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;
            float nPercent = 0;

            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = ((float)size / (float)sourceWidth);
                    break;
                default:
                    nPercent = ((float)size / (float)sourceHeight);
                    break;
            }

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            var grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth, destHeight),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image FixedSize(Image imgPhoto, int width, int height)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;

            float nPercent;

            var nPercentW = ((float)width / (float)sourceWidth);
            var nPercentH = ((float)height / (float)sourceHeight);

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int)((width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int)((height - (sourceHeight * nPercent)) / 2);
            }

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            var grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                              new Rectangle(destX, destY, destWidth, destHeight),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image CreateImageFromBase64(string data)
        {          
            byte[] bytes = Convert.FromBase64String(data);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        public static void SaveImageFromBase64(string data, string imagePath)
        {
            var img = CreateImageFromBase64(data);
            img.Save(imagePath);
        }

        public static string GetImageBase64(string fullPath, string prefix = "")
        {
            using (Image image = Image.FromFile(fullPath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return prefix + base64String;
                }
            }
        }
    }
}