using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    string remoteLayerName = "RemotePlayer";
    public string dontDrawLayerName ="DontDraw";
    [SerializeField]
    private GameObject PlayerUIPrefab;
    [HideInInspector]
    public GameObject PlayerUIInstance;

    public PlayerUI ui;

    private void Start() {
        if(!isLocalPlayer){
            DisableComponents();
            AssignRemoteLayer();
        } else {
            GameManager.instance.SetsceneCameraActive(false);
            PlayerUIInstance = Instantiate(PlayerUIPrefab);
            PlayerUIInstance.name = PlayerUIPrefab.name;

            ui = PlayerUIInstance.GetComponent<PlayerUI>();
            GetComponent<PlayerController>().playerUI = ui;
            GetComponent<ChatSystem>().SetMessageManager(ui.messageManager);
            ui.messageManager.chatSystem = GetComponent<ChatSystem>();

            ui.SetController(GetComponent<PlayerController>());
            GetComponent<Player>().SetupPlayer();
        }
        
    }

    public override void OnStartClient(){
        base.OnStartClient();
        string netID= GetComponent<NetworkIdentity>().netId.ToString();
        Player _player=GetComponent<Player>();
        GameManager.RegisterPlayer(netID,_player);
        Debug.Log("Ping: " + NetworkManager.singleton.client.GetRTT());
    }

    void RegisterPlayer(){
        string ID= "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name= ID;        
    }
    private void OnDisable() {
        Destroy(PlayerUIInstance);
        if(isLocalPlayer){
        GameManager.instance.SetsceneCameraActive(true);
        }
        GameManager.UnregisterPlayer(transform.name);
    }
    
    void AssignRemoteLayer(){
        gameObject.layer=LayerMask.NameToLayer(remoteLayerName);
    }
    void DisableComponents(){
        for(int i =0; i< componentsToDisable.Length; i++){
                componentsToDisable[i].enabled=false;
            }
    }
}
