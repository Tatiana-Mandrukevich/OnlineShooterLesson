using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private GameObject player;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
           SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        var spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Length)];
        PhotonNetwork.Instantiate(player.name, spawnPosition.position, spawnPosition.rotation);
    }
}
