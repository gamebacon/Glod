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
    if ((bool) (UnityEngine.Object) this.nameText)
    {
      this.nameText.text = "";
      TextMeshProUGUI nameText = this.nameText;
      nameText.text = nameText.text + "\n<size=100%>" + this.username;
    }
    // this.hitable.SetId(this.id);
  }

  public void SetDesiredPosition(Vector3 position)
  {
    if (!(bool) (UnityEngine.Object) this.onlinePlayer)
      return;
    this.onlinePlayer.desiredPos = position;
  }

  public void SetDesiredRotation(float orientationY, float orientationX)
  {
    if (!(bool) (UnityEngine.Object) this.onlinePlayer)
      return;
    this.onlinePlayer.orientationY = orientationY;
    this.onlinePlayer.orientationX = orientationX;
  }

  public void SetDesiredHpRatio(float ratio) => this.onlinePlayer.hpRatio = ratio;

  public int CompareTo(object obj) => 0;

  public Collider GetCollider() => this.collider;
}
