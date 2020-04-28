using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private Collider2D coll;
    private BoxCollider2D box;

    //FSM
    private enum State {bunnyIdle, bunnyRunning, bunnyJumping, bunnyFalling, snakeIdle, snakeRunning, snakeJumping, snakeFalling, owlIdle, owlRunning, owlJumping, owlFalling, bunnyHurt, snakeHurt, owlHurt}
    private State state = State.bunnyIdle;

    //Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int character = 1;
    [SerializeField] private int life = 3;
    [SerializeField] private Text lifeText;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private int jump = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        box = GetComponent<BoxCollider2D>();
        lifeText.text = life.ToString();
    }

    private void Update()
    {
        CharacterSelection();
        Bunny();
        Snake();
        Owl();
        if(state != State.bunnyHurt && state != State.snakeHurt && state != State.owlHurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); //sets animation based on Enumerator state
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if (state == State.owlFalling)
            {
                Destroy(other.gameObject);
                Jump();
            }
            else
            {
                life = life - 1;
                lifeText.text = life.ToString();
                if (character == 1)
                {
                    state = State.bunnyHurt;
                }
                else if (character == 2)
                {
                    state = State.snakeHurt;
                }
                else if (character == 3)
                {
                    state = State.owlHurt;
                }
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to my right therefore I should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //Enemy is to my left therefore I should be damages and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void Bunny()
    {
        if(character == 1)
        {
            jumpForce = 25f;
            if (state == State.bunnyJumping || state == State.bunnyFalling)
            {
                speed = 5f;
            }
            else
            {
                speed = 15f;
            }
        }
    }

    private void Snake()
    {
        if (character == 2)
        {
            jumpForce = 10f;
            speed = 7f;
            box.size = new Vector2(0.9442511f, .9f);
            box.offset = new Vector2(0.013978f, -.5f);

        }
        else
        {
            box.size = new Vector2(0.9442511f, 1.231185f);
            box.offset = new Vector2(0.013978f, -0.3570752f);
        }
    }

    private void Owl()
    {
        if (character == 3)
        {
            jumpForce = 20f;
            speed = 8f;
            if (state == State.owlFalling)
            {
                rb.drag = 20;
            }
            else
            {
                rb.drag = 0;
            }
        }
    }

    private void CharacterSelection()
    {
        if(Input.GetButton("Bunny"))
        {
            character = 1;
        }
        if (Input.GetButton("Snake"))
        {
            character = 2;
        }
        if (Input.GetButton("Owl"))
        {
            character = 3;
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        //Moving Left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);

        }
        //Moving Right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }
        //Jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {

            if (jump == 1)
            {
                Jump();
            }

        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jump = 0;
        if (character == 1)
        {
            state = State.bunnyJumping;
        }
        if (character == 2)
        {
            state = State.snakeJumping;
        }
        if (character == 3)
        {
            state = State.owlJumping;
        }
    }
    

    private void AnimationState()
    {
        if (character == 1)
        {
            if (state == State.bunnyJumping)
            {
                if (rb.velocity.y < .1f)
                {
                    state = State.bunnyFalling;
                }
            }
            else if (state == State.bunnyFalling)
            {
                if (coll.IsTouchingLayers(ground))
                {
                    if (jump == 1)
                    {
                        state = State.bunnyIdle;
                    }
                    else if (rb.velocity.y == 0)
                    {
                        jump = 1;
                    }
                }
            }
            else if (state == State.bunnyHurt)
            {
                if (Mathf.Abs(rb.velocity.x) < .1f)
                {
                    state = State.bunnyIdle;
                    jump = 1;
                }
            }
            else if (Mathf.Abs(rb.velocity.x) > 2f)
            {
                //Moving
                state = State.bunnyRunning;
                jump = 1;
            }
            else
            {
                state = State.bunnyIdle;
                jump = 1;
            }
        }
        else if (character == 2)
        {
            if (state == State.snakeJumping)
            {
                if (rb.velocity.y < .1f)
                {
                    state = State.snakeFalling;
                }
            }
            else if (state == State.snakeFalling)
            {
                if (coll.IsTouchingLayers(ground))
                {
                    if (jump == 1)
                    {
                        state = State.snakeIdle;
                    }
                    else if (rb.velocity.y == 0)
                    {
                        jump = 1;
                    }
                }
            }
            else if (state == State.snakeHurt)
            {
                if (Mathf.Abs(rb.velocity.x) < .1f)
                {
                    state = State.snakeIdle;
                    jump = 1;
                }
            }
            else if (Mathf.Abs(rb.velocity.x) > 2f)
            {
                //Moving
                state = State.snakeRunning;
                jump = 1;
            }
            else
            {
                state = State.snakeIdle;
                jump = 1;
            }
        }
        else if (character == 3)
        {
            if (state == State.owlJumping)
            {
                if (rb.velocity.y < .1f)
                {
                    state = State.owlFalling;
                }
            }
            else if (state == State.owlFalling)
            {
                if (coll.IsTouchingLayers(ground))
                {
                    if (jump == 1)
                    {
                        state = State.owlIdle;
                    }
                    else if (rb.velocity.y == 0)
                    {
                        jump = 1;
                    }
                }
            }
            else if (state == State.owlHurt)
            {
                if (Mathf.Abs(rb.velocity.x) < .1f)
                {
                    state = State.owlIdle;
                    jump = 1;
                }
            }
            else if (Mathf.Abs(rb.velocity.x) > 2f)
            {
                //Moving
                state = State.owlRunning;
                jump = 1;
            }
            else
            {
                state = State.owlIdle;
                jump = 1;
            }
        }
    }
}
