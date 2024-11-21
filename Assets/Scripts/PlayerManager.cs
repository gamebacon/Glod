using System;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IComparable
{
  public int id;
  public string username;
  public bool isDead;
  public Color color;
  public OnlinePlayer onlinePlayer;
  public int kills;
  public int deaths;
  public int ping;
  public bool isDisconnected;
  public bool loaded;
  public TextMeshProUGUI nameText;
  private Collider collider;
  public Transform spectateOrbit;

  public int graveId { get; set; }

  private void Awake()
  {
    this.collider = this.GetComponent<Collider>();
  }

  private void Start()
  {
    if (nameText)
    {
      nameText.text = "";
      nameText.text = nameText.text + "\n<size=100%>" + this.username;
    }
    // this.hitable.SetId(this.id);
  }

  public void SetDesiredPosition(Vector3 position)
  {
    if (!onlinePlayer)
      return;
    onlinePlayer.desiredPos = position;
  }

  public void SetDesiredRotation(float orientationY, float orientationX)
  {
    if (!onlinePlayer)
      return;
    onlinePlayer.orientationY = orientationY;
    onlinePlayer.orientationX = orientationX;
  }

  public void SetDesiredHpRatio(float ratio) => onlinePlayer.hpRatio = ratio;

  public int CompareTo(object obj) => 0;

  public Collider GetCollider() => this.collider;
}
