using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Object = System.Object;

public class PlayerHealth : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private int health = 100;
    private int _currentHealth;

    private PhotonView _view;

    private void Awake()
    {
        _view = GetComponent<PhotonView>();
        _currentHealth = health;
    }

    void Dead()
    {
        PhotonNetwork.Destroy(gameObject);
        FindObjectOfType<PlayerSpawner>().SpawnPlayer();
    }

    public void TakeDamage(int damage,string playerName)
    {
        _view.RPC("TakeDamageRPC", RpcTarget.All, new Object[] { damage, playerName });
    }

    [PunRPC]
    public void TakeDamageRPC(int damage,string playerName)
    {
        if (_view.IsMine == false)
        {
            return;
        }

        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Dead();
        }
    }
}

public interface IDamageable
{
    void TakeDamage(int damage,string playerName);
}