using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;

//https://www.youtube.com/watch?v=3oNYmLMZ6-I
public class PlayfabManager : MonoBehaviour
{

    public bool logedin;
    public string Name;
    public string PFP;
    public int Steps;
    public int DMerits;
    public static PlayfabManager Instance;
    
    [SerializeField] Menu titleMenu;
    [SerializeField] TMP_Text SystemText;

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
    private void OnError(PlayFabError error)//Error msg
    {
        SystemText.text = error.ErrorMessage;
        //Debug.Log(error.GenerateErrorReport());
    }
    
    public void Login()
    {
        SystemText.text = "Loggin in...";
        var request = new LoginWithEmailAddressRequest
        {
            Email = EmailLoginInput.text,
            Password = PasswordLoginInput.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
            }
        };

        /*
        var request = new LoginWithEmailAddressRequest
        {
            Email = "test@test.com",
            Password = "testtest",

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
            }
        };
        */
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
        SystemText.text = " ";
        GetUserInventory();
        
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
        SystemText.text = "New Account Is Successfully Created";
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
        SystemText.text = "Recovery Mail Sent To Registered Mail";
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

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        Destroy(gameObject);
    }

    public void GetUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySucess, OnError);
    }

    private void OnGetUserInventorySucess(GetUserInventoryResult result)
    {
        Steps = result.VirtualCurrency["ST"];
        DMerits = result.VirtualCurrency["DM"];
        Launcher.Instance.SetPlayerDetails(Name, Steps, DMerits);
    }

    public void AddVirtualCurrency(string CurrencyCode, int Amount)
    {
        var request = new AddUserVirtualCurrencyRequest(){
            VirtualCurrency = CurrencyCode,
            Amount = Amount
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, OnUserVirtualCurrencyAdded, OnError);
    }

    private void OnUserVirtualCurrencyAdded(ModifyUserVirtualCurrencyResult result)
    {
        GetUserInventory();
    }
}
