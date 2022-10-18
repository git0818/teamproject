using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PhotonView PV;
    int dir;

    void Start()
    {
        Destroy(gameObject, 3.5f);
    }

    void Update()
    {
        transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);
    }

    [PunRPC]
    void DirRPC(int dir)
    {
        this.dir = dir;
    }
}
