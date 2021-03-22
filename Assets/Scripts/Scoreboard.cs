using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    private Transform ScoreBoardContainer;
    [SerializeField]
    private GameObject PlayerScorePrefab;
    void OnEnable() {
        //Do the title
        AddPlayer("NAME","KILLS","DEATHS");

        Player[] players=GameManager.GetAllPlayers();
        foreach (Player player in players)
        {
            AddPlayer(player.transform.name,player.kills.ToString(),player.deaths.ToString());
        }
    }
    void OnDisable() {
        foreach (Transform child in ScoreBoardContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    private void AddPlayer(string Username, string Kills, string Deaths){
        GameObject _PlayerScore = Instantiate(PlayerScorePrefab,Vector3.zero,Quaternion.identity);
        _PlayerScore.GetComponent<ScoreBoardItemSetter>().SetScore( Username ,Kills ,Deaths );
        _PlayerScore.transform.SetParent(ScoreBoardContainer);
        _PlayerScore.transform.localScale = new Vector3(1,1,1);
    }
}
