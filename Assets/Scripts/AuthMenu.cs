using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthMenu : MonoBehaviour
{
    public GameObject objectToHide;
    public Button buttonToPress;
    public Transform originalPosition;
    public Transform targetPosition;
    private bool isObjectVisible = true;


    public void signIn()
    {
        
    }

    public void signUp(){

    }

    public void guestEnter(){
        SceneManager.LoadScene("Game");
    }

    public void ToggleVisibility()
    {
        isObjectVisible = !isObjectVisible;
        objectToHide.SetActive(isObjectVisible);
    }

    public void MoveToTarget(){transform.position = targetPosition.position;}
    public void ReturnPosition(){transform.position = originalPosition.position;}
}
