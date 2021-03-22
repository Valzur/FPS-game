using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    public static HostGame instance;
    [SerializeField]
    private uint roomSize=6;
    private string roomName;
    private NetworkManager networkManager;
    void Start(){
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker==null){
            networkManager.StartMatchMaker();
        }
        
    }
    void Update(){
        if(instance!=this){
            instance = this;
        }
    }
    public void SetRoomName(string name){
        roomName=name;
    }

    public void CreateRoom(){
        if(roomName!=null && roomName!=""){
            Debug.Log("Creating room"+ roomName+ "with a size of "+ roomSize+ " Players");
            networkManager.matchMaker.CreateMatch(roomName,roomSize,true,"","","",0,0,networkManager.OnMatchCreate);
        }
    }
}
