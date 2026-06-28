using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform arms;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine == false)
        {
            player.gameObject.SetActive(true);
            arms.gameObject.SetActive(false);
        }
    }
}