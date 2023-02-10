using UnityEngine;
using UnityEngine.UI;
using ZXing;

namespace Qarth
{
    ///扫描二维码（）=》识别二维码信息
    public class QRCodeScanDemo : MonoBehaviour
    {
        BarcodeReader BarcodeReader;//库文件的对象（二维码信息保存的位置）

        private bool isScanning = false;//扫描开关
        private float interval = 3;//扫描识别时间间隔
        private WebCamTexture WebCamTexture;//摄像机映射纹理
        private Color32[] data;//让信息以像素点的形式 按照数据存放

        public Button Sacnning;//扫描Button
        public RawImage CameraTexture;//摄像机映射显示区域
        public Text text;//用来显示扫描的信息


        private void Start()
        {
            Sacnning.onClick.AddListener(SacnningButton);
        }

        private void Update()
        {
            if (isScanning)//每三秒扫描一次
            {
                interval += Time.deltaTime;
                if (interval >= 3)
                {
                    interval = 0;
                    SacnQRCode();//开始扫描
                }
            }
        }




        /// <summary>
        /// 开启摄像机 前期准备工作
        /// </summary>
        void DeviceInit()
        {
            WebCamDevice[] devices = WebCamTexture.devices;//获取所有摄像机的硬件 比如前置 后置

            WebCamTexture = new WebCamTexture(devices[0].name, 300, 300);//创建一个摄像机显示的区域 （device[0]一般是后置摄像头，400,300为大小）

            CameraTexture.texture = WebCamTexture;//显示图片信息

            WebCamTexture.Play();//打开摄像机进行识别

            BarcodeReader = new BarcodeReader();//实例化二维码信息，并存储对象

        }


        /// <summary>
        /// 识别二维码信息
        /// </summary>
        void SacnQRCode()
        {
            data = WebCamTexture.GetPixels32();//获取摄像机中的像素点数组的信息

            Result result = BarcodeReader.Decode(data, WebCamTexture.width, WebCamTexture.height);//获取二维码上的信息

            if (result != null)//判断是否有信息 有则识别成功
            {
                text.text = result.Text;//显示 二维码上的信息

                isScanning = false;//关闭识别

                WebCamTexture.Stop();//停止识别

                Sacnning.onClick.AddListener(SacnningButton);
            }

        }


        /// <summary>
        /// Sacnning按钮
        /// </summary>
        void SacnningButton()
        {
            Sacnning.onClick.RemoveListener(SacnningButton);

            DeviceInit();//开始摄像机

            isScanning = true;//打开扫描开关

            text.text = null;//清空文本

        }


    }
}
