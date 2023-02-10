using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;

namespace Qarth
{
    public class QRCodeHelper
    {
        private static BarcodeWriter m_BarcodeWriter;

        public enum QRCodeNestPosEnum
        {
            LeftBottom,
            RightBottom,
        }

        // 单独生成二维码贴图
        public static Texture2D GenQRCodeTexture(string formatStr, int width = 256, int height = 256, ZXing.QrCode.QrCodeEncodingOptions options = null)
        {
            if (width < 256 || height < 256)
            {
                Log.e("二维码分辨率不可低于256");
                return null;
            }

            if (m_BarcodeWriter == null)
            {
                m_BarcodeWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            }

            if (options == null)
            {
                options = new ZXing.QrCode.QrCodeEncodingOptions();
                options.CharacterSet = "UTF-8";
                options.Width = width;
                options.Height = height;
                options.Margin = 1;//二维码留白 （值越大，留白越大，二维码越小）
            }
            m_BarcodeWriter.Options = options;

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels32(m_BarcodeWriter.Write(formatStr));

            texture.Apply();

            return texture;
        }

        // 根据原始图片生成二维码嵌套图
        public static Texture2D GenNestedQRCodeTexture(Texture2D srcTex, string formatStr, QRCodeNestPosEnum posEnum, int offsetX = 10, int offsetY = 10, int width = 256, int height = 256, ZXing.QrCode.QrCodeEncodingOptions options = null)
        {
            Texture2D texture = new Texture2D(srcTex.width, srcTex.height, TextureFormat.RGB24, false);
            for (int x = 0; x < srcTex.width; x++)
            {
                for (int y = 0; y < srcTex.height; y++)
                {
                    texture.SetPixel(x, y, srcTex.GetPixel(x, y));
                }
            }
            texture.Apply();
            Texture2D textureQrCode = GenQRCodeTexture(formatStr, width, height, options);

            switch (posEnum)
            {
                case QRCodeNestPosEnum.LeftBottom:
                    for (int x = 0; x < textureQrCode.width; x++)
                    {
                        for (int y = 0; y < textureQrCode.height; y++)
                        {
                            texture.SetPixel(x + offsetX, y + offsetY, textureQrCode.GetPixel(x, y));
                        }
                    }
                    break;
                case QRCodeNestPosEnum.RightBottom:
                    for (int x = 0; x < textureQrCode.width; x++)
                    {
                        for (int y = 0; y < textureQrCode.height; y++)
                        {
                            texture.SetPixel(x + texture.width - width - offsetX, y + offsetY, textureQrCode.GetPixel(x, y));
                        }
                    }
                    break;
            }

            texture.Apply();

            return texture;
        }
    }

}