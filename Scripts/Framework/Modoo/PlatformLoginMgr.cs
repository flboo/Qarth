//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TaurusXAdSdk.Api;
namespace Qarth
{
    [TMonoSingletonAttribute("[App]/PlatformLoginMgr")]
    public class PlatformLoginMgr : TMonoSingleton<PlatformLoginMgr>
    {
        [SerializeField]
        private Transform m_TrsUIPanelRoot;
        private Action m_LoginShowedCB;

        void Awake()
        {
            if (m_TrsUIPanelRoot == null)
                m_TrsUIPanelRoot = UIMgr.S.uiRoot.panelRoot.transform;
        }

        public void InitPlatformLogin(Action callback)
        {
            DataAnalysisMgr.S.CustomEventDailySingle("InitPlatform", "start");
            WeGameSdkAdapter.S.Init(false);
            m_LoginShowedCB = callback;

            // AdsMgr.S.ShowSpalshAD("");
            // DataAnalysisMgr.S.CustomEvent("ShowSplashAd");
            Show();
            DataAnalysisMgr.S.CustomEventDailySingle("InitPlatform", "end");
        }

        void Show()
        {
            var obj = Resources.Load("UI/Panels/ChsPanel/PlatformPanel");
            //添加平台logo
            if (!FileMgr.S.FileExists("platform_logo.png") || obj == null)
            {
                LoadLoginPanel();
            }
            else
            {

                GameObject plat = Instantiate(obj as GameObject, m_TrsUIPanelRoot, false);
                CustomExtensions.CallWithDelay(this, () =>
                {
                    Destroy(plat);
                    LoadLoginPanel();
                }, 1);
            }
        }

        void LoadLoginPanel()
        {
            var obj = Resources.Load("UI/Panels/ChsPanel/LoginPanel");
#if UNITY_ANDROID && !UNITY_EDITOR
            if (obj == null 
                || TaurusXConfigUtil.GetChannel() == "taptap" 
                || TaurusXConfigUtil.GetChannel() == "toutiao"
                || TaurusXConfigUtil.GetChannel() == "bytedance" 
                || TaurusXConfigUtil.GetChannel() == "xiaomi"
                || TaurusXConfigUtil.GetChannel() == "kuaishou"
                || TaurusXConfigUtil.GetChannel() == "uc"
                || TaurusXConfigUtil.GetChannel() == "wifi")
            {
                PlayerPrefs.SetInt("channel_exit_key", 1);
                if (m_LoginShowedCB != null)
                    m_LoginShowedCB.Invoke();
                return;
           }
#endif


            GameObject login = Instantiate(obj as GameObject, m_TrsUIPanelRoot, false);
            login.GetComponent<BaseLoginPanel>().callBackAction = delegate
            {
                if (m_LoginShowedCB != null)
                    m_LoginShowedCB.Invoke();
            };
        }

        void OnDestroy()
        {
            m_TrsUIPanelRoot = null;
            m_LoginShowedCB = null;
        }
    }
}
