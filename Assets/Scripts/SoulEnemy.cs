using UnityEngine;
using UnityEngine.EventSystems;

public class SoulEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject InteractionPanelObject;
    [SerializeField] private GameObject ActionsPanelObject;
    [SerializeField] private SpriteRenderer EnemySpriteRenderer;

    public GameObject Combat_Button_Obj;
    public GameObject Bow_Button_Obj;
    public GameObject Sword_Button_Obj;

    private SpawnPoint _enemyPosition;

    [SerializeField] private int EXP_Value;
    [SerializeField] private float Weak_Multiplyer;
    [SerializeField] private bool Weak_To_Bow;


    public GameObject Return_Active_Button()
    {
        if (Combat_Button_Obj.activeInHierarchy)
            return Combat_Button_Obj;
        else
            return Bow_Button_Obj;
    }

    public bool Return_ActionsPanel_State()
    {
        if (ActionsPanelObject.activeInHierarchy)
            return true;
        else
            return false;
    }

    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint)
    {
        EnemySpriteRenderer.sprite = sprite;
        _enemyPosition = spawnPoint;
        gameObject.SetActive(true);

        GameEvents.In_Game_Enter += Enable_Buttons;
        GameEvents.In_Game_Leave += Disable_Buttons;

        EXP_Value = Score_Controller.Basic_Enemy_Value;
        Weak_Multiplyer = Score_Controller.Weakness_Multiplyer;



        int rng = Random.Range(0, 2);  // 0 or 1

        if (rng == 0)
            Weak_To_Bow = true;
        else
            Weak_To_Bow = false;
    }

    private void Enable_Buttons()
    {
        Combat_Button_Obj.SetActive(true);
        Bow_Button_Obj.SetActive(true);
        Sword_Button_Obj.SetActive(true);

    }

    private void Disable_Buttons()
    {
        Combat_Button_Obj.SetActive(false);
        Bow_Button_Obj.SetActive(false);
        Sword_Button_Obj.SetActive(false);
    }

    public SpawnPoint GetEnemyPosition()
    {
        return _enemyPosition;
    }

    public GameObject GetEnemyObject()
    {
        return this.gameObject;
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);
    }

    private void ActiveInteractionPanel(bool active)
    {
        InteractionPanelObject.SetActive(active);
    }

    private void ActiveActionPanel(bool active)
    {
        ActionsPanelObject.SetActive(active);
    }

    private void UseBow()
    {
        // USE BOW
        GameEvents.EnemyKilled?.Invoke(this);
        GameEvents.In_Game_Enter -= Enable_Buttons;
        GameEvents.In_Game_Leave -= Disable_Buttons;
        Add_Score(true);
    }

    private void UseSword()
    {
        GameEvents.EnemyKilled?.Invoke(this);
        GameEvents.In_Game_Enter -= Enable_Buttons;
        GameEvents.In_Game_Leave -= Disable_Buttons;
        // USE SWORD

        Add_Score(false);
    }

    private void Add_Score(bool Used_Bow)
    {
        if (Used_Bow == Weak_To_Bow)
            Score_Controller.Modify_Score(Mathf.RoundToInt(EXP_Value * Weak_Multiplyer));
        else
            Score_Controller.Modify_Score(EXP_Value);
    }



    #region OnClicks

    public void Combat_OnClick()
    {
        EventSystem.current.SetSelectedGameObject(Bow_Button_Obj);
        GameEvents.Combat_Started?.Invoke();
        ActiveCombatWithEnemy();
    }

    public void Bow_OnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        UseBow();
    }

    public void Sword_OnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
        UseSword();
    }

    #endregion
}


public interface IEnemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
}
