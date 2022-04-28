using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettingsGO : MonoBehaviour
{
    public MatchSettings MatchSettings; 

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
