// Created by Krista Plagemann //
// On a room list item so that we can assign a name and functionality to the button.


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    private string _roomName;

    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
        GetComponentInChildren<TextMeshProUGUI>().SetText(roomName);
    }

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(SelectRoom);
    }

    /// <summary>
    /// When the button of this is pressed we select the room this item belongs to.
    /// </summary>
    private void SelectRoom()
    {
        LobbyManager.Instance.JoinExistingRoom(_roomName);
    }
}
