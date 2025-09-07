using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Startup_Button_Selector : MonoBehaviour
{
    [SerializeField] private bool Adv_Selector;
    

    [SerializeField] private int Selector_Priority;
    [SerializeField] private float Delay;

    [SerializeField] private GameObject defaultButton; 
    [SerializeField] private float checkInterval = 0.2f;

    [SerializeField] private EnenmiesController EnenmiesController;


    public void Force_Select(GameObject Selectable)
    {
        defaultButton = Selectable;
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }


    private void Reset_Button()
    {
        defaultButton = EventSystem.current.currentSelectedGameObject;
    }

    private void X(IEnemy enemy)
    {
        Force_Select(EnenmiesController.A());
    }


    private void OnEnable()
    {
        if (Adv_Selector)
        {
            GameEvents.EnemyKilled += X;
            GameEvents.Combat_Started += Reset_Button;
        }

        StartCoroutine(HandleSelectorWithDelay(Delay));
    }

    private IEnumerator HandleSelectorWithDelay(float delay)
    {

        yield return new WaitForSeconds(delay);

        if (defaultButton != null)
        {

            EventSystem.current.SetSelectedGameObject(null);
            Activate_Selector();
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }
    private float timer = 0f;
    private void Update()
    {
        if (Adv_Selector)
            return;

        timer += Time.unscaledDeltaTime;

        if (timer >= checkInterval)
        {
            CheckSelectedButton();
            timer = 0f;
        }
    }





    private void Activate_Selector()
    {
        InvokeRepeating(nameof(CheckSelectedButton), checkInterval, checkInterval);
    }
    private void Deactivate_Selector()
    {
        CancelInvoke(nameof(CheckSelectedButton));
    }

    private void OnDisable()
    {
        if (defaultButton != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (Adv_Selector)
        {
            GameEvents.EnemyKilled -= X;
            GameEvents.Combat_Started -= Reset_Button;
        }



        Deactivate_Selector();
    }

    private void CheckSelectedButton()
    {
        if (EventSystem.current.currentSelectedGameObject != null)


        if (!gameObject.activeInHierarchy)
            return;



        if (EventSystem.current.currentSelectedGameObject == null && defaultButton != null && defaultButton.activeSelf! && Adv_Selector)
            Force_Select(EnenmiesController.A());

        if (EventSystem.current.currentSelectedGameObject == null && defaultButton == null && Adv_Selector)
            Force_Select(EnenmiesController.A());

        if (EventSystem.current.currentSelectedGameObject == null && defaultButton != null && defaultButton.activeInHierarchy) // If nothing is selected
        {
            Debug.Log("No button selected. Selecting the default one.");
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }
}















