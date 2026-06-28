using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Object = System.Object;

public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField] private WeaponAnimation weaponAnimation;
    [SerializeField] private Transform shootCamera;
    [SerializeField] private float fireRate;
    [SerializeField] private int damage;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private int amountBullets;

    private float currentFireRate;
    private bool isCanShoot;
    private bool isReloading;
    private int currentBullets;
    private PhotonView view;

    private void Start()
    {
        currentBullets = amountBullets;
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

            ReloadMethod();
            if (currentBullets <= 0 || isReloading)
            {
                return;
            }
            if (Input.GetKey(KeyCode.Mouse0) && isCanShoot)
            {
                currentBullets--;
                isCanShoot = false;
                currentFireRate = fireRate;
                Shot();
            }
        }
    }

    private void ReloadMethod()
    {
        if (currentBullets != amountBullets && Input.GetKeyDown(KeyCode.R) && isReloading == false)
        {
            isReloading = true;
            weaponAnimation.SetReload(true);
            DOVirtual.DelayedCall(weaponAnimation.GetReloadDuration(), () =>
            {
                isReloading = false;
                currentBullets = amountBullets;
            });
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
                photonView.RPC("ShotEffectDecalRPC", RpcTarget.All,
                    new Object[] { hit.point, Quaternion.LookRotation(hit.normal) });
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