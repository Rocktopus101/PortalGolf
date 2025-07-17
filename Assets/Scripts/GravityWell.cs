using UnityEngine;

[ExecuteAlways]
public class GravityWell : MonoBehaviour
{
    public float rotationSpeed = 90f; 
    public float radius = 1.5f;
    public Transform center;

    private Rigidbody2D targetRB;
    private float angle;
    private bool isInside = false;

    void Start()
    {
        if (center == null) center = transform;
    }

    void FixedUpdate()
    {
        if (isInside && targetRB != null)
        {
            BallController controller = targetRB.GetComponent<BallController>();
            if (controller != null && controller.currentState != BallController.BallState.InGravityWell)
            {
                isInside = false;
                targetRB = null;
                return;
            }

            
            angle += rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;

            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 targetPos = (Vector2)center.position + offset;

            targetRB.MovePosition(targetPos); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        targetRB = other.attachedRigidbody;
        if (targetRB == null) return;

        Vector2 dir = (targetRB.position - (Vector2)center.position).normalized;
        angle = Mathf.Atan2(dir.y, dir.x);

        Vector2 snappedPos = (Vector2)center.position + dir * radius;
        targetRB.position = snappedPos;

        targetRB.linearVelocity = Vector2.zero;
        targetRB.angularVelocity = 0f;
        targetRB.gravityScale = 0f;
        targetRB.isKinematic = false; 

        isInside = true;
        BallController controller = targetRB.GetComponent<BallController>();
        if (controller != null)
        {
            controller.currentState = BallController.BallState.InGravityWell;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody == targetRB)
        {
            targetRB.gravityScale = 1f;
            targetRB.isKinematic = false;
            isInside = false;
            targetRB = null;
        }
    }

    public void ReleaseBall(Rigidbody2D rbToRelease)
    {
        if (targetRB == rbToRelease)
        {
            isInside = false;
            targetRB = null;
        }
    }
    private void OnDrawGizmos()
    {
        if (center == null) center = transform;
        Gizmos.color = Color.cyan;
        int segments = 60;
        float angleStep = 2 * Mathf.PI / segments;
        Vector3 prevPoint = center.position + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float nextAngle = i * angleStep;
            Vector3 nextPoint = center.position + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle), 0) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}