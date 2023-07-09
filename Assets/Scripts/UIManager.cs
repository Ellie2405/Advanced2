using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] GameObject LoginUI;
    [SerializeField] GameObject RegisterUI;
    [SerializeField] GameObject GameUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }

    public void ShowLoginScreen()
    {
        LoginUI.SetActive(true);
        RegisterUI.SetActive(false);
    }

    public void ShowRegisterScreen()
    {
        RegisterUI.SetActive(true);
        LoginUI.SetActive(false);
    }

    public void ShowGameUI()
    {
        GameUI.SetActive(true);
        LoginUI.SetActive(false);
    }
}
