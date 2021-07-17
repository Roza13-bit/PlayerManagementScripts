using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;

    public FirebaseAuth auth;

    public FirebaseUser User;

    public string usernameReference;

    public DatabaseReference DBreference;


    [SerializeField] InputField registerUsernameInputField;

    [SerializeField] InputField registerEmailInputField;

    [SerializeField] InputField registerPasswordInputField;

    [SerializeField] InputField registerPasswordVerifyInputField;

    [SerializeField] InputField loginUsernameIF;

    [SerializeField] InputField loginPasswordIF;

    [SerializeField] Text warningLoginText;

    [SerializeField] GameObject registerPanel;

    // [SerializeField] GameObject wrongPanel;

    [SerializeField] Text warningRegisterText;

    //User Data variables
    //[Header("UserData")]
    //public Text usernameField;
    //public Text foodFloat;
    //public Text waterFloat;
    //public Text moneyFloat;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Authentication"); if (objs.Length > 1)
        {
            Destroy(this.gameObject);

        }

        DontDestroyOnLoad(this.gameObject);

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();


                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    auth.SignOut();

                }

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
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(loginUsernameIF.text, loginPasswordIF.text));

    }

    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(registerEmailInputField.text, registerPasswordInputField.text, registerUsernameInputField.text));
    }

    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        SceneManager.LoadScene(0);
        ClearRegisterFeilds();
        ClearLoginFeilds();

    }

    public void ClearLoginFeilds()
    {
        loginUsernameIF.text = "";
        loginPasswordIF.text = "";
    }
    public void ClearRegisterFeilds()
    {
        registerUsernameInputField.text = "";
        registerEmailInputField.text = "";
        registerPasswordInputField.text = "";
        registerPasswordVerifyInputField.text = "";
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
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
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";

            //usernameField.text = User.DisplayName;

            StartCoroutine(LoginCoroutine());

            ClearLoginFeilds();
            ClearRegisterFeilds();
        }

    }

    public IEnumerator LoginCoroutine()
    {
        usernameReference = User.DisplayName;

        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene(1);

    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (registerPasswordInputField.text != registerPasswordVerifyInputField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";

        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
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
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        registerPanel.SetActive(false);
                        warningRegisterText.text = "";

                        ClearLoginFeilds();
                        ClearRegisterFeilds();

                    }
                }
            }
        }
    }

    public void OpenRegisterMenu()
    {
        registerPanel.SetActive(true);

    }

    public void CloseRegisterMenu()
    {
        registerPanel.SetActive(false);

    }

}
