using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //CapsuleColiderコンポーネントを入れる
    private CapsuleCollider myCollider;
    //移動させるコンポーネントを入れる
    private Rigidbody myRigidbody;
    //前方向の速度
    private float velocityZ = 16f;
    //横方向の速度
    private float velocityX = 12f;
    //上方向の速度
    private float velocityY = 4f;
    //横方向の移動量
    private float setpositionX = 2f;
    //左右の移動できる範囲
    private float movableRange = 2f;
    //横方向の現在位置
    private float nowpositionX = 0f;
    //横方向の入力による速度
    private float inputVelocityX = 0;
    //横移動許可
    private bool movableX = true;
    //スライディングのパラメータ
    private float slidespan = 1.0f;
    private float slidedelta = 0;
    //攻撃のパラメータ
    private float atkspan = 0.5f;
    private float atkdelta = 0;
    //BoxColiderコンポーネントを入れる
    private BoxCollider atkCollider;
    //Score
    private GameObject score;
    //Bossオブジェクト
    private GameObject BossCube;
    //BossのHP（仮）
    private int BossHP = 3;
    //playerの攻撃力（仮）
    private int playeratk = 1;
    //ゴール位置
    private float goalpos;
    //boss戦闘中状態
    public bool bossbattlestate = false; 


    // Start is called before the first frame update
    void Start()
    {

        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //走るアニメーションを開始
        this.myAnimator.SetFloat("Speed", 1);

        //Rigidbodyコンポーネントを取得
        this.myRigidbody = GetComponent<Rigidbody>();

        //CapsuleColiderコンポーネントを取得
        this.myCollider = GetComponent<CapsuleCollider>();

        //BoxColiderコンポーネントを取得
        this.atkCollider = GetComponent<BoxCollider>();

        //Score
        this.score = GameObject.Find("ScoreDirector");

        //Boss
        this.BossCube = GameObject.Find("BossCube");

        //ゴール位置
        this.goalpos = BossCube.transform.position.z;

    }


    // Update is called once per frame
    void Update()
    {
        
        //横方向の入力による速度
        float inputVelocityY = 0;

        //プレイヤーを矢印キーまたはボタンに応じて左右に移動させる
        if (Input.GetKeyDown(KeyCode.LeftArrow) && -this.movableRange < this.transform.position.x && movableX == true)
        {
            movableX = false;
            nowpositionX -= this.setpositionX;
            inputVelocityX = -this.velocityX;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && this.transform.position.x < this.movableRange && movableX == true)
        {
            movableX = false;
            nowpositionX += this.setpositionX;
            inputVelocityX = this.velocityX;
        }

        //横方向移動したら停止する（3か所）
        if(inputVelocityX < 0)
        {
            if (this.transform.position.x <= nowpositionX)
            {
                inputVelocityX = 0;
                this.transform.position = new Vector3(nowpositionX, this.transform.position.y, this.transform.position.z);
                movableX = true;
            }
        }
        else
        {
            if (this.transform.position.x >= nowpositionX)
            {
                inputVelocityX = 0;
                this.transform.position = new Vector3(nowpositionX, this.transform.position.y, this.transform.position.z);
                movableX = true;
            }
        }


        //ジャンプ
        if (Input.GetKeyDown(KeyCode.UpArrow) &&
            (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
        {
            //ジャンプアニメを再生
            this.myAnimator.SetBool("Jump", true);
            //上方向への速度を代入
            inputVelocityY = this.velocityY;
        }
        else
        {
            //現在のY軸の速度を代入
            inputVelocityY = this.myRigidbody.velocity.y;
        }

        //スライディング
        if (Input.GetKeyDown(KeyCode.DownArrow) &&
            (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
        {
            //スライドアニメを再生
            this.myAnimator.SetBool("Slide", true);
            myCollider.center = new Vector3(0, 0.35f, 0);
            myCollider.height = 0.3f;
            this.slidedelta += Time.deltaTime;
        }
        if (this.slidedelta > 0)
        {
            this.slidedelta += Time.deltaTime;
            if(this.slidedelta > this.slidespan)
            {
                myCollider.center = new Vector3(0, 0.8f, 0);
                myCollider.height = 1.5f;
                this.slidedelta = 0;
            }
        }

        //攻撃
        if (Input.GetKeyDown(KeyCode.Space) && 
            (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
        {
            atkCollider.enabled = true;
            this.atkdelta += Time.deltaTime;
        }
        if (this.atkdelta > 0)
        {
            this.atkdelta += Time.deltaTime;
            if (this.atkdelta > this.atkspan)
            {
                atkCollider.enabled = false;
                this.atkdelta = 0;
            }
        }

        //ボス前は停止
        if (this.transform.position.z - goalpos >= -5)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, goalpos - 5);
            velocityZ = 0;
            this.myAnimator.SetFloat("Speed", 0);
            this.bossbattlestate = true;
        }
        

        //Jumpステートの場合はJumpにfalseをセットする
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            this.myAnimator.SetBool("Jump", false);
        }
        //Slideステートの場合はSlideにfalseをセットする
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            this.myAnimator.SetBool("Slide", false);
        }

        //プレイヤーに速度を与える
        this.myRigidbody.velocity = new Vector3(inputVelocityX, inputVelocityY, velocityZ);
    }

    //攻撃当てたとき
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "enemy1")
        {
            this.score.GetComponent<ScoreController>().DefeatEnemy();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Boss")
        {
            BossHP -= playeratk;
            Debug.Log(BossHP);

            if (BossHP <= 0)
            {
                this.score.GetComponent<ScoreController>().DefeatBoss();
                Destroy(other.gameObject);
            }
        }
    }

}