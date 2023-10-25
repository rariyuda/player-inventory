using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/UI Data")]
public class UIData : ScriptableObject
{
    public Text NameText;
    public Text CashText;
    public Text BreadText;
    public Text RiceText;

    public InputField EmailInput;
    public InputField PassInput;
    public InputField NameInput;
}
