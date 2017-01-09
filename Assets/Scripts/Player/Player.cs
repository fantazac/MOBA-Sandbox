using UnityEngine;
using System.Collections;

public class Player : PlayerBase
{

    public int health;
    public Team team;

    protected override void Start()
    {
        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 15;
        base.Start();
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        SerializeState(stream, info);
        PlayerMovement.SerializeState(stream, info);
    }

    public override void SerializeState(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            float oldHealth = health;
            health = (int)stream.ReceiveNext();

            if(oldHealth != health)
            {
                //OnHealthChanged();
            }
        }
    }
}
