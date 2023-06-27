
using TMPro;
using UnityEngine;

public class SpeedDisplay : MonoBehaviour
{
    private TextMeshProUGUI _textComp;
    [SerializeField] private PlayerTag _ForThisPlayer;

    private void Start()
    {
        _textComp = GetComponent<TextMeshProUGUI>();
        if (_ForThisPlayer == PlayerTag.Player1)
            GameHandler.Instance.OnPlayer1Speed += UpdateSpeed;
        else
            GameHandler.Instance.OnPlayer2Speed += UpdateSpeed;
    }

    private void UpdateSpeed(float speed)
    {
        _textComp.text = speed.ToString();
    }

    private void OnDisable()
    {
        if (_ForThisPlayer == PlayerTag.Player1)
            GameHandler.Instance.OnPlayer1Speed -= UpdateSpeed;
        else
            GameHandler.Instance.OnPlayer2Speed -= UpdateSpeed;
    }
}
