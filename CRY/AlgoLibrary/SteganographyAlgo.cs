using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace CRY.AlgoLibrary
{
    /// <summary>
    /// Image is represented as an NxM for grayscale or NxMx3 matrix in memory, in case of color images. Each entry is value of a pixel.
    /// The pixels can be decomposed into three primary component colors, Red, Green, Blue, and the transparency, Alpha.
    /// If you use Alpha, matrix size should be NxMx4.
    /// </summary>
    public class SteganographyAlgo
    {
        public static void Encode(byte[] encMsg, string inputLocation, string outputLocation)
        {
            // Loading the data we want to hide to a byte array
            byte[] hiddenLengthBytes = BitConverter.GetBytes(encMsg.Length);
            // We need to hide the encripted msg length + the enc. msg ; enc. msg = enc.msg.len + enc.msg. + rsa signature
            byte[] hiddenCombinedBytes = SteganographyAlgoUtil.Combine(hiddenLengthBytes, encMsg);

            // Loading an image we want to store the hidden data in to a byte array
            Image hideYourDataInMe = Image.FromFile(inputLocation);
            byte[] rgbComponents = SteganographyAlgoUtil.RgbComponentsToBytes(hideYourDataInMe);

            // Check if the encrypted message can be hidden in the image
            if (SteganographyAlgoUtil.CanItFit(encMsg.Length * 8, new Bitmap(hideYourDataInMe)) == false)
            {
                throw new CryptedMessageParser.Exceptions.SteganographyException("Encrypted Message can't be embedded into an image " + inputLocation);
            }

            // Encoding the hidden data into the image, and storing it to file
            byte[] encodedRgbComponents = EncodeBytes(hiddenCombinedBytes, rgbComponents);
            Bitmap encodedBmp = SteganographyAlgoUtil.ByteArrayToBitmap(encodedRgbComponents, hideYourDataInMe.Width, hideYourDataInMe.Height);
            encodedBmp.Save(outputLocation, ImageFormat.Png);
        }

        /// <summary>
        /// Changing the value of one bit in a color byte will result in a small color change.
        /// </summary>
        /// <param name="hiddenBytes"></param>
        /// <param name="rgbComponents"></param>
        /// <returns></returns>
        private static byte[] EncodeBytes(byte[] hiddenBytes, byte[] rgbComponents)
        {
            BitArray hiddenBits = new BitArray(hiddenBytes);
            byte[] encodedBitmapRgbComponents = new byte[rgbComponents.Length];
            for (int i = 0; i < rgbComponents.Length; i++)
            {
                if (i < hiddenBits.Length)
                {
                    byte evenByte = (byte)(rgbComponents[i] - rgbComponents[i] % 2);
                    encodedBitmapRgbComponents[i] = (byte)(evenByte + (hiddenBits[i] ? 1 : 0));
                }
                else
                {
                    encodedBitmapRgbComponents[i] = rgbComponents[i];
                }
            }
            return encodedBitmapRgbComponents;
        }

        public static byte[] Decode(string imgLocation)
        {
            Bitmap loadedEncodedBmp = new Bitmap(imgLocation);
            byte[] loadedEncodedRgbComponents = SteganographyAlgoUtil.RgbComponentsToBytes(loadedEncodedBmp);

            byte[] loadedHiddenLengthBytes = DecodeBytes(loadedEncodedRgbComponents, 0, 4); // 4 - len of msg
            int loadedHiddenLength = BitConverter.ToInt32(loadedHiddenLengthBytes, 0);
            byte[] loadedHiddenBytes = DecodeBytes(loadedEncodedRgbComponents, 4, loadedHiddenLength);
            return loadedHiddenBytes;
        }

        private static byte[] DecodeBytes(byte[] encMsg, int byteIndex, int byteCount)
        {
            int bitCount = byteCount * 8;
            int bitIndex = byteIndex * 8;
            bool[] loadedHiddenBools = new bool[bitCount];
            for (int i = 0; i < bitCount; i++)
            {
                loadedHiddenBools[i] = encMsg[i + bitIndex] % 2 == 1;
            }
            BitArray loadedHiddenBits = new BitArray(loadedHiddenBools);
            byte[] loadedHiddenBytes = new byte[loadedHiddenBits.Length / 8];
            loadedHiddenBits.CopyTo(loadedHiddenBytes, 0);
            return loadedHiddenBytes;
        }
    }
}