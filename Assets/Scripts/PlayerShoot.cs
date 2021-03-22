using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(AudioManager))]
public class PlayerShoot : NetworkBehaviour
{
    #region Initializations
    private const string PLAYER_TAG = "Player";
    
    private PlayerWeapon currentWeapon;
    [SerializeField]
    private PlayerUI playerUI;

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    private WeaponManager weaponManager;
    private GameObject wep;
    private AudioManager audioManager;
    private Animator animator;
    private int BurstAmt = 3;
    private bool isShooting = false;
    private float ShootDelay;
    #endregion
    private void Start() {
        if (cam==null){
            this.enabled=false;
        }
        weaponManager = GetComponent<WeaponManager>();
        audioManager = GetComponent<AudioManager>();
        playerUI = GetComponent<PlayerSetup>().ui;
        if(weaponManager.GetcurrentGraphics())
            animator = weaponManager.GetcurrentGraphics().GetComponent<Animator>();
    
    }

    private void Update() {
        currentWeapon=weaponManager.GetcurrentWeapon();
        if(PauseMenu.IsOn)
            return;

        CheckShoot();
    }
    public void SetCurrentWeaponAnimator(Animator animator){
        this.animator = animator;
    }
    public void SetCurrentWeapon(PlayerWeapon playerWeapon){
        if(!playerWeapon)
            currentWeapon = null;
        else
            currentWeapon = playerWeapon;
    }
    private void CheckShoot(){
        if(!currentWeapon)
            return;

        switch(currentWeapon.fireType){
            case FireType.Automatic:{
                if(Input.GetButtonDown("Fire1") & isLocalPlayer){
                    InvokeRepeating("Shoot",0f,1f/currentWeapon.firerate);
                    if(!animator.GetBool("Shoot"))
                        SetAnimator("Shoot",true);
    
                } else if(Input.GetButtonUp("Fire1") & isLocalPlayer){
                    CancelInvoke("Shoot");
                    if(animator.GetBool("Shoot"))
                        SetAnimator("Shoot",false);
    
                }
                break;
            }
            case FireType.Burst:{
                    if(Input.GetButtonDown("Fire1") & isLocalPlayer & !isShooting)
                    {
                        StartCoroutine(BurstShoot());
                    } 
                break;
            }
            case FireType.SemiAutomatic:{
                if(ShootDelay>0){
                    ShootDelay -=Time.deltaTime;
                }
                if(Input.GetButtonDown("Fire1") & isLocalPlayer)
                {
                    if(ShootDelay <= 0){
                        Shoot();
                        ShootDelay = 1/currentWeapon.firerate;
                        if(!animator.GetBool("Shoot"))
                            SetAnimator("Shoot",true);
                    }

                }
                else
                {
                    if(animator.GetBool("Shoot"))
                        SetAnimator("Shoot",false);
                }
                break;
            }
        }
    }
    private IEnumerator BurstShoot(){
        isShooting = true;
        if(BurstAmt > 0){
            Shoot();
            BurstAmt--;
        }
        yield return new WaitForSeconds(1/currentWeapon.firerate);
        if(BurstAmt > 0){
            StartCoroutine(BurstShoot());
        }else{
            BurstAmt = 3;
            isShooting = false;
        }

    }
    [Command]
    void CmdOnShoot(){
        RpcDoShootEffect();
    }
    [ClientRpc]
    void RpcDoShootEffect(){
        //Graphics
        weaponManager.GetcurrentGraphics().muzzleFlash.Play();
    }
    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal){
        RpcDoHitEffect(pos,normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal){
        GameObject hiteff=  (GameObject)Instantiate(weaponManager.GetcurrentGraphics().hitEffect,pos,Quaternion.LookRotation(normal));
        Destroy(hiteff,2f); 
    }
    private void Shoot(){
        if (!isLocalPlayer){
            return;
        }        
        //Sounds
        currentWeapon.GetComponent<WeaponEffects>().TriggerSound();
        CmdOnShoot();

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)){
            if(hit.collider.tag == PLAYER_TAG){
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, this.gameObject.name);
                playerUI.SwitchCrosshairColor();
            }
            CmdOnHit(hit.point, hit.normal);

        }

    }
    public void SetAnimator(string boolName, bool isOn){
        if(isOn & !animator.GetBool(boolName)){
            animator.SetBool(boolName,true);
        }else{
            animator.SetBool(boolName,false);
        }
        
    }
    [Command]
    void CmdPlayerShot(string PID, int dmg, string Shooter){
        // Shot player
        Player _player = GameManager.GetPlayer(PID);
        _player.RpcGetDamaged(dmg,Shooter);
    }
}
