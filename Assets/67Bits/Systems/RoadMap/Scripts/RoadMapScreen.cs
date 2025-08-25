using Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

namespace SSBRoadmap
{
    public class RoadMapScreen : MonoBehaviour
    {
        [SerializeField] private RoadMapFillBar _fillBar;
        [SerializeField] private RoadMapStageButton _buttonPrefab;
        [SerializeField] private RoadMapStateDot _dotPrefab;
        [SerializeField] private UpgradeDataList _upgradesDataList;

        [Header("Spawn Configs")]
        [SerializeField] private RectTransform _buttonsParent;
        [SerializeField] private RectTransform _dotsParent;

        [Header("Debug")]
        [SerializeField] private ItemID _goldData;
        [SerializeField] private int _amountToAdd;

        private List<RoadMapStage> _stages = new List<RoadMapStage>();
        private bool _createdStages;
        private string _lastRoadMapKey = "LastRoadMapBought";
        private int LastBoughtId
        {
            get
            {
                return PlayerPrefs.GetInt(_lastRoadMapKey, -1);
            }
            set
            {
                PlayerPrefs.SetInt(_lastRoadMapKey, value);
            }
        }

        public Action OnUpdateProgress { get; set; }
        public List<RoadMapStage> Stages => _stages;
        public RoadMapStage LastBought
        {
            get
            {
                int id = Math.Max(0, LastBoughtId);
                return _stages[id];
            }
        }

        private void OnEnable()
        {
            if (_createdStages)
                UpdateStages();
        }

        private void Start()
        {
            DeletePrefabStages();
            CreateStages();
            Load();
            UpdateStages();
        }

        private void Load()
        {
            for (int i = 0; i <= LastBoughtId; i++)
            {
                _stages[i].SetState(RoadMapStageState.Bought, true);
            }
        }

        private void Save()
        {
            LastBoughtId = _stages.FindLastIndex(x => x.RoadMapState == RoadMapStageState.Bought);
        }

        private void CreateStages()
        {
            var upgrades = GetUpgrades();

            foreach (var upgrade in upgrades)
            {
                var dot = Instantiate(_dotPrefab);
                dot.transform.SetParent(_dotsParent);

                var button = Instantiate(_buttonPrefab);
                button.transform.SetParent(_buttonsParent);

                var roadMapStage = new RoadMapStage(dot, button, upgrade);
                _stages.Add(roadMapStage);

                roadMapStage.OnPurchase += UpdateStages;
            }

            _createdStages = true;
        }

        protected virtual IEnumerable<UpgradeData> GetUpgrades()
        {
            return _upgradesDataList.List;
        }

        private void UpdateStages()
        {
            for (int i = 0; i < _stages.Count; i++)
            {
                var currentStage = _stages[i];
                bool hasMoney = CheckHasCurrency(currentStage.UpgradeData);
                bool bought = _stages[i].RoadMapState == RoadMapStageState.Bought;

                if (bought)
                    continue;

                if (i == 0 && hasMoney)
                {
                    currentStage.SetState(RoadMapStageState.Available);
                    continue;
                }
                else if (i == 0 && !hasMoney)
                {
                    currentStage.SetState(RoadMapStageState.NotEnoughMoney);
                    continue;
                }
                else if (i == 0)
                {
                    continue;
                }

                var previousStage = _stages[i - 1];

                if (previousStage.RoadMapState == RoadMapStageState.Available && hasMoney)
                {
                    currentStage.SetState(RoadMapStageState.Next);
                }
                else if (previousStage.RoadMapState == RoadMapStageState.Bought && hasMoney)
                {
                    currentStage.SetState(RoadMapStageState.Available);
                }
                else if (previousStage.RoadMapState == RoadMapStageState.Bought && !hasMoney)
                {
                    currentStage.SetState(RoadMapStageState.NotEnoughMoney);
                }
                else
                {
                    currentStage.SetState(RoadMapStageState.Locked);
                }
            }

            Save();
            OnUpdateProgress?.Invoke();
        }

        private bool CheckHasCurrency(UpgradeData data)
        {
            int cost = data.Price.Quantity;
            int playerAmount = InventoryService.Instance.GetItemCount(data.Price.Unit);

            return playerAmount >= cost;
        }

        private void DeletePrefabStages()
        {
            var buttons = _buttonsParent.childCount;

            for (var i = 0; i < buttons; i++)
            {
                Destroy(_buttonsParent.GetChild(i).gameObject);
            }

            var dots = _dotsParent.childCount;

            for (var i = 0; i < buttons; i++)
            {
                Destroy(_dotsParent.GetChild(i).gameObject);
            }
        }


        [ContextMenu("Add Gold")]
        private void AddGold()
        {
            InventoryService.Instance.TryAddOnItemCount(_goldData, _amountToAdd);
            UpdateStages();
        }
    }
}