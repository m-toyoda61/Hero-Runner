using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    GameObject scoreText;
    float time = 0;
    int timescore = 0;
    int timescoremagnification = 10;
    int defeatscore = 0;
    int score = 0;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        this.scoreText = GameObject.Find("Score");
        this.Player = GameObject.Find("unitychan");
    }

    // Update is called once per frame
    void Update()
    {
        //経過時間スコア
        if (this.Player.GetComponent<PlayerController>().bossbattlestate == false)
        {
            this.time += Time.deltaTime;
            this.timescore = timescoremagnification * (int)(time * 10);
        }

        //経過時間スコアと敵倒したポイントをスコアとして表示
        this.score = defeatscore + timescore;
        this.scoreText.GetComponent<Text>().text = "Score：" + this.score.ToString();
    }

    //敵を倒すとスコア加算
    public void DefeatEnemy()
    {
        this.defeatscore += 10000;
    }
    //ボスを倒すとスコア加算
    public void DefeatBoss()
    {
        this.defeatscore += 50000;
    }

}
