using Inventory;
using System;
using UnityEngine;
using Upgrades;

namespace SSBRoadmap
{
    public class RoadMapStage
    {
        private RoadMapStateDot _dot;
        private RoadMapStageButton _button;
        private RoadMapStageState _state;
        private UpgradeData _upgradeData;

        public RoadMapStageState RoadMapState => _state;
        public RoadMapStateDot Dot => _dot;
        public RoadMapStageButton Button => _button;
        public UpgradeData UpgradeData => _upgradeData;
        public float YPos => _button.transform.position.y;
        public Action OnPurchase { get; set; }

        public RoadMapStage(RoadMapStateDot dot, RoadMapStageButton button, UpgradeData upgradeData)
        {
            _dot = dot;
            _button = button;
            _upgradeData = upgradeData;

            _button.UpgradeData = upgradeData;
            _button.Button.onClick.AddListener(Buy);
        }

        public void SetState(RoadMapStageState newState, bool loading = false)
        {
            if (newState == _state)
                return;

            _state = newState;
            switch (newState)
            {
                case RoadMapStageState.Locked:
                    SetLocked(loading);
                    break;
                case RoadMapStageState.Next:
                    SetNext(loading);
                    break;
                case RoadMapStageState.Available:
                    SetAvailable(loading);
                    break;
                case RoadMapStageState.Bought:
                    SetBought(loading);
                    break;
                case RoadMapStageState.NotEnoughMoney:
                    SetNotEnoughMoney(loading);
                    break;
                default:
                    break;
            }
        }

        private void SetLocked(bool loading = false)
        {
            _dot.SetLocked(loading);
            _button.SetLocked(loading);
        }

        private void SetNext(bool loading = false)
        {
            _dot.SetNext(loading);
            _button.SetNext(loading);
        }

        private void SetAvailable(bool loading = false)
        {
            _dot.SetAvailable(loading);
            _button.SetAvailable(loading);
        }

        private void SetBought(bool loading = false)
        {
            _dot.SetBought(loading);
            _button.SetBought(loading);
            if (!loading)
            {
                OnPurchase?.Invoke();
            }
        }

        private void SetNotEnoughMoney(bool loading = false)
        {
            _button.SetNotEnoughMoney(loading);
        }

        private void Buy()
        {
            _state = RoadMapStageState.Bought;
            InventoryService.Instance.TryAddOnItemCount(_upgradeData.Price.Unit, -_upgradeData.Price.Quantity);
            _upgradeData.GetUpgrade();

            SetBought();
        }
    }

    public enum RoadMapStageState
    {
        Locked,
        Next,
        Available,
        Bought,
        NotEnoughMoney
    }

}