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
    [SerializeField] TMP_Dropdown _playerType1;
    [SerializeField] TMP_Dropdown _playerType2;

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
        _matchSettings.PlayerType1 = (MatchSettings.PlayerType)_playerType1.value;
        _matchSettings.PlayerType2 = (MatchSettings.PlayerType)_playerType2.value;
    }

    public void Play()
    {
        UpdateMatchSettingsFromUI();
        LoadScene("Local Game");
    }
}
