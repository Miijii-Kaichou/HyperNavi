using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    private static PlayFabLogin Instance;

    [SerializeField]
    private GameObject loginPanel;

    [SerializeField]
    private GameObject addLoginPanel;

    [SerializeField]
    private GameObject recoverButton;

    private string userName;
    private string userEmail;
    private string userPassword;

    public static bool isGetCurrencyRequestDone = false;

    private void OnEnable()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "892A0";
        }
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");

            //Try to log in with the given userEmail and userPassword
            var request = new LoginWithEmailAddressRequest
            {
                Email = userEmail,
                Password = userPassword
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest
            {
                AndroidDeviceId = GetUniqueIdentifier(),
                CreateAccount = true
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnMobileLoginSuccess, OnMobileLoginFailure);
#endif // UNITY_ANDROID
#if UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = GetUniqueIdentifier(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnMobileLoginSuccess, OnMobileLoginFailure);
#endif //UNITY_IOS
        }
    }

    #region Login
    private void OnMobileLoginSuccess(LoginResult obj)
    {
#if UNITY_EDITOR
        Debug.Log("Congratulations, you made your first successful API call!");
#endif //sUNITY_EDITOR

        GetStats();
        GetUserCurrency();
    }

    private void OnMobileLoginFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }


    private string GetUniqueIdentifier()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    /// <summary>
    /// On Successful Login
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result)
    {
#if UNITY_EDITOR
        Debug.Log("Congratulations, you made your first successful API call!"); 
#endif //UNITY_EDITOR

        //Remember the player
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        GetStats();
        GetUserCurrency();
    }


    /// <summary>
    /// On Unsuccessful Login
    /// </summary>
    /// <param name="error"></param>
    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = userEmail,
            Password = userPassword,
            Username = userName
        };

        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    /// <summary>
    /// On successful account registration
    /// </summary>
    /// <param name="result"></param>
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
#if UNITY_EDITOR
        Debug.Log("Congratulations, you made your first successful API call!");
#endif //UNITY_EDITOR

        //Remember the player
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        GetStats();
        GetUserCurrency();
    }


    /// <summary>
    /// On successful add account registration
    /// </summary>
    /// <param name="result"></param>
    private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
#if UNITY_EDITOR
        Debug.Log("Congratulations, you made your first successful API call!");
#endif //UNITY_EDITOR

        //Remember the player
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        GetStats();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
#if UNITY_EDITOR
        Debug.LogError(error.GenerateErrorReport());
#endif //UNITY_EDITOR
    }

    /// <summary>
    /// Get the current user's email
    /// </summary>
    /// <param name="emailIn"></param>
    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    /// <summary>
    /// Get the current user's password
    /// </summary>
    /// <param name="passwordIn"></param>
    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    /// <summary>
    /// On authentication sumbission
    /// </summary>
    public void OnSubmit()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = userEmail,
            Password = userPassword
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    /// <summary>
    /// Get the current user's username
    /// </summary>
    /// <param name="userNameIn"></param>
    public void GetUserName(string userNameIn)
    {
        userName = userNameIn;
    }

    public void OpenAddLogin()
    {
        addLoginPanel.SetActive(true);
    }

    public void OnSubmitAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest
        {
            Email = userEmail,
            Password = userPassword,
            Username = userName
        };

        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFailure);

    }
    #endregion

    #region PlayerStatistics
    public static void SetCloudStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats",
            FunctionParameter = new { HighScore = GameManager.CurrentHighScore, Gems = GameManager.CurrentCurrency },
            GeneratePlayStreamEvent = true
        }, cloudResult =>
        {
#if UNITY_EDITOR
            Debug.Log("Statistics successfully updated!");
#endif //UNITY_EDITOR
        }, cloudError =>
        {
#if UNITY_EDITOR
            Debug.LogError(cloudError.GenerateErrorReport());
#endif //UNITY_EDITOR
        });
    }

    public static void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    static void OnGetStats(GetPlayerStatisticsResult result)
    {
#if UNITY_EDITOR
        Debug.Log("Received the following Statistics:");
#endif //UNITY_EDITOR
        foreach (var eachStat in result.Statistics)
        {
#if UNITY_EDITOR
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
#endif //UNITY_EDITOR
            switch (eachStat.StatisticName)
            {
                case "HighScore":
                    ScoreSystem.UpdateHighScore(eachStat.Value);
                    break;
            }
        }
    }
    #endregion

    #region User Currency

    public static void DepositUserCurrency(int amount)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "DepositGemCurrency",
            FunctionParameter = new { Amount = amount },
            GeneratePlayStreamEvent = true
        },
        cloudResult =>
        {
            CurrencySystem.SubmitToManager();
#if UNITY_EDITOR
            Debug.Log("Successfully Deposited Player Currency!!!");
#endif //UNITY_EDITOR
        }, cloudError =>
        {
#if UNITY_EDITOR
            Debug.LogError(cloudError.GenerateErrorReport());
#endif //UNITY_EDITOR
        });
    }

    public static void WithdrawUserCurrency(int amount)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "WithdrawGemCurrency",
            FunctionParameter = new { Amount = amount },
            GeneratePlayStreamEvent = true
        },
        cloudResult =>
        {
            CurrencySystem.SubmitToManager();
#if UNITY_EDITOR
            Debug.Log("Successfully Withdrew Player Currency!!!");
#endif //UNITY EDITOR
        }, cloudError =>
        {
#if UNITY_EDITOR
            Debug.LogError(cloudError.GenerateErrorReport());
#endif //UNITY_EDITOR
        });
    }

    public static void GetUserCurrency()
    {
        isGetCurrencyRequestDone = false;

        var request = new GetUserInventoryRequest();
        
        PlayFabClientAPI.GetUserInventory(request, OnCurrencySuccessful, OnCurrencyFailure);
    }

    private static void OnCurrencySuccessful(GetUserInventoryResult result)
    {
        
        if (result.VirtualCurrency.TryGetValue("GM", out int amount))
        {
            CurrencySystem.UpdateBalance(amount);
            CurrencySystem.SubmitToManager();
        }
        isGetCurrencyRequestDone = true;
    }

    private static void OnCurrencyFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion //User Currency
}