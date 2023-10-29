using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class LoginPagePlayfab : MonoBehaviour
{

    public bool logedin;
    public string Name;
    public string PFP;
    public static LoginPagePlayfab Instance;
    
    [SerializeField] Menu titleMenu;
    [SerializeField] TMP_Text MessageText;

    [Header("Login")]
    [SerializeField] TMP_InputField EmailLoginInput;
    [SerializeField] TMP_InputField PasswordLoginInput;
    [SerializeField] GameObject LoginPage;

    [Header("Register")]
    [SerializeField] TMP_InputField UsernameRegisterInput;
    [SerializeField] TMP_InputField EmailRegisterInput;
    [SerializeField] TMP_InputField PasswordRegisterInput;
    [SerializeField] GameObject RegisterPage;

    [Header("Recovery")]
    [SerializeField] TMP_InputField EmailRecoveryInput;
    [SerializeField] GameObject RecoverPage;

    public void Awake() 
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    
    private void Update() 
    {   
        logedin = PlayFabClientAPI.IsClientLoggedIn();
        if(Input.GetKeyDown(KeyCode.Return) && PlayFabClientAPI.IsClientLoggedIn() == false)
        {
            Login();
        }
    } 


    ///<summary>
    /// Logout of PlayFab (Forget All Credentials). Also Destroys Login Playfab Game Object
    ///</summary>
    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        Destroy(gameObject);
    }

    #region Button Functions

    public void Login()
    {
        MessageText.text = "Loggin in...";
        var request = new LoginWithEmailAddressRequest
        {
            Email = EmailLoginInput.text,
            Password = PasswordLoginInput.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess ,OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {   
        if(result.InfoResultPayload != null)
        {
            Name = result.InfoResultPayload.PlayerProfile.DisplayName;
            PFP = result.InfoResultPayload.PlayerProfile.AvatarUrl;
        }
        MenuManager.Instance.OpenMenu(titleMenu);
        MessageText.text = " ";
        Launcher.Instance.SetPlayerDetails(Name);
    }

    public void RegisterUser()
    {
        //if pass is less than 6 chars
        //MessageText.text = "Password Too Short";
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = UsernameRegisterInput.text,
            Email = EmailRegisterInput.text,
            Password = PasswordRegisterInput.text,

            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,OnRegisterSuccess,OnError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        MessageText.text = "New Account Is Successfully Created";
        OpenLogin();
    }

    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = EmailRecoveryInput.text,
            TitleId = "FA25D",
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnError);
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        OpenLogin();
        MessageText.text = "Recovery Mail Sent To Registered Mail";
    }

    private void OnError(PlayFabError error)//Error msg
    {
        MessageText.text = error.ErrorMessage;
        //Debug.Log(error.GenerateErrorReport());
    }
    public void OpenLogin()
    {
        LoginPage.SetActive(true);
        RegisterPage.SetActive(false);
        RecoverPage.SetActive(false);
    }

    public void OpenRegister()
    {
        LoginPage.SetActive(false);
        RegisterPage.SetActive(true);
        RecoverPage.SetActive(false);
    }

    public void OpenRecovery()
    {
        LoginPage.SetActive(false);
        RegisterPage.SetActive(false);
        RecoverPage.SetActive(true);
    }

    #endregion
}
