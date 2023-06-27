using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogEventMessage : MonoBehaviour
{
    public static LogEventMessage Instance { private set; get; }

    private void Awake() => Instance = this;

    [SerializeField] private TextMeshProUGUI _SimpleTextField;

    public void LogText(string message)
    {
        _SimpleTextField.SetText(message);
        _SimpleTextField.gameObject.SetActive(true);
        Invoke("DisableText", 3f);
    }

    private void DisableText()
    {
        _SimpleTextField.SetText("");
        _SimpleTextField.gameObject.SetActive(false);
    }
}
