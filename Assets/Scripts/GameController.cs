using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
    public GameObject hazard;
    public int hazardCount;

    public GameObject hazard2;
    public int hazardCount2;

    public GameObject hazard3;
    public int hazardCount3;

    public GameObject enemy;
    public int enemyCount;

    public Vector3 spawnValues;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public GUIText scoreText;
    public GUIText currUserBestScoreText;
    public GUIText bestScoreText;
    
    public GUIText menuText;

    public GUIText restartText;
    public GUIText levelText;

    public GUIText gameOverText;
    public GUIText tonyText;	
    public GUIText unityText;
    public GUIText versionText;

    private bool isGameOver;
    private bool isRestart;
    private bool isResetGame;
    private bool isMuteMusic;

    private int iScore;
    private int iCurrBestScore;
    private int iBestScore;
    public bool isPause;
    private string txtMuteMusic;
    private int isLoaded;
    private string strCurrPlayerName;
    private string strBestPlayerName;
    private int iLevel;

    private int iNextLevelScore;
    public int NextLevelAt;
    
    private float native_width, native_height;

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds (startWait);
        while(true)
        {
            for(int i = 0; i < hazardCount; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds (spawnWait);
            }
            yield return new WaitForSeconds (waveWait);

            if(isGameOver)
            {
                //restartText.text = "Press 'R' for Restart or Touch Screen";
                restartText.text = "Touch Screen to Restart";
                isRestart = true;
                break;
            }
        }
    }

    void Start()
    {
        isGameOver = false;
        isRestart = false;
        isPause = false;
        restartText.text = "";
        gameOverText.text = "";
        currUserBestScoreText.text = "";
        bestScoreText.text = "";
        scoreText.text = "";		
        menuText.text = "Menu";
        tonyText.text = "";
        unityText.text = "";
        versionText.text = "";

        isMuteMusic = (PlayerPrefs.GetInt("isMuteMusic") == 1);
        SetMusic();

        isLoaded = PlayerPrefs.GetInt("isLoaded");

        //iNextLevelScore = PlayerPrefs.GetInt("NextLevelScore");
        //iLevel = PlayerPrefs.GetInt("Level");

        if(iNextLevelScore == 0){iNextLevelScore = NextLevelAt;}
        if(iLevel == 0){iLevel = 1;}

        iScore = 0;
//		iCurrBestScore = PlayerPrefs.GetInt(strCurrPlayerName);
        iCurrBestScore = PlayerPrefs.GetInt("CurrBestScore");

//		iBestScore = PlayerPrefs.GetInt("BestScore");
//		strBestPlayerName = PlayerPrefs.GetString("BestPlayerName");

        UpdateScore();
        StartCoroutine (SpawnWaves());
        
        native_width = 600;
        native_height = 1024;
            
        float rx, ry; 
        rx = Screen.width / native_width;
        ry = Screen.height / native_height;

        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

        FixLabels();
    }

    void FixLabels()
    {
        scoreText.fontSize = Screen.height / 36;
        currUserBestScoreText.fontSize = Screen.height / 44;
        bestScoreText.fontSize = Screen.height / 44;

        menuText.fontSize = Screen.height / 43;

        levelText.fontSize = Screen.height / 36;
        restartText.fontSize = Screen.height / 44;

        gameOverText.fontSize = Screen.height / 27;
        tonyText.fontSize = Screen.height / 43;
        unityText.fontSize = Screen.height / 43;
        versionText.fontSize = Screen.height / 43;
    }

    void GetPlayerName()
    {
        string _player_name = "";
        string _tooltip = "Name?";

        GUI.BeginGroup(new Rect(0, 0, 200, 30), new GUIContent("", _tooltip));
        _player_name = GUI.TextField(new Rect(0, 0, 200, 30), _player_name);
        GUI.EndGroup();


        if(isLoaded == 0)
        {
            strCurrPlayerName = "Tony 1";
        }
        else
        {
            strCurrPlayerName = "Tony 2";
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
//			PlayerPrefs.SetInt("isLoaded", 0);
            SaveGameData();
            return;
        }

        if(!isGameOver)
        {
            if(Input.GetKeyUp(KeyCode.M) || Input.GetKeyUp(KeyCode.P) || Input.mousePresent && Input.GetMouseButton(1))
            {
                Pause();
                return;
            }

            foreach (Touch touch in Input.touches) 
            {			
                if (!isPause)
                {
                    if (menuText.HitTest (touch.position)) 
                    {
                        Pause();
                    } 
                }
            }
        }

        if(isRestart)
        {
            if(Input.GetKeyDown(KeyCode.R) || Input.mousePresent && Input.GetMouseButton(0) || Input.touchCount > 0)
            {
//				PlayerPrefs.SetInt("isLoaded", 1);
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

    void OnGUI()
    {
        //GetPlayerName();

        if(isPause)
        {
            float iPos, btnWidth, btnHeight;
            btnWidth = Screen.width/2.5f;
            btnHeight = Screen.height/5;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = Screen.height / 25;
            btnStyle.normal.textColor = Color.white;
            btnStyle.hover.textColor = Color.red;

            GUI.backgroundColor = Color.white;
            //GUI.Box(new Rect(10, 120, Screen.width / 1.6f, Screen.height / 1.2f), "Pause");

            if(isMuteMusic){txtMuteMusic = "Music On";}
            else{txtMuteMusic = "Music Off";}

            if(GUI.Button(new Rect(10, 10, btnWidth, btnHeight), "Play", btnStyle))
            {
                isPause = false;
                Pause();
            }
            iPos = 10 + btnHeight + 10;
            if(GUI.Button(new Rect(10, iPos, btnWidth, btnHeight), txtMuteMusic, btnStyle))
            {
                GameMusic();
            }
            iPos = iPos + btnHeight + 10;
            if(GUI.Button(new Rect(10, iPos, btnWidth, btnHeight), "Reset Game", btnStyle))
            {
                ResetGame();
            }
            iPos = iPos + btnHeight + 10;
            if(GUI.Button(new Rect(10, iPos, btnWidth, btnHeight), "Quit", btnStyle))
            {
                SaveGameData();
            }
        }
    }

    void ResetGame()
    {
        PlayerPrefs.DeleteAll();

        isPause = false;
        Pause();

        Application.LoadLevel(Application.loadedLevel);
    }

    void SetMusic()
    {
        this.gameObject.GetComponent<AudioSource>().mute = isMuteMusic;
    }

    void GameMusic()
    {
        isMuteMusic = !isMuteMusic;
        SetMusic();
    }

    void Pause()
    {
        if(Time.timeScale == 1)
        {
            Time.timeScale = 0;
            isPause = true;
            menuText.text = "";
        }
        else
        {
            Time.timeScale = 1;
            isPause = false;
            menuText.text = "Menu";
        }
    }

    public void AddScore(int newScoreValue)
    {
        iScore += newScoreValue;
        UpdateScore();
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over";
        tonyText.text = "Game by Tony Studios";
        unityText.text = "Powered by Unity";
        versionText.text = "Space Shooter v1.02";
        menuText.text = "";
        isGameOver = true;

        SaveGameData();
    }

    void SaveScore()
    {
        if(iScore > iCurrBestScore)
        {
            iCurrBestScore = iScore;
        }
        //PlayerPrefs.SetInt(strCurrPlayerName, iCurrBestScore);	
        PlayerPrefs.SetInt("CurrBestScore", iCurrBestScore);
        currUserBestScoreText.text = strCurrPlayerName + " Best Score: " + iCurrBestScore;
        currUserBestScoreText.text = currUserBestScoreText.text.Trim();

//		if(iScore > iBestScore)
//		{
//			iBestScore = iScore;
//			strBestPlayerName = strCurrPlayerName;
//		}
//		PlayerPrefs.SetInt("BestScore", iBestScore);
//		PlayerPrefs.SetString("BestPlayerName", strBestPlayerName);
//		bestScoreText.text = strBestPlayerName + " Best Score: " + iBestScore;
//		bestScoreText.text = bestScoreText.text.Trim();

        //PlayerPrefs.GetInt("NextLevelScore", iNextLevelScore);
        //PlayerPrefs.SetInt("Level", iLevel);
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + iScore;
        currUserBestScoreText.text = strCurrPlayerName + " Best Score: " + iCurrBestScore;
        currUserBestScoreText.text = currUserBestScoreText.text.Trim();
//		bestScoreText.text = strBestPlayerName + " Best Score: " + iBestScore;
//		bestScoreText.text = bestScoreText.text.Trim();

        if(iScore >= iNextLevelScore)
        {
            iNextLevelScore += NextLevelAt;
            iLevel++;
        }

        levelText.text = "Level: " + iLevel;
    }

    void SaveGameData()
    {
        SaveScore();

        PlayerPrefs.SetInt("isMuteMusic", isMuteMusic ? 1 : 0);

        Application.Quit();
    }
}
