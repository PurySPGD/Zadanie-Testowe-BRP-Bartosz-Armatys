using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryView : UiView
{
    [Header("Inventory Elements")] [SerializeField]
    private SoulInformation SoulItemPlaceHolder;

    [SerializeField] private Text Description;
    [SerializeField] private Text Name;
    [SerializeField] private Image Avatar;
    [SerializeField] private Button UseButton;
    [SerializeField] private Button DestroyButton;

    private RectTransform _contentParent;
    private GameObject _currentSelectedGameObject;
    private GameObject _lastSelectedGameObject;

    private SoulInformation _currentSoulInformation;


    [SerializeField] public Startup_Button_Selector Inventory_Selector;
    [SerializeField] private List<GameObject> Soul_Informations;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 10f; // higher = faster scroll

    private Coroutine scrollCoroutine;

    public void Switch_Inventory_Selector(bool Value)
    {
            Inventory_Selector.enabled = Value;

        BackButon.gameObject.SetActive(Value);

    }


    private void ScrollToItem(RectTransform target)
    {
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = StartCoroutine(ScrollCoroutine(target));
    }

    private IEnumerator ScrollCoroutine(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        Vector3 childLocalPos = target.localPosition;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        float maxOffset = Mathf.Max(0f, contentHeight - viewportHeight);

        float currentOffset = content.anchoredPosition.y;
        float targetY = -childLocalPos.y;
        float desiredOffset = currentOffset;

        // top overflow
        if (targetY + target.rect.height > currentOffset + viewportHeight)
            desiredOffset = targetY + target.rect.height - viewportHeight;
        // bottom overflow
        else if (targetY < currentOffset)
            desiredOffset = targetY;

        desiredOffset = Mathf.Clamp(desiredOffset, 0f, maxOffset);

        // smooth scroll
        while (Mathf.Abs(content.anchoredPosition.y - desiredOffset) > 0.1f)
        {
            float newY = Mathf.Lerp(content.anchoredPosition.y, desiredOffset, Time.unscaledDeltaTime * scrollSpeed);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
            yield return null;
        }

        // snap exactly to final value
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, desiredOffset);
        scrollCoroutine = null;
    }
    void Update()
    {
        GameObject Current_Selected = EventSystem.current.currentSelectedGameObject;

        if (Current_Selected != null && Current_Selected.transform.IsChildOf(scrollRect.content) && Current_Selected != _lastSelectedGameObject)
        {
            _lastSelectedGameObject = Current_Selected;
            ScrollToItem(Current_Selected.GetComponent<RectTransform>());
        }
    }
    public override void Awake()
    {
        base.Awake();
        _contentParent = (RectTransform)SoulItemPlaceHolder.transform.parent;
       // InitializeInventoryItems(); przeniesione do Start ze względu na Score_Controller
    }

    private void Start()
    {
        InitializeInventoryItems();
    }

    private void InitializeInventoryItems()
    {


        for (int i = 0, j = SoulController.Instance.Souls.Count; i < j; i++)
        {
            SoulInformation newSoul = Instantiate(SoulItemPlaceHolder.gameObject, _contentParent).GetComponent<SoulInformation>();
            newSoul.SetSoulItem(SoulController.Instance.Souls[i], () => SoulItem_OnClick(newSoul));

            if(EventSystem.current.currentSelectedGameObject == null)
            {
                Inventory_Selector.Force_Select(newSoul.gameObject);
            }
            Soul_Informations.Add(newSoul.gameObject);
        }

        SoulItemPlaceHolder.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ClearSoulInformation();
    }

    private void ClearSoulInformation()
    {

        Description.text = "";
        Name.text = "";
        Avatar.sprite = null;
        SetupUseButton(false);
        SetupDestroyButton(false);
        _currentSelectedGameObject = null;
        _currentSoulInformation = null;
    }

    public void SoulItem_OnClick(SoulInformation soulInformation)
    {
        _currentSoulInformation = soulInformation;
        _currentSelectedGameObject = soulInformation.gameObject;
        SetupSoulInformation(soulInformation.soulItem);

        Inventory_Selector.Force_Select(_currentSelectedGameObject);

    }

    private void SetupSoulInformation(SoulItem soulItem)
    {
        Description.text = soulItem.Description;
        Name.text = soulItem.Name;
        Avatar.sprite = soulItem.Avatar;
        SetupUseButton(soulItem.CanBeUsed);
        SetupDestroyButton(soulItem.CanBeDestroyed);

        bool isInCorrectLocalization = GameControlller.Instance.IsCurrentLocalization(_currentSoulInformation.soulItem.UsableInLocalization);
        UseButton.interactable = isInCorrectLocalization;



        // Porzucone dla zwiększenia intuicyjnosci. zaimplementowanie wymagało by większej zmianu UI.
        // if (UseButton.IsInteractable())
        //     EventSystem.current.SetSelectedGameObject(UseButton.gameObject);
        // else if (DestroyButton.gameObject.activeSelf)
        //     EventSystem.current.SetSelectedGameObject(DestroyButton.gameObject);

    }




    private void CantUseCurrentSoul()
    {
        PopUpInformation popUpInfo = new PopUpInformation { DisableOnConfirm = true, UseOneButton = true, Header = "CAN'T USE", Message = "THIS SOUL CANNOT BE USED IN THIS LOCALIZATION" };
        GUIController.Instance.ShowPopUpMessage(popUpInfo);
    }

    private void UseCurrentSoul(bool canUse)
    {

        if (!canUse)
        {
            CantUseCurrentSoul();

        }
        else
        {
            //USE SOUL
            Score_Controller.Modify_Score(_currentSoulInformation.Soul_Value);
            DestroyCurrentSoul();
        }
    }

    private void DestroyCurrentSoul()
    {

        Soul_Informations.Remove(_currentSelectedGameObject);
        Destroy(_currentSelectedGameObject);
        ClearSoulInformation();



        if (Soul_Informations.Count > 0)
        Inventory_Selector.Force_Select(Soul_Informations[0]);

    }

   

    private void SetupUseButton(bool active)
    {
        UseButton.onClick.RemoveAllListeners();
        if (active)
        {
            bool isInCorrectLocalization = GameControlller.Instance.IsCurrentLocalization(_currentSoulInformation.soulItem.UsableInLocalization);
            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = isInCorrectLocalization,
                UseOneButton = false,
                Header = "USE ITEM",
                Message = "Are you sure you want to USE: " + _currentSoulInformation.soulItem.Name + " ?",
                Confirm_OnClick = () => UseCurrentSoul(isInCorrectLocalization)

            };
            UseButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
        }

        UseButton.gameObject.SetActive(active);
    }

    private void SetupDestroyButton(bool active)
    {
        DestroyButton.onClick.RemoveAllListeners();
        if (active)
        {
            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = true,
                UseOneButton = false,
                Header = "DESTROY ITEM",
                Message = "Are you sure you want to DESTROY: " + Name.text + " ?",
                Confirm_OnClick = () => DestroyCurrentSoul()
            };
            DestroyButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));

        }

        DestroyButton.gameObject.SetActive(active);
    }
}