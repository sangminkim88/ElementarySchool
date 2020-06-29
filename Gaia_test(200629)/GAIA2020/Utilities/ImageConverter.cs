namespace GAIA2020.Utilities
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Defines the <see cref="ImageConverter" />.
    /// </summary>
    public static class ImageConverter
    {
        #region Methods

        /// <summary>
        /// Bitmap을 BitmapImage로 변환 한다.
        /// </summary>
        /// <param name="bitmap">.</param>
        /// <param name="imgFormat">.</param>
        /// <param name="imgSize">.</param>
        /// <returns>.</returns>
        public static BitmapImage BitMapToBitmapImage(Bitmap bitmap, ImageFormat imgFormat, Size imgSize = new Size())
        {
            Bitmap objBitmap;
            BitmapImage bitmapImage = new BitmapImage();
            using (var ms = new System.IO.MemoryStream())
            {
                if (imgSize.Width > 0 && imgSize.Height > 0)
                    objBitmap = new Bitmap(bitmap, new Size(imgSize.Width, imgSize.Height));
                else
                    objBitmap = new Bitmap(bitmap, new Size(bitmap.Width, bitmap.Height));
                objBitmap.Save(ms, imgFormat);

                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.DecodePixelWidth = imgSize.Width;
                bitmapImage.DecodePixelHeight = imgSize.Height;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        /// <summary>
        /// 바이너리 데이터를 BitmapImage로 변환한다.
        /// </summary>
        /// <param name="imageData">.</param>
        /// <returns>.</returns>
        public static BitmapImage ByteArrToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new System.IO.MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        #endregion
    }
}
