using TMPro;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public PlayerLogic playerLogic;
    public TMP_Text NameText;
    public TMP_Text CashText;
    public TMP_Text BreadText;
    public TMP_Text RiceText;

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Quit play mode in the Unity Editor
        #else
            Application.Quit(); // Quit the standalone build
        #endif
    }

    public void UpdateNameText()
    {
        NameText.text = "Name"+ playerLogic.GetName();
    }

    public void UpdateCashText()
    {
        CashText.text = "Cash"+ playerLogic.GetCash();
    }

    public void UpdateBreadText()
    {
        BreadText.text = "Bread"+ playerLogic.GetBread();
    }

    public void UpdateRiceText()
    {
        RiceText.text = "Rice"+ playerLogic.GetRice();
    }
}
