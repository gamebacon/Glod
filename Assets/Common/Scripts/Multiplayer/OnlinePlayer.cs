using UnityEngine;

public class OnlinePlayer : MonoBehaviour
{
  public Animator animator;
  public Rigidbody rb;
  public Vector3 desiredPos;
  public float orientationY;
  public float orientationX;
  private float blendX;
  private float blendY;
  public bool grounded;
  public bool dashing;
  public LayerMask whatIsGround;
  public GameObject jumpSfx;
  public GameObject dashFx;
  private float moveSpeed = 15f;
  private float rotationSpeed = 13f;
  // private float animationBlendSpeed = 8f;
  public GameObject weapon;
  private MeshFilter filter;
  private MeshRenderer _renderer;
  public Transform hpBar;
  public Transform upperBody;
  public SkinnedMeshRenderer[] armor;
  private float currentTorsoRotation;
  private float lastFallSpeed;
  public GameObject footstepFx;
  private float distance;
  private float fallSpeed;
  public GameObject smokeFx;
  public Transform jumpSmokeFxPos;
  private float speed;

  public float hpRatio { get; set; } = 1f;

  private void Start()
  {
    this.grounded = true;
    /*
    this.filter = this.weapon.GetComponent<MeshFilter>();
    this.renderer = this.weapon.GetComponent<MeshRenderer>();
    */
  }

  private void FixedUpdate()
  {
    /*
    this.fallSpeed = Mathf.Abs(this.rb.linearVelocity.y);
    */
    this.rb.MovePosition(Vector3.Lerp(this.rb.position, this.desiredPos, Time.deltaTime * this.moveSpeed));
  }

  private void Update()
  {
    /*
    this.grounded = Physics.Raycast(this.transform.position, Vector3.down, 2.4f, (int) this.whatIsGround);
    this.Animate();
    this.Sfx();
    this.FootSteps();
    this.hpBar.localScale = new Vector3(Mathf.Lerp(this.hpBar.localScale.x, this.hpRatio, Time.deltaTime * 10f), 1f, 1f);
    */
    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0.0f, this.orientationY, 0.0f), Time.deltaTime * this.rotationSpeed);
  }

  private void LateUpdate()
  {
    /*
    this.currentTorsoRotation = Mathf.Lerp(this.currentTorsoRotation, this.orientationX, Time.deltaTime * this.rotationSpeed);
    this.upperBody.localRotation = Quaternion.Euler(this.currentTorsoRotation, this.upperBody.localRotation.y, this.upperBody.localRotation.z);
    this.lastFallSpeed = this.rb.linearVelocity.y;
    */
  }

  private void FootSteps()
  {
    if ((double) this.DistToPlayer() > 30.0 || !this.grounded)
      return;
    float num1 = 1f;
    float num2 = this.rb.linearVelocity.magnitude;
    if ((double) num2 > 20.0)
      num2 = 20f;
    this.distance += (float) ((double) num2 * (double) Time.deltaTime * 50.0);
    if ((double) this.distance <= 300.0 / (double) num1)
      return;
    Object.Instantiate<GameObject>(this.footstepFx, this.transform.position, Quaternion.identity);
    this.distance = 0.0f;
  }

  public int currentWeaponId { get; set; } = -1;


  private void Sfx()
  {
    double player = (double) this.DistToPlayer();
  }

  public void SpawnSmoke()
  {
    if ((double) this.DistToPlayer() > 30.0)
      return;
    Object.Instantiate<GameObject>(this.smokeFx, this.jumpSmokeFxPos.position, Quaternion.LookRotation(Vector3.up));
  }

  private void Animate()
  {
    this.speed = Mathf.Lerp(this.speed, Mathf.Clamp(this.rb.linearVelocity.magnitude * 0.1f, 0.0f, 1f), Time.deltaTime * 10f);
    this.animator.SetBool("Grounded", this.grounded);
    this.animator.SetFloat("FallSpeed", this.lastFallSpeed);
    this.animator.SetFloat("Speed", this.speed);
  }

  // private float DistToPlayer() => !(bool) (Object) PlayerMovement.Instance ? 1000f : Vector3.Distance(PlayerMovement.Instance.transform.position, this.transform.position);
  private float DistToPlayer() => 3;

  public void NewAnimation(int animation, bool b)
  {
    switch ((OnlinePlayer.SharedAnimation) animation)
    {
      case OnlinePlayer.SharedAnimation.Attack:
        this.animator.Play("Attack");
        break;
      case OnlinePlayer.SharedAnimation.Eat:
        this.animator.SetBool("Eating", b);
        break;
      case OnlinePlayer.SharedAnimation.Charge:
        this.animator.SetBool("Charging", b);
        break;
    }
  }

  public enum SharedAnimation
  {
    Attack,
    Eat,
    Charge,
  }
}
