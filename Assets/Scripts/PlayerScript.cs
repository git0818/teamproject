using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField]
    private Rigidbody2D RB;
    [SerializeField]
    private Animator AN;
    [SerializeField]
    private SpriteRenderer SR;
    [SerializeField]
    private PhotonView PV;
    [SerializeField]
    private TextMeshProUGUI NicknameText;
    [SerializeField]
    private Image HealthImage;

    private GameObject JumpGO;
    private Button JumpBtn;
    bool isGround;
    Vector3 curPos;
    bool gamestart = false;

    void Awake()
    {
        NicknameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NicknameText.color = PV.IsMine ? Color.green : Color.red;
        if (PhotonNetwork.IsMasterClient)
            JumpGO = GameObject.Find("JumpBtn(Local)");
        else 
            JumpGO = GameObject.Find("JumpBtn(Remote)");
        JumpBtn = JumpGO.GetComponent<Button>();
        JumpBtn.onClick.AddListener(Jump);
        AN.SetBool("idle", true);
    }

    void Update()
    {
        if (NetworkManager.gamestartcheck == true)
        {
            if (gamestart == false)
            {
                gamestart = true;
                AN.SetBool("idle", false);
            }

            if (PV.IsMine)
            {
                Debug.DrawRay(transform.position, new Vector3(0, -1f, 0), new Color(0, 1, 0));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
                if (hit.collider == null)
                {
                    isGround = false;
                    AN.SetBool("jump", true);
                }
                else
                {
                    isGround = true;
                    AN.SetBool("jump", false);
                }
            }
            else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
            else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
        }
    }


    // ���� ��ư�� �Ҵ�
    public void Jump()
    {
        if(isGround == true)
        {
            PV.RPC("JumpRPC", RpcTarget.All);
        }
    }
    [PunRPC]
    void JumpRPC()
    {
        RB.AddForce(Vector2.up * 700);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
