using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTabListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text playerScoreText;
    [SerializeField] TMP_Text playerStepsText;
    Player player;

    public void SetUp(Player _player, PlayerController PC)
    {
        player = _player;

        while(true)
        {
            playerNameText.text = _player.NickName;
            playerScoreText.text = PC.transform.position.z.ToString("0");
            playerStepsText.text = PC.StepsCount.ToString();
        }
    }

    public void SetUp(Player _player)
    {
        player = _player;

        playerNameText.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

}
