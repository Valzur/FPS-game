using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(AudioSource))]
public class WeaponEffects : NetworkBehaviour
{
    private AudioSource audioSource;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffect;
    public GameObject WeaponHalo;
    private GameObject currentWeaponHalo;
    [SerializeField]
    private AudioClip ShootSound;
    void Start(){
        audioSource = GetComponent<AudioSource>();
        ToggleHalo(true);
    }
    public void ToggleHalo(bool Active){
        CmdServerToggleHalo(Active);
    }
    [Command]
    private void CmdServerToggleHalo(bool Active){
        RpcToggleHalo(Active);
    }
    [ClientRpc]
    private void RpcToggleHalo(bool Active){
        if(!Active)
        {
            if(currentWeaponHalo)
                currentWeaponHalo.gameObject.SetActive(false);

            return;
        }      
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position,Vector3.down,out hit))
        {
            if(!currentWeaponHalo)
            {
                currentWeaponHalo = Instantiate(WeaponHalo,hit.point,Quaternion.identity);
            }
            else
            {
                currentWeaponHalo.transform.position = hit.point;
                currentWeaponHalo.SetActive(true);
            }
            currentWeaponHalo.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
            currentWeaponHalo.transform.position += new Vector3(0,0.001f,0);
        }
    }
    public void TriggerSound()
    {
        CmdTriggerSound();
    }
    [Command]
    private void CmdTriggerSound()
    {
        RpcTriggerSound();
    }
    private void RpcTriggerSound()
    {
        audioSource.PlayOneShot(ShootSound);
    }
}