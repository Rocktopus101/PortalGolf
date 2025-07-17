using UnityEngine;

public class Star : MonoBehaviour
{
    public GameObject collectionEffect;
    public AudioClip collectionSound;  

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // telling the GameManager that a star was collected
            GameManager.Instance.CollectStar();

            // Destroy the star object
            Destroy(gameObject);
        }
    }
}