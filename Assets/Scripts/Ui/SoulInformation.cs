using System;
using UnityEngine;
using UnityEngine.UI;

public class SoulInformation : MonoBehaviour
{
    [SerializeField] private Image MainImage;
    [SerializeField] private Button SoulButton;
    [HideInInspector] public SoulItem soulItem;
    public int Soul_Value;

    public void SetSoulItem(SoulItem _soulItem, Action OnSoulClick = null)
    {
        soulItem = _soulItem;
        MainImage.sprite = soulItem.Avatar;
        Soul_Value = UnityEngine.Random.Range(Score_Controller.Basic_Soul_Value_Min, Score_Controller.Basic_Soul_Value_Max);
        if (OnSoulClick != null) SoulButton.onClick.AddListener(() => OnSoulClick());
    }
}