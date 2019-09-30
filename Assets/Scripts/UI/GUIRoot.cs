using Core;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

namespace Assets.Scripts.UI
{
    class GUIRoot : Singleton<GUIRoot>
    {
        private static Color32 red = new Color32(187, 39, 45, 255);

        [SerializeField] private RectTransform objectLibrary;
        [SerializeField] private RectTransform windowShoot;
        [SerializeField] private RectTransform doorShoot;
        [SerializeField] private Text dialogText;
        [SerializeField] private Image selectedMode;

        private Animator animator;
        protected override void Awake()
        {
            animator = GetComponent<Animator>();
            GetComponentInChildren<Animation>().Play();
        }

        public void EnableWindowshoot(bool enable)
        {
            incr = 0;
            ResetColors(doorShoot);
            ResetColors(windowShoot);
            windowShoot.gameObject.SetActive(enable);
            ShotHappened();
        }

        public void EnableDoorshoot(bool enable)
        {
            incr = 0;
            ResetColors(doorShoot);
            ResetColors(windowShoot);
            doorShoot.gameObject.SetActive(enable);
            ShotHappened();
        }


        int incr = 0;
        public void ShotHappened()
        {
            if(windowShoot.gameObject.activeSelf)
            {
                IncrementWindow();
                if(incr > 4)
                {
                    ResetColors(windowShoot);

                    incr = 0;
                }
            }
            if (doorShoot.gameObject.activeSelf)
            {
                IncrementDoor();
                if (incr > 3)
                {
                    ResetColors(doorShoot);

                    incr = 0;
                }
            }
        }

        private void IncrementDoor()
        {
            ResetColors(doorShoot);
            doorShoot.GetChild(incr++).GetComponent<Image>().color = red;
        }

        private void IncrementWindow()
        {
            ResetColors(windowShoot);
            windowShoot.GetChild(incr++).GetComponent<Image>().color = red;
        }

        private void ResetColors(RectTransform trans)
        {
            foreach (Image img in trans.GetComponentsInChildren<Image>())
            {
                img.color = Color.white;
            }
        }

        public void AnimateDialog(string msg)
        {
            dialogText.text = msg;
            animator.SetTrigger("on");
        }

        public void ChangeSelectedMode(ShotMode mode)
        {
            selectedMode.sprite = Resources.Load<Sprite>("Icons/" + mode.ToString());
        }

        public void EnableObjectLibrary(bool enable)
        {
            objectLibrary.gameObject.SetActive(enable);
        }
    }
}
