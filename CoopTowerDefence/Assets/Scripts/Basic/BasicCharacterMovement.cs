using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacterMovement : Photon.MonoBehaviour
{
    [Header("Basic")]
    public float walkSpeed;
    public float jumpHeight;
    public GameObject cam;
    public float lookSensetivity;
    public Rigidbody rig;
    [Header("Networking")]
    public float lerp;
    Vector3 position;
    Quaternion rotation;

    public void Start()
    {
        if (photonView.isMine)
            cam.SetActive(true);
        else
            StartCoroutine(UpdateData());
    }

    public void Update()
    {
        if (photonView.isMine)
        {
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * walkSpeed);
            transform.Rotate(Vector3.up * Time.deltaTime * lookSensetivity * Input.GetAxis("Mouse X"));
            cam.transform.Rotate(Vector3.left * Time.deltaTime * lookSensetivity * Input.GetAxis("Mouse Y"));
        }
    }

    public IEnumerator UpdateData()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerp);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerp);
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
