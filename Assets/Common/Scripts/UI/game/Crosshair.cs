using UnityEngine;
using TMPro;
class CrossHair : MonoBehaviour 
{

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject crossHairActive;

    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text subTitle;

    private Transform _camTransform;
    private PlayerInteraction _interaction;
    private ControlManager _controlManager;

    private void Start()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");

        _interaction = playerGameObject.GetComponent<PlayerInteraction>();
        _camTransform = Camera.main.transform;
        _controlManager = GameManager.GetInstance().controlManager;
    }

    private void Update()
    {
       RaycastActive();
    }

    private void RaycastActive()
    {
        // improve performance, cache old pos?

        if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _interaction.interactionRange, LayerMask.GetMask("Interactable")))
        {
            Item item = hit.transform.GetComponent<Item>();
            SetItemInfo(item);
            ShowItemInfo(true);
            crossHairActive.SetActive(true);
        }
        else
        {
            crossHairActive.SetActive(false);
            ShowItemInfo(false);
        }
    }

    private void SetItemInfo(Item item)
    {
        title.text = item.itemInfo.GetItemName();
        subTitle.text = $"[{_controlManager.GetKeyString(PlayerAction.INTERACT)}] Pick up";
    }

    private void ShowItemInfo(bool show)
    {
        title.enabled = show;
        subTitle.enabled = show;
    }


    public void SetActive(bool active)
    {
        crosshair.SetActive(active);
    }

}