using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLogic : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text hpText;
    int score;
    int hp;

    void OnEnable()
    {
        score = int.Parse(scoreText.text);
        hp = int.Parse(hpText.text);
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void IncreaseHP()
    {
        hp++;
        hpText.text = hp.ToString();
    }
}
