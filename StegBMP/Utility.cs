using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("StegBMPUnitTest")]
namespace StegBMP
{
    internal class Utility
    {
        #region Constructor

        #endregion

        #region Internal Method

        /// <summary>
        /// 配列 a の idx 番目の要素を削除し，後続の要素を前に詰める。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="idx"></param>
        internal void StringArrayRemoveAt(ref string[] a, int idx)
        {
            if (null == a)
            {
                throw new ArgumentNullException("a == null @ Utility.ArrayRemoveAt");
            }

            if ((idx < 0) || ((a.Length - 1) < idx))
            {
                throw new ArgumentOutOfRangeException("idx is out of range @ Utility.ArrayRemoveAt");
            }

            if (1 == a.Length)
            {
                a = null;
                return;
            }

            Array.Copy(
                a, idx + 1,
                a, idx,
                a.Length - idx - 1
                );

            Array.Resize<string>(ref a, a.Length - 1);
        }

        internal Bitmap ImageToBitmap(string path)
        {
            if (null == path)
            {
                throw new ArgumentNullException("path");
            }

            string extention = Path.GetExtension(path);
            if (null == extention)
            {
                throw new InvalidExtentionException("extention == null @ Utility.ImageToBitmap");
            }

            if (
                (".bmp" != extention) && (".BMP" != extention) &&
                (".png" != extention) && (".PNG" != extention) &&
                (".tiff" != extention) && (".TIFF" != extention) &&
                (".jpg" != extention) && (".JPG" != extention)
                )
            {
                throw new InvalidExtentionException("invalid extention @ Utility.ImageToBitmap");
            }

            Bitmap bitmap = new Bitmap(path);
            Bitmap formatedBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(formatedBitmap);
            graphics.DrawImage(bitmap, new Point(0, 0));
            graphics.Dispose();

            return formatedBitmap;
        }

        #endregion
    }
}
