using UnityEngine;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using Facebook.Unity;
using GameAnalyticsSDK;


/*
 * Class Description: public class GameManager
 *  Controls game state, updates highscore, updates coin amount, saves current game progress, and loads game upon reopening app
 *  Also controls game mechanic 'MoveDown()' which enables objects that can move down to move down
 *  Also controls game mechanic 'SpawnBalls()' which spawns balls at Vector2 position of first ball to touch bottom barrier after ball has been flung
 */

public class GameManager : MonoBehaviour
{
    enum GameState { Preparing, Ready, Active };
    enum GameRunning { Running, NotRunning };

    public delegate void DeathAction();
    public static event DeathAction OnDeath;
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    public delegate void OnStartRound();
    public static event OnStartRound OnNewRound;

    //game UI's
    public Text scoreUI;
    public Text highScoreUI, coinUI;

    public static GameManager gm = null;

    // game performance
    public int score = 0, highScore, coinsCollected, gamesPlayed, gameRound, adFrequency = 6, numBalls = 1,
        groundedBalls = 1, ballsCollected = 1, blockColor = 1, numFeed = 0, numBlocks = 0, numGems = 0;
    public bool newRound, firstLand, gameOn;
    public Transform target;
    public GameObject spawner, ballClone, touchScreen, mainBall, lighting, finger;
    GameState gameState;
    GameRunning gameRunning;
    private float timeSinceRoundStart;



    void Start()
    {
        gameRunning = GameRunning.NotRunning;
        highScore = PlayerPrefManager.GetHighscore();
        UpdateHighScore();
        // setup reference to game manager
        if (gm == null)
        {
            gm = this.GetComponent<GameManager>();
        }
        else
            Destroy(gameObject);

        if (PlayerPrefManager.GetBool("gameRunning") == true)
        {
            Debug.Log("gamerunning");
            numBalls = 1;
            LoadGame();

            coinUI.text = coinsCollected.ToString();
            scoreUI.text = score.ToString();

            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }

     

        }
        else
        {

            coinsCollected = PlayerPrefManager.GetCoins();
            coinUI.text = coinsCollected.ToString();

            highScore = PlayerPrefManager.GetHighscore();


            //getGamesPlayed
            gamesPlayed = PlayerPrefManager.GetGamesPlayed();


            //update score
            if (scoreUI)
                scoreUI.text = score.ToString();


            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }
        }
        Debug.Log(highScore);
    }

    void Update()
    {

        if (gameState == GameState.Active)
        {
            timeSinceRoundStart += Time.deltaTime;

            if (timeSinceRoundStart > 10.0f)
                lighting.SetActive(true); // turn on option to increase speed of balls
        }

        // on finger up
        if ((Input.GetButtonUp("Touch") || CrossPlatformInputManager.GetButtonUp("Touch")) && gameState == GameState.Ready)
        {
            if (mainBall.GetComponent<PlayerInput>().FlingBalls() == true)
            {
                timeSinceRoundStart = 0.0f;
                gameState = GameState.Active;
                firstLand = false;
                touchScreen.SetActive(false);
                finger.SetActive(false);
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }
        }
    }

    public void UpdateHighScore()
    {
        if (highScoreUI)
            highScoreUI.text = highScoreUI.text + " " + PlayerPrefManager.GetHighscore().ToString();
    }

    public bool CheckBallCount()
    {
        if (numBalls == groundedBalls)
        {
            gameState = GameState.Preparing;
            StartNewRound();
            newRound = false;
            return true;
        }
        else
            return false;
    }

    public void StartNewRound()
    {

        lighting.SetActive(false);//turn off option to increase speed of balls

        if (blockColor > 70)//since hue can't go over 359 (and it's blockColor * 5)
            blockColor = 1;//set back to 1
        blockColor++;//else go up every time gameRound increases
        gameRound++;
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Coin");
        GameObject[] ballSpawn = GameObject.FindGameObjectsWithTag("BallSpawn");
        MoveDown(blocks);
        MoveDown(gems);
        MoveDown(ballSpawn);


        if (numBalls < ballsCollected)
        {
            SpawnBalls();
        }

        spawner.GetComponent<BlockSpawner>().SpawnBlocks();
        touchScreen.SetActive(true);

        StartCoroutine(Wait());
    }

    public void Ready()
    {
        gameState = GameState.Ready;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        touchScreen.SetActive(true);
        Ready();
    }

    void MoveDown(GameObject[] prefabs)
    {
        for (int i = 0; i < prefabs.Length; i++)
            if (prefabs[i].GetComponent<MoveDown>())
            {
                prefabs[i].GetComponent<MoveDown>().SetNewPos();
                prefabs[i].GetComponent<MoveDown>().isMoving = true;
            }
    }

    // public function to add points and update the gui and highscore player prefs accordingly
    public void AddPoints(int amount)
    {
        // increase score
        score += amount;
        // if score>highscore then update the highscore UI too
        if (score > highScore)
        {
            if (score < 9999999)
            {
                highScore = score;

            }
            PlayerPrefManager.SetHighscore(highScore);
        }

        //update score
        scoreUI.text = score.ToString();
    }


    // starts a new game
    public void PlayGame()
    {
        Debug.Log("Game Start");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
        touchScreen.SetActive(true);


        //set up score
        score = 0;
        //set round to one
        gameRound = 1;
        //set hue to one
        blockColor = 1;
        //set num of balls to one
        numBalls = 1;
        //reset the number of grounded balls to zero
        // since main ball will always be used simply enable it in sceen
        mainBall.SetActive(true);
        // enable spawner
        spawner.SetActive(true);

        groundedBalls = 0;

        scoreUI.text = score.ToString();

        MainMenuManager.instance.MenuHandler();

        //set game state to ready so player can play game
        Ready();
        gameRunning = GameRunning.Running;
        finger.SetActive(true);
    }

    // public funcntion when player crashes
    public void GameOver()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", score);

        //destroy all objects
        DestroyAll();

        //set num of balls to one
        numBalls = 1;
        //reset the number of grounded balls to zero
        groundedBalls = 0;

        ballsCollected = 1;

        //update number of games played
        gamesPlayed += 1;

        //disable main ball
        mainBall.SetActive(false);
        //disable spawner in scene
        spawner.SetActive(false);


        //should an ad pop up after game?
        if (gamesPlayed % adFrequency == 0 && PlayerPrefManager.GetNoAdsPurchased() == false) //check if product id for no ads is false
        {
            Debug.Log("Ad is showing");
            if (PlayAd.pa)
                PlayAd.pa.ShowAd();
        }

        if (highScore > 1000)// **find better substitue for when player should be requested to play a game**
        {
            iOSReviewRequest.Request();
        }

        // update GUI
        SavePlayerState();

        MainMenuManager.instance.MenuHandler();

        //set highscore active
        if (highScoreUI)
            highScoreUI.text = "Best score: " + PlayerPrefManager.GetHighscore().ToString();
        else
            Debug.LogWarning("High Score UI not set");

        gameRunning = GameRunning.NotRunning;
        PlayerPrefManager.SetBool("gameRunning", false);
    }

    public void AddCoin(int value)
    {
        coinsCollected += value;
        coinUI.text = coinsCollected.ToString();
    }

    public void SavePlayerState()
    {

        PlayerPrefManager.SavePlayerState(score, highScore, coinsCollected, gameRound);
        PlayerPrefManager.SetGamesPlayed(gamesPlayed);
    }

    public void SpawnBalls()
    {
        for (int i = numBalls; i < ballsCollected; i++)
        {
            GameObject newBall = Instantiate(ballClone, target);
            numBalls++;

            if (newBall != null)
            {
                newBall.gameObject.name = "ball " + numBalls;
                if (target != null && newBall != null)
                    newBall.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, 0);
                newBall.transform.localScale = new Vector3(1, 1, 1);
                newBall.transform.parent = null;
            }
        }
    }

    public void DestroyAll()
    {
        OnDeath();
    }

    public void SpeedUp()
    {
        if (gameState == GameState.Active)
            OnClicked();
    }

    void OnApplicationQuit()
    {

        if (gameRunning == GameRunning.Running)
        {
            SaveGame();
        }
        else
            PlayerPrefManager.SetBool("gameRunning", false);
    }

    void SaveGame()
    {
        PlayerPrefs.SetInt("numBalls", ballsCollected);
        PlayerPrefs.SetInt("gameRound", gameRound);
        PlayerPrefs.SetInt("numGems", coinsCollected);
        PlayerPrefs.SetInt("numFeed", numFeed);
        PlayerPrefs.SetInt("numBlocks", numBlocks);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefManager.SetBool("gameRunning", true);
    }

    void LoadGame()
    {
        if (PlayerPrefs.HasKey("numBalls"))
            ballsCollected = PlayerPrefs.GetInt("numBalls");
        else
            ballsCollected = 1;

        if (PlayerPrefs.HasKey("gameRound"))
        {
            gameRound = PlayerPrefs.GetInt("gameRound");
            blockColor = PlayerPrefs.GetInt("gameRound");
        }
        else
        {
            gameRound = 1;
            blockColor = 1;
        }

        if (PlayerPrefs.HasKey("numGems"))
            coinsCollected = PlayerPrefs.GetInt("numGems");
        else
            coinsCollected = PlayerPrefManager.GetCoins();

        if (PlayerPrefs.HasKey("numFeed"))
            numFeed = PlayerPrefs.GetInt("numFeed");
        else
            numFeed = 0;

        if (PlayerPrefs.HasKey("numBlocks"))
            numBlocks = PlayerPrefs.GetInt("numBlocks");
        else
            numBlocks = 0;

        if (PlayerPrefs.HasKey("score"))
            score = PlayerPrefs.GetInt("score");
        else
            score = 0;

        // since main ball will always be used simply enable it in sceen
        mainBall.SetActive(true);
        // enable spawner
        spawner.SetActive(true);

        MainMenuManager.instance.MenuHandler();

        //set game state to ready so player can play game
        Ready();
        gameRunning = GameRunning.Running;
        finger.SetActive(true);

        SpawnBalls();

    }










}
