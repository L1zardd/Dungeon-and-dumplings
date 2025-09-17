using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lives = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float r = 1f;
    private bool isGrounded = false;


    private Rigidbody2D rb;
    private SpriteRenderer sprite;


    private void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        sprite = rb.GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        print(isGrounded);
        if (Input.GetButton("Horizontal"))
            Run();

        if (isGrounded && Input.GetButton("Jump"))
            Jump();
    }

    private void Run()
    {
        //куда двигается
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        //изменение позиции
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed*Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void Jump()
    {
        rb.AddForce(transform.up*jumpForce,ForceMode2D.Impulse);
    }
    
    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, r);
        isGrounded = collider.Length > 1;
    }
}
