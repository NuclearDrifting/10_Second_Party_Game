using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField]
    private float speed;
    bool isOnGround;
    [SerializeField]
    private float jumpForce;
    bool gamestarted = false;
    bool gamefinished = false;
    bool playerlost = false;

    [SerializeField]
    private Sprite left;
    [SerializeField]
    private Sprite right;
    [SerializeField]
    private Sprite fallLeft;
    [SerializeField]
    private Sprite fallRight;
    [SerializeField]
    private SpriteRenderer SpriteRenderer;

    [SerializeField]
    private TextMeshProUGUI score;
    [SerializeField]
    private TextMeshProUGUI timeLeft;
    [SerializeField]
    private TextMeshProUGUI result;
    [SerializeField]
    private float timeleft = 10;
    private string time;
    private int scoreValue = 0;

    [SerializeField]
    private AudioSource EnvironmentAudio;
    [SerializeField]
    private AudioSource Crunch;
    [SerializeField]
    private AudioSource Win;
    [SerializeField]
    private AudioSource Lose;
    [SerializeField]
    private AudioClip background;
    [SerializeField]
    private AudioClip win;
    [SerializeField]
    private AudioClip lose;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        SpriteRenderer.sprite = right;
        result.text = "Press P to start";
        EnvironmentAudio.Play();
    }

    void Update()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        rb2d.velocity = new Vector2(hozMovement * speed, rb2d.velocity.y);
        score.text = ($"Score: {scoreValue.ToString()}");
        UpdateTimer();

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        Jump();
        SpriteUpdate();
        States();
    }
    private void UpdateTimer()
    {
        if (Input.GetKey(KeyCode.P) && gamefinished == false)
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            gamestarted = true;
            result.text = "";
            EnvironmentAudio.clip = background;
        }

        if (gamestarted == true)
        {
            timeleft = timeleft -= Time.deltaTime;
            timeLeft.text = ($"Time Left: {timeleft.ToString("F1")}");
        }

        if (timeleft < 0.04)
        {
            gamestarted = false;
            gamefinished = true;
            playerlost = true;
            rb2d.bodyType = RigidbodyType2D.Static;
            States();
        }
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.W) && isOnGround == true)
        {
            isOnGround = false;

            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    //fix default state being looking right unless you are actively moving left
    private void SpriteUpdate()
    {
        if (rb2d.velocity.x <= 0 && rb2d.velocity.y > -0.01 && rb2d.velocity.y < 0.01)
        {
            SpriteRenderer.sprite = left;
        }
        if (rb2d.velocity.x >= 0 && rb2d.velocity.y > -0.01 && rb2d.velocity.y < 0.01)
        {
            SpriteRenderer.sprite = right;
        }

        if (rb2d.velocity.x <= 0 && rb2d.velocity.y < -0.1)
        {
            SpriteRenderer.sprite = fallLeft;
        }
        if (rb2d.velocity.x >= 0 && rb2d.velocity.y < -0.1)
        {
            SpriteRenderer.sprite = fallRight;
        }

        if (rb2d.velocity.x <= 0 && rb2d.velocity.y > 0.1)
        {
            SpriteRenderer.sprite = fallLeft;
        }
        if (rb2d.velocity.x >= 0 && rb2d.velocity.y > 0.1)
        {
            SpriteRenderer.sprite = fallRight;
        }
    }

    private void States()
    {
        if (scoreValue == 5)
        {
            gamestarted = false;
            gamefinished = true;
            rb2d.bodyType = RigidbodyType2D.Static;
            result.text = "You won!\nPress R to restart.\nPres Q to quit.";
            Win.Play();
        }

        if (scoreValue < 5 && playerlost == true)
        {
            gamestarted = false;
            gamefinished = true;
            rb2d.bodyType = RigidbodyType2D.Static;
            result.text = "You lost!\nPress R to restart.\nPres Q to quit.";
            Lose.Play();
        }

        if (Input.GetKey(KeyCode.R) && gamefinished == true)
        {
            SceneManager.LoadScene("SampleScene");
        }

        if (Input.GetKey(KeyCode.Q) && gamefinished == true)
        {
            Application.Quit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Seed")
        {
            Crunch.Play();
            scoreValue += 1;
            score.text = scoreValue.ToString();
            Destroy(collision.collider.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && isOnGround == false)
        {
            isOnGround = true;
        }
    }
}