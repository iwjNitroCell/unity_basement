using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed; // 속도
    [SerializeField]
    private float maxSpeed; // 최대 속도
    private float drag = 0f; // 저항(움직임)
    [SerializeField]
    float jumpForce; // 점프 힘
    [SerializeField]
    int maxJumpCount = 1; // 최대 점프 횟수
    [SerializeField]
    int curJumpCount; // 남은 점프 횟수

    [SerializeField]
    private bool isGround = false; // 땅에 닿아 있는가?
    private float velocityX;
    private LayerMask layerMask;

    private Rigidbody2D rigid;
    private CapsuleCollider2D capsuleCol2D;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsuleCol2D = GetComponent<CapsuleCollider2D>();

    }

    void Update()
    {
        IsStep();
        Move();
        TryJump();
    }

    private void FixedUpdate()
    {
        // 최대 속력 제한
        if (rigid.velocity.x > maxSpeed)
        {
            //rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            rigid.AddForce(new Vector2(-rigid.velocity.x * 0.5f, 0f));
        }
        else if (rigid.velocity.x < maxSpeed * (-1))
        {
            //rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
            rigid.AddForce(new Vector2(-rigid.velocity.x * 0.5f, 0f));
        }
    }

    private void IsStep()
    {
        // 박스 형태로 충돌한 콜라이더 감지
        if (Physics2D.BoxCast(capsuleCol2D.bounds.center,    // Vector2 origin    -> 시작지점
            capsuleCol2D.bounds.size,                       // Vector2 size      -> 박스의 크기
            0f,                                             // float   angle     -> 박스의 각도
            Vector2.down,                                   // vector2 direction -> 박스의 방향
            0.02f,                                          // float   distance  -> 박스의 최대 거리
                                                            // LayerMask -> 특정 레이어에서만 충돌 확인
            layerMask = LayerMask.GetMask("NormalField") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("iceField")))
        {
            if (rigid.velocity.y <= 0) // 떨어지고 있을 때
            {
                isGround = true;
                curJumpCount = maxJumpCount;
            }
        }
        else
        {
            isGround = false;
        }
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        if (moveDirX == 0f)
        {
            rigid.AddForce(new Vector2(-rigid.velocity.x * 0.5f, 0f)); // 땅 위에서의 마찰력
        }
        else
        {
            if (moveDirX < 0 )
            {
                gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            }
            else if (moveDirX > 0)
            {
                gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }
            if (!isGround) // 공중에 있을 때는 좀 느리게 움직이도록 한다.
            {
                drag = 0.8f; // 그냥 공기저항
            }
            else
            {
                drag = 1f;
            }
            velocityX = moveDirX * speed * drag;
            rigid.AddForce(new Vector2(velocityX, 0f));
        }
    }

    private void TryJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }
        if (isGround)
        {
            Jump();
        }
        /*if (curJumpCount > 0)
        {
            curJumpCount--;
            Jump();
        } */
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
    }
}
