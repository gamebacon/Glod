using UnityEngine;
using TMPro;

public class LobbyPlayer : MonoBehaviour
{
    public TMP_Text gamerTag; 
    public string playerId; 

    public void Init(string name, string id) 
    {
        this.gamerTag.text = name;
        this.playerId = id;
    }
}
