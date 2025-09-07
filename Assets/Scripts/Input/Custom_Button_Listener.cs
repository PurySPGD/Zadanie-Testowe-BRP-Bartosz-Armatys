using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Custom_Button_Listener : MonoBehaviour
{
    [SerializeField] private Button targetButton;   
    [SerializeField] private InputActionReference actionRef;

    private void OnEnable()
    {
        actionRef.action.performed += OnActionPerformed;
    }

    private void OnDisable()
    {
        actionRef.action.performed -= OnActionPerformed;
    }

    private void OnActionPerformed(InputAction.CallbackContext ctx)
    {


        targetButton.onClick.Invoke(); 
    }
}
