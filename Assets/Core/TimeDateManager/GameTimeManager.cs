using UnityEngine;
using System;

/// <summary>
/// Manages game time, day/night cycle, seasons and time-based events.
/// Optimized for performance and clean separation of concerns.
/// </summary>
public class GameTimeManager : MonoBehaviour
{
    #region Configuration
    [Header("Time Configuration")]
    [Tooltip("How many real-time minutes equal one game hour?")]
    [Range(0.1f, 60.0f)]
    public float realMinutesPerGameHour = 5f;

    [Header("Start Settings")]
    [Tooltip("Starting day of the week")]
    public DayOfWeek startDay = DayOfWeek.Monday;
    [Tooltip("Starting hour (0-23)")]
    [Range(0, 23)]
    public int startHour = 23;
    [Tooltip("Starting day in season")]
    [Range(1, 28)]
    public int startDayInSeason = 1;

    [Header("Season Settings")]
    [SerializeField] private int seasonLengthInDays = 28;
    #endregion

    #region Private Variables
    private float timeAccumulator;
    private float gameTimeInHours;
    private DayOfWeek currentDay;
    private int dayCounter = 0;
    private int currentDayInSeason;
    private Seasons currentSeason = Seasons.Spring;

    private float secondsPerGameMinute;
    #endregion

    #region Properties
    public static GameTimeManager Instance { get; private set; }

    // Time Properties
    public float GameTimeInHours => gameTimeInHours;
    public int Hour => Mathf.FloorToInt(gameTimeInHours);
    public int Minute => Mathf.FloorToInt((gameTimeInHours - Hour) * 60f);
    public string FormattedTime => $"{Hour:00}:{Minute:00}";
    public string FormattedTimeAMPM
    {
        get
        {
            int displayHour = Hour;
            string period = displayHour < 12 ? "AM" : "PM";

            if (displayHour == 0) displayHour = 12;
            else if (displayHour > 12) displayHour -= 12;

            return $"{displayHour:00}:{Minute:00} {period}";
        }
    }

    // Day Properties
    public DayOfWeek CurrentDay => currentDay;
    public int DayCounter => dayCounter;
    public string FormattedDay => currentDay.ToString();

    // Season Properties
    public Seasons CurrentSeason => currentSeason;
    public int DayInSeason => currentDayInSeason;
    public string FormattedSeason => GetSeasonName(currentSeason);

    // Calculated Properties
    public float TotalGameTimeInMinutes => (dayCounter * 24f * 60f) + (gameTimeInHours * 60f);
    #endregion

    #region Events
    public event Action<int, int> OnTimeChanged;           // hour, minute
    public event Action<DayOfWeek> OnDayChanged;           // new day
    public event Action<Seasons> OnSeasonChanged;          // new season
    public event Action OnGameMinutePassed;                // Every game minute
    public event Action OnGameHourPassed;                  // Every game hour
    public event Action OnNewDay;                          // New day started
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeSingleton();
        secondsPerGameMinute = realMinutesPerGameHour * 60f / 60f;
        InitializeTime();
    }

    private void Update()
    {
        UpdateGameTime();
    }
    #endregion

    #region Initialization
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTime()
    {
        currentDay = startDay;
        gameTimeInHours = startHour;
        currentDayInSeason = startDayInSeason;
        currentSeason = Seasons.Spring;

        // Trigger initial events
        TriggerTimeEvents();
        OnDayChanged?.Invoke(currentDay);
        OnSeasonChanged?.Invoke(currentSeason);
    }
    #endregion

    #region Time Update Logic
    private void UpdateGameTime()
    {
        timeAccumulator += Time.deltaTime;

        // Check if a game minute has passed
        if (timeAccumulator >= realMinutesPerGameHour)
        {
            timeAccumulator -= realMinutesPerGameHour;
            AdvanceGameMinute();
        }
    }

    private void AdvanceGameMinute()
    {
        int previousHour = Hour;

        gameTimeInHours += 1f / 60f; // Add one minute

        // Check for new day
        if (gameTimeInHours >= 24f)
        {
            gameTimeInHours -= 24f;
            AdvanceDay();
        }

        // Check for new hour
        if (Hour != previousHour)
        {
            OnGameHourPassed?.Invoke();
        }

        // Trigger events
        OnGameMinutePassed?.Invoke();
        TriggerTimeEvents();

        // Notify other systems (moved to event-based system)
        NotifyTimeBasedSystems();
    }

    private void AdvanceDay()
    {
        dayCounter++;

        // Advance day of week
        int nextDayIndex = ((int)currentDay + 1) % 7;
        currentDay = (DayOfWeek)nextDayIndex;

        // Check for season change
        AdvanceSeason();

        // Trigger events
        OnNewDay?.Invoke();
        OnDayChanged?.Invoke(currentDay);

        Debug.Log($"New Day: {currentDay} (Day {dayCounter})");
    }

    private void AdvanceSeason()
    {
        currentDayInSeason++;

        if (currentDayInSeason > seasonLengthInDays)
        {
            currentDayInSeason = 1;
            currentSeason = GetNextSeason(currentSeason);
            OnSeasonChanged?.Invoke(currentSeason);
            Debug.Log($"New Season: {currentSeason}");
        }
    }
    #endregion

    #region Event Triggers
    private void TriggerTimeEvents()
    {
        OnTimeChanged?.Invoke(Hour, Minute);
    }

    private void NotifyTimeBasedSystems()
    {
        // Let other systems subscribe to events instead of direct calls
        // This removes coupling and improves performance
    }
    #endregion

    #region Utility Methods
    private string GetSeasonName(Seasons season)
    {
        return season switch
        {
            Seasons.Winter => "Winter",
            Seasons.Spring => "Spring",
            Seasons.Summer => "Summer",
            Seasons.Fall => "Fall",
            _ => "Unknown"
        };
    }

    private Seasons GetNextSeason(Seasons currentSeason)
    {
        return currentSeason switch
        {
            Seasons.Winter => Seasons.Spring,
            Seasons.Spring => Seasons.Summer,
            Seasons.Summer => Seasons.Fall,
            Seasons.Fall => Seasons.Winter,
            _ => Seasons.Spring
        };
    }
    #endregion

    #region Time Manipulation (for Save/Load and Testing)
    public void SetTime(float hours, DayOfWeek day, int dayCount, Seasons season, int dayInSeason)
    {
        gameTimeInHours = hours;
        currentDay = day;
        dayCounter = dayCount;
        currentSeason = season;
        currentDayInSeason = dayInSeason;

        TriggerTimeEvents();
        OnDayChanged?.Invoke(currentDay);
        OnSeasonChanged?.Invoke(currentSeason);
    }

    public void AddTime(float hours)
    {
        gameTimeInHours += hours;
        while (gameTimeInHours >= 24f)
        {
            gameTimeInHours -= 24f;
            AdvanceDay();
        }
        TriggerTimeEvents();
    }
    #endregion

    #region Save/Load Support
    [System.Serializable]
    public class TimeData
    {
        public float gameTimeInHours;
        public int dayCounter;
        public DayOfWeek currentDay;
        public Seasons currentSeason;
        public int currentDayInSeason;
    }

    public TimeData GetSaveData()
    {
        return new TimeData
        {
            gameTimeInHours = this.gameTimeInHours,
            dayCounter = this.dayCounter,
            currentDay = this.currentDay,
            currentSeason = this.currentSeason,
            currentDayInSeason = this.currentDayInSeason
        };
    }

    public void LoadSaveData(TimeData data)
    {
        SetTime(data.gameTimeInHours, data.currentDay, data.dayCounter,
                data.currentSeason, data.currentDayInSeason);
    }
    #endregion
}