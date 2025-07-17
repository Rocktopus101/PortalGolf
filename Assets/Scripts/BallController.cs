using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    // Added new states for the gravity well
    public enum BallState { Idle, Dragging, Flying, InGravityWell, AimingInWell }
    public BallState currentState = BallState.Idle;

    private Rigidbody2D rb;
    private Vector2 dragStartPos;
    private bool isDragging = false;
    private bool launched = false;

    public float launchPower = 10f;
    public float stopThreshold = 0.05f;
    public LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        currentState = BallState.Idle;

        if (lineRenderer) lineRenderer.enabled = false;
    }

    void Update()
    {
        // Logic to stop the ball when it's moving slowly
        if (launched && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            launched = false;
            currentState = BallState.Idle;
        }

        // Checking for mouse click to start aiming from the well
        if (currentState == BallState.InGravityWell && (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            Vector2 inputPos = Input.GetMouseButtonDown(0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(inputPos);

            if (Vector2.Distance(mouseWorld, rb.position) < 1f)
            {
                EnterBulletTimeAiming();
            }
        }

        // Allowing dragging when idle OR aiming from the well
        if ((currentState == BallState.Idle || currentState == BallState.AimingInWell) && !launched)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) StartDrag(Input.mousePosition);
        else if (Input.GetMouseButton(0) && isDragging) Drag(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0) && isDragging) Release(Input.mousePosition);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) StartDrag(touch.position);
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging) Drag(touch.position);
            else if (touch.phase == TouchPhase.Ended && isDragging) Release(touch.position);
        }
    }

    // logic to handle aiming from the well
    void EnterBulletTimeAiming()
    {
        Time.timeScale = 0.2f; 
        isDragging = true;
        dragStartPos = rb.position;
        currentState = BallState.AimingInWell;

        if (lineRenderer)
        {
            lineRenderer.enabled = true;
        }
    }

    void StartDrag(Vector2 screenPos)
    {
        if (launched && currentState != BallState.AimingInWell) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        if (currentState == BallState.AimingInWell || Vector2.Distance(worldPos, rb.position) < 1f)
        {
            isDragging = true;
            dragStartPos = (currentState == BallState.AimingInWell) ? rb.position : worldPos;
            if (lineRenderer) lineRenderer.enabled = true;
        }
    }

    void Drag(Vector2 screenPos)
    {
        if (!isDragging) return;
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 start = (currentState == BallState.AimingInWell) ? rb.position : dragStartPos;
        Vector2 direction = start - currentPos;

        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, rb.position);
            lineRenderer.SetPosition(1, rb.position + direction);
        }
    }

    //  logic to handle exiting the well
    void Release(Vector2 screenPos)
    {
        Vector2 releasePos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 start = (currentState == BallState.AimingInWell) ? rb.position : dragStartPos;
        Vector2 force = (start - releasePos) * launchPower;

        rb.isKinematic = false;
        rb.gravityScale = 1f;
        rb.AddForce(force, ForceMode2D.Impulse);

        isDragging = false;
        launched = true;
        currentState = BallState.Flying;

        if (lineRenderer) lineRenderer.enabled = false;
        Time.timeScale = 1f;
        FindObjectOfType<GravityWell>()?.ReleaseBall(rb);

        GameManager.Instance?.IncrementStrokes();
    }
}