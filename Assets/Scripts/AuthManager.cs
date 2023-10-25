using UnityEngine;
using Firebase;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{
    FirebaseAuth auth;
    public PlayerManager1 playerManager;
    public DbManager dbManager;
    public GameObject playerObject;
    


    void Start()
    {
        // Initialize Firebase Authentication
        FirebaseApp app = FirebaseApp.DefaultInstance;
        auth = FirebaseAuth.GetAuth(app);
    }

    public async void CreateGuestUser()
    {
        // auth.SignOut();
        var authTask = auth.SignInAnonymouslyAsync();
        await authTask;

        if (await authTask != null && authTask.IsCompletedSuccessfully)
        {
            // Guest user signed in successfully
            FirebaseUser user = authTask.Result.User;
            dbManager.userID = user.UserId;

            // Get the values from the PlayerLogic script
            playerManager.InitializePlayerData(playerObject);
            /* string name = playerLogic.GetName();
            float cash = playerLogic.GetCash();
            int bread = playerLogic.GetBread();
            int rice = playerLogic.GetRice();
            Debug.Log("Successfully created: "+" "+user.UserId+", "+name+" "+cash+", "+bread+", "+rice);*/
            dbManager.UpdatePlayerData();
    

            Debug.Log("Guest user signed in with ID: " + user.UserId);
        }
        else
        {
            // Handle sign-in error
            Debug.LogError("Guest user sign-in failed: " + authTask.Exception);
        }
    }
}
