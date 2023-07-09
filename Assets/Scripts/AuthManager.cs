using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;

public class AuthManager : MonoBehaviour
{
    [SerializeField] DependencyStatus dependencyStatus;
    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference DBreference;

    [SerializeField] TMP_InputField loginEmailField;
    [SerializeField] TMP_InputField loginPasswordField;
    [SerializeField] TMP_Text loginWarningText;
    [SerializeField] TMP_Text loginSuccessText;

    [SerializeField] TMP_InputField registerEmailField;
    [SerializeField] TMP_InputField registerPasswordField;
    [SerializeField] TMP_InputField registerConfirmPasswordField;
    [SerializeField] TMP_Text registerWarningText;

    [SerializeField] TMP_Text playerHP;
    [SerializeField] TMP_Text playerScore;

    void Awake()
    {
        DontDestroyOnLoad(this);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void LoginButton()
    {
        StartCoroutine(Login(loginEmailField.text, loginPasswordField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(registerEmailField.text, registerPasswordField.text));
    }

    public void SaveButton()
    {
        StartCoroutine(UpdateHP(int.Parse(playerHP.text)));
        StartCoroutine(UpdateScore(int.Parse(playerScore.text)));
    }

    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => LoginTask.IsCompleted);
        //handle any errors
        if (LoginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            loginWarningText.text = message;
        }
        else
        {
            //User is logged in
            user = LoginTask.Result.User;
            Debug.LogFormat("Signed in as: {0}", user.Email);
            loginWarningText.text = "";
            loginSuccessText.text = "Logged in";
            StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(3);
            UIManager.instance.ShowGameUI();
        }
    }

    private IEnumerator Register(string _email, string _password)
    {
        //check if passwords match
        if (registerPasswordField.text != registerConfirmPasswordField.text)
        {
            registerWarningText.text = "Password Does Not Match!";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(() => RegisterTask.IsCompleted);
            //handle any errors
            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                registerWarningText.text = message;
            }
            else
            {
                //User has been created
                user = RegisterTask.Result.User;

                //return to login screen
                UIManager.instance.ShowLoginScreen();
                registerWarningText.text = "";
            }
        }
    }

    private IEnumerator UpdateHP(int _HP)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("HP").SetValueAsync(_HP);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateScore(int _score)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("Score").SetValueAsync(_score);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        }
    }

    private IEnumerator LoadUserData()
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            playerHP.text = "0";
            playerScore.text = "0";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot dSnapshot = DBTask.Result;

            playerHP.text = dSnapshot.Child("HP").Value.ToString();
            playerScore.text = dSnapshot.Child("Score").Value.ToString();
        }
    }
}
