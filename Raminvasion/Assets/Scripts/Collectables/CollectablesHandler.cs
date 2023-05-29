
using System;
using UnityEngine;

public class CollectablesHandler : MonoBehaviour
{
    #region Singleton

    public static CollectablesHandler Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #endregion


    public event Action<float> OnPlayer1SpeedChange;

    public event Action<float> OnPlayer2SpeedChange;


    public void ChangeSpeed(PlayerTag player, float speed)
    {
        switch(player)
        {
            case PlayerTag.Player1: OnPlayer1SpeedChange(speed);
                break;
            case PlayerTag.Player2: OnPlayer2SpeedChange(speed);
                break;
        }
    }
}
