using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private WeaponAnimation _weaponAnimation;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float gravity;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float smoothTimeForPun = 0.1f;

    private CharacterController controller;
    private PhotonView view;
    private Vector3 velocity;
    private float xRotation;

    private Vector3 currentPosition;
    private Quaternion currentRotation;


    private void Awake()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            playerCamera.GetComponent<Camera>().enabled = false;
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            LookAround();
            Move();
            currentPosition = transform.position;
            currentRotation = transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * smoothTimeForPun);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, currentRotation, Time.deltaTime * smoothTimeForPun);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentPosition);
            stream.SendNext(currentRotation);
        }
        else
        {
            currentPosition = (Vector3)stream.ReceiveNext();
            currentRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Move()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * y;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
        _weaponAnimation.SetWalk(x != 0 || y != 0);
        _weaponAnimation.SetRun(Input.GetKey(KeyCode.LeftShift));
        controller.Move(move * speed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}