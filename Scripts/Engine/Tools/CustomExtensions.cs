using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using GameWish.Game;
using Object = UnityEngine.Object;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Qarth
{
    public static class CustomExtensions
    {
        #region helper
        public static void ShowToast(string str)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"); ;
            AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", str);
                Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT")).Call("show");
            }
            ));
#else
            FloatMessage.S.ShowMsg(str);
#endif
        }

        public static void ShuffleList<T>(this List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static List<T> GetRandomList<T>(List<T> inputList)
        {
            //Set outputList and random
            List<T> outputList = new List<T>();

            while (inputList.Count > 0)
            {
                //Select an index and item
                int rdIndex = RandomHelper.Range(0, inputList.Count);
                T remove = inputList[rdIndex];

                //remove it from copyList and add it to output
                inputList.Remove(remove);
                outputList.Add(remove);
            }
            return outputList;
        }

        public static T CreateHandler<T>(string className) where T : class
        {
            if (string.IsNullOrEmpty(className))
            {
                return null;
            }

            Type type = Type.GetType(className);
            if (type == null)
            {
                Debug.LogError("Not Find Handler Class:" + className);
                return null;
            }

            try
            {
                object obj = Activator.CreateInstance(type, true);

                return obj as T;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return null;
        }

        public static void DoBuzz(long milliseconds = 100)
        {
            if (PlayerPrefs.GetInt(EngineDefine.BUZZ_STATE, 1) > 0 && CustomVibration.HasVibrator())
            {
                CustomVibration.Vibrate(milliseconds);
            }
        }

        public static T GetStringEnum<T>(string val)
        {
            return (T)Enum.Parse(typeof(T), val);
        }

        public static Coroutine CallWithDelay(this MonoBehaviour obj, System.Action call, float delay)
        {
            return obj.StartCoroutine(doCallWithDelay(call, delay));
        }

        static IEnumerator doCallWithDelay(System.Action call, float delay)
        {
            if (delay <= 0)
                yield return null;
            else
            {
                float start = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup < start + delay)
                {
                    yield return null;
                }
            }

            if (call != null)
                call.Invoke();
        }

        static public T AddMissingComponent<T>(this GameObject go) where T : Component
        {
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
            T comp = go.GetComponent<T>();
#endif
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }
#if UNITY_FLASH
		return (T)comp;
#else
            return comp;
#endif
        }

        public static Toggle GetActive(this ToggleGroup aGroup)
        {
            return aGroup.ActiveToggles().FirstOrDefault();
        }

        public static string TrimHeadChar(string str, string item)
        {
            if (str.Substring(0, 1) == item)
            {
                return TrimHeadChar(str.Substring(1, str.Length - 1), item);
            }
            else
                return str;
        }
        #endregion


        #region transform
        // 归零
        static public void ResetTrans(this Transform trans, float scaleRate = 1, bool local = true)
        {
            trans.localPosition = Vector3.zero;
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one * scaleRate;
        }

        static public void SetLocalPos(this GameObject obj, Vector3 pos)
        {
            obj.transform.localPosition = pos;
        }
        static public void SetPos(this GameObject obj, Vector3 pos)
        {
            obj.transform.position = pos;
        }
        static public void SetAngle(this GameObject obj, Vector3 angle)
        {
            obj.transform.localEulerAngles = angle;
        }
        /// 设置X，只改变X c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetX(this Transform trans, float x)
        {
            trans.position = new Vector3(x, trans.position.y, trans.position.z);
        }
        /// <summary>
        /// 设置Y，只改变Y c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetY(this Transform trans, float y)
        {
            trans.position = new Vector3(trans.position.x, y, trans.position.z);
        }
        /// <summary>
        /// 设置Z，只改变Z c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetZ(this Transform trans, float z)
        {
            trans.position = new Vector3(trans.position.x, trans.position.y, z);
        }
        /// <summary>
        /// 设置LocalX，只改变LocalX c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetLocalX(this Transform trans, float x)
        {
            trans.localPosition = new Vector3(x, trans.localPosition.y, trans.localPosition.z);
        }
        /// <summary>
        /// 设置LocalY，只改变LocalY c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetLocalY(this Transform trans, float y)
        {
            trans.localPosition = new Vector3(trans.localPosition.x, y, trans.localPosition.z);
        }
        /// <summary>
        /// 设置LocalZ，只改变LocalZ c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetLocalZ(this Transform trans, float z)
        {
            trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
        }
        /// <summary>
        /// 在游戏中，朝向的时候只朝向某个坐标，不低头 c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="target"></param>
        static public void LookAtXZ(this Transform trans, Vector3 target)
        {
            trans.LookAt(new Vector3(target.x, trans.position.y, target.z));
        }
        /// <summary>
        /// 遍历go c
        /// </summary>
        /// <param name="go"></param>
        /// <param name="handle"></param>
        static public void IterateGameObject(this GameObject go, Action<GameObject> handle)
        {
            Queue q = new Queue();
            q.Enqueue(go);
            while (q.Count != 0)
            {
                GameObject tmpGo = (GameObject)q.Dequeue();
                foreach (Transform t in tmpGo.transform)
                {
                    q.Enqueue(t.gameObject);
                }
                if (handle != null)
                {
                    handle(tmpGo);
                }
            }
        }
        /// <summary>
        /// 设置go层级关系 c
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        static public void SetAllLayer(this GameObject go, int layer)
        {
            IterateGameObject(go, (g) =>
            {
                g.layer = layer;
            });
        }
        /// <summary>
        /// 重置某个物体的三围等 c
        /// </summary>
        /// <param name="go"></param>
        static public void ResetTrs(this GameObject go)
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        static public void ResetLocalAngle(this GameObject go)
        {
            go.transform.localEulerAngles = Vector3.zero;
        }
        static public void ResetLocalPos(this GameObject go)
        {
            go.transform.localPosition = Vector3.zero;
        }

        static public List<Transform> GetChildTrsList(this Transform trsRoot)
        {
            List<Transform> parts = new List<Transform>();
            for (int i = 0; i < trsRoot.childCount; i++)
                parts.Add(trsRoot.GetChild(i));
            return parts;
        }

        public static float Pixel2DP(float pixel)
        {
            return pixel * 160 / Screen.dpi;
        }

        public static float DP2Pixel(float dp)
        {
            return dp * (Screen.dpi / 160);
        }
        #endregion

        #region physics
        static public void SetRigidBodiesKinematic(this GameObject obj, bool state)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].gameObject.tag == "StayKinematic")
                    bodies[i].isKinematic = true;
                else
                    bodies[i].isKinematic = state;
            }
        }
        static public void SetColliderEnable(this GameObject obj, bool state)
        {
            var cols = obj.GetComponentsInChildren<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = state;
            }
        }

        static public void AddRigidBodiesForce(this GameObject obj, Vector3 force, ForceMode mode = ForceMode.Force)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
                bodies[i].AddForce(force, mode);
        }
        static public void SetRigidBodiesDrag(this GameObject obj, float dragVal = 0)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
                bodies[i].drag = dragVal;
        }
        static public void SetRigidBodiesAngularDrag(this GameObject obj, float dragVal = 0.05f)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
                bodies[i].angularDrag = dragVal;
        }
        #endregion


        #region render
        public static Color HexToColor(string hex, bool alpha = false)
        {
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            if (alpha)
            {
                byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                float a = cc / 255f;
                return new Color(r, g, b, a);
            }
            else
                return new Color(r, g, b);
        }

        public static Texture2D toTexture2D(this RenderTexture rTex, int size = 256)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }

        static public void SampleAnim(this Animator animator, string stateName, float normalizedTime, int layer = -1)
        {
            animator.StopPlayback();
            animator.Play(stateName, layer, normalizedTime);
            animator.StartPlayback();
        }
        static public void SampleAnim(this Animation anim, string stateName, float normalizedTime)
        {
            anim.Play(stateName);
            anim[stateName].speed = 1;
            anim[stateName].normalizedTime = normalizedTime;
            anim[stateName].speed = 0;
        }
        static public void PlayAnim(this Animation anim, string stateName)
        {
            anim.Play(stateName);
            anim[stateName].speed = 1;
        }

        //获取网格的size
        static public Vector3 GetMeshSize(this GameObject objRoot)
        {
            var mesh = objRoot.GetComponent<Renderer>();
            return mesh == null ? Vector3.zero : mesh.bounds.size;
        }
        //获取网格的中心
        static public Vector3 GetMeshCenter(this GameObject objRoot)
        {
            Vector3 boundsCenter = Vector3.zero;
            var meshes = objRoot.GetComponentsInChildren<Renderer>();
            for (int j = 0; j < meshes.Length; j++)
            {
                boundsCenter += meshes[j].bounds.center;
            }
            boundsCenter /= meshes.Length * 1.0f;
            return boundsCenter;
        }

        private static SpritesHandler m_GlobalSprHandler = new SpritesHandler();
        public static Sprite FindSprite(ResLoader loader, string spriteName, string assetName = "", int pixelPerUnit = 100)
        {
            Sprite result = null;
            Object obj = loader.LoadSync(spriteName) as Object;

            Texture2D text = obj as Texture2D;
            if (text != null)
            {
                result = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(0.5f, 0.5f), pixelPerUnit);
            }

            if (result == null)
            {
                var data = loader.LoadSync(assetName) as SpritesData;
                if (data != null)
                {
                    m_GlobalSprHandler.SetData(new SpritesData[] { data });
                    return m_GlobalSprHandler.FindSprite(spriteName) as Sprite;
                }
            }
            return result;
        }
        #endregion

        #region time
        private static long Jan1st1970Ms = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local).Ticks;

        public static long CurrentTimeMillis()
        {
            return (System.DateTime.UtcNow.Ticks - Jan1st1970Ms) / 10000;
        }
        public static long CurrentTimeMillis(long ticks)
        {
            return (ticks - Jan1st1970Ms) / 10000;
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string GetTimeStampUni()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string GetTimeStampUniSec()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


        public static DateTime GetTimeFromTimestamp(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtStart.AddMilliseconds(long.Parse(timestamp));
        }

        public static long GetSecFromTimestamps(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtLast = dtStart.AddMilliseconds(long.Parse(timestamp));
            return (long)(DateTime.Now - dtLast).TotalSeconds;
        }

        public static string GetTimeStrToTimestamps(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtEnd = dtStart.AddMilliseconds(long.Parse(timestamp));
            var ts = dtEnd - DateTime.Now;
            return ts.Minutes + ":" + ts.Seconds;
        }

        public static string GetTimeStrToDayEnd(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtnow = DateTime.Now;
            DateTime dtend = new DateTime(dtnow.Year, dtnow.Month, dtnow.Day + 1, 0, 0, 0, 0);
            //var dtNow = dtStart.AddMilliseconds(long.Parse(timestamp));
            var ts = dtend - DateTime.Now;
            return ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds;
        }

        public static long GetSecToTimestamps(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtEnd = dtStart.AddMilliseconds(long.Parse(timestamp));
            return (long)(dtEnd - DateTime.Now).TotalSeconds;
        }

        public static string GetTimeStampAfterSec(string timestamp, int sec)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtLast = dtStart.AddMilliseconds(long.Parse(timestamp)).AddSeconds(sec);
            return Convert.ToInt64((dtLast - dtStart).TotalMilliseconds).ToString();
        }
        public static string GetTimeStamp(DateTime dt)
        {
            TimeSpan ts = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static void ProcessFuncRunTime(string tag, Action run)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start(); //  开始监视代码运行时间
            run.Invoke();
            stopwatch.Stop(); //  停止监视

            //  获取当前实例测量得出的总时间
            System.TimeSpan timespan = stopwatch.Elapsed;
            //   double hours = timespan.TotalHours; // 总小时
            //    double minutes = timespan.TotalMinutes;  // 总分钟
            //    double seconds = timespan.TotalSeconds;  //  总秒数
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数

            //打印代码执行时间
            Debug.Log(string.Format("{0} 运行用时 : {1} ", tag, milliseconds));
        }

        public static bool CheckIsNewDayByTimeStamp(string timeStamp)
        {
            DateTime lastSignDate;
            if (!string.IsNullOrEmpty(timeStamp))
            {
                lastSignDate = GetTimeFromTimestamp(timeStamp);
                //if ()
                {
                    DateTime today = DateTime.Today;

                    if (lastSignDate.DayOfYear != DateTime.Now.DayOfYear)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
        }

        public static int GetPassDayByTimeStamp(string timeStamp)
        {
            DateTime lastSignDate;
            if (!string.IsNullOrEmpty(timeStamp))
            {
                lastSignDate = GetTimeFromTimestamp(timeStamp);
                //if ()
                {
                    DateTime today = DateTime.Today;

                    if (lastSignDate.DayOfYear != DateTime.Now.DayOfYear)
                    {
                        return today.DayOfYear - lastSignDate.DayOfYear;
                    }
                    else
                        return 0;
                }
            }

            return -1;
        }

        public static int CheckIsNewDay(string dayKey = "lastsignstr--12354371")
        {
            string timeString = PlayerPrefs.GetString(dayKey, "");
            DateTime lastSignDate;
            if (!string.IsNullOrEmpty(timeString))
            {
                if (DateTime.TryParse(timeString, out lastSignDate))
                {
                    DateTime today = DateTime.Today;
                    TimeSpan pass = today - lastSignDate;

                    if (pass.TotalDays >= 1)
                    {
                        PlayerPrefs.SetString(dayKey, DateTime.Today.ToShortDateString());
                    }
                    return pass.Days;
                }
                else
                {
                    PlayerPrefs.SetString(dayKey, DateTime.Today.ToShortDateString());
                    return -1;
                }
            }
            else
            {
                PlayerPrefs.SetString(dayKey, DateTime.Today.ToShortDateString());
                return 999999;
            }
        }
        #endregion

        #region screen_pos
        public static void ScreenPosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScreen, Transform uiTarget)
        {
            Vector3 viewportPos = sceneCamera.ScreenToViewportPoint(posInScreen);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            uiTarget.position = worldPos;
            Vector3 localPos = uiTarget.localPosition;
            localPos.z = 0f;
            uiTarget.localPosition = localPos;
        }
        public static Vector3 GetScreenPosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScreen, Transform uiTarget)
        {
            Vector3 originPos = uiTarget.position;
            Vector3 viewportPos = sceneCamera.ScreenToViewportPoint(posInScreen);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            uiTarget.position = worldPos;
            Vector3 localPos = uiTarget.localPosition;
            localPos.z = 0f;

            uiTarget.position = originPos;
            return localPos;
        }
        public static void ScenePosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScene, Transform uiTarget)
        {
            Vector3 viewportPos = sceneCamera.WorldToViewportPoint(posInScene);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            uiTarget.position = worldPos;
            Vector3 localPos = uiTarget.localPosition;
            localPos.z = 0f;
            uiTarget.localPosition = localPos;
        }
        public static Vector3 GetScenePosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScene, Transform uiTarget)
        {
            Vector3 originPos = uiTarget.position;
            Vector3 viewportPos = sceneCamera.WorldToViewportPoint(posInScene);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            uiTarget.position = worldPos;
            Vector3 localPos = uiTarget.localPosition;
            localPos.z = 0f;

            uiTarget.position = originPos;
            return localPos;
        }

        public static Vector3 UIPosToScenePos(Camera sceneCamera, Camera uiCamera, Vector3 uiPos)
        {
            Vector3 viewPos = uiCamera.WorldToViewportPoint(uiPos);
            Vector3 worldPos = sceneCamera.ViewportToWorldPoint(viewPos);
            worldPos.z = 0;
            return worldPos;
        }
        #endregion

        #region sdk
        //普通广告准备判断
        public static bool IsAdReady(string adInterface)
        {
#if UNITY_EDITOR
            FloatMessage.S.ShowMsg("UNITY_EDITOR: 广告准备好了");
            return true;
#endif
            if (AdsMgr.S.GetAdInterface(adInterface) != null && AdsMgr.S.GetAdInterface(adInterface).isAdReady)
            {
                return true;
            }

            return false;
        }

        //信息流准备判断
        public static bool IsMixViewAdReady(string adPlacement)
        {
            var plm = AdsMgr.S.GetAdPlacement(adPlacement);
            var inter = AdsMgr.S.GetAdInterfaceByPlacementID(adPlacement, 0);

            if (plm != null && !plm.IsTimeShowAble())
            {
                return false;
            }

            if (inter != null && inter.isAdReady)
            {
                return true;
            }

            return false;
        }

        private static int m_AdNotShowCount = 0;
        private static Action m_AdShowCommonCallback;

        //设置视频广告通用回调
        public static void SetAdCommonCallback(Action callback)
        {
            m_AdShowCommonCallback = callback;
        }

        //播放视频广告
        public static void PlayAd(string tag, Action<bool> callBack, Action failCallback = null, string inter = "Reward0", string pId = "MainReward")
        {
#if UNITY_EDITOR
            FloatMessage.S.ShowMsg("看了一个广告");
            callBack.Invoke(false);
            if (m_AdShowCommonCallback != null)
                m_AdShowCommonCallback.Invoke();
            return;
#endif
            if (IsAdReady(inter))
            {
                m_AdNotShowCount = 0;
                AdDisplayer.Builder()
                    .SetOnAdShowResultCallback((isShowSuccess, isRewardSuccess, isClickAd, rewardID, tid, ecpm) =>
                    {
                        if (isShowSuccess && isRewardSuccess)
                        {
                            callBack.Invoke(isClickAd);
                            if (m_AdShowCommonCallback != null)
                                m_AdShowCommonCallback.Invoke();
                            DataAnalysisMgr.S.SatoriEvt_ADReward(tag);
                        }
                        else
                        {
                            if (failCallback != null)
                                failCallback.Invoke();
                        }
                    })
                    .SetForceShow(true)
                    .SetPlacementID(pId)
                    .Show(tag);
            }
            else
            {
                m_AdNotShowCount++;
                if (pId == "MainReward")
                {
                    if (m_AdNotShowCount >= 15)
                    {
                        FloatMessage.S.ShowMsg("正在加载视频，请稍后再试");
                    }
                    else
                    {
                        FloatMessage.S.ShowMsg("正在加载视频，请稍后再试");
                    }
                }

                if (failCallback != null)
                    failCallback.Invoke();
            }
        }

        //播放视频广告带tid参数，获取ecpm奖励
        public static void PlayAd(string tag, Action<bool, string, float> callBack, Action failCallback = null, string inter = "Reward0", string pId = "MainReward")
        {
#if UNITY_EDITOR
            FloatMessage.S.ShowMsg("看了一个广告");
            callBack.Invoke(true, "", 12);
            if (m_AdShowCommonCallback != null)
                m_AdShowCommonCallback.Invoke();
            return;
#endif
            if (IsAdReady(inter))
            {
                m_AdNotShowCount = 0;
                AdDisplayer.Builder()
                    .SetOnAdShowResultCallback((isShowSuccess, isRewardSuccess, isClickAd, rewardID, tid, ecpm) =>
                    {
                        if (isShowSuccess && isRewardSuccess)
                        {
                            callBack.Invoke(isClickAd, tid, ecpm);
                            if (m_AdShowCommonCallback != null)
                                m_AdShowCommonCallback.Invoke();
                            DataAnalysisMgr.S.SatoriEvt_ADReward(tag);
                        }
                        else
                        {
                            if (failCallback != null)
                                failCallback.Invoke();
                        }
                    })
                    .SetForceShow(true)
                    .SetPlacementID(pId)
                    .Show(tag);
            }
            else
            {
                m_AdNotShowCount++;
                if (m_AdNotShowCount >= 15)
                {
                    FloatMessage.S.ShowMsg("正在加载视频，请稍后再试");
                }
                else
                {
                    FloatMessage.S.ShowMsg("正在加载视频，请稍后再试");
                }

                if (failCallback != null)
                    failCallback.Invoke();
            }
        }

        public static bool IsAfNonOrganic()
        {
#if UNITY_EDITOR
            return true;
#else
            string attr = PlayerPrefs.GetString("AppsFlyerAttr", "");
            return attr.Trim().ToLower().Equals("non-organic");
#endif
        }


        public static bool IsPermissionGranted(string permission)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidPermissionsManager.IsPermissionGranted(permission);
#endif
            return true;
        }

        public static void RequestPermission(
            string permission,
            Action<string> onGrantedCallback,
            Action<string> onDeniedCallback,
            Action<string> onDeniedAndDontAskAgainCallback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!IsPermissionGranted(permission))
            {
                AndroidPermissionsManager.RequestPermission(new[] { permission },
                    new AndroidPermissionCallback(onGrantedCallback, onDeniedCallback, onDeniedAndDontAskAgainCallback));
            }
#endif
        }

        public static void RequestPermission(string permission,
            Action<string> onGrantedCallback,
            Action<string> onDeniedCallback,
            Action<string> onDeniedAndDontAskAgainCallback,
            Action grantedCallback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!IsPermissionGranted(permission))
            {
                AndroidPermissionsManager.RequestPermission(new[] { permission },
                    new AndroidPermissionCallback(onGrantedCallback, onDeniedCallback, onDeniedAndDontAskAgainCallback));
            }
            else
            {
                grantedCallback?.Invoke();
            }
#endif
        }

        public static void RequestPermissions(
            string[] permissions,
            Action<string> onGrantedCallback,
            Action<string> onDeniedCallback,
            Action<string> onDeniedAndDontAskAgainCallback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            if (permissions == null)
            {
                onGrantedCallback?.Invoke("");
                return;
            }

            List<string> need = new List<string>();

            for (int i = 0; i < permissions.Length; i++)
            {
                if (!IsPermissionGranted(permissions[i]))
                {
                    need.Add(permissions[i]);
                }
            }

            if (need.Count <= 0)
            {
                onGrantedCallback?.Invoke("");
            }
            else
            {
                AndroidPermissionsManager.RequestPermission(need.ToArray(),
                    new AndroidPermissionCallback(onGrantedCallback, onDeniedCallback,
                        onDeniedAndDontAskAgainCallback));
            }
#endif
        }

        public static void RequestSensitivePermissions(Action grantedCallback = null)
        {
            RequestPermission(AndroidPermissionDefine.READ_PHONE_STATE, (p) => { grantedCallback?.Invoke(); }, (p) => { }, (p) => { });
            RequestPermission(AndroidPermissionDefine.WRITE_EXTERNAL_STORAGE, (p) => { }, (p) => { }, (p) => { });
            RequestPermission(AndroidPermissionDefine.ACCESS_COARSE_LOCATION, (p) => { }, (p) => { }, (p) => { });
        }

        public static string GetSDKChannel()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string channelId = SplitAppHandler.GetChannelId();
            Log.i("channel id: "+channelId);
            if (!string.IsNullOrEmpty(channelId))
            {
                return channelId;
            }

            return TaurusXAdSdk.Api.TaurusXConfigUtil.GetChannel();
