using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using Bello.Unity;
using UnityEngine.Events;
using Febucci.UI.Core;

namespace SSB.DialogueSystem
{
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<DialogueUI>();
                    if (_instance == null)
                        Debug.LogError("No DialogueUI found in the scene.");
                }
                return _instance;
            }
        }
        private static DialogueUI _instance;

        [Header("References"), Space(5)]
        [SerializeField] private Transform _dialogueUITransform;
        [SerializeField] private Transform _continueIndicator;

        [Header("Actor Settings"), Space(5)]
        [SerializeField] private TextMeshProUGUI _actorNameText;
        [SerializeField] private Transform _actorPosition;

        [Header("Text Settings"), Space(5)]
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private TypewriterCore _dialogueWriter;

        [FoldoutGroup("Events")] public UnityEvent<Dialogue> OnStartDialogue;
        [FoldoutGroup("Events")] public UnityEvent<Dialogue> OnFinishDialogue;

        [Header("Status"), Space(5)]
        [ReadOnly, SerializeField] private Dialogue _currentDialogue;
        [ReadOnly, SerializeField] private DialogueActor _currentActor;
        [ReadOnly, SerializeField] private int _sentenceId;
        [ReadOnly, SerializeField] private bool _skipped;
        [ReadOnly, SerializeField] private Sentence _currentSentence;

        private void Awake()
        {
            _instance = this;
            _dialogueUITransform.gameObject.SetActive(false);
        }
        public void StartDialogue(Dialogue newDialogue)
        {
            _dialogueUITransform.gameObject.SetActive(true);
            Time.timeScale = 0;
            newDialogue.OnStart?.Invoke();
            _currentDialogue = newDialogue;
            _sentenceId = 0;
            ShowSentence(newDialogue.Sentences[_sentenceId]);
            OnStartDialogue?.Invoke(_currentDialogue);
        }
        public void ShowSentence(Sentence newSentence)
        {
            _continueIndicator.gameObject.SetActive(false);
            if (_currentSentence == null || newSentence.Actor != _currentSentence.Actor)
            {
                _actorPosition.DestroyAllChildern();
                _currentActor = Instantiate(newSentence.Actor, _actorPosition);
            }
            _currentActor?.Animator.CrossFadeInFixedTime(newSentence.TalkingAnimation, newSentence.TalkTransitionDuration);
            _actorPosition.gameObject.SetActive(newSentence.UseActor);

            newSentence.OnStart?.Invoke();
            _dialogueWriter.onTextShowed.RemoveAllListeners();

            void FinishSentence()
            {
                _currentActor?.Animator.CrossFadeInFixedTime(newSentence.StaticAnimation, newSentence.StaticTransitionDuration);
                newSentence.OnFinish?.Invoke();
                _continueIndicator.gameObject.SetActive(true);
                _skipped = true;
            }
            _dialogueWriter.onTextShowed.AddListener(FinishSentence);

            _currentSentence = newSentence;

            _actorNameText.text = _currentSentence.UseActor ? newSentence.Actor.Name : newSentence.Name;
            _dialogueText.text = newSentence.Text;
        }
        public void ShowNextSentence()
        {
            _sentenceId++;
            if (_sentenceId < _currentDialogue.Sentences.Length)
            {
                ShowSentence(_currentDialogue.Sentences[_sentenceId]);
            }
            else
            {
                OnFinishDialogue?.Invoke(_currentDialogue);
                _currentDialogue.OnFinish?.Invoke();
                _dialogueUITransform.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
        public void Next()
        {
            if (_skipped)
            {
                _skipped = false;
                ShowNextSentence();
            }
            else
            {
                _skipped = true;
                _dialogueWriter.SkipTypewriter();
            }
        }
    }
}
