using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private float pickupRange= 2f;
    [SerializeField]
    private PlayerWeapon secondaryWeapon;
    [SerializeField]
    private PlayerWeapon meleeWeapon;
    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private Transform WepHolder;
    private PlayerWeapon currentWeapon;
    private WeaponEffects currentGraphics;
    [SerializeField]
    private string weaponLayerName ="Weapon";
    private PlayerController controller;
    private GameObject WepInst;
    private PlayerUI playerUI;
    public GameObject GetWepInstance(){
        return WepInst;
    }
    void Awake()
    {
        
    }
    void Start(){
        if(isLocalPlayer){
            Utility.SetLayerRecursively( WepInst,LayerMask.NameToLayer(weaponLayerName));
        }
        playerUI = GetComponent<PlayerSetup>().ui;
    }
    void FixedUpdate(){
        if(!isLocalPlayer)
            return;

        CheckWeapon();
    }
    private void CheckWeapon(){
        if(Input.GetKeyDown(KeyCode.G) && currentWeapon){
            UnequipWeapon(currentWeapon);
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        bool rayhit = Physics.Raycast(ray,out RaycastHit hitInfo, pickupRange);
        Debug.DrawLine(ray.origin, hitInfo.point, Color.blue, 0);
        if(rayhit){
            if(hitInfo.transform.tag == "Weapon"){
                EquipTextSetup(hitInfo.transform.name.ToString());
                if(Input.GetKeyDown(KeyCode.E))
                    EquipWeapon(hitInfo.transform.GetComponent<PlayerWeapon>());

            }else{
                playerUI.EquipText.enabled = false;
                playerUI.Crosshair.enabled = true;
            }
        }else{
            if(!playerUI)
                return;

            playerUI.EquipText.enabled = false;
            playerUI.Crosshair.enabled = true;
        }
    }
    public void EquipTextSetup(string Name){
        playerUI.EquipText.text = "Press E to Equip " + Name;
        playerUI.Crosshair.enabled = false;
        playerUI.EquipText.enabled = true;
    }

    public WeaponEffects GetcurrentGraphics(){
        return currentGraphics;
    }

    public PlayerWeapon GetcurrentWeapon(){
        return currentWeapon;
    }
    void EquipWeapon(PlayerWeapon wep){
        CmdEquipWep(wep.GetComponent<NetworkIdentity>());
    }
    [Command]
    void CmdEquipWep(NetworkIdentity weaponId){
        RpcEquipWeaponOnAllClients(weaponId);
    }
    [ClientRpc]
    void RpcEquipWeaponOnAllClients(NetworkIdentity wepId){
        PlayerWeapon wep = wepId.GetComponent<PlayerWeapon>();
        if(wep == currentWeapon )
            return;

        if(currentWeapon)
            UnequipWeapon(currentWeapon);

        WepInst = wep.gameObject;
        Debug.Log("Equipped " + WepInst);
        WepInst.transform.SetParent(WepHolder);
        WepInst.transform.localScale = Vector3.one;
        WepInst.transform.localEulerAngles = Vector3.zero;
        WepInst.GetComponent<Rigidbody>().useGravity = false;
        WepInst.GetComponent<Rigidbody>().isKinematic = true;
        WepInst.GetComponent<BoxCollider>().enabled = false;
        WepInst.GetComponent<WeaponEffects>().ToggleHalo(false);
        WepInst.GetComponent<Animator>().enabled = true;

        currentWeapon = wep;
        GetComponent<PlayerShoot>().SetCurrentWeapon(currentWeapon);
        GetComponent<PlayerShoot>().SetCurrentWeaponAnimator(currentWeapon.GetComponent<Animator>());
        currentGraphics = WepInst.GetComponent<WeaponEffects>();

        if(isLocalPlayer)
        {
            switch (wep.weaponType)
            {
                case WeaponType.Primary:
                {
                    primaryWeapon = wep;
                    break;
                }
                case WeaponType.Secondary:
                {
                    secondaryWeapon = wep;
                    break;
                }
                case WeaponType.Melee:
                {
                    meleeWeapon = wep;
                    break;
                } 
            }
            SetLayerRecursively(currentWeapon.gameObject,11);
        }

        if(currentGraphics == null){
            Debug.Log("No WeaponGraphics component on the weapon object." + WepInst.name);
        }
    }
    void UnequipWeapon(PlayerWeapon weapon){
        CmdUnEquipWep(weapon.GetComponent<NetworkIdentity>());
    }
    [Command]
    void CmdUnEquipWep(NetworkIdentity wepId){
        RpcUnequipWeaponOnAllClients(wepId);
    }
    [ClientRpc]
    void RpcUnequipWeaponOnAllClients(NetworkIdentity wepId){
        PlayerWeapon wep = wepId.GetComponent<PlayerWeapon>();
        SetLayerRecursively(wep.gameObject,0);
        
        wep.gameObject.transform.SetParent(null);
        if(isLocalPlayer){
            switch (wep.weaponType)
            {
                case WeaponType.Primary:
                {
                    primaryWeapon = null;
                    break;
                }
                case WeaponType.Secondary:
                {
                    secondaryWeapon = null;
                    break;
                }
                case WeaponType.Melee:
                {
                    meleeWeapon = null;
                    break;
                } 
            }
        }

        GetComponent<PlayerShoot>().SetCurrentWeapon(null);
        GetComponent<PlayerShoot>().SetCurrentWeaponAnimator(null);  
        currentWeapon = null;
        currentGraphics = null;
        wep.GetComponent<WeaponEffects>().ToggleHalo(true);
        wep.GetComponent<Rigidbody>().useGravity = true;
        wep.GetComponent<Rigidbody>().isKinematic = false;
        wep.GetComponent<BoxCollider>().enabled = true;
        wep.GetComponent<Animator>().enabled = false;
        Debug.Log("UnEquipped " + WepInst);
        WepInst = null;
    }
    public static void SetTagRecursively(GameObject Object, string tag){
        Object.tag = tag;
        GameObject []Children = Object.transform.GetComponentsInChildren<GameObject>();
        foreach (var item in Children)
        {
            item.tag = tag;
        }
    }
    public static void SetLayerRecursively(GameObject Object, LayerMask layer){
        Object.layer = layer;
        Transform []Children = Object.transform.GetComponentsInChildren<Transform>();
        foreach (var item in Children)
        {
            item.gameObject.layer = layer;
        }
    }
}
