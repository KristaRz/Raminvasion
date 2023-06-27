using System.Collections;
using System.Collections.Generic;
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

    private void SelectRoom()
    {
        LobbyManager.Instance.JoinExistingRoom(_roomName);
    }
}
