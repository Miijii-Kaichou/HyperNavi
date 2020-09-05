using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
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

    //TODO: Make a seperate script for this
    #region Login
    private void OnMobileLoginSuccess(LoginResult obj)
    {
        Debug.Log("Congratulations, you made your first successful API call!");

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
        Debug.Log("Congratulations, you made your first successful API call!");

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
        Debug.Log("Congratulations, you made your first successful API call!");

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
        Debug.Log("Congratulations, you made your first successful API call!");

        //Remember the player
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        GetStats();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void OnSubmit()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = userEmail,
            Password = userPassword
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

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

    //TODO: Make this a seperate script
    #region PlayerStatistics
    public static void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            //Request.Statistics is a list, so multiple StatisticsUpate objects can be defined if required.
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate{StatisticName = "HighScore", Value = GameManager.CurrentHighScore},
                new StatisticUpdate{ StatisticName = "Gems", Value = GameManager.CurrentCurrency}
            }
        },
        result =>
        {
            Debug.Log("user statistics updated");
        },
        error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            }
        );
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
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
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
            Debug.Log("Successfully Deposited Player Currency!!!");
        }, cloudError =>
        {
            Debug.LogError(cloudError.GenerateErrorReport());
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
            Debug.Log("Successfully Withdrew Player Currency!!!");
        }, cloudError =>
        {
            Debug.LogError(cloudError.GenerateErrorReport());
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