using UnityEngine;

public class Goal : MonoBehaviour
{
    public Color goalColor = Color.magenta; 
    public AudioClip goalSound;             
    private AudioSource audioSource;

    private void Start()
    {

        GetComponent<SpriteRenderer>().color = goalColor;
        if (goalSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = goalSound;
            audioSource.playOnAwake = false;
        }
    }

private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Ball")) return;
    
    if (!GameManager.Instance.AllStarsCollected())
    {
        UIManager.Instance.ShowMessage("Collect all the stars!");
        return;
    }

    Debug.Log("🎯 Goal reached!");

    // Stop the ball
    Rigidbody2D rb = other.attachedRigidbody;
    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
    }

    GameManager.Instance.LevelComplete();

    GetComponent<Collider2D>().enabled = true;
}
}
