using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qarth
{
    public class BasePlatformPanel : MonoBehaviour
    {
        protected WWW w;
        public Image bg;

        void Start()
        {
            OnPanelInit();
        }

        protected virtual void OnPanelInit()
        {
            string url = "file://" + Application.streamingAssetsPath + "/platform_logo.png";

#if UNITY_ANDROID && !UNITY_EDITOR
	        url = Application.streamingAssetsPath + "/platform_logo.png";
#endif

            StartCoroutine(LoadImg());
            w = new WWW(url);
        }

        IEnumerator LoadImg()
        {
            yield return w;
            Texture2D tex = w.texture;
            bg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }
    }

}