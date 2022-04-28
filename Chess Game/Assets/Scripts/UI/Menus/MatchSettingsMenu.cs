using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchSettingsMenu : Menu
{
    [SerializeField] MatchSettingsGO _matchSettingsGO;
    [SerializeField] TMP_Dropdown _goalTypeUI;
    [SerializeField] Toggle _atomicCapturesUI;
    [SerializeField] Toggle _aliceChessUI;

    MatchSettings _matchSettings;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        _matchSettings = new MatchSettings();
        _matchSettingsGO.MatchSettings = _matchSettings;
    }

    void UpdateMatchSettingsFromUI()
    {
        _matchSettings.Goal = (MatchSettings.GoalType)_goalTypeUI.value;
        _matchSettings.AliceChess = _aliceChessUI.isOn;
        _matchSettings.AtomicCaptures = _atomicCapturesUI.isOn;
    }

    public void Play()
    {
        UpdateMatchSettingsFromUI();
        LoadScene("Local Game");
    }
}
