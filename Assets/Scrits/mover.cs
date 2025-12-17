using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mover : MonoBehaviour
{
    //elegir tipo de enemigo 
    //se elige en el inspector
    public enum EnemyType
    {
        Skeleton,
        Slime,
        Eye
    }
    public EnemyType enemyType;
    /*********Movimiento***********/
    public float speed = 2f;
    protected int direction = 1;
    /*********Deteccion de jugador pared y suleo***********/
    public float playerDetectionRange = 5f;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    protected Rigidbody2D rb;
    protected Transform player;
    /*************************/
    Animator animator;



    //Se necesita dentro de los enemigos dos empty object que se llamen groundcheck y wallcheck mover su posicion a donde se quiera detectar suelo y pared
    // luego asignarles la etiqueta correspondiente

    /*********Deteccion de suelo esta si que si***********/
    public Transform groundCheck;
    public float groundCheckDistance = 0.3f;
    /*********Deteccion de pared***********/
    public Transform wallCheck;
    public float wallCheckDistance = 0.3f;


    // pilla el rb de los enemuigos y el transform del jugador
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

       

        animator = GetComponent<Animator>();
    }

    // Depende del enemigo hace un movimiento u otro
    void Update()
    {
        switch (enemyType)
        {
            //detecta al jugador y se mueve hacia el
            //funciona
            case EnemyType.Skeleton:
                SkeletonBehaviour();
                break;
            //se mueve y cambia de direccion al detectar pared o suelo
            //funciona
            case EnemyType.Slime:
                SlimeBehaviour();
                break;
            //hace lo mismo que el slime pero sin gravedad
            //funciona poner el ground check en el suelo no vale que este en el aire te prometo que si esta en el aire no detecta suelo u no va
            case EnemyType.Eye:
                EyeBehaviour();
                break;
        }
    }

    //mecanica del esqueleto
    void SkeletonBehaviour()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= playerDetectionRange)
        {
            FacePlayer();
            Move();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetFloat("xVelocity", 0);
        }
    }
    void FacePlayer()
    {
        if (player.position.x > transform.position.x && direction < 0)
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 1, 1);
        }
        else if (player.position.x < transform.position.x && direction > 0)
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    //mecanica del slime
    void SlimeBehaviour()
    {
        Move();

        if (!IsGroundAhead() || IsWallAhead())
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 0.7f, 0.7f);
        }
    }

    //mecanica del ojo
    void EyeBehaviour()
    {
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(direction * speed, 0);

        if (!IsGroundAhead() || IsWallAhead())
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    //movimineto del esqueleto y el slime
    protected void Move()
    {
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        animator.SetFloat("xVelocity",Mathf.Abs(direction * speed));
    }

    ////cambia la direccion del enemigo
    //protected void Flip()
    //{
    //    direction *= -1;
    //    transform.localScale = new Vector3(direction, 0.7f, 0.7f);
    //}

    //comprueba si el enemigo debe girar para mirar al jugador
    //solo lo usa el esqueleto para que mire al jugador
    //protected void CheckFlip()
    //{
    //    if (direction > 0 && rb.velocity.x < 0 ||
    //        direction < 0 && rb.velocity.x > 0)
    //    {
    //        Flip();
    //    }
    //}
    //detecta el suelo
    protected bool IsGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance
        );

        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

    //detecta la pared
    protected bool IsWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            wallCheckDistance
        );

        if (hit.collider == null) return false;

        return hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Wall");
    }

}