#elif UNITY_IOS && !UNITY_EDITOR
            return "ios";
#endif
            return "bytedance";
        }

        #endregion


        #region http
        public static Proyecto26.RequestHelper GetRequestHelper_GetRemoteConf(string appName, string key, string channelId,
            string baseUrl = "http://remoteconf.freeqingnovel.com", Dictionary<string, string> headers = null)
        {
            var path = string.Format("/v1/kv/{0}/{1}/{2}",
                appName, channelId, key);
            return new Proyecto26.RequestHelper
            {
                Uri = baseUrl + path,
                Method = "GET",
                Timeout = 15,
                Retries = 3,
                RetrySecondsDelay = 5,
                ContentType = "application/json",
                Headers = headers,
                CertificateHandler = new BypassCertificate(),
            };
        }

        public static void FetchRemoteConfParams(string appName, string key,
            Action<string> responseCallback,
            Action failedCallback = null,
            string channelId = "all",
            string baseUrl = "http://remoteconf.freeqingnovel.com",
            Dictionary<string, string> headers = null)
        {
            Proyecto26.RestClient.Request(GetRequestHelper_GetRemoteConf(appName, key, channelId, baseUrl, headers))
                .Then(response =>
                {
                    if (response.StatusCode == 200 && string.IsNullOrEmpty(response.Error))
                    {
                        //Debug.Log(response.Text);
                        var data = LitJson.JsonMapper.ToObject<RemoteConfResponse>(response.Text);
                        if (data != null && data.success && !string.IsNullOrEmpty(data.data.v))
                        {
                            responseCallback?.Invoke(data.data.v);
                        }
                        else
                        {
                            failedCallback?.Invoke();
                        }
                    }
                    else
                    {
                        failedCallback?.Invoke();

                        Debug.LogError(response.StatusCode + " >>> " + response.Error);
                    }
                }, reject =>
                {
                    failedCallback?.Invoke();
                    Debug.LogError(reject.Message);
                })
            .Catch(e => Debug.LogError("remote conf :" + e.StackTrace));
        }


        #endregion

        public static Color StringToColor(string colorStr)
        {
            if (string.IsNullOrEmpty(colorStr))
            {
                return new Color();
            }
            int colorInt = int.Parse(colorStr, System.Globalization.NumberStyles.AllowHexSpecifier);
            return IntToColor(colorInt);
        }

        public static Color IntToColor(int colorInt)
        {
            float basenum = 255;

            int b = 0xFF & colorInt;
            int g = 0xFF00 & colorInt;
            g >>= 8;
            int r = 0xFF0000 & colorInt;
            r >>= 16;
            return new Color((float)r / basenum, (float)g / basenum, (float)b / basenum, 1);

        }


        public static string StringForMultiLine(this string target, int lineByteCount)
        {
            string nextLine = "\n";
            StringBuilder sb = new StringBuilder();

            var listStr = target.MySplit(lineByteCount);
            if (listStr.Length <= 1)
            {
                return target;
            }

            for (int i = 0; i < listStr.Length; i++)
            {
                sb.Append(listStr[i]);
                if (i < listStr.Length - 1)
                    sb.Append(nextLine);


            }
            return sb.ToString();
        }

        static string[] MySplit(this string str, int count)
        {
            var list = new List<string>();
            int length = (int)Math.Ceiling((double)str.Length / count);

            for (int i = 0; i < length; i++)
            {
                int start = count * i;
                if (str.Length <= start)
                {
                    break;
                }
                if (str.Length < start + count)
                {
                    list.Add(str.Substring(start));
                }
                else
                {
                    list.Add(str.Substring(start, count));
                }
            }

            return list.ToArray();
        }

        static public string ToStringNormal(this float sum)
        {
            //向下取整// 非四舍五入
            return (((int)(sum * 100)) * 0.01f).ToString("0.##");
        }

        static public string ToStringNormal(this double sum)
        {
            //向下取整  //非四舍五入//  
            return (((int)(sum * 100)) * 0.01f).ToString("0.##");
        }

    }

}