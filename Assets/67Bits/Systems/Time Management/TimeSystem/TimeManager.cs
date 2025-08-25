using UnityEngine;
using UnityEngine.InputSystem;

public class TimeManager
{
    /// <summary> FixedDeltaTime before any change. </summary>
    private static float? OriginalfixedDeltaTime;

    /// <summary> Timescale to return after pause function. </summary>
    private static float? LastNonZeroValue;

    /// <summary> Update mode before time scale zero, since ProcessEventsInFixedUpdate won't work. </summary>
    private static InputSettings.UpdateMode? InitialUpdateMode;

    public static float TimeScale
    {
        get { return GetTimeScale(); }
        set { SetTimeScale(value); }
    }

    public static bool IsPaused
    {
        get { return GetIsPaused(); }
        set { SetIsPaused(value); }
    }

    #region Timescale

    public static float GetTimeScale()
        => Time.timeScale;

    public static void SetTimeScale(in float value)
    {
        if (OriginalfixedDeltaTime == null)
            OriginalfixedDeltaTime = Time.fixedDeltaTime;

        if (InitialUpdateMode == null)
            InitialUpdateMode = InputSystem.settings.updateMode;

        Time.timeScale = value;

        if (value > 0)
        {
            Time.fixedDeltaTime = (float)OriginalfixedDeltaTime / value;
            InputSystem.settings.updateMode = (InputSettings.UpdateMode)InitialUpdateMode;
            LastNonZeroValue = value;
        }
        else
        {
            InputSystem.settings.updateMode =
                InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        }
    }

    #endregion

    #region Pause

    public static bool GetIsPaused()
        => Time.timeScale <= 0;

    public static void SetIsPaused(in bool isPaused)
    {
        if (isPaused)
            SetTimeScale(0);
        else
            SetTimeScale((LastNonZeroValue) ?? 1);
    }

    #endregion
}
