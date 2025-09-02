using UnityEngine;
using HomaGames.HomaBelly;

namespace TS
{
    public class SDKManager : MonoBehaviour
    {
        public void Awake()
        {
            if (!HomaBelly.Instance.IsInitialized)
            {
                // Listen event for initialization
                Events.onInitialized += OnHomaBellyInitialized;
            }
            else
            {
                OnHomaBellyInitialized();
            }
        }

        private void OnDisable()
        {
            Events.onInitialized -= OnHomaBellyInitialized;
        }

        private void OnHomaBellyInitialized()
        {
            Analytics.GameplayStarted();
            Analytics.LevelStarted(1);
        }

        public void OnVictory()
        {
            Analytics.LevelCompleted();
        }

        public void OnDefeat()
        {
            Analytics.LevelFailed("Death");
        }
    }
}
