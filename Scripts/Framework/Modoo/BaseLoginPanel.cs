using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qarth
{
    public class BaseLoginPanel : MonoBehaviour
    {
        [SerializeField] protected Button m_LoginBtn;

        public Action callBackAction;

        void Start()
        {
            OnPanelInit();
        }

        // Use this for initialization
        protected virtual void OnPanelInit()
        {
            m_LoginBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                if (callBackAction != null)
                {
                    callBackAction.Invoke();
                    Destroy(gameObject);
                }
#else
                WeGameSdkAdapter.S.Login((success) =>
                {
                    if (success)
                    {
                        if (callBackAction != null)
                        {
                            callBackAction.Invoke();
                            Destroy(gameObject);
                        }
                    }
                });
#endif
            });
        }

    }
}