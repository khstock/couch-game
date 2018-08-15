using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CG_Player : MonoBehaviour
{
    public string name;
    public int score;
    public bool host;

    public TextMeshProUGUI tmp_name;
    public TextMeshProUGUI tmp_score;
    public TextMeshProUGUI tmp_host;

    public void refresh()
    {
        tmp_name.text = name;
        tmp_score.text = "Score : " + score;
        if (host) tmp_host.alpha = 1;
        Debug.Log("refreshed");
    }
}
