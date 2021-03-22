using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MatchSettings matchSet;
    [SerializeField]
    private GameObject sceneCam;
    void Awake(){
        if (instance!=null){
        
        } else{
        instance=this;
        }
    }
    public void SetsceneCameraActive(bool isActive){
        if (sceneCam==null)
            return;
        
        sceneCam.SetActive(isActive);
    }
    #region Player tracking
    private const string PLAYER_ID_PREFIX ="Player ";
    private static Dictionary<string, Player> Players= new Dictionary<string, Player>();
    // Start is called before the first frame update
    void Start()
    {
    Cursor.visible=false;   
    }
    public static void UnregisterPlayer(string PlayerID){
        Players.Remove(PlayerID);
    }

    public static void RegisterPlayer(string netID, Player _player){
        string PlayerID= PLAYER_ID_PREFIX + netID;
        Players.Add(PlayerID, _player);
        _player.transform.name= PlayerID;
    }

    public static Player GetPlayer(string PlayerID){
        return Players[PlayerID];
    }
    public static Player[] GetAllPlayers(){
        return Players.Values.ToArray();
    } 
/*     private void OnGUI() {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));    
        GUILayout.BeginVertical();

        foreach(string PlayerID in Players.Keys){
            GUILayout.Label(PlayerID+" - "+Players[PlayerID].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    } */
    #endregion
}
