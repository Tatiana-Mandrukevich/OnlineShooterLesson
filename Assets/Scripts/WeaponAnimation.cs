using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class WeaponAnimation : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private Transform _weapon;
    [SerializeField] private Transform _handsForWeapon;
    private bool isWalk;
    private bool isRun;
    private bool isReload;
    private bool isReloadFlag;

    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (_photonView.IsMine == false)
        {
            _weapon.parent = _handsForWeapon;
            _weapon.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void SetWalk(bool isWalk)
    {
        this.isWalk = isWalk;
    }

    public void SetRun(bool isRun)
    {
        this.isRun = isRun;
    }

    public void SetReload(bool isReload)
    {
        this.isReload = isReload;
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            _animator.SetBool("IsWalk", isWalk);
            _animator.SetBool("IsRun", isRun);
            if (isReload)
            {
                _animator.SetTrigger("Reload");
                isReload = false;
                isReloadFlag = true;
            }
        }
        else
        {
            _characterAnimator.SetBool("IsWalk", isWalk);
            _characterAnimator.SetBool("IsRun", isRun);
            if (isReload)
            {
                _characterAnimator.SetTrigger("Reload");
                isReload = false;
            }
        }
    }

    public float GetReloadDuration()
    {
        return _animator.runtimeAnimatorController.animationClips.First(t => t.name == "A_FP_PCH_AR_01_Reload").length;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isWalk);
            stream.SendNext(isRun);
            stream.SendNext(isReloadFlag);
            isReloadFlag = false;
        }
        else
        {
            isWalk = (bool)stream.ReceiveNext();
            isRun = (bool)stream.ReceiveNext();
            isReload = (bool)stream.ReceiveNext();
        }
    }
}