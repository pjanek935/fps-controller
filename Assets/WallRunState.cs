using UnityEngine;

[RequireComponent(typeof (InAirState))] 
public class WallRunState : CharacterStateBase
{
    public class WallRunRaycastResult
    {
        public bool Success = false;
        public float Distance = -1f;
        public Collider ColliderThatWasHit;
        public WallRunType WallRunType;
        public RaycastHit Hit;
        public Vector3 Direction;
        public Vector3 RunDirection;
    }

    public enum WallRunType
    {
        NONE, LEFT, RIGH
    }

    enum State
    {
        RUNNIG, FALLING
    }

    [SerializeField]
    [Tooltip ("How far from wall player character " +
        "can be to be able to initialize wall run.")] float raycastLength = 2f;
    [SerializeField] float minVelocity = 5f;
    [SerializeField] float maxVelocity = 20f;
    [SerializeField] float duration = 1f;

    float durationTimer = 0f;
    State currentWallRunState = State.RUNNIG;
    InAirState inAirState;
    float movementSpeed = 5f;
    float initializationTimer = 0f;
    bool isInitializationTimerRunning = false;
    int layerMask;

    private new void OnEnable ()
    {
        base.OnEnable ();

        inAirState = GetComponent<InAirState> ();
    }

    private void Awake ()
    {
        layerMask = LayerMask.GetMask ("Default");
    }

    public WallRunRaycastResult RaycastResult
    {
        get;
        protected set;
    }

    public bool ShouldStartWallRunState (CharacterControllerBase parent)
    {
        bool result = false;

        if (isInitializationTimerRunning && initializationTimer >= 0.2f)
        {
            if (TryToWallRun (parent))
            {
                result = true;
            }
        }

        return result;
    }

    void updateInitializationTimer ()
    {
        initializationTimer += Time.deltaTime;
    }

    public void StartInitializationTimer ()
    {
        isInitializationTimerRunning = true;
        initializationTimer = 0f;
    }

    public void StopInitializationTimer ()
    {
        isInitializationTimerRunning = false;
        initializationTimer = 0f;
    }

    private void Update ()
    {
        if (isInitializationTimerRunning)
        {
            updateInitializationTimer ();
        }
    }

    public override void UpdateState ()
    {
        switch (currentWallRunState)
        {
            case State.RUNNIG:

                updateRunning ();

                break;

            case State.FALLING:

                updateFalling ();

                break;
        }
    }

    protected override void onEnter ()
    {
        base.onEnter ();

        durationTimer = 0f;
        currentWallRunState = State.RUNNIG;
        Vector3 currentVelocity = parent.Velocity;
        currentVelocity.y = 0;
        movementSpeed = Mathf.Clamp (currentVelocity.magnitude, minVelocity, maxVelocity);
        setVelocity (Vector3.zero);
    }

    protected override void onExit ()
    {
        Vector3 newVelocity = RaycastResult.RunDirection * movementSpeed;
        newVelocity.y = parent.Velocity.y;
        setVelocity (newVelocity);

        base.onExit ();
    }

    void updateRunning ()
    {
        durationTimer += Time.deltaTime;

        if (durationTimer >= duration)
        {
            currentWallRunState = State.FALLING;
        }

        move (RaycastResult.RunDirection * movementSpeed * Time.deltaTime);

        if (! checkIfIsTouchingTheWall ())
        {
            if (parent.IsGrounded)
            {
                requestNewState<GroundState> ();
            }
            else
            {
                requestNewState<InAirState> ();
            }
        }
    }

    void updateFalling ()
    {
        if (parent.IsGrounded)
        {
            requestNewState<GroundState> ();
        }
        else
        {
            requestNewState<InAirState> ();
        }
    }

    bool checkIfIsTouchingTheWall ()
    {
        Vector3 direction = Vector3.zero;
        bool result = false;

        switch (RaycastResult.WallRunType)
        {
            case WallRunType.LEFT:

                direction = -parent.transform.right;

                break;

            case WallRunType.RIGH:

                direction = parent.transform.right;

                break;
        }

        RaycastHit raycastHit;
        Ray ray = new Ray (parent.transform.position, direction);

        if (RaycastResult.ColliderThatWasHit != null &&
            RaycastResult.ColliderThatWasHit.Raycast (ray, out raycastHit, raycastLength))
        {
            result = true;
        }

        return result;
    }

    protected void jump ()
    {
        Vector3 newVelocity = RaycastResult.RunDirection * movementSpeed;
        newVelocity.y = Mathf.Sqrt (inAirState.JumpHeight * 2f * inAirState.Gravity);
        setVelocity (newVelocity);
        requestNewState<PlayerInAirState> ();
    }

    /// <summary>
    /// Returns true if raycast hit wall and a wall run
    /// can be intiated.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    protected static bool raycastWalls (Transform transform, float raycastLenght, out WallRunRaycastResult wallRunRaycastResult, int layerMask)
    {
        WallRunRaycastResult result = new WallRunRaycastResult ();
        GameObject rightWall;
        GameObject leftWall;
        RaycastHit leftHit;
        RaycastHit rightHit;
        float distFromRight = raycastInDirection (transform, transform.right, out rightWall, out rightHit, raycastLenght, layerMask);
        float distFromLeft = raycastInDirection (transform, -transform.right, out leftWall, out leftHit, raycastLenght, layerMask);

        if (distFromLeft > 0 && distFromLeft > distFromRight)
        {
            result.Success = true;
            result.WallRunType = WallRunType.LEFT;
            result.ColliderThatWasHit = leftWall.GetComponent <Collider> ();
            result.Distance = distFromLeft;
            result.Hit = leftHit;
            result.Direction = (leftHit.point - transform.position).normalized;
        }
        else if (distFromRight > 0 && distFromRight > distFromLeft)
        {
            result.Success = true;
            result.WallRunType = WallRunType.RIGH;
            result.ColliderThatWasHit = rightWall.GetComponent<Collider> ();
            result.Distance = distFromRight;
            result.Hit = rightHit;
            result.Direction = (rightHit.point - transform.position).normalized;
        }

        wallRunRaycastResult = result;
        return result.Success;
    }

    /// <summary>
    /// Returns true if wall run can be intiated.
    /// Caches WallRunRaycastResult into a RaycastResult variable.
    /// </summary>
    /// <returns></returns>
    public bool TryToWallRun (CharacterControllerBase parent)
    {
        bool success = false;
        Collider lastWallRunCollider = null;
        WallRunRaycastResult result;

        if (RaycastResult != null)
        {
            lastWallRunCollider = RaycastResult.Hit.collider;
        }

        if (raycastWalls (parent.transform, raycastLength, out result, layerMask))
        {
            if (lastWallRunCollider == null ||
                (lastWallRunCollider != RaycastResult.Hit.collider) ||
                (lastWallRunCollider == RaycastResult.Hit.collider))
            {
                success = true;
                RaycastResult = result;
                Vector3 v = parent.GetLookDirection ();
                Vector3 n = RaycastResult.Hit.normal;
                Vector3 vtn = Vector3.Cross (v, n);
                Vector3 res = Vector3.Cross (n, vtn);
                res.y = 0f;
                RaycastResult.RunDirection = res;

                Debug.DrawRay (RaycastResult.Hit.point, res * 10, Color.magenta, 1f);
            }
        }

        return success;
    }

    protected static float raycastInDirection (Transform transform, Vector3 direction,
        out GameObject objectThatWasHit, out RaycastHit hit, float raycastLength, int layerMask)
    {
        float result = -1f;
        objectThatWasHit = null;

        if (Physics.Raycast (transform.position,
            direction,
            out hit,
            raycastLength,
            layerMask))
        {
            result = hit.distance;
            objectThatWasHit = hit.collider.gameObject;
            Debug.DrawRay (transform.position, direction * hit.distance, Color.yellow, 1f);
        }

        return result;
    }
}
