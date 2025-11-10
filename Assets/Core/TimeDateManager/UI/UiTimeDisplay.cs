using System;
using UnityEngine;

/// <summary>
/// Handles time display UI with proper event-based updates
/// </summary>
public class TimeDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMPro.TMP_Text timeText;
    [SerializeField] private TMPro.TMP_Text dayText;
    [SerializeField] private TMPro.TMP_Text seasonText;

    [Header("Display Options")]
    [SerializeField] private bool use24HourFormat = false;
    [SerializeField] private bool showDayCounter = false;

    #region Unity Lifecycle
    private void Start()
    {
        SubscribeToEvents();
        UpdateAllDisplays(); // Initial update
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Event Management
    private void SubscribeToEvents()
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogError("GameTimeManager not found! TimeDisplayUI disabled.");
            enabled = false;
            return;
        }

        var timeManager = GameTimeManager.Instance;
        timeManager.OnTimeChanged += UpdateTimeDisplay;
        timeManager.OnDayChanged += UpdateDayDisplay;
        timeManager.OnSeasonChanged += UpdateSeasonDisplay;
    }

    private void UnsubscribeFromEvents()
    {
        if (GameTimeManager.Instance != null)
        {
            var timeManager = GameTimeManager.Instance;
            timeManager.OnTimeChanged -= UpdateTimeDisplay;
            timeManager.OnDayChanged -= UpdateDayDisplay;
            timeManager.OnSeasonChanged -= UpdateSeasonDisplay;
        }
    }
    #endregion

    #region Display Updates
    private void UpdateAllDisplays()
    {
        if (GameTimeManager.Instance == null) return;

        var timeManager = GameTimeManager.Instance;
        UpdateTimeDisplay(timeManager.Hour, timeManager.Minute);
        UpdateDayDisplay(timeManager.CurrentDay);
        UpdateSeasonDisplay(timeManager.CurrentSeason);
    }

    private void UpdateTimeDisplay(int hour, int minute)
    {
        if (timeText == null) return;

        timeText.text = use24HourFormat ?
            GameTimeManager.Instance.FormattedTime :
            GameTimeManager.Instance.FormattedTimeAMPM;
    }

    private void UpdateDayDisplay(DayOfWeek day)
    {
        if (dayText == null) return;

        string dayDisplay = day.ToString();
        if (showDayCounter)
        {
            dayDisplay += $" (Day {GameTimeManager.Instance.DayCounter})";
        }

        dayText.text = dayDisplay;
    }

    private void UpdateSeasonDisplay(Seasons season)
    {
        if (seasonText == null) return;

        seasonText.text = $"{GameTimeManager.Instance.FormattedSeason} - Day {GameTimeManager.Instance.DayInSeason}";
    }
    #endregion
}