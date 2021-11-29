using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInputController input;

    [Header("Invulnerable")]
    [SerializeField] int numberOfBlinks = 8;
    [SerializeField] float timeBetweenColors = 2;
    [Header("Music")]
    [SerializeField] AudioClip musicGameover;
    [Header("Sounds")]
    [SerializeField] AudioClip soundPlayerDie;
    [SerializeField] AudioClip soundCollisionWithEnemy;
    ShotsTypesController shotsTypes;
    Vector3 moveDirections = Vector3.zero;
    Rigidbody2D rb2D;
    [SerializeField] int numberOfLives = 4;
    public int Lives { get; private set; }
    public int MaxLives { get; private set; }
    [SerializeField] float speedMoveNormal = 0.5f;
    [SerializeField] float limitMoveX = 4f;
    [SerializeField] float limitMoveY = 4f;
    public float IsMove { get; private set; }
    public bool IsDead { get; private set; }
    GameObject mainHUDObject;
    MainHUD mainHUD;
    SpriteRenderer spritePlayer;
    CapsuleCollider2D colliderPlayer;
    public bool PlayerColliderWithEnemy { get; private set; }

    void Start()
    {
        GameMode.Instance.Resume();

        mainHUDObject = GameObject.FindWithTag("MainHUD");
        mainHUD = mainHUDObject.GetComponent<MainHUD>();
        spritePlayer = GetComponentInChildren<SpriteRenderer>();
        colliderPlayer = GetComponentInChildren<CapsuleCollider2D>();

        MaxLives = numberOfLives;
        Lives = numberOfLives;

        input = GetComponent<PlayerInputController>();
        shotsTypes = GetComponent<ShotsTypesController>();
        rb2D = GetComponent<Rigidbody2D>();

        PlayerColliderWithEnemy = true;
    }

    void FixedUpdate()
    {
        if (!mainHUD.IsCountSeconds)
        {
            Move(input.Movements());

            if (input.Shoot())
            {
                shotsTypes.ShootShot(input.Shoot());
            }

            Die();
        }
    }

    void Move(Vector3 move)
    {
        IsMove = move.x;

        moveDirections += move * speedMoveNormal * Time.fixedDeltaTime;
        moveDirections.x = Mathf.Clamp(moveDirections.x, -limitMoveX, limitMoveX);
        moveDirections.y = Mathf.Clamp(moveDirections.y, -limitMoveY, limitMoveY);

        rb2D.MovePosition(moveDirections);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && PlayerColliderWithEnemy)
        {
            AudioController.Instance.PlayAudioCue(soundCollisionWithEnemy);
            ShakeScreenController.Instance.ShakeNow();
            if (Lives > 0)
            {
                Lives--;
            }
            else
            {
                Lives = 0;
            }

            if (Lives > 0)
            {
                StartCoroutine(Invulnerable());
            }
        }

        if (col.CompareTag("Life"))
        {
            if (Lives < numberOfLives && Lives > 0)
            {
                Lives++;
            }
        }
    }

    IEnumerator Invulnerable()
    {
        PlayerColliderWithEnemy = false;
        int loops = numberOfBlinks;

        while (loops >= 0)
        {
            spritePlayer.color = Color.red;
            yield return new WaitForSeconds(timeBetweenColors);
            spritePlayer.color = Color.white;
            yield return new WaitForSeconds(timeBetweenColors);
            loops--;
        }
        PlayerColliderWithEnemy = true;
    }

    void Die()
    {
        if (Lives == 0 && !IsDead)
        {
            AudioController.Instance.PlayMusic(musicGameover);
            AudioController.Instance.PlayAudioCue(soundPlayerDie);
            GameMode.Instance.SlowMotion();
            colliderPlayer.enabled = false;
            PlayerColliderWithEnemy = false;
            input.enabled = false;
            IsDead = true;
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3);
        mainHUD.GameOverActive();
    }
}
