using System;
using System.Collections.Generic;
using Modifiables;
using UnityEngine;

namespace Upgrades
{
    public class UpgradesService : IService
    {
        #region Static references
        private static UpgradesSettings _settings;
        public static UpgradesSettings SaveSettings
        {
            get
            {
                if (_settings == null)
                {
                    UpgradesSettings[] savesSettings = Resources.LoadAll<UpgradesSettings>("");
                    if (savesSettings == null || savesSettings.Length < 1)
                        return null;
                    _settings = savesSettings[0];
                }
                return _settings;
            }
        }

        private static UpgradesService _singletonInstance;
        public static UpgradesService Instance
        {
            get
            {
                if (_singletonInstance == null)
                    Debug.LogError("Missing instance of UpgradesService");
                return _singletonInstance;
            }
        }
        #endregion

        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnApplicationInicialization()
        {
            if (SaveSettings.AutoInitialize)
                (new UpgradesService()).TryInitilializeSingleton();
        }

        public bool TryInitilializeSingleton()
        {
            if (_singletonInstance != null)
                return false;

            _singletonInstance = this;
            return true;
        }
        #endregion

        public Action<IUpgrade> OnTempUpgradeAdded = (m) => { };
        public Action<IUpgrade> OnPermanentUpgradeAdded = (m) => { };

        public Action OnClearAllTempUpgrades = () => { };

        public void AddTempUpgrade(IUpgrade upgrade)
        {
            OnTempUpgradeAdded?.Invoke(upgrade);
        }

        public void AddPermanentUpgrade(IUpgrade upgrade)
        {
            OnPermanentUpgradeAdded?.Invoke(upgrade);
        }

        public void RemoveAllTempUpgrade()
        {
            OnClearAllTempUpgrades?.Invoke();
        }
    }
}
