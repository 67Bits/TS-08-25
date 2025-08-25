using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSB.Debugger
{
    public class DebuggerDefaultCommands : MonoBehaviour
    {
        [Debugger]
        public void TimeScaleUp()
        {
            Time.timeScale += 0.25f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f; // Adjust fixed delta time to match the new time scale
            Debug.Log($"Time Scale increased to {Time.timeScale}");
        }
        [Debugger]
        public void TimeScaleDown()
        {
            Time.timeScale -= 0.25f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f; // Adjust fixed delta time to match the new time scale
            Debug.Log($"Time Scale decreased to {Time.timeScale}");
        }
        [Debugger] public void NextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        [Debugger] public void PreviousScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        [Debugger] public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        [Debugger] public void ShowDeveloperConsole() => Debug.developerConsoleVisible = true;
        [Debugger] public void HideDeveloperConsole() => Debug.developerConsoleVisible = false;
    }
}
