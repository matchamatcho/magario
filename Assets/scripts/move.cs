using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; // ← これを忘れずに！
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{
    private float speed = 6.0f;
    private float jumpPower = 20.0f;

    private float playerSpeed;

    public GameObject maincamera;
    public GameObject chara;
    private float minirate = 0.99f;
    private float reducenum = 0.00007f;

    Rigidbody2D rigidbody2D;
    private AudioSource eataudio;
    public GameObject prefab;          // 生成するオブジェクトのプレハブ
    private int spawnInterval = 5;     // 何フレームごとに生成するか
    private int lifeTime = 1000;          // 生成されたオブジェクトの寿命（フレーム単位）
    private int frameCount = 0;
    private Vector2 spawnRangeMin = new Vector2(2, -5);
    private Vector2 spawnRangeMax = new Vector2(180, 10);

    private string allalphabet = "AKSTNHMYRW";
    private int alphabetcount = 0;
    private bool isgameover = false;
    private int gameovercount = 60;
    private string generated_object_name = "generated_circle";

    private HashSet<char> bebiglist = new HashSet<char>() { 'K', 'S', 'T', 'H', 'W' };
    private float eatalpha = 0.8f;
    public List<Sprite> spritelist;
    private SpriteRenderer nextsprite;
    // Unity Editor で音声を登録
    public List<AudioClip> audiolist;
    private Dictionary<string, AudioClip> audiomap = new Dictionary<string, AudioClip>();
    private string wheneat = "wheneat";
    private string wheneaten = "wheneaten";
    private bool isgoal = false;

    private float nospondis = 5.0f;

    public AudioClip stagemusic;





    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        eataudio = GetComponent<AudioSource>();
        nextsprite = GetComponent<SpriteRenderer>();

        // AudioClipリストから辞書に変換
        foreach (AudioClip clip in audiolist)
        {
            audiomap[clip.name] = clip;
        }
        if(stagemusic!=null){
            eataudio.PlayOneShot(stagemusic);
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        // 左右移動処理
        MoveUpdate();
        // ジャンプ入力処理
        JumpUpdate();
        // サイズ変更処理
        SizeChangeUpdate();
        // 物体生成
        GenerateObjects();


        Goal();
        Gameover();

        Restart();




    }
    private void Restart()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Debug.Log("aaaa");

            // 現在のシーン名を取得して再読み込み
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Debug.Log("BBB");
        }
    }

    private void MoveUpdate()
    {
        // 左キーを押したら左方向へ進む
        if (Input.GetKey(KeyCode.A))
        {
            playerSpeed = -speed;
            // Debug.Log("hidari");
        }
        // 右キーを押したら右方向へ進む
        else if (Input.GetKey(KeyCode.D)) playerSpeed = speed;
        // 何もおさなかったら止まる
        else playerSpeed = 0;

        rigidbody2D.velocity = new Vector2(playerSpeed, rigidbody2D.velocity.y);

        Vector3 pos = maincamera.transform.position;
        pos.x = chara.transform.position.x;

        maincamera.transform.position = pos;

    }

    /// <summary>
    /// Updateから呼び出されるジャンプ入力処理
    /// </summary>
    private void JumpUpdate()
    {
        // ジャンプ操作
        if (Input.GetKeyDown(KeyCode.Space))
        {// ジャンプ開始
         // ジャンプ力を計算

            // ジャンプ力を適用
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpPower);
        }
    }

    private void SizeChangeUpdate()
    {
        if (chara.transform.localScale.x < 0.1) return;
        Vector3 size = chara.transform.localScale;
        Vector3 size99 = new Vector3(reducenum, reducenum, reducenum);




        // chara.transform.localScale = size*minirate;
        chara.transform.localScale = size - size99;



    }
    private void GenerateObjects()
    {
        frameCount++;

        if (frameCount % spawnInterval == 0)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnRangeMin.x, spawnRangeMax.x),
                Random.Range(spawnRangeMin.y, spawnRangeMax.y)
            );
            float dis = Vector2.Distance(randomPos, new Vector2(chara.transform.localPosition.x, chara.transform.localPosition.y));
            if (dis < nospondis) return;


            frameCount = 0;
            GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity);
            obj.AddComponent<AutoDestroy>().Init(lifeTime);
            obj.tag = $"char_{allalphabet[alphabetcount]}";
            obj.name = generated_object_name;


            // TextMeshPro を探してテキストを変更
            TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                // tmp.text = "生成番号: " + Random.Range(1000, 9999); // 例としてランダムな番号を表示
                tmp.text = $"{allalphabet[alphabetcount]}";


                alphabetcount = (alphabetcount + 1) % (allalphabet.Length);
            }

            // Debug.Log(randomPos);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        GameObject collision_object = collision.gameObject;
        // Debug.Log("接触したタグ: " + tag);
        if (isgameover) return;
        if (collision_object.name != generated_object_name) return;

        // Vector3 objectsize = collision_object.transform.localScale;
        float nextsize = 0;
        if (!bebiglist.Contains(tag[tag.Length - 1]))
        {

            nextsize = Mathf.Max(0.1f, chara.transform.localScale.x - eatalpha * collision_object.transform.localScale.x);
            if (audiomap.ContainsKey(wheneaten))
            {
                eataudio.PlayOneShot(audiomap[wheneaten]);
            }
            else
            {
                Debug.LogWarning("音がない");
            }
        }
        else
        {
            nextsize = Mathf.Min(5.0f, chara.transform.localScale.x + eatalpha * collision_object.transform.localScale.x);
            if (audiomap.ContainsKey(wheneat))
            {
                eataudio.PlayOneShot(audiomap[wheneat]);
            }
            else
            {
                Debug.LogWarning("音がない");
            }

        }
        // Debug.Log(tag);
        // chara.transform.localScale = size*minirate;
        // Debug.Log(tag[0].GetType());
        chara.transform.localScale = new Vector3(nextsize, nextsize, nextsize);


        // タグ名と一致する画像を検索
        Sprite matchedSprite = spritelist.Find(sprite => sprite.name == tag);

        if (matchedSprite != null)
        {
            nextsprite.sprite = matchedSprite;
        }
        else
        {
            // nextsprite.sprite = matchedSprite;
            Debug.LogWarning("一致する画像が見つかりません: " + tag);
        }










        // eataudio.Play();



        Destroy(collision_object);



    }
    private void Goal()
    {
        if (isgameover) return;
        if (chara.transform.localPosition.x > 80) isgoal = true;
        if (!isgoal) return;

        SceneManager.LoadScene("clear");



    }

    private void Gameover()
    {
        if (isgoal) return;

        if (chara.transform.localScale.x <= 0.1) isgameover = true;
        if (chara.transform.localPosition.x < -1) isgameover = true;
        if (chara.transform.localPosition.y <= -5) isgameover = true;

        if (!isgameover) return;




        if (gameovercount > 0)
        {
            gameovercount--;
        }
        if (gameovercount == 0)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        isgameover = true;



    }
}
