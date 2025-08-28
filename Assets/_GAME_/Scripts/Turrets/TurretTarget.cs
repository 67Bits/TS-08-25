using UnityEngine;
using System.Collections.Generic;

namespace Turret
{
    public class TurretTarget : MonoBehaviour
    {
        [SerializeField] private TurretSensor turretTargetBy;

        public void SetTargetBy(TurretSensor turretSensor)
        {
            if (turretTargetBy == null)
            {
                turretTargetBy = turretSensor;
            }
        }

        public void RemoveTargetBy()
        {
            turretTargetBy = null;
        }

        public bool IsTargeted()
        {
            return turretTargetBy != null;
        }
        
        public bool IsTargeted(TurretSensor sensor)
        {
            return turretTargetBy == sensor;
        }
    }
}
