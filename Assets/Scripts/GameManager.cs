using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RamailoGames;

public class GameManager : MonoBehaviour
{

    #region Tmp Text

    public TMP_Text GameOverScoreText;
    public TMP_Text GameOverhighscoreText;
    public TMP_Text gamePlayScoreText;
    public TMP_Text gamePlayhighscoreText;
    public TMP_Text timeText;

    #endregion

    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    #endregion

    #region Transforms
    public Transform[] spawnPos;
    public Transform[] rayCastPos;
    #endregion

    #region Prefabs
    [SerializeField] GameObject ballsPrefab;
    #endregion

    #region List of objects
    public List<BallsWithColor> availableColors;
    public List<PowerUpWithType> availablePowerUps;
    #endregion


    #region Private Serialized Fields
    [SerializeField] int score;
    [SerializeField] float remainingTime;
    [SerializeField] float warningTime;
    #endregion

    #region Private Fields

    bool paused;
    float startTime;
    bool gameStarted;
    float highscore;

    #endregion

    #region Public Fields
    [Range(0,100)]
    public int powerUpSpawnChance;
    #endregion

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        ScoreAPI.GameStart((bool s) => {
        });
        StopCoroutine(nameof(SpawnBallsCour));
        StartCoroutine(nameof(SpawnBallsCour));
        Time.timeScale = 1;
        startTime = Time.time;
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                highscore = d.high_score;
            }
        });
        setHighScore(gamePlayhighscoreText);

    }


    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
            return;
        if (UIManager.instance.activeUIPanel.uiPanelType != UIPanelType.mainGame)
            return;
        if(remainingTime <= 0)
        {
            GameOver();
        }
        
        else
        {
            remainingTime -= Time.deltaTime;
            timeText.text = ((int)remainingTime).ToString();
            if (remainingTime < warningTime)
            {
                SoundManager.instance.PlaySound(SoundType.warningSound);
            }
            else
            {
                SoundManager.instance.StopSound(SoundType.warningSound);
            }
        }
        if (!checkBallHit())
        {
            SpawnBall();
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Ball"))
                {
                    BallsController controller = hit.collider.gameObject.GetComponent<BallsController>();
                    if (controller != null)
                    {
                        if (controller.hasConnectedLine() || controller.myType==PowerUpType.deleteTime)
                        {
                            controller.DestroyOnlyMe();
                            SoundManager.instance.PlaySound(SoundType.popSound);
                        }
                    }

                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        foreach (Transform pos in rayCastPos)
        {
            Gizmos.DrawLine(pos.position, new(pos.position.x + 10,pos.position.y));
        }
    }

    #endregion

    #region Public Functions

    public void StartGame()
    {
        gameStarted = true;
        UIManager.instance.SwitchCanvas(UIPanelType.mainGame);
    }
    public BallsWithColor GetRandomColor()
    {
        return availableColors[Random.Range(0, availableColors.Count)];
    }
    public PowerUpWithType GetRandomPowerUp()
    {
        return availablePowerUps[Random.Range(0, availablePowerUps.Count)];
    }
    public void PauseGame()
    {
        
        Time.timeScale = 0;
        paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        paused = false;
    }

    public void AddScore(int amount)
    {
        score += amount ;
        gamePlayScoreText.text = score.ToString();
        setHighScore(gamePlayhighscoreText);
    }
    public void AddTime(float amount)
    {
        remainingTime += amount;
    }
    public void ReduceTime(float amount)
    {
        remainingTime -= amount;
        if (remainingTime <= 0)
            remainingTime = 0;
    }
    #endregion

    #region Private Functions
    bool checkBallHit()
    {
        foreach (Transform pos in rayCastPos)
        {
            RaycastHit2D ray = Physics2D.Raycast(pos.position, Vector2.right, 10);
            if (ray.collider != null)
            {
                if (ray.collider.CompareTag("Ball"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    void SpawnBall()
    {
        for (int j = 0; j < 4; j++)
        {
            float posx = Random.Range(spawnPos[0].position.x, spawnPos[1].position.x);
            Instantiate(ballsPrefab, new(posx, spawnPos[0].position.y, 0), Quaternion.identity);
        }
    }
    void GameOver()
    {
        PauseGame();
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        GameOverScoreText.text =score.ToString();
        int playTime = (int)(Time.unscaledTime - startTime);
        ScoreAPI.SubmitScore(score, playTime, (bool s, string msg) => { });
        GetHighScore();
        SoundManager.instance.StopSound(SoundType.warningSound);

    }

    void setHighScore(TMP_Text highscroreTextUI)
    {
        if (score >= highscore)
        {
            highscore = score;
        }
        highscroreTextUI.text = highscore.ToString();
    }

    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                if (score >= d.high_score)
                {
                    GameOverhighscoreText.text = score.ToString();

                }
                else
                {
                    GameOverhighscoreText.text =d.high_score.ToString();
                }

            }
        });

    }

    #endregion

    #region Coroutines
    IEnumerator SpawnBallsCour()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float posx = Random.Range(spawnPos[0].position.x, spawnPos[1].position.x);
                Instantiate(ballsPrefab, new(posx,spawnPos[0].position.y,0), Quaternion.identity);
            }
            yield return new WaitForSeconds(.5f);
        }
    }
    #endregion
}
