using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSBTutorial
{
    public class ObjectDisabler : MonoBehaviour
    {
        [SerializeField] private float _timeToDisable;
        private void OnEnable()
        {
            StartCoroutine(WaitToDisable());
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(_timeToDisable);
            gameObject.SetActive(false);
        }
    }
}
