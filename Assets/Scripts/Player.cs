using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public enum WeaponSlot{
    Primary,
    Melee
}

[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ChatSystem))]
public class Player : NetworkBehaviour
{
    #region Initializations
    public WeaponSlot weaponSlot{
        get{return _weaponSlot;}
        protected set{_weaponSlot = value;}
    }
    [SyncVar]
    private WeaponSlot _weaponSlot = WeaponSlot.Primary;
    [SyncVar]
    private bool _isDead=false;
    public bool isDead{
        get{return _isDead;}
        protected set{_isDead=value;}
    }
    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private float maxArmor = 100.0f;
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled; 

    [SyncVar]
    private int currentHealth;
    [SyncVar]
    private int currentArmor;

    public int kills;
    private int _kills;
    public int deaths;

    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private ParticleSystem p1;
    [SerializeField]
    private ParticleSystem p2;
    [SerializeField]
    private GameObject DeathEffect;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;
    [SerializeField]
    private MeshRenderer mesh;
    [SerializeField]
    private GameObject SpawnEffect;
    private bool isFirst=true;
    private AudioManager audioManager;
    private Animator animator;
    private PlayerMotor motor;
    [SerializeField]
    private PlayerSetup playerSetup;
    [SerializeField]
    public ChatSystem chatSystem;
    [SerializeField]
    public Image KillSprite;
    public bool StationaryModeActive = false;
    [SerializeField]
    private const float StationaryModeTime = 2f;
    private float currentStationaryModeTime = 0f;
    #endregion
    IEnumerator LateStart(){
        yield return new WaitForSeconds(0.4f);
        if(isLocalPlayer){
            playerSetup.ui.UpdateHealthAndArmorBars(currentHealth/maxHealth,currentArmor/maxArmor);
            KillSprite = playerSetup.ui.Kill;
        }
    }
    private void Start(){
        audioManager = GetComponent<AudioManager>();
        motor = GetComponent<PlayerMotor>();
        animator = GetComponent<Animator>();
        chatSystem = GetComponent<ChatSystem>();
        if(isLocalPlayer)
            mesh.gameObject.layer = LayerMask.NameToLayer(playerSetup.dontDrawLayerName);
    
        StartCoroutine(LateStart());
    }
    [Client]
    public void SetupPlayer() {
        if (isLocalPlayer){
            GameManager.instance.SetsceneCameraActive(false);
            playerSetup.ui.HitVignette.enabled = false;
            playerSetup.PlayerUIInstance.SetActive(true);
        }
        CmdBroadCastNewPlayerSetup();
    }
    [Command]
    private void CmdBroadCastNewPlayerSetup(){
        RpcSetupPlayerOnAllClients();
    }
    [ClientRpc]
    private void RpcSetupPlayerOnAllClients(){
        if(isFirst)
        {
            wasEnabled= new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabled[i]=disableOnDeath[i].enabled;
            }
            isFirst=false;
        }
        SetDefaults();
    }
    [Command]
    public void CmdJumpStop(){
        RpcStopJump();
    }
    [ClientRpc]
    public void RpcStopJump(){
        p1.Stop();
        p2.Stop();
    }
    [Command]
    public void CmdJumpStart(){
        RpcStartJumpEffect();
    }
    [ClientRpc]
    public void RpcStartJumpEffect(){
        p1.Play();
        p2.Play();
    }
    [ClientRpc]
    public void RpcGetDamaged(int Amount, string _sourceID){
        if(isDead)
        return;
        
        //Audio
        audioManager.PlayOneShot("PlayerHit");
        currentArmor -= Amount;
        if(currentArmor<=0){
            currentHealth += currentArmor;
            currentArmor=0;
        }
        //Update UI
        if(isLocalPlayer){
            InitiateDamageVFX();
            playerSetup.ui.UpdateHealthAndArmorBars(currentHealth/maxHealth,currentArmor/maxArmor);
        }
        
        if(currentHealth<=0){
            Die(_sourceID);
        }
    }

    private void Update(){
        if(!isLocalPlayer){
            return;
        }else{
            if(kills!=_kills){
                StartCoroutine(KillReward());
                _kills=kills;
            }
            if(StationaryModeActive){
                currentStationaryModeTime-=Time.deltaTime;
                if(currentStationaryModeTime<= 0){
                    currentStationaryModeTime = 0;
                    StationaryModeSwitch();
                }
            }else{
                currentStationaryModeTime +=Time.deltaTime;
                if(currentStationaryModeTime > StationaryModeTime){
                    currentStationaryModeTime = StationaryModeTime;
                }
            }
            if(Input.GetKeyDown(KeyCode.LeftControl)){
                StationaryModeSwitch();
            }
            if(Input.mouseScrollDelta.y!=0){
                int ScrollAmt = (int)_weaponSlot + (int)Input.mouseScrollDelta.y;
                if(ScrollAmt< 0){
                    ScrollAmt = 1;
                }else if(ScrollAmt > 1){
                    ScrollAmt = 0;
                }
                _weaponSlot = (WeaponSlot)(ScrollAmt);
            }
        }
        
    }
    private void Die(string _sourceID){
        isDead=true;

        if(_sourceID!=null){
            Player sourcePlayer = GameManager.GetPlayer(_sourceID);
            if (sourcePlayer!=null){
                sourcePlayer.kills++;
            }
        }
            

        deaths++;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
        disableOnDeath[i].enabled=false;
        }
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        mesh.enabled=false;

        if(isLocalPlayer){
            GameManager.instance.SetsceneCameraActive(true);
            playerSetup.PlayerUIInstance.SetActive(false);
        }

        audioManager.PlayOneShot("DeathExplosion");
        GameObject GFXInst = (GameObject)Instantiate(DeathEffect,transform.position,Quaternion.identity);
        Destroy(GFXInst,3f);
        StartCoroutine(Respawn());
    }

    IEnumerator KillReward(){
        KillSprite.gameObject.SetActive(true);
        KillSprite.GetComponent<KillEffect>().enabled = true;
        audioManager.PlayOneShot("Kill");
        yield return new WaitForSeconds(0.4f);
        //KillSprite.gameObject.SetActive(false);
    }

    private IEnumerator Respawn(){
        yield return new WaitForSeconds(GameManager.instance.matchSet.respawnTime);
        Transform startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        yield return new WaitForSeconds(0.1f);
        SetupPlayer();
    }
    
    public void SetDefaults(){
        isDead=false;
        currentHealth = (int)maxHealth;
        currentArmor = (int)maxArmor;

        if(isLocalPlayer)
            playerSetup.ui.UpdateHealthAndArmorBars(currentHealth/maxHealth,currentArmor/maxArmor);
        
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled=wasEnabled[i];
        }
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }
        mesh.enabled=true;
        if(!audioManager)
            audioManager = GetComponent<AudioManager>();

        audioManager.PlayOneShot("Respawn");
        GameObject SpawnGFX= (GameObject)Instantiate(SpawnEffect, transform.position, Quaternion.identity);
        Destroy(SpawnGFX,3f);
    }
    public void ScreenShake(){
        animator.SetBool("IsHit", true);
        animator.SetBool("IsHit", false);
    }
    private void InitiateDamageVFX(){
        playerSetup.ui.DamageVFX();
    }

    private void StationaryModeSwitch(){
        if(!StationaryModeActive){
            Debug.Log("In Melee mode!");
            StationaryModeActive = true;
            //Stop movement
            rb.isKinematic = true;
            GetComponent<PlayerMotor>().SetMovement(false);
        }else{
            Debug.Log("NOT In Melee mode!");
            StationaryModeActive = false;
            //Resume movement
            rb.isKinematic = false;
            GetComponent<PlayerMotor>().SetMovement(true);
        }
    }
}