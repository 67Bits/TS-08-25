using UnityEngine;
using TGR;
using Turret;


namespace TS
{
    [CreateAssetMenu(fileName = "TurretShopItem", menuName = "Scriptable Objects/TurretShopItem")]
    public class TurretShopItem : ShopItem
    {
        public TurretSensor TurretPrefab;
        public override void Apply()
        {
            TurretStack.Instance.AddTurret(TurretPrefab);
        }

        public override void Reset()
        {

        }
    }
}
