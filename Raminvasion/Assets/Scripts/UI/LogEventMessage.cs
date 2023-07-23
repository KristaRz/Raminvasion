// Created by Krista Plagemann //
// Logs a message in the center of the scene for a set amount of time.


using TMPro;
using UnityEngine;

public class LogEventMessage : MonoBehaviour
{
    public static LogEventMessage Instance { private set; get; }

    private void Awake() => Instance = this;

    [SerializeField] private TextMeshProUGUI _SimpleTextField;

    /// <summary>
    /// Shows the given text in the center of the screen for 3 seconds.
    /// </summary>
    /// <param name="message">text to show</param>
    public void LogText(string message)
    {
        _SimpleTextField.SetText(message);
        _SimpleTextField.gameObject.SetActive(true);
        Invoke("DisableText", 3f);
    }

    /// <summary>
    /// Shows the given text in the center of the screen for given seconds.
    /// </summary>
    /// <param name="message">Text to show.</param>
    /// <param name="timeUntilDisable">For how long it is shown.</param>
    public void LogTextForTime(string message, float timeUntilDisable)
    {
        _SimpleTextField.SetText(message);
        _SimpleTextField.gameObject.SetActive(true);
        Invoke("DisableText", timeUntilDisable);
    }

    private void DisableText()
    {
        _SimpleTextField.SetText("");
        _SimpleTextField.gameObject.SetActive(false);
    }
}
