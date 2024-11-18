// Decompiled with JetBrains decompiler
// Type: PingController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8DB17789-D6D5-48DE-86AB-E696A5FF6B2B
// Assembly location: D:\SteamLibrary\steamapps\common\Muck\Muck_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class PingController : MonoBehaviour
{
  public LayerMask whatIsPingable;
  public GameObject pingPrefab;
  private float pingCooldown = 1f;
  private bool readyToPing;
  public static PingController Instance;

  private void Awake()
  {
    PingController.Instance = this;
    this.readyToPing = true;
  }

  private void Update()
  {
    if (!Input.GetMouseButtonDown(2))
      return;
    this.LocalPing();
  }

  private void LocalPing()
  {
    if (!this.readyToPing)
      return;
    this.readyToPing = false;
    this.Invoke("PingCooldown", this.pingCooldown);
    Vector3 pingPos = this.FindPingPos();
    if (pingPos == Vector3.zero)
      return;
    this.MakePing(pingPos, GameManager.players[LocalClient.instance.myId].username, "");
    ClientSend.PlayerPing(pingPos);
  }

  private Vector3 FindPingPos()
  {
    return Vector3.zero;
    /*
    Transform playerCam = PlayerMovement.Instance.playerCam;
    RaycastHit hitInfo;
    if (!Physics.Raycast(playerCam.position, playerCam.forward, out hitInfo, 1500f))
      return Vector3.zero;
    Vector3 vector3 = Vector3.zero;
    if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
      vector3 = Vector3.one;
    return hitInfo.point + vector3;
    */
  }

  public void MakePing(Vector3 pos, string name, string pingedName) => Debug.Log("ping fix"); // Object.Instantiate<GameObject>(this.pingPrefab, pos, Quaternion.identity).GetComponent<PlayerPing>().SetPing(name, pingedName);

  private void PingCooldown() => this.readyToPing = true;
}
