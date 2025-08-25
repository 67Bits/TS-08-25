using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialoguePlayer : MonoBehaviour
{

    [SerializeField, ReadOnlyAttribute] private bool _waitingforHUD = false;
    [SerializeField, ReadOnlyAttribute] private bool _waitingforDialogue = false;

    public float _textSpeed = 30.0f;
    public float _textOnScreenDuration = 3.0f;
    public bool _onlySkipWithClick = false;
    public Button SkipTalkingButton;

    private bool _skipTextSelected = false;

    private DialogueContent lastDialogue;
    private Coroutine DialogueRoutine;

    bool AvoidDoubleClick = false;
    bool hasPlayingDialogue = false;

    [Space]
    public UnityEvent OnAnyDialogueStart;
    public UnityEvent OnAnyDialogueEnd;

    [Space] public List<DialogueContent> DialoguesList = new();
    [Space] public List<DialogueChar> DialogueCharsList = new();

#if DEBUG
    [Header("Debug")]
    [SerializeField] private bool _debugDialogueRoutine = false;
    [SerializeField] private bool _debugSkipButton = false;
#endif

    public void StartDialogue(int index)
    {
        if (index >= DialoguesList.Count)
        {
            Debug.LogError($"DialoguePlayer: Missing dialogue on index{index}.\n\n", this);
            return;
        }

        if (AvoidDoubleClick)
            return;

        StartCoroutine(StartDialogueCoroutine(index));
    }

    IEnumerator StartDialogueCoroutine(int index)
    {
        while (hasPlayingDialogue)
        {
            _waitingforDialogue = true;
            yield return new WaitForEndOfFrame();
        }
        _waitingforDialogue = false;

        lastDialogue = DialoguesList[index];
        DialogueRoutine = StartCoroutine(DialogueCoroutine(lastDialogue));
    }

    IEnumerator DialogueCoroutine(DialogueContent dialogue)
    {
        AvoidDoubleClick = true;
        hasPlayingDialogue = true;

#if UNITY_EDITOR
        DialogueBeingPlayed = dialogue.dialogueIndex;
#endif
#if DEBUG
        if (_debugDialogueRoutine)
            Debug.Log($"DialoguePlayer: Dialogue {dialogue.dialogueIndex} started.\n\n", this);
#endif

        // Check if UI is free or wait the right moment
        // ...

        AvoidDoubleClick = false;

        SetAllCharsDisabled();

        OnAnyDialogueStart?.Invoke();
        dialogue.OnStartUE?.Invoke();

        DialogueChar dChar = GetChar(dialogue.charIndex);
        dChar.EnableChar();

        float buttonEnablingTime = 0.2f;
        float buttonEnablingTimePassed = 0.0f;

        for (int i = 0; i < dialogue.phrasesList.Count; i++)
        {
            string text = dialogue.phrasesList[i];
            float lettersCount = 0.0f;
            float textCompleteDuration = 0.0f;
            bool textComplete = false;
            _skipTextSelected = false;

            foreach (var Uevent in dialogue.EventOnPhraseList)
            {
                if (Uevent.phraseIndex == i)
                    Uevent.UEvent?.Invoke();
            }

            while (!_skipTextSelected && (textCompleteDuration < _textOnScreenDuration || _onlySkipWithClick))
            {
                yield return new WaitForEndOfFrame();
                if (!IsSkipButtonEnabled())
                {
                    buttonEnablingTimePassed += Time.unscaledDeltaTime;
                    if (buttonEnablingTimePassed >= buttonEnablingTime)
                        EnableSkipButton();
                }

                if (textComplete)
                    textCompleteDuration += Time.unscaledDeltaTime;

#if UNITY_EDITOR
                DialogueTimerRemaining = _textOnScreenDuration - textCompleteDuration;
#endif

                int countFloor = (int)math.floor(lettersCount);
                if (countFloor < text.Length)
                {
                    lettersCount += Time.unscaledDeltaTime * _textSpeed;
                    DialogueLettersRemaining = text.Length - countFloor;

                    // Start by selecting the visible text up to countFloor
                    string visibleText = text.Substring(0, countFloor);

                    // Check if the last character of the visibleText is part of an incomplete tag
                    if (visibleText.EndsWith("<"))
                    {
                        // Find the closing '>' and include the full tag
                        int closingIndex = text.IndexOf('>', countFloor);
                        if (closingIndex != -1)
                        {
                            visibleText = text.Substring(0, closingIndex + 1);
                            lettersCount = closingIndex + 1; // Update counter to include the tag
                        }
                    }

                    dChar.talkingText.text = visibleText;
                }
                else
                {
                    dChar.talkingText.text = text;
                    textComplete = true;
                }
            }

            DisableSkipButton();
            buttonEnablingTimePassed = 0;
        }

        dChar.DisableChar();
        hasPlayingDialogue = false;

#if UNITY_EDITOR
        DialogueBeingPlayed = -1;
#endif

#if DEBUG
        if (_debugDialogueRoutine)
            Debug.Log($"DialoguePlayer: Dialogue {dialogue.dialogueIndex} ended.\n\n", this);
#endif
        dialogue.OnEndUE?.Invoke();
        OnAnyDialogueEnd?.Invoke();
    }

    DialogueChar GetChar(int index)
    {
        if (index >= DialogueCharsList.Count)
        {
            Debug.LogError($"DialoguePlayer: Missing char on index{index}.\n\n", this);
            return null;
        }

        DialogueCharsList[index].EnableChar();

        return DialogueCharsList[index];
    }

    void SetAllCharsDisabled()
    {
        foreach (var dChar in DialogueCharsList)
        {
            dChar.DisableChar();
        }
    }

    bool IsSkipButtonEnabled()
    {
        return SkipTalkingButton.gameObject.activeInHierarchy;
    }

    void EnableSkipButton()
    {
        SkipTalkingButton.gameObject.SetActive(true);
    }

    void DisableSkipButton()
    {
        SkipTalkingButton.gameObject.SetActive(false);
    }

    public void OnSkipButtonClick()
    {
#if DEBUG
        if (_debugSkipButton)
            Debug.Log($"DialoguePlayer: Skip button click registered.\n\n", this);
#endif
        _skipTextSelected = true;
        DisableSkipButton();
    }

    [SerializeField, ReadOnlyAttribute] private int DialogueLettersRemaining = 0;
#if UNITY_EDITOR
    [Header("Tests")]
    [SerializeField, ReadOnlyAttribute] private int DialogueBeingPlayed = -1;
    [SerializeField, ReadOnlyAttribute] private float DialogueTimerRemaining = 0;
    [SerializeField] private int dialogueIndex = 0;

    private void TextDialogue()
    {
        StartDialogue(dialogueIndex);
    }

    private void OnValidate()
    {
        for (int i = 0; i < DialoguesList.Count; i++)
        {
            DialoguesList[i].dialogueIndex = i;
        }

        for (int i = 0; i < DialogueCharsList.Count; i++)
        {
            DialogueCharsList[i].charIndex = i;
        }
    }
#endif
}