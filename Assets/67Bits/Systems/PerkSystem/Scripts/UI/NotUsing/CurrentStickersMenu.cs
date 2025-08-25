using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SSBPerks
{
    public class CurrentStickersMenu : MonoBehaviour
    {
        [SerializeField] Transform parentSpawn;
        [SerializeField] PauseSticker stickerPrefab;
        List<TemporaryPerk> temporaryStickerList;

        private void Start()
        {
            temporaryStickerList = PerksManager.Instance.EquippedTemporaryPerks;

            foreach (var sticker in temporaryStickerList)
            {
                CreateSticker(sticker);
            }
        }

        public void UpdateEqquipedStickers()
        {
            foreach (Transform item in parentSpawn)
            {
                item.gameObject.SetActive(false);
            }
            for (int i = 0; i < PerksManager.Instance.EquippedTemporaryPerks.Count; i++)
            {
                CreateSticker(PerksManager.Instance.EquippedTemporaryPerks[i]);
            }
        }

        public void CreateSticker(TemporaryPerk sticker)
        {
            PauseSticker newSticker = Instantiate(stickerPrefab, parentSpawn);
            newSticker.SetSticker(sticker);
        }
    }
}