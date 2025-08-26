using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TGR
{
    public class ShopManager : MonoBehaviour
    {
        public bool isDebug = true;
        public List<ShopOption> shopOptions;
        public int optionsLimit = 3;
        public List<ShopItem> options;
        public Transform itemsParent;
        public GameObject storeTab;
        public ShopItemUI itemPrefab;
        public UnityEvent OnStoreOpened;
        public UnityEvent OnStoreClosed;
        public UnityEvent OnStoreEnd;
        public List<ShopItemUI> shopItemUIs;

        public int shopCallsBeforeCombat = 3;
        public float delayBetweenShops = 1f;

        private int currentShop;
        private int currentShopIndex = 0;

        [System.Serializable]
        public class ShopOption
        {
            public List<ShopItem> items;
        }

        private void Start()
        {
            foreach (ShopItem item in options)
            {
                item.Reset();
            }
        }
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        Debug.Log("Store toggled");
        //        ToggleStore();
                
        //    }
        //}

        public void StartShop()
        {
            currentShop = shopCallsBeforeCombat;
            storeTab.SetActive(true);
            ToggleStore();
        }

        public void ToggleStore()
        {
            OnStoreOpened?.Invoke();
            PopulateShop();
        }

        private void PopulateShop()
        {
            if (itemsParent && itemPrefab && options != null && options.Count > 0)
            {
                foreach (Transform child in itemsParent)
                {
                    Destroy(child.gameObject);
                }

                shopItemUIs.Clear();

                for (int i = 0; i < optionsLimit; i++)
                {
                    ShopItem item;
                    if (isDebug)
                        item = shopOptions[currentShopIndex].items[i];
                    else
                        item = options[Random.Range(0, options.Count)];
                    ShopItemUI itemUI = Instantiate(itemPrefab, itemsParent);
                    shopItemUIs.Add(itemUI);

                    itemUI.SetUpItem(item, () =>
                    {
                        foreach (var ui in shopItemUIs.ToArray())
                        {
                            if (ui != itemUI)
                                ui.OnDisable.Invoke();
                        }
                        shopItemUIs.Clear();
                        shopItemUIs.Add(itemUI);

                        
                        OnStoreClosed?.Invoke();
                        //StartCoroutine(SetActiveAfterTime(1.5f, storeTab, false));
                        StartCoroutine(ReopenStore());
                    });
                }

                currentShopIndex++;
                currentShop--;
            }
        }

        private IEnumerator SetActiveAfterTime(float time, GameObject obj, bool active)
        {
            yield return new WaitForSeconds(time);
            obj.SetActive(active);
        }

        private IEnumerator ReopenStore(float customDelay = 0)
        {
            if (customDelay > 0)
                yield return new WaitForSeconds(customDelay);
            else
                yield return new WaitForSeconds(delayBetweenShops);

            if (currentShop <= 0)
            {
                OnStoreEnd.Invoke();
                storeTab.SetActive(false);
            }
            else
                ToggleStore();
        }
    }
}
