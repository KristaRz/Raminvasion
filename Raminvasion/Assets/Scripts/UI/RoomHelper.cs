// Created by Krista Plagemann //
// Creates a new room and joins or joins an existing room.


using TMPro;
using UnityEngine;

public class RoomHelper : MonoBehaviour
{
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
        }
    }
}
