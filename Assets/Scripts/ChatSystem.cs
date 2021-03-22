using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class ChatSystem : NetworkBehaviour
{
    public static ChatSystem instance;
    #region Initializations
    [SerializeField]
    private string netId;
    [SerializeField]
    private MessageManager messageManager;
    #endregion
    void Start(){
        netId = this.transform.name;
        messageManager.SetID(netId);
    }
    void Awake(){
        if (instance!=null){
        
        } else if(isLocalPlayer){
        instance=this;
        }
    }
    
    public void SetMessageManager(MessageManager manager){
        messageManager = manager;
    }
    
    private void Update(){
        if(Input.GetButtonDown("Enter")){
            messageManager.SwitchChat();
        }
    }
    
    
    [Client]
    public void SendMessage(string PlayerName,string message){
        CmdUpdateMessage(PlayerName,message);
    }
    [Command]
    public void CmdUpdateMessage(string PlayerName, string message){
        Player[] _Players = GameManager.GetAllPlayers();
        foreach (Player _player in _Players)
        {
            _player.chatSystem.RpcRecieveMessage(PlayerName, message);
        }
    }
    [ClientRpc]
    public void RpcRecieveMessage(string PlayerName, string message){
        if(isLocalPlayer){
            messageManager.AddMessage(PlayerName, message);
        }
    }
}
