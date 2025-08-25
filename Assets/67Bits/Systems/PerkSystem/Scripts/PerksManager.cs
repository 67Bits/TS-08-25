using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SSBPerks
{
    public class PerksManager : Singleton<PerksManager>
    {
        private static TemporaryPerk[] _termporaryPerks;
        private string _resourcesPath = "TemporaryPerksScriptables/LV1";
        public TemporaryPerk[] AllTStickers
        {
            get
            {
                if (_termporaryPerks == null) _termporaryPerks = Resources.LoadAll<TemporaryPerk>("");
                return _termporaryPerks;
            }
        }

        public List<TemporaryPerk> AvailableTemporaryPerks = new List<TemporaryPerk>();
        public List<TemporaryPerk> EquippedTemporaryPerks = new List<TemporaryPerk>();
        public List<TemporaryPerk> EquippedTemporaryBufferPerks = new List<TemporaryPerk>();

        protected override void Awake()
        {
            base.Awake();
            ResetPerks();

            SceneManager.sceneLoaded += ClearPerks;
            SceneManager.sceneLoaded += EquipAllPerks;
            SceneManager.sceneLoaded += EquipAllBufferPerks;
        }
        public void ResetPerks()
        {
            TemporaryPerk[] stickers = Resources.LoadAll<TemporaryPerk>(_resourcesPath);
            AvailableTemporaryPerks = new List<TemporaryPerk>(stickers);
        }

        public void ClearPerks(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 1)
            {
                EquippedTemporaryPerks.Clear();
                EquippedTemporaryBufferPerks.Clear();
            }
        }

        private void EquipAllPerks(Scene scene, LoadSceneMode loadSceneMode)
        {
            foreach (TemporaryPerk sticker in EquippedTemporaryPerks)
            {
                sticker.Equip();
            }
        }
        private void EquipAllBufferPerks(Scene scene, LoadSceneMode loadSceneMode)
        {
            foreach (TemporaryPerk sticker in EquippedTemporaryBufferPerks) sticker.Equip();
        }

        public void EquipTemporaryPerk(TemporaryPerk temporarySticker)
        {
            temporarySticker.Equip(false);
            EquippedTemporaryPerks.Add(temporarySticker);

            if (EquippedPerksCount(temporarySticker) >= temporarySticker.MaxEquipped)
                AvailableTemporaryPerks.Remove(temporarySticker);
            if (temporarySticker.NextLevel) AvailableTemporaryPerks.Add(temporarySticker.NextLevel);
        }

        public void EqquipTemporaryBufferPerk(TemporaryPerk temporarySticker)
        {
            temporarySticker.Equip(false);

            EquippedTemporaryBufferPerks.Add(temporarySticker);
        }

        public void ConsumePerk(TemporaryPerk temporarySticker)
        {
            temporarySticker.Equip();
        }

        public TemporaryPerk[] GetAvailableTemporaryPerks(int perksCount = 3)
        {
            List<TemporaryPerk> available = new List<TemporaryPerk>();

            while (available.Count < perksCount && AvailableTemporaryPerks.Count > available.Count)
            {
                int randomId = Random.Range(0, AvailableTemporaryPerks.Count);
                TemporaryPerk sticker = AvailableTemporaryPerks[randomId];
                if (!available.Contains(sticker) && EquippedPerksCount(sticker) < sticker.MaxEquipped && Random.Range(0, 100) < sticker.DropChance)
                    available.Add(sticker);
            }


            return available.ToArray();
        }

        public int EquippedPerksCount(TemporaryPerk sticker)
        {
            int count = 0;
            foreach (var item in EquippedTemporaryPerks)
            {
                if (item == sticker)
                    count++;
            }

            return count;
        }

    }
}