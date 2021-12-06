using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public int maxHealth = 5;
    int currentHealth;
    public int health { get { return currentHealth; } }

    public float speed = 3.0f;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public GameObject projectilePrefab;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public ParticleSystem hurtEffect;
    public ParticleSystem healthUp;
    AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip throwSound;
    public Text GameOverText;
    public int score;
    public Text killBots;
    bool gameOver = false;
    bool canMove = true;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip main;
    public AudioSource Music;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        score = 0;
        audioSource = GetComponent<AudioSource>();
        killBots.text = "Fixed Robots: " + score.ToString();
        GameOverText.text = "";
        Music.clip = main;
        Music.Play();
        Music.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove == true)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer < 0)
                    isInvincible = false;

            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Launch();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    NPC character = hit.collider.GetComponent<NPC>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }



        if (score == 6)
        {
            GameOverText.text = "You Won! Game By Joseph Donnelly! Press R to restart!";
            gameOver = true;
        }

        if (health == 0)
        {
            canMove = false;
            gameOver = true;
            GameOverText.text = "You Lost! Press R to restart!";
            speed = 0;
            animator.SetFloat("Look X", 0);
            animator.SetFloat("Look Y", 0);
            animator.SetFloat("Speed", 0);
        }

        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }

        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.y = position.y + speed * vertical * Time.deltaTime;
        position.x = position.x + speed * horizontal * Time.deltaTime;

        rigidbody2d.MovePosition(position);

    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            animator.SetTrigger("Hit");
            Instantiate(hurtEffect, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
            isInvincible = true;
            invincibleTimer = timeInvincible;
            PlaySound(hurtSound);
            speed = 3.0f;


        }

        if (amount > 0)
        {
            Instantiate(healthUp, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UiHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth == 0)
        {
            Music.Stop();
            Music.clip = lose;
            Music.Play();
            Music.loop = false;
        }
    }

    public void ChangeScore(int addScore)
    {

        score += addScore;
        killBots.text = "Fixed Robots: " + score.ToString();
        if (score == 6)
        {
            Music.Stop();
            Music.clip = win;
            Music.Play();
            Music.loop = false;
        }

    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
