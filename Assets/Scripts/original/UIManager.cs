using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text NameText;
    public TMP_Text CashText;
    public TMP_Text BreadText;
    public TMP_Text RiceText;

    public InputField EmailInput;
    public InputField PassInput;
    public InputField NameInput;

    // Method to update the UI based on player data
    public void UpdateUI(PlayerData playerData)
    {
        NameText.text = "Name: " + playerData.name;
        CashText.text = "Cash: " + playerData.cash.ToString();
        BreadText.text = "Bread: " + playerData.bread.ToString();
        RiceText.text = "Rice: " + playerData.rice.ToString();
    }

    public void ClearUI(InputField NameInput, InputField EmailInput, InputField PassInput, TMP_Text NameText, TMP_Text CashText, TMP_Text BreadText, TMP_Text RiceText)
    {
        NameText.text = "Name: ";
        CashText.text = "Cash: ";
        BreadText.text = "Bread: ";
        RiceText.text = "Rice: ";
        EmailInput.text = string.Empty;
        PassInput.text = string.Empty;
        NameInput.text = string.Empty;
    }

    public void ToggleVisibility(bool isActive, GameObject objectToHide)
    {
        bool isObjectVisible = !isActive;
        objectToHide.SetActive(isObjectVisible);
    }

    public static GameObject FindGameObject(string name)
    {
        Debug.LogWarning("Finding GameObject..");
        GameObject obj = GameObject.Find(name);
        Debug.LogWarning("GameObject Found!");
        if (obj == null)
        {
            Debug.LogError("GameObject not found: " + name);
        }
        return obj;
    }

}