using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Static Text Elements")]
    public TextMeshProUGUI gameTitleText;
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI levelNumberText;

    [Header("Dynamic Message Elements")]
    public TextMeshProUGUI messageText;
    public float messageDisplayDuration = 2.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Clear any previous messages on start
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        if (messageText == null) return;
        
        StopAllCoroutines();
        StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDisplayDuration);

        messageText.gameObject.SetActive(false);
    }

    public void UpdateLevelText(int levelNum)
    {
        if (levelNumberText != null)
        {
            levelNumberText.text = $"Level: {levelNum}";
        }
    }
}