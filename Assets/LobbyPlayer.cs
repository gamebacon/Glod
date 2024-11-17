using UnityEngine;
using TMPro;

public class LobbyPlayer : MonoBehaviour
{
    public TMP_Text tag; 
    public string playerId; 

    public void Init(string name, string id) 
    {
        this.tag.text = name;
        this.playerId = id;
    }
}
