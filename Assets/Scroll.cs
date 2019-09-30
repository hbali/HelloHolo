using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

namespace UI
{

    public class Scroll : MonoBehaviour, INavigationHandler
    {
        private ScrollRect scrollRect;

        public void OnNavigationCanceled(NavigationEventData eventData)
        {

        }

        public void OnNavigationCompleted(NavigationEventData eventData)
        {

        }

        public void OnNavigationStarted(NavigationEventData eventData)
        {

        }

        public void OnNavigationUpdated(NavigationEventData eventData)
        {
            scrollRect.verticalNormalizedPosition += eventData.NormalizedOffset.y / 50;
        }

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

    }

}