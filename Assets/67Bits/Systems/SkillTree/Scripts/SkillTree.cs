using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Upgrades;
using Inventory;
using System.Linq;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private SkillTreePrefabType[] _buttonPrefabs;
    [SerializeField] private UpgradeDataList UDataList;

    private List<SkillTreeButton> AllButtons = new List<SkillTreeButton>();

    private string PlayerPrefsKey = "LastSkillTreePurchasedItem";

    public IEnumerator Start()
    {
        AllButtons.Clear();

        for (int i = ContentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(ContentTransform.GetChild(i).gameObject);
        }

        int index = 0;

        if (UDataList == null)
        {
            Debug.LogError("UpgradeDataList is missing");
            yield break;
        }

        foreach (var upgradeData in UDataList.List)
        {
            GameObject target = _buttonPrefabs.First(x => x.Type == upgradeData.SkillTreeUpgradeType).Prefab;
            GameObject buttonGO = Instantiate(target, ContentTransform);

            buttonGO.name = upgradeData.Name + " " + index++;

            SkillTreeButton buttonScript = buttonGO.GetComponent<SkillTreeButton>();
            buttonScript.upgradeData = upgradeData;
            buttonScript.OnUpgradePurchase += OnUpgradePurchase;
            buttonScript.InventoryService = InventoryService.Instance;

            AllButtons.Add(buttonScript);

            // Sending last button to the top.
            // Inverting order so first button stayst at UI bottom (list go up in UI)
            ContentTransform.GetChild(ContentTransform.childCount - 1).SetSiblingIndex(0);

            buttonGO.SetActive(true);
        }

        Load();

        // Force rebuild will ajust the content size to his new amount of content.
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentTransform as RectTransform);
        UpdateUI();
    }

    void OnUpgradePurchase(SkillTreeButton skillButton)
    {
        //... price cost
        InventoryService.Instance.TryAddOnItemCount(skillButton.upgradeData.Price.Unit, -skillButton.upgradeData.Price.Quantity);

        skillButton.WasPurchased = true;
        UpgradesService.Instance.OnPermanentUpgradeAdded(skillButton.upgradeData.GetUpgrade());

        skillButton.SetButtonState(SkillTreeButton.ButtonState.Buying);
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool PreviousWasPurchased = true;
        bool purchasable;
        SkillTreeButton.ButtonState state;

        foreach (var button in AllButtons)
        {
            purchasable = PreviousWasPurchased
                && !button.WasPurchased
                && button.HasItemAmountToPurchase();

            button.SetButtonInteractable(purchasable);

            if (!button.WasPurchased)
                PreviousWasPurchased = false;

            bool isFirst = button == AllButtons[0];

            if (button.WasPurchased)
                state = SkillTreeButton.ButtonState.Bought;
            else if (purchasable)
                state = SkillTreeButton.ButtonState.Available;
            else
                state = SkillTreeButton.ButtonState.Blocked;

            button.UpdateUI(!isFirst, state);
        }
    }

    private void Save()
    {
        int savedIndex = AllButtons.Count - 1;

        for (int i = 0; i < AllButtons.Count; i++)
        {
            if (!AllButtons[i].WasPurchased)
            {
                savedIndex = i - 1;
                break;
            }
        }

        PlayerPrefs.SetInt(PlayerPrefsKey, savedIndex);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (UDataList == null)
            throw new UnassignedReferenceException("UpgradeDataList is missing");

        bool hasData = PlayerPrefs.HasKey(PlayerPrefsKey);

        if (!hasData)
            return;

        int savedIndex = PlayerPrefs.GetInt(PlayerPrefsKey);

        for (int i = 0; i <= savedIndex; i++)
        {
            AllButtons[i].WasPurchased = true;
        }
    }

    private void OnEnable()
    {
        //Load();
    }

    private void OnDisable()
    {
        Save();
    }


    [ContextMenu("Debug Add Gold")]
    private void AddGold()
    {
        InventoryService.Instance.TryAddOnItemCount(ItemID.Gold, 10);
        UpdateUI();
    }
}
