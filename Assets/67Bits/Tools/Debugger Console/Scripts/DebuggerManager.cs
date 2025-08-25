using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSB.Debugger
{
    public class DebuggerManager : MonoBehaviour
    {
        [Tooltip("Uncheck to auto hide debugger in realese builds")]
        [SerializeField] private bool forceDebug = true;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private Transform ui;
        [SerializeField] private Transform layout;
        [SerializeField] private TextMeshProUGUI logs;
        [SerializeField] private TextMeshProUGUI lastLog;
        [SerializeField] private ScrollRect logsView;
        public static DebuggerManager Instance { get; private set; }
        public Dictionary<string, Action> DebbugerCommands { get; private set; } = new Dictionary<string, Action>();
        private void Awake()
        {
            if (!forceDebug && Debug.isDebugBuild)
            {
                gameObject.SetActive(false);
                return; // Disable the DebbugerManager in debug builds to avoid unnecessary overhead
            }
            Instance = this;
            var methods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x => x.IsClass) // only yields classes
                .SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
                .Where(x => x.GetCustomAttributes(typeof(DebuggerAttribute), false).FirstOrDefault() != null); // returns only methods that have the InvokeAttribute

            foreach (var method in methods) // iterate through all found methods
            {
                var obj = Activator.CreateInstance(method.DeclaringType); // Instantiate the class
                AddCommand(method.Name, () => method.Invoke(obj, null)); // invoke the method
            }
            Application.logMessageReceived += HandleLog; // Subscribe to the log message event
            ui.gameObject.SetActive(false); 
            DontDestroyOnLoad(gameObject); // Ensure the DebbugerManager persists across scenes
        }
        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E)) Debug.LogError("This is an error log for testing purposes."); // Example of how to log an error for testing
            if(Input.GetKeyDown(KeyCode.L)) Debug.Log("This is an log for testing purposes."); // Example of how to log an error for testing
            if(Input.GetKeyDown(KeyCode.W)) Debug.LogWarning("This is an warning log for testing purposes."); // Example of how to log an error for testing
        }
        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            var color = type switch // Determine the color based on the log type
            {
                LogType.Error or LogType.Exception => "<color=red>",
                LogType.Assert => "<color=yellow>",
                LogType.Warning => "<color=orange>",
                _ => "<color=white>"
            };
            if (logs.text.Length > 5000) logs.text.Remove(Mathf.Max(0, 5000 - stackTrace.Length - condition.Length - 200));
            logs.text = $"{color}{condition}</color> {DateTime.Now:HH:mm:ss} [{type}] {stackTrace}\n" + logs.text; // Append the log message to the logs text
            logsView.verticalNormalizedPosition = 0; // Scroll to the end of the logs view to show the latest log
            lastLog.text = $"{color}{condition}</color> {DateTime.Now:HH:mm:ss} [{type}]"; // Update the last log text
        }

        public void AddCommand(string command, Action onExecute)
        {
            if (Instance == null)
            {
                Debug.LogWarning("DebbugerManager instance is not initialized. Make sure to have a DebbugerManager in the scene.");
                return;
            }
            if (DebbugerCommands.ContainsKey(command)) return; // If the command already exists, do not add it again
            DebbugerCommands.Add(command, onExecute);
            var newButton = Instantiate(buttonPrefab, layout);
            newButton.onClick.AddListener(() => onExecute.Invoke());
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = command; // Set the button text to the command name
            newButton.gameObject.SetActive(true); // Ensure the button is active
        }
    }
}
