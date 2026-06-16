using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Object = System.Object;

public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform shootCamera;
    [SerializeField] private float fireRate;
    [SerializeField] private int damage;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private GameObject decalPrefab;

    private float currentFireRate;
    private bool isCanShoot;
    private PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (view.IsMine)
        {
            if (currentFireRate <= 0)
            {
                isCanShoot = true;
            }
            else
            {
                currentFireRate -= Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Mouse0) && isCanShoot)
            {
                isCanShoot = false;
                currentFireRate = fireRate;
                Shot();
            }
        }
    }

    private void Shot()
    {
        if (Physics.Raycast(shootCamera.transform.position, shootCamera.transform.forward, out RaycastHit hit))
        {
            var idamage = hit.collider.GetComponent<IDamageable>();
            if (idamage != null)
            {
                idamage.TakeDamage(damage, PhotonNetwork.NickName);
            }
            else
            {
                photonView.RPC("ShotEffectDecalRPC", RpcTarget.All, new Object[] { hit.point, Quaternion.LookRotation(hit.normal) });
            }
        }

        photonView.RPC("ShotRPC", RpcTarget.All);
    }

    [PunRPC]
    private void ShotEffectDecalRPC(Vector3 position, Quaternion rotation)
    {
        var newItem = Instantiate(decalPrefab, position, rotation);
        Destroy(newItem.gameObject, 6);
    }

    [PunRPC]
    private void ShotRPC()
    {
        var newShootEffect = Instantiate(particles, particles.transform.position, particles.transform.rotation);
        newShootEffect.Play();
        Destroy(newShootEffect, 4);
    }
}