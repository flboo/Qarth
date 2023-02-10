using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public class GPSHelper
    {
        private const string m_PrefKey_UnderTheWall = "GpsHelper_UnderTheWall";
        private static string m_IpUrl = "http://icanhazip.com/";
        private static string m_GpsUrl = "http://ip.taobao.com/service/getIpInfo.php?ip=";
        private static string m_IPAPIUrl = "http://ip-api.com/json";

        //请求ip位置
        public static void RequestIPPos(MonoBehaviour mono, System.Action<TbIPData> cbNormal, System.Action cbError)
        {
            mono.StartCoroutine(RequestIPPos(cbNormal, cbError));
        }

        public static void RequestIPAPI(MonoBehaviour mono, System.Action<string> cbNormal, System.Action cbError)
        {
            mono.StartCoroutine(RequestIPAPI(cbNormal, cbError));
        }

        //是否内部ip
        public static bool IsUnderTheWall(TbIPData data)
        {
            if (!string.IsNullOrEmpty(data.country_id) || !string.IsNullOrEmpty(data.country))
            {
                if (data.country == "中国" || data.country_id == "CN")
                {
                    if (data.region_id == EngineDefine.TW_REGION_ID
                    || data.region_id == EngineDefine.HK_REGION_ID
                    || data.region_id == EngineDefine.MO_REGION_ID
                    || data.region.Contains(EngineDefine.TW_REGION_NAME)
                    || data.region.Contains(EngineDefine.HK_REGION_NAME)
                    || data.region.Contains(EngineDefine.MO_REGION_NAME))
                    {
                        PlayerPrefs.SetInt(m_PrefKey_UnderTheWall, 0);
                        return false;
                    }
                    else
                    {
                        PlayerPrefs.SetInt(m_PrefKey_UnderTheWall, 1);
                        return true;
                    }
                }
                else
                {
                    PlayerPrefs.SetInt(m_PrefKey_UnderTheWall, 0);
                    return false;
                }
            }
            return true;
        }

        //是否内部ip
        public static bool IsUnderTheWall(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                if (countryCode == "CN")
                {
                    PlayerPrefs.SetInt(m_PrefKey_UnderTheWall, 1);
                    return true;
                }
                else
                {
                    PlayerPrefs.SetInt(m_PrefKey_UnderTheWall, 0);
                    return false;
                }
            }
            return true;
        }

        //拉不到ip信息时候看系统语言
        public static bool IsUnderTheWallFromLang()
        {
            return Application.systemLanguage == SystemLanguage.ChineseSimplified
                || Application.systemLanguage == SystemLanguage.Chinese
                || Application.systemLanguage == SystemLanguage.ChineseTraditional;
        }

        //存档定位
        public static int IsUnderTheWallFromPrefs()
        {
            return PlayerPrefs.GetInt(m_PrefKey_UnderTheWall, -1);
        }

        static IEnumerator GetIp()
        {
            UnityWebRequest wr = UnityWebRequest.Get(m_IpUrl);
            yield return wr.SendWebRequest();
            //异常处理
            if (wr.isHttpError || wr.isNetworkError)
                Log.e("ipgeterror: " + wr.error);
            else
            {
                Log.e(wr.downloadHandler.text);
            }
        }

        static IEnumerator RequestIPAPI(System.Action<string> cbNormal, System.Action cbError)
        {
            UnityWebRequest wrGps = UnityWebRequest.Get(m_IPAPIUrl);
            wrGps.timeout = 5;
            yield return wrGps.SendWebRequest();
            if (wrGps.isHttpError || wrGps.isNetworkError)
            {
                Log.e("ipapigeterror: " + wrGps.error);
                cbError.Invoke();
            }
            else
            {
                ResponseBody_APIIPInfo res = LitJson.JsonMapper.ToObject<ResponseBody_APIIPInfo>(wrGps.downloadHandler.text);
                //Log.e(">>>res get");
                if (res != null && res.status == "success")
                {
                    cbNormal.Invoke(res.countryCode);
                    //Log.e(">>>res ok");
                }
                else
                {
                    cbError.Invoke();
                    Log.e(">>>res error");
                }
            }
        }

        static IEnumerator RequestIPPos(System.Action<TbIPData> cbNormal, System.Action cbError)
        {
            UnityWebRequest wrIp = UnityWebRequest.Get(m_IpUrl);
            wrIp.timeout = 5;
            //先获取外网IP
            yield return wrIp.SendWebRequest();
            //异常处理
            if (wrIp.isHttpError || wrIp.isNetworkError)
            {
                Log.e("ipgeterror: " + wrIp.error);
                cbError.Invoke();
            }
            else
            {
                string ip = wrIp.downloadHandler.text;
                //Log.e(">>>ip: " + ip);
                //通过淘宝IP的第三方库获取IP的详细信息
                UnityWebRequest wrGps = UnityWebRequest.Get(m_GpsUrl + ip);
                wrGps.timeout = 5;
                yield return wrGps.SendWebRequest();
                if (wrGps.isHttpError || wrGps.isNetworkError)
                {
                    Log.e("ipinfogeterror: " + wrGps.error);
                    cbError.Invoke();
                }
                else
                {
                    ResponseBody_TbIPInfo res = LitJson.JsonMapper.ToObject<ResponseBody_TbIPInfo>(wrGps.downloadHandler.text);
                    //og.e(">>>res get");
                    if (res != null && res.code == 0)
                    {
                        cbNormal.Invoke(res.data);
                        //Log.e(">>>res ok");
                    }
                    else
                    {
                        cbError.Invoke();
                        Log.e(">>>res error");
                    }
                }
            }
        }

        #region taobao
        public class ResponseBody_TbIPInfo
        {
            public int code;
            public TbIPData data;
        }

        public class TbIPData
        {
            public string ip;
            public string country;
            public string area;
            public string region;
            public string city;
            public string county;
            public string isp;
            public string country_id;
            public string area_id;
            public string region_id;
            public string city_id;
            public string county_id;
            public string isp_id;
        }
        #endregion

        #region ip-api
        public class ResponseBody_APIIPInfo
        {
            public string city;
            public string country;
            public string countryCode;
            public string isp;
            public string org;
            public string query;
            public string region;
            public string regionName;
            public string status;
            public string timezone;
            public string zip;
        }
        #endregion
    }
}