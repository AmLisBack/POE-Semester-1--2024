using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class score : MonoBehaviour
{
    private float redScore = 0;
    private float blueScore = 0;

    public GameObject[] blueTally;
    public GameObject[] redTally;

    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI roundText;
    public GameObject panel;
    public GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        redScore = EnemyFinite.enemyScore;
        blueScore = EnemyFinite.playerScore;
        for(int i = 0; i < blueTally.Length; i++)
        {
            blueTally[i].SetActive(false);
        }
        for(int x = 0; x < redTally.Length; x++)
        {
            redTally[x].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float round = redScore + blueScore + 1;
        roundText.text = "Round " + round.ToString();
        redScore = EnemyFinite.enemyScore;
        blueScore = EnemyFinite.playerScore;
        switch (redScore)
        {
            case 0:
                break;

            case 1:
                redTally[0].SetActive(true);
                break;

            case 2:
                redTally[1].SetActive(true);
                break;

            case 3:
                redTally[2].SetActive(true);
                break;

            case 4:
                redTally[3].SetActive(true);
                break;
            case 5:
                redTally[4].SetActive(true);
                panel.SetActive(true);
                winnerText.gameObject.SetActive(true);
                winnerText.text = "Red has won! You will get them next time";
                break;
        }
        switch (blueScore)
        {
            case 0:
                break;

            case 1:
                blueTally[0].SetActive(true);
                break;

            case 2:
                blueTally[1].SetActive(true);
                break;

            case 3:
                blueTally[2].SetActive(true);
                break;

            case 4:
                blueTally[3].SetActive(true);
                break;
            case 5:
                blueTally[4].SetActive(true);
                panel.SetActive(true);
                winnerText.gameObject.SetActive(true);
                winnerText.text = "You have won! Congratulations";
                break;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

}
