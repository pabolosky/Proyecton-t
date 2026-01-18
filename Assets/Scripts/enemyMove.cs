using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMove : MonoBehaviour
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
    public int direction = 1;
    /*********Deteccion de jugador pared y suleo***********/
    public float playerDetectionRange = 10f;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public Rigidbody2D rb;
    protected Transform player;
    /*************************/
    public Animator animator;

    /**************nuevo**************/
    public bool canMove = true; // si el bicho se puede mover


    //Se necesita dentro de los enemigos dos empty object que se llamen groundcheck y wallcheck mover su posicion a donde se quiera detectar suelo y pared
    // luego asignarles la etiqueta correspondiente

    /*********Deteccion de suelo esta si que si***********/
    public Transform groundCheck;
    public float groundCheckDistance = 0.3f;
    /*********Deteccion de pared***********/
    public Transform wallCheck;
    public float wallCheckDistance = 0.3f;



    /*****************atack************************/
    private bool canDamage = false;
    public Transform enemyatack;
    public LayerMask playerlayer;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    private float attackTimer;


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

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= playerDetectionRange)
        {
            FacePlayer();

            if (PlayerInAttackRange())
            {
                TryAttack();
            }
            else
            {
                Move();
            }
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

    void SlimeBehaviour()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange)
        {
            TryAttack();
            return;
        }

        if (canMove)
        {
            Move();

            if (!IsGroundAhead() || IsWallAhead())
            {
                direction *= -1;
                transform.localScale = new Vector3(direction, 0.7f, 0.7f);
            }
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
        /**************nuevo**************/
        if (canMove) // que solo se mueva si puede
        {
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
            animator.SetFloat("xVelocity", Mathf.Abs(direction * speed));
            
        }
    }

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



    //cosas para atacar
    bool PlayerInAttackRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    void TryAttack()
    {
        if (attackTimer > 0) return;

        animator.SetTrigger("attack");
        canMove = false;
        rb.velocity = Vector2.zero;

        attackTimer = attackCooldown;
    }


    public void StartAttack()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        canDamage = false;
    }


    public void EnableDamage()
    {
        if (canDamage) return; // evita doble golpe

        canDamage = true;

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(
            enemyatack.position,
            attackRange,
            playerlayer
        );

        if (hitPlayer.Length > 0)
        {
            hitPlayer[0]
                .GetComponent<playerMovement>()
                .changeHealth(-1);
        }
    }


    public void endAttack()
    {
        canDamage = false;
        canMove = true;
    }


  
}


