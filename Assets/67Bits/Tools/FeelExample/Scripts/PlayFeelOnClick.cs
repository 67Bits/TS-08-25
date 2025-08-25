using MoreMountains.Feedbacks;
using ObjectPooling;
using UnityEngine;

namespace SSB.FeelExample
{
    public class PlayFeelOnClick : MonoBehaviour
    {
        [SerializeField] private bool _usePool;
        [SerializeField] private MMF_Player _feel;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(cameraRay, out var hit))
                {
                    PlayFeel(hit.transform);
                }
            }
        }
        public void PlayFeel(in Transform target)
        {
            var newFeel = _usePool ? _feel.gameObject.InstantiateFromPool().GetComponent<MMF_Player>() : _feel;

            if (_usePool)
            {
                void Disable()
                {
                    newFeel.gameObject.SetActive(false);
                }
                newFeel.Events.OnComplete.RemoveListener(Disable);
                newFeel.Events.OnComplete.AddListener(Disable);
            }

            for (int i = 0; i < newFeel.FeedbacksList.Count; i++)
            {
                MMF_Feedback feedback = newFeel.FeedbacksList[i];
                switch (feedback)
                {
                    case MMF_Rotation rotation:
                        rotation.AnimateRotationTarget = target;
                        break;
                    case MMF_SquashAndStretch squashAndStretch:
                        squashAndStretch.SquashAndStretchTarget = target;
                        break;
                }
            }
            newFeel.transform.position = target.position;
            newFeel.PlayFeedbacks();
        }
    }
}
