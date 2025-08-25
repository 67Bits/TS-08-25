using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class DialogueChar
{
    [ReadOnly]
    public int charIndex = 0;

    public TMPro.TextMeshProUGUI talkingText;

    public UnityEvent OnEnableChar;
    public UnityEvent OnDisableChar;

    public void EnableChar()
    {
        OnEnableChar?.Invoke();
    }

    public void DisableChar()
    {
        OnDisableChar?.Invoke();
    }
}