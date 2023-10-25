/*
    Contains methods related to user authentication
*/

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserAuthentication : MonoBehaviour
{
    public InputField NameInput;
    public InputField EmailInput;
    public InputField PassInput;
    public TMP_Text NameText;
    public TMP_Text CashText;
    public TMP_Text BreadText;
    public TMP_Text RiceText;
    public Transform originalPosition;
    public Transform targetPosition;
    
    public static string userID;
    
    private DatabaseReference dbReference;
    private UIManager uiManager;

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        uiManager = FindObjectOfType<UIManager>(); // Assuming there's only one UIManager in the scene
    }

    #region User Authentication

    public void LoginAsGuest()
    {
        // Check if action is valid
        // if (PlayerManager.eventStatus == "Guest") { Debug.LogWarning("Already a Guest"); return; }
        // else if (PlayerManager.eventStatus == "SignIn") { Debug.LogWarning("Already Sign In, no need to be a Guest!"); return; }
        SignOutUser();

        // Authenticate the user anonymously
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("Anonymous sign-in was canceled!");
            }
            else if (task.IsFaulted)
            {
                FirebaseException exception = task.Exception.Flatten().InnerExceptions[0] as FirebaseException;
                if (exception != null)
                {
                    Debug.LogError("Failed to sign in anonymously: " + exception.Message);
                }
            }
            else if (task.IsCompleted)
            {
                AuthResult authResult = task.Result;
                FirebaseUser user = authResult.User;
                Debug.Log("User signed in anonymously: " + user.UserId);
                userID = user.UserId;
                PlayerManager.eventStatus = "Guest";
            }
        });
    }
    public void CreateUser()
    {
        // Check if action is valid
        if (PlayerManager.eventStatus == "SignIn") { Debug.LogWarning("Already Sign In!"); return; }

        if (dbReference == null)
        {
            Debug.LogError("Database reference is null.");
            return;
        }

        string email = EmailInput.text;
        string password = PassInput.text;

        // Check if email or password is empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Email and password cannot be empty.");
            return;
        }

        int startingCash = 100;
        int startingBread = 5;
        int startingRice = 3;

        // Authenticate user with provided email and password
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                FirebaseException exception = task.Exception?.InnerExceptions[0] as FirebaseException;
                if (exception != null && exception.Message.Contains("email"))
                {
                    Debug.LogWarning("Email is already in use: " + email);
                }
                else
                {
                    Debug.LogError("Failed to create user: " + task.Exception);
                }
            }
            else if (task.IsCompleted)
            {
                AuthResult authResult = task.Result;
                FirebaseUser user = authResult.User;

                if (user != null)
                {
                    // Generate a unique user ID based on email (can be replaced with other methods as needed)
                    string uid = user.UserId;
                    string username = GenerateUserIDFromEmail(email);

                    // Create the User object and store it in the database
                    PlayerData newUser = new (username, startingCash, startingBread, startingRice);
                    string json = JsonUtility.ToJson(newUser);

                    dbReference.Child("users").Child(uid).SetRawJsonValueAsync(json);

                    Debug.Log("User created successfully: " + uid);
                    PlayerManager.eventStatus = "SignIn";
                    userID = uid;
                }
            }
        });
    }
    private string GenerateUserIDFromEmail(string email) // Method to get a user ID based on email
    {
        // Split the email at the '@' character and take the left portion
        string[] parts = email.Split('@');
        if (parts.Length > 0)
        {
            return parts[0];
        }

        // If no '@' character found or the email is empty, return a full string
        return email;
    }

    public void SignInUser()
    {
        // Check if action is valid
        if (PlayerManager.eventStatus == "SignIn") { Debug.LogWarning("Already Sign In!"); return; }
        
        string email = EmailInput.text;
        string password = PassInput.text;

        // Validate email and password
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Email and password cannot be empty.");
            return;
        }

        // Authenticate user with email and password
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("Sign-in was canceled.");
            }
            else if (task.IsFaulted)
            {
                // Handle sign-in failure
                FirebaseException exception = task.Exception.Flatten().InnerExceptions[0] as FirebaseException;
                if (exception != null)
                {
                    Debug.LogWarning("Failed to sign in: The email or password is incorrect.");   
                }
            }
            else if (task.IsCompleted)
            {
                // Handle sign-in success
                AuthResult authResult = task.Result;
                FirebaseUser user = authResult.User;
                Debug.Log("User signed in successfully: " + user.UserId);
                userID = user.UserId;
                PlayerManager.eventStatus = "SignIn";
                
            }
        });
    }

    public void SignOutUser()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            userID=null;
            Debug.Log("User signed out successfully.");
            uiManager.ClearUI(NameInput, EmailInput, PassInput, NameText, CashText, BreadText, RiceText);
            PlayerManager.eventStatus = "NoAccount";
        }
        else
        {
            Debug.LogWarning("No user is currently signed in.");
        }
    }

    public void DeleteAccount()
    {
        if (dbReference == null)
        {
            Debug.LogError("Database reference is null.");
            return;
        }

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            string uid = auth.CurrentUser.UserId;
            uiManager.ClearUI(NameInput, EmailInput, PassInput, NameText, CashText, BreadText, RiceText);

            // Remove the userID node from the database
            dbReference.Child("users").Child(uid).RemoveValueAsync().ContinueWith(removeUserIDTask =>
            {
                if (removeUserIDTask.IsCanceled || removeUserIDTask.IsFaulted)
                {
                    Debug.LogError("Failed to remove userID from the database: " + removeUserIDTask.Exception);
                }
                else if (removeUserIDTask.IsCompleted)
                {
                    // User deleted from the database, now delete the account
                    auth.CurrentUser.DeleteAsync().ContinueWith(deleteTask =>
                    {
                        if (deleteTask.IsCanceled || deleteTask.IsFaulted)
                        {
                            Debug.LogError("Failed to delete user: " + deleteTask.Exception);
                        }
                        else if (deleteTask.IsCompleted)
                        {
                            Debug.Log("User account deleted and removed from the database: " + uid);
                            PlayerManager.eventStatus = "NoAccount";
                        }
                    });

                }
            });
        }
        else
        {
            Debug.LogWarning("No user is currently signed in.");
        }
    }
    #endregion
    public void MoveToTarget(){transform.position = targetPosition.position;}
    public void ReturnPosition(){transform.position = originalPosition.position;}

    #region User Menu Behavior
    public void AuthSuccess()
    {
        Debug.Log("Performing if auth success...");

        //Hide AuthMenu
        GameObject AuthMenu = GameObject.Find("AuthMenu");
        uiManager.ToggleVisibility(AuthMenu.activeSelf,AuthMenu);
        GameObject GuestButton = GameObject.Find("GuestButton");
        uiManager.ToggleVisibility(GuestButton.activeSelf,GuestButton);

        //Hide Sign In/Sign Up
        GameObject SignInUp = GameObject.Find("SignIn/SignUpButton");
        uiManager.ToggleVisibility(SignInUp.activeSelf,SignInUp);
        
        Debug.LogWarning("Successfully Hidden!");

        //Show PlayerStatus
        GameObject playerStatus = UIManager.FindGameObject("PlayerStatus");
        Debug.LogWarning("Activating UI");
            uiManager.ToggleVisibility(playerStatus.activeSelf, playerStatus);
        Debug.LogWarning("UI is activated");
        
        //Show Sign Out
        GameObject SignOutButton = UIManager.FindGameObject("SignOutButton");
        uiManager.ToggleVisibility(SignOutButton.activeSelf,SignOutButton);
        

    }
    #endregion
}
