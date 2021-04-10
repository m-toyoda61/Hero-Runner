using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState {

    protected PlayerSetting m_Setting;
    
    public PlayerState(PlayerSetting setting) {
        m_Setting = setting;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Initialize() {
        
    }

    /// <summary>
    /// 解放
    /// </summary>
    public virtual void Release() {
        
    }
    /// <summary>
    /// 更新
    /// </summary>
    public virtual void OnUpdate() {
        
    }
}

public class PlayerStateJump : PlayerState {
    public override void Initialize() {
        m_Setting.myAnimator.SetBool("Jump",true);
        var velo = m_Setting.myRigidbody.velocity;
        velo.y = m_Setting.velocityY;
        m_Setting.myRigidbody.velocity = velo;
    }

    public override void OnUpdate() {
        Debug.Log("Jump");
    }

    public override void Release() {
        m_Setting.myAnimator.SetBool("Jump", false);
    }

    public PlayerStateJump(PlayerSetting setting) : base(setting) {
    }
}

public class PlayerStateSliding : PlayerState{
    public override void Initialize() {
        m_Setting.myAnimator.SetBool("Slide", true);
        m_Setting.myCollider.center = new Vector3(0, 0.35f, 0);
        m_Setting.myCollider.height = 0.3f;
        m_Setting.slidedelta += Time.deltaTime;
    }

    public override void OnUpdate() {
        if (m_Setting.slidedelta > 0) {
            m_Setting.slidedelta += Time.deltaTime;
            if(m_Setting.slidedelta > m_Setting.slidespan)
            {
                m_Setting.myCollider.center = new Vector3(0, 0.8f, 0);
                m_Setting.myCollider.height = 1.5f;
                m_Setting.slidedelta = 0;
            }
        }
    }

    public override void Release() {
        m_Setting.myAnimator.SetBool("Slide", false);
    }

    public PlayerStateSliding(PlayerSetting setting) : base(setting) {
    }
}

public class PlayerStateAttack : PlayerState {
    public override void Initialize() {
        m_Setting.atkCollider.enabled = true;
        m_Setting.atkdelta = 0.0f;
    }

    public override void OnUpdate() {
        m_Setting.atkdelta += Time.deltaTime;
    }

    public override void Release() {
        m_Setting.atkCollider.enabled = false;
        m_Setting.atkdelta = 0;
    }

    public PlayerStateAttack(PlayerSetting setting) : base(setting) {
    }
}

public class PlayerSetting {
    //アニメーションするためのコンポーネントを入れる
    public Animator myAnimator;
    //CapsuleColiderコンポーネントを入れる
    public CapsuleCollider myCollider;
    //移動させるコンポーネントを入れる
    public Rigidbody myRigidbody;
    //前方向の速度
    public float velocityZ = 16f;
    //横方向の速度
    public float velocityX = 12f;
    //上方向の速度
    public float velocityY = 4f;
    //横方向の移動量
    public float setpositionX = 2f;
    //左右の移動できる範囲
    public float movableRange = 2f;
    //横方向の現在位置
    public float nowpositionX = 0f;
    //横方向の入力による速度
    public float inputVelocityX = 0;
    //横移動許可
    public bool movableX = true;
    //スライディングのパラメータ
    public float slidespan = 1.0f;
    public float slidedelta = 0;
    //攻撃のパラメータ
    public float atkspan = 0.5f;
    public float atkdelta = 0;
    
    //BoxColiderコンポーネントを入れる
    public BoxCollider atkCollider;
}

public class PlayerController : MonoBehaviour
{
    public enum StateType {
        Idle = 0,
        Jump,
        Sliding,
        Attack,
    }


    private PlayerSetting Setting;
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

    private Dictionary<StateType, PlayerState> m_StateMap = new Dictionary<StateType, PlayerState>();
    
    private PlayerState m_CurrentState;


    // Start is called before the first frame update
    void Start() {
        Setting = new PlayerSetting();
        //アニメータコンポーネントを取得
        Setting.myAnimator = GetComponent<Animator>();

        //走るアニメーションを開始
        Setting.myAnimator.SetFloat("Speed", 1);

        //Rigidbodyコンポーネントを取得
        Setting.myRigidbody = GetComponent<Rigidbody>();

        //CapsuleColiderコンポーネントを取得
        Setting.myCollider = GetComponent<CapsuleCollider>();

        //BoxColiderコンポーネントを取得
        Setting.atkCollider = GetComponent<BoxCollider>();

        //Score
        this.score = GameObject.Find("ScoreDirector");

        //Boss
        this.BossCube = GameObject.Find("BossCube");

        //ゴール位置
        this.goalpos = BossCube.transform.position.z;
        
        m_StateMap.Add(StateType.Idle, new PlayerState(Setting));
        m_StateMap.Add(StateType.Jump, new PlayerStateJump(Setting));
        m_StateMap.Add(StateType.Sliding, new PlayerStateSliding(Setting));
        m_StateMap.Add(StateType.Attack, new PlayerStateAttack(Setting));

        m_CurrentState = m_StateMap[StateType.Idle];
    }

    private void ChangeState(StateType state) {
        m_CurrentState.Release();
        m_CurrentState = m_StateMap[state];
        m_CurrentState.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        //横方向の入力による速度
        float inputVelocityY = 0;

        //プレイヤーを矢印キーまたはボタンに応じて左右に移動させる
        if (Input.GetKeyDown(KeyCode.LeftArrow) && -Setting.movableRange < this.transform.position.x && Setting.movableX == true)
        {
            Setting.movableX = false;
            Setting.nowpositionX -= Setting.setpositionX;
            Setting.inputVelocityX = -Setting.velocityX;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && this.transform.position.x < Setting.movableRange &&Setting.movableX == true)
        {
            Setting.movableX = false;
            Setting.nowpositionX += Setting.setpositionX;
            Setting.inputVelocityX = Setting.velocityX;
        }

        //横方向移動したら停止する（3か所）
        if(Setting.inputVelocityX < 0)
        {
            if (this.transform.position.x <= Setting.nowpositionX)
            {
                Setting.inputVelocityX = 0;
                this.transform.position = new Vector3(Setting.nowpositionX, this.transform.position.y, this.transform.position.z);
                Setting.movableX = true;
            }
        }
        else
        {
            if (this.transform.position.x >= Setting.nowpositionX)
            {
                Setting.inputVelocityX = 0;
                this.transform.position = new Vector3(Setting.nowpositionX, this.transform.position.y, this.transform.position.z);
                Setting.movableX = true;
            }
        }

        m_CurrentState.OnUpdate();
        
        //ジャンプ
        if (Input.GetKeyDown(KeyCode.UpArrow) &&
            (Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
        {
            ChangeState(StateType.Jump);
        }

        //スライディング
        if (Input.GetKeyDown(KeyCode.DownArrow) &&
            (Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
        {
            ChangeState(StateType.Sliding);
        }

        //攻撃
        if (Input.GetKeyDown(KeyCode.Space) && 
            (Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") || Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")))
        {
            ChangeState(StateType.Attack);
        }

        //ボス前は停止
        if (this.transform.position.z - goalpos >= -5)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, goalpos - 5);
            Setting.velocityZ = 0;
            Setting.myAnimator.SetFloat("Speed", 0);
            this.bossbattlestate = true;
        }
        

        //Jumpステートの場合はJumpにfalseをセットする
        if (Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            ChangeState(StateType.Idle);
        }
        //Slideステートの場合はSlideにfalseをセットする
        if (Setting.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            ChangeState(StateType.Idle);
        }

        if (Setting.atkdelta > Setting.atkspan) {
            ChangeState(StateType.Idle);
        }

        //プレイヤーに速度を与える
        Setting.myRigidbody.velocity = new Vector3(Setting.inputVelocityX, Setting.myRigidbody.velocity.y, Setting.velocityZ);
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