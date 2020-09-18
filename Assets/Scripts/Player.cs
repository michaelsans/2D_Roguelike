using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointPerBarril;
    public int pointPerPotion;
    public int health;
    public float restartLevelDelay = .1f;
    public Text HP;
    public Text Points;

    private Animator anim;
    private int points;

    protected override void Start()
    {
        anim = GetComponent<Animator>();

        points = GameManager.instance.playerPoints;
        health = GameManager.instance.playerHealthPoints;
        HP.text = "HP: " + health;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerPoints = points;
    }
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Walls>(horizontal, vertical);
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
        }
        CheckIfGameOver();
        
        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Escada")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(other.tag == "Barril")
        {
            points += pointPerBarril;
            Points.text = "Points: " + pointPerBarril;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Pocao")
        {
            points += pointPerPotion;
            Points.text = "Points: " + pointPerPotion;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Walls hitWall = component as Walls;
        hitWall.DamageWall(wallDamage);
        anim.SetTrigger("Chop");
    }

    [System.Obsolete]
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void loseHP (int loss)
    {
        anim.SetTrigger("hit");

        health -= loss;
        HP.text = "HP: " + health;
        CheckIfGameOver();
    }
    private void CheckIfGameOver()
    {
        if (health <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
