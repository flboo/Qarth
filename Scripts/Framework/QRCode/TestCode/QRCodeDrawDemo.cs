using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace Qarth
{
    //生成二维码
    public class QRCodeDrawDemo : MonoBehaviour
    {
        public Texture2D m_TextTest;
        public RawImage QRCode;//绘制好的二维码
        public Button DrawButton;//生成按钮
        public string QRCodeText = null;//二维码内容，自己填 
        BarcodeWriter BarcodeWriter;//二维码绘制类

        private void Start()
        {
            DrawButton.onClick.AddListener(() => DrowQRCode(QRCodeText));
        }


        /// <summary>
        /// 将制定字符串信息转换成二维码图片信息
        /// </summary>
        /// <param name="formatStr">要产生二维码的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns></returns>
        Color32[] GeneQRCode(string formatStr, int width, int height)
        {
            ZXing.QrCode.QrCodeEncodingOptions options = new ZXing.QrCode.QrCodeEncodingOptions();//绘制二维码之前 进行设置

            options.CharacterSet = "UTF-8";//设置字符编码，确保字符串信息保持正确

            options.Width = width;//设置二维码宽
            options.Height = height;//设置二维码高
            options.Margin = 1;//设置二维码留白 （值越大，留白越大，二维码越小）

            BarcodeWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = options };//实例化字符串绘制二维码工具

            return BarcodeWriter.Write(formatStr);
        }


        /// <summary>
        /// 根据二维码图片信息绘制制定字符串信息的二维码到指定区域
        /// </summary>
        /// <param name="str">字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns></returns>
        Texture2D ShowQRCode(string str, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            Color32[] colors = GeneQRCode(str, width, height);

            texture.SetPixels32(colors);

            texture.Apply();

            return texture;
        }


        /// <summary>
        /// 绘制二维码
        /// </summary>
        /// <param name="formatStr">二维码信息</param>
        void DrowQRCode(string formatStr)
        {
            Texture2D texture = new Texture2D((int)QRCode.rectTransform.rect.width, (int)QRCode.rectTransform.rect.height, TextureFormat.RGB24, false);
            for (int x = 0; x < m_TextTest.width; x++)
            {
                for (int y = 0; y < m_TextTest.height; y++)
                {
                    texture.SetPixel(x + 10, y + texture.height - m_TextTest.height - 10, m_TextTest.GetPixel(x, y));
                }
            }
            texture.Apply();

            Texture2D textureQrCode = ShowQRCode(formatStr, 256, 256);
            for (int x = 0; x < textureQrCode.width; x++)
            {
                for (int y = 0; y < textureQrCode.height; y++)
                {
                    texture.SetPixel(x + texture.width - 256 - 10, y + 10, textureQrCode.GetPixel(x, y));
                }
            }
            texture.Apply();


            //Texture2D texture = ShowQRCode(formatStr, 256, 256);//注意：这个宽高度大小256不要变。不然生成的信息不正确
            //256有可能是这个ZXingNet插件指定大小的绘制像素点数值
            QRCode.texture = texture;//显示到UI界面的图片上
        }



    }
}