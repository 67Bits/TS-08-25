using System;

namespace Ads
{
    public interface IAdsInterstitial
    {
        public void ShowInterstitial(Action OnStartAdEvent = null, Action OnFinishAdEvent = null, Action OnFailedAdEvent = null);
        public void HideBanner();
    }
}
