using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ModalHelper
    {
        private static GameObject _modalObj;
        private static Text _titleTxt;
        private static Text _contentTxt;
        private static Button _okBtn;
        private static Button _applyBtn;
        private static Button _cancleBtn;

        public static void ModalInit(GameObject modalObj)
        {
            _modalObj = modalObj;

            var _contentPanel = _modalObj.transform.Find("Panel");
            _titleTxt = _contentPanel.Find("Title").GetComponent<Text>();
            _contentTxt = _contentPanel.Find("Content").GetComponent<Text>();
            _okBtn = _contentPanel.Find("OKButton").GetComponent<Button>();
            _applyBtn = _contentPanel.Find("ApplyButton").GetComponent<Button>();
            _cancleBtn = _contentPanel.Find("CancleButton").GetComponent<Button>();
        }

        public static void WarningMessage(string content, Action OKAct = null)
        {
            SetModal("Tip", content, true, false, false, OKAct, null, null);
        }

        public static void ConfirmMessage(string content, Action applyAct = null, Action cancleAct = null)
        {
            SetModal("Confirm", content, false, true, true, null, applyAct, cancleAct);
        }


        private static void SetModal(string title, string content, bool okbtn, bool applybtn, bool canclebtn, Action okAct, Action applyAct, Action cancleAct)
        {
            _modalObj.SetActive(true);
            _titleTxt.text = title;
            _contentTxt.text = content;

            _okBtn.onClick.RemoveAllListeners();
            _applyBtn.onClick.RemoveAllListeners();
            _cancleBtn.onClick.RemoveAllListeners();
            _okBtn.gameObject.SetActive(okbtn);
            _applyBtn.gameObject.SetActive(applybtn);
            _cancleBtn.gameObject.SetActive(canclebtn);

            if (okbtn)
            {
                _okBtn.onClick.AddListener(()=> { 
                    okAct?.Invoke();
                    _modalObj.SetActive(false);
                });
            }

            if (applybtn)
            {
                _applyBtn.onClick.AddListener(() => { 
                    applyAct?.Invoke();
                    _modalObj.SetActive(false);
                });
            }

            if (canclebtn)
            {
                _cancleBtn.onClick.AddListener(() => { 
                    cancleAct?.Invoke();
                    _modalObj.SetActive(false);
                });
            }
        }
    }
}
