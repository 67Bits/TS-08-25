using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueContent
{
    [ReadOnly]
    public int dialogueIndex = 0;
    public int charIndex = 0;
    public bool waitUIBeforePlay = true;
    public List<string> phrasesList = new();
    public Sprite upperSprite = null;
    public List<EventOnPhrase> EventOnPhraseList = new();

    public UnityEvent OnStartUE;
    public UnityEvent OnEndUE;

    [System.Serializable]
    public class EventOnPhrase
    {
        public int phraseIndex = 0;
        public UnityEvent UEvent = new ();
    }
}