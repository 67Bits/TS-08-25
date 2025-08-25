using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MZW.Player
{
    public class PlayerFootsteps : MonoBehaviour
    {
        public UnityEvent leftStep, rightStep;

        public void LeftStep()
        {
            leftStep.Invoke();
        }
        public void RightStep()
        {
            rightStep.Invoke();
        }
    }
}
