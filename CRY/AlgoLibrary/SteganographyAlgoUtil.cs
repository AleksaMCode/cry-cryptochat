using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRY.AlgoLibrary
{
    public class SteganographyAlgoUtil
    {
        /// <summary>
        /// Each bit of msg has 8b of data to hide in a image.
        /// </summary>
        /// <param name="encMessagePixelNumber"></param>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static bool CanItFit(int encMessagePixelNumber, Bitmap bmp)
        {
            if (bmp.Height * bmp.Width * 3 > encMessagePixelNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static byte[] Combine(byte[] encMsgLen, byte[] encMsg)
        {
            byte[] combined = new byte[encMsgLen.Length + encMsg.Length];
            Buffer.BlockCopy(encMsgLen, 0, combined, 0, encMsgLen.Length);
            Buffer.BlockCopy(encMsg, 0, combined, encMsgLen.Length, encMsg.Length);
            return combined;
        }

        public static byte[] RgbComponentsToBytes(Image img)
        {
            Bitmap innocuousBmp = new Bitmap(img);
            int counter = 0;
            byte[] components = new byte[3 * innocuousBmp.Width * innocuousBmp.Height];
            for (int y = 0; y < innocuousBmp.Height; y++)
            {
                for (int x = 0; x < innocuousBmp.Width; x++)
                {
                    Color c = innocuousBmp.GetPixel(x, y);
                    components[counter++] = c.R;
                    components[counter++] = c.G;
                    components[counter++] = c.B;
                }
            }
            return components;
        }

        public static Bitmap ByteArrayToBitmap(byte[] rgbComponents, int width, int hight)
        {
            Queue<byte> rgbComponentQueue = new Queue<byte>(rgbComponents);
            Bitmap bitmap = new Bitmap(width, hight);
            for (int y = 0; y < hight; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(rgbComponentQueue.Dequeue(), rgbComponentQueue.Dequeue(), rgbComponentQueue.Dequeue()));
                }
            }
            return bitmap;
        }
    }
}
