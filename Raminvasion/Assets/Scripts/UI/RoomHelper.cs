
using TMPro;
using UnityEngine;

public class RoomHelper : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject LengthWarning;

    public void SetNewRoomAndJoin(TMP_InputField inputText)
    {
        if (inputText.text.Length <= 1)
        {
            LengthWarning.SetActive(true);
        }
        else
        {
            LobbyManager.Instance.SetNewRoomAndJoin(inputText.text);
            MenuCanvas.SetActive(false);
        }
    }
    public void JoinExistingRoom(TMP_InputField inputText)
    {
        if (inputText.text.Length <= 1)
        {
            LengthWarning.SetActive(true);
        }
        else
        {
            LobbyManager.Instance.JoinExistingRoom(inputText.text);
            MenuCanvas.SetActive(false);
        }
    }
}
