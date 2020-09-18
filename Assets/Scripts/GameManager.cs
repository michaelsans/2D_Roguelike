using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = .1f;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardscript;
    public int playerHealthPoints = 100;
    public int playerPoints = 0;
    [HideInInspector]public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardscript = GetComponent<BoardManager>();
        InitGame();
    }
    private void OnLevelWasLoaded(int index)
    {
        
        InitGame();
        level++;
    }
    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("levelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Floor " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardscript.SetupScene(level);
    }
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    public  void GameOver()
    {
        levelText.text = "After reaching floor" + level + " you died.";
        levelImage.SetActive(true);
        enabled = false;
    }
    void Update()
    {
        if (playersTurn || enemiesMoving)
            return;
        StartCoroutine(MoveEnemies());
    }
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
