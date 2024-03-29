using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;

public class ActionsExpression : MonoBehaviour
{
    [SerializeField]
    PlayerInput action;
    [SerializeField]
    GameObject[] actExpressions,returnKeyIcons;
    [SerializeField]
    TextMeshProUGUI returnText;
    void Start()
    {
        switch(SceneManager.GetActiveScene().buildIndex) 
        {
            case 0:
                break;
            case 1:
            case 2:
                action=GameObject.Find("Player").GetComponent<PlayerInput>();
                break;
        }
    }

    void Update()
    {

        //デバイスの取得
        var DeviceNames=Input.GetJoystickNames();
        if(DeviceNames.Length ==0 ) //キーボードだけだったら
        {
            if(action.actions.FindActionMap("Player").enabled)
            {
                
                switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                {
                    case true:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(true);
                        actExpressions[2].SetActive(false);
                        actExpressions[3].SetActive(false);
                        break;
                    case false:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(true);
                        break;
                }
            }
            else //メニュー画面の場合
            {
                
                switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                {
                    case true:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(false);
                        actExpressions[2].SetActive(false);
                        actExpressions[3].SetActive(true);
                        break;
                    case false:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(true);
                        break;
                }
            }
            if (returnText != null)
            {
                returnText.text = "\r\n\r\nRキーで戻ります";
                returnKeyIcons[0].SetActive(false);
                returnKeyIcons[1].SetActive(true);

            }
            return;
        }else
        if(DeviceNames != null && DeviceNames[0]!="" )//コントローラーが接続されてるかどうか 
        {
            var LastDevice = action.devices[action.devices.Count - 1];
            if (LastDevice is Keyboard||LastDevice is Mouse) //キーボード、マウスなら
            {
                if (action.actions.FindActionMap("Player").enabled)
                {
                    switch(SceneManager.GetActiveScene().buildIndex!=0)//タイトルシーン以外かどうか 
                    {
                        case true:
                            actExpressions[0].SetActive(false);
                            actExpressions[1].SetActive(true);
                            actExpressions[2].SetActive(false);
                            actExpressions[3].SetActive(false);
                            break;
                        case false:
                            actExpressions[0].SetActive(false);
                            actExpressions[1].SetActive(true);
                            break; 
                    }
                }
                else //メニュー画面の場合
                {
                    switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                    {
                        case true:
                            actExpressions[0].SetActive(false);
                            actExpressions[1].SetActive(false);
                            actExpressions[2].SetActive(false);
                            actExpressions[3].SetActive(true);
                            break;
                        case false:
                            actExpressions[0].SetActive(false);
                            actExpressions[1].SetActive(true);
                            break;
                    }
                }
                if (returnText!=null)
                {
                    returnText.text = "\r\n\r\nRキーで戻ります";
                    returnKeyIcons[0].SetActive(false) ;
                    returnKeyIcons[1].SetActive(true);

                }
            }
            else if (LastDevice is Gamepad)//XBoxコントローラーなら
            {
                if (action.actions.FindActionMap("Player").enabled)
                {
                    switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                    {
                        case true:
                            actExpressions[0].SetActive(true);
                            actExpressions[1].SetActive(false);
                            actExpressions[2].SetActive(false);
                            actExpressions[3].SetActive(false);
                            break;
                        case false:
                            actExpressions[0].SetActive(true);
                            actExpressions[1].SetActive(false);
                            break;
                    }
                }
                else //メニュー画面の場合
                {
                    switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                    {
                        case true:
                            actExpressions[0].SetActive(false);
                            actExpressions[1].SetActive(false);
                            actExpressions[2].SetActive(true);
                            actExpressions[3].SetActive(false);
                            break;
                        case false:
                            actExpressions[0].SetActive(true);
                            actExpressions[1].SetActive(false);
                            break;
                    }
                }
                if (returnText != null)
                {
                    returnText.text = "\r\n\r\nXボタンで戻ります";
                    returnKeyIcons[0].SetActive(true);
                    returnKeyIcons[1].SetActive(false);
                }
            }
        }else
        {
            if (action.actions.FindActionMap("Player").enabled)
            {
                switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                {
                    case true:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(true);
                        actExpressions[2].SetActive(false);
                        actExpressions[3].SetActive(false);
                        break;
                    case false:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(true);
                        break;
                }
            }
            else //メニュー画面の場合
            {
                switch (SceneManager.GetActiveScene().buildIndex != 0)//タイトルシーン以外かどうか 
                {
                    case true:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(false);
                        actExpressions[2].SetActive(false);
                        actExpressions[3].SetActive(true);
                        break;
                    case false:
                        actExpressions[0].SetActive(false);
                        actExpressions[1].SetActive(true);
                        break;
                }
            }
            if (returnText != null)
            {
                returnText.text = "\r\n\r\nRキーで戻ります";
                returnKeyIcons[0].SetActive(false);
                returnKeyIcons[1].SetActive(true);
            }
        }
        
    }
}
