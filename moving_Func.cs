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


    // Execute inside the Awake function
    {
        rigid = GetComponent<Rigidbody2D>();
        capsuleCol2D = GetComponent<CapsuleCollider2D>();
    }

    
    // Execute inside the Update function
    private void IsStep() // 무언가 밟았는가를 감지하는 함수 
    {
        // 박스 형태로 충돌한 콜라이더 감지
        if (Physics2D.BoxCast(capsuleCol2D.bounds.center,    // Vector2 origin    -> 시작지점
            capsuleCol2D.bounds.size,                       // Vector2 size      -> 박스의 크기
            0f,                                             // float   angle     -> 박스의 각도
            Vector2.down,                                   // vector2 direction -> 박스의 방향
            0.02f,                                          // float   distance  -> 박스의 최대 거리
                                                            // LayerMask -> 특정 레이어에서만 충돌 확인
            layerMask = LayerMask.GetMask("")) // <- string 레이어 이름
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
    
    // Execute inside the Update function
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
    
    // Execute inside the FixedUpdate function
    private void LimitMaxSpeed() // 최대 속력 제한
    {    
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
    
    // Execute inside the Update function
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
        
    // Execute inside the Update function
    private void LookAtUD() // 위 아래 바라보기
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            head.transform.localEulerAngles = new Vector3(-55.4f, 0f, 43.38f);  // <- this codes will be changed 
            head.transform.localPosition = new Vector3(-0.14f, 1.65f, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            head.transform.localEulerAngles = new Vector3(-50f, 0f, -60f);
            head.transform.localPosition = new Vector3(0.88f, 1.64f, 0f);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            head.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            head.transform.localPosition = new Vector3(0.4f, 1.91f, 0f);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            head.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            head.transform.localPosition = new Vector3(0.4f, 1.91f, 0f);
        }
    }

