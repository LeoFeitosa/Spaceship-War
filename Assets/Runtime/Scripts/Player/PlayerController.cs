using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInputController input;
    ShotsTypesController shotsTypes;
    Vector3 moveDirections = Vector3.zero;
    Rigidbody2D rb2D;
    [SerializeField] int numberOfLives = 4;
    public int Lives { get; private set; }
    [SerializeField] float speedMoveNormal = 0.5f;
    [SerializeField] float limitMoveX = 4f;
    [SerializeField] float limitMoveY = 4f;
    public float IsMove { get; private set; }
    public bool IsDead { get; private set; }
    [SerializeField] GameObject playerPrefabExplosion;

    void Start()
    {
        Lives = numberOfLives;

        input = GetComponent<PlayerInputController>();
        shotsTypes = GetComponent<ShotsTypesController>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move(input.Movements());
    }

    void Update()
    {
        if (input.Shoot())
        {
            shotsTypes.ShootShot(input.Shoot());
        }

        Die();
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
        if (col.CompareTag("Enemy"))
        {
            if (Lives >= 1)
            {
                Lives--;
            }
        }

        if (col.CompareTag("Life"))
        {
            if (Lives < numberOfLives)
            {
                Lives++;
            }
        }
    }

    void Die()
    {
        if (Lives == 0)
        {
            input.enabled = false;
            IsDead = true;
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3);
        GameOverController.Instance.GameOverActive();
    }
}
