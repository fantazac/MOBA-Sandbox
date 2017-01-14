using UnityEngine;
using System.Collections;

public class EntityTeam : MonoBehaviour
{
    [SerializeField]
    private Team team;

    public Team Team { get { return team; } }

    public PhotonView PhotonView { get; private set; }

    public void SetPhotonView(PhotonView photonView)
    {
        PhotonView = photonView;
    }

    public void SetTeam(Team team)
    {
        PhotonView.RPC("SetTeamOnNetwork", PhotonTargets.AllBufferedViaServer, team);
    }

    [PunRPC]
    private void SetTeamOnNetwork(Team team)
    {
        this.team = team;
    }
}
