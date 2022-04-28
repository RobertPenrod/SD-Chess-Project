using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : Menu
{
    [SerializeField] GameObject _menuHolder;
    [SerializeField] TextMeshProUGUI _winText;

    public void GameOver(int winningTeamNum)
    {
        _menuHolder.SetActive(true);
        _winText.text = "Team " + winningTeamNum.ToString() + " Wins!";
    }

    public void ExitGame()
    {
        GameObject matchSettingsGO = FindObjectOfType<MatchSettingsGO>()?.gameObject;
        if (matchSettingsGO != null)
        {
            Destroy(matchSettingsGO);
        }

        SceneManager.LoadScene("Main Menu");
    }
}
