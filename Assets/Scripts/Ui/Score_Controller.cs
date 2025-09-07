using UnityEngine;
using TMPro;

public class Score_Controller : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Score_Text;
    [SerializeField] private int Score;
    [SerializeField] private float Weakness_Multiplyer_;
    [SerializeField] private int Basic_Enemy_Value_;
    [SerializeField] private int Basic_Soul_Value_Max_;
    [SerializeField] private int Basic_Soul_Value_Min_;

    private static Score_Controller instance;
    
    public static float Weakness_Multiplyer;
    public static int Basic_Enemy_Value;
    public static int Basic_Soul_Value_Max;
    public static int Basic_Soul_Value_Min;

    private void Awake()
    {
        instance = this;

        instance.Score_Text.text = "Score: 0";

        Weakness_Multiplyer = Weakness_Multiplyer_;
        Basic_Enemy_Value = Basic_Enemy_Value_;
        Basic_Soul_Value_Max = Basic_Soul_Value_Max_;
        Basic_Soul_Value_Min = Basic_Soul_Value_Min_;
    }
   

    public static void Modify_Score(int Value)
    {
        Debug.Log("Modify Score by:" + Value);

        instance.Score = instance.Score + Value;
        instance.Score_Text.text = "Score: " + instance.Score.ToString();
    }
}
