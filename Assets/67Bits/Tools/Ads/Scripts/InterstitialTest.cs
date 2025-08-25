using System;
using UnityEngine;

namespace Ads
{
    [Serializable]
    public class InterstitialTest : MonoBehaviour, IAdsInterstitial
    {
        public void ShowInterstitial(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null)
        {
            Debug.Log("ShowInterstitial");
            OnStartAdEvent?.Invoke();
            OnFinishAdEvent?.Invoke();
        }
        public void HideBanner()
        {
            Debug.Log("HideBanner");
        }
    }
}
