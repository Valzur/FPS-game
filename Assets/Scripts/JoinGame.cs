using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
public class JoinGame : MonoBehaviour
{
    #region Initializations
    List<GameObject> roomList= new List<GameObject>();
    private NetworkManager networkManager;
    [SerializeField]
    private Text status;
    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;
    #endregion
    void Start(){
        networkManager=NetworkManager.singleton;
        if (networkManager.matchMaker==null){
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList(){
        ClearRoomList();

        if(networkManager.matchMaker==null){
            networkManager.StartMatchMaker();
        }
        networkManager.matchMaker.ListMatches(0,20,"",true,0,0,OnMatchList);
        status.text="Loading...";
    }

    public void OnMatchList(bool success, string extendedinfo, List<MatchInfoSnapshot> matchList){
        status.text="";
        if(matchList==null|| !success){
            status.text="Could not retrieve room list";
            return;
        }

        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject roomListItemGameObject = Instantiate(roomListItemPrefab);
            roomListItemGameObject.transform.SetParent(roomListParent);
            roomListItemGameObject.transform.localScale = new Vector3(1,1,1);
            RoomListItem _roomListItem= roomListItemGameObject.GetComponent<RoomListItem>();
            if(_roomListItem!=null){
                _roomListItem.Setup(match, JoinRoom);
            }

            roomList.Add(roomListItemGameObject);
        }
        if(roomList.Count==0){
            status.text="No rooms available at the moment...";
        }
    }
    void ClearRoomList(){
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }
        roomList.Clear();
    }
    public void JoinRoom(MatchInfoSnapshot _match){
        networkManager.matchMaker.JoinMatch(_match.networkId,"","","",0,0,networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }
    IEnumerator WaitForJoin(){
        ClearRoomList();
        int countDown=10;
        while (countDown>0)
        {
            status.text="Joining.. ("+ countDown +")";
            yield return new WaitForSeconds(1f);
            countDown--;
        }

        status.text="Failed to connect";
        yield return new WaitForSeconds(1f);

        MatchInfo matchInfo=networkManager.matchInfo;
        if (matchInfo!=null){
            networkManager.matchMaker.DropConnection(matchInfo.networkId,matchInfo.nodeId,0,networkManager.OnDropConnection);
            networkManager.StopHost();
        }

        RefreshRoomList();
    }
}