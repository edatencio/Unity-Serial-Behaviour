using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInput : MonoBehaviour, ISerialReceiver
{
    /*************************************************************************************************
    *** Classes
    *************************************************************************************************/
    private class SerialInput
    {
        public class Buttons
        {
            public const string Left = "@Left";
            public const string Right = "@Right";
            public const string Up = "@Up";
            public const string Down = "@Down";
            public const string Credit = "@Credit";
        }

        public class Tickets
        {
            public const string Remove = "#print";
            public const string Reset = "#reset";
            public const string Error = "#error";
        }
    }

    private class Button
    {
        public string name = "";
        public bool isPressed;
    }

    /*************************************************************************************************
    *** Variables
    *************************************************************************************************/
    [SerializeField] private SerialAsset serialAsset;
    private new static string name;
    private static CustomInput instance;
    private Coroutine resetInput;
    private List<string> buffer;

    private static Button[] buttons = new Button[]
    {
        new Button() { name = SerialInput.Buttons.Left }
        , new Button() { name = SerialInput.Buttons.Right }
        , new Button() { name = SerialInput.Buttons.Up }
        , new Button() { name = SerialInput.Buttons.Down }

        , new Button() { name = SerialInput.Buttons.Credit }
        , new Button() { name = SerialInput.Tickets.Remove }
        , new Button() { name = SerialInput.Tickets.Reset }
        , new Button() { name = SerialInput.Tickets.Error }
    };

    /*************************************************************************************************
    *** Start
    *************************************************************************************************/
    private void Start()
    {
        name = base.name;

        if (instance != null && instance != this)
        {
            Log.Warning(name, "there is other instance running");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        serialAsset.RegisterReceiver(this);
        resetInput = StartCoroutine(HandleInput());
        buffer = new List<string>();
    }

    /*************************************************************************************************
    *** OnDestroy
    *************************************************************************************************/
    private void OnDestroy()
    {
        StopCoroutine(resetInput);
    }

    /*************************************************************************************************
    *** Properties
    *************************************************************************************************/
    public static bool AnyPressed
    {
        get { return Left || Right || Up || Down || Credit; }
    }

    public static bool Left
    {
        get { return Input.GetAxis("Horizontal") < 0f || GetButtonPressed(SerialInput.Buttons.Left); }
    }

    public static bool Right
    {
        get { return Input.GetAxis("Horizontal") > 0f || GetButtonPressed(SerialInput.Buttons.Right); }
    }

    public static bool Up
    {
        get { return Input.GetAxis("Vertical") > 0f || GetButtonPressed(SerialInput.Buttons.Up); }
    }

    public static bool Down
    {
        get { return Input.GetAxis("Vertical") < 0f || GetButtonPressed(SerialInput.Buttons.Down); }
    }

    public static bool Credit
    {
        get { return /*Input.GetButtonDown(Constants.Input.Buttons.Credit) ||*/ GetButtonPressed(SerialInput.Buttons.Credit); }
    }

    public static bool TicketsRemove
    {
        get { return /*Input.GetButtonDown(Constants.Input.Tickets.Remove) ||*/ GetButtonPressed(SerialInput.Tickets.Remove); }
    }

    public static bool TicketsReset
    {
        get { return /*Input.GetButtonDown(Constants.Input.Tickets.Reset) ||*/ GetButtonPressed(SerialInput.Tickets.Reset); }
    }

    public static bool TicketsError
    {
        get { return /*Input.GetButtonDown(Constants.Input.Tickets.Error) ||*/ GetButtonPressed(SerialInput.Tickets.Error); }
    }

    /*************************************************************************************************
    *** Methods
    *************************************************************************************************/
    private static bool GetButtonPressed(string buttonName)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == buttonName)
                return buttons[i].isPressed;
        }

        Log.Error(name, '\'', buttonName, "' doesn't exists");

        return false;
    }

    public void OnSerialMessageReceived(string msg)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == msg)
                buffer.Add(msg);
        }
    }

    private IEnumerator HandleInput()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (buffer.Count > 0)
            {
                string[] buffer = this.buffer.ToArray();
                this.buffer.Clear();

                for (int j = 0; j < buttons.Length; j++)
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if (buttons[j].name == buffer[i])
                            buttons[j].isPressed = true;
                    }
                }
            }

            yield return new WaitForEndOfFrame();

            for (int i = 0; i < buttons.Length; i++)
                buttons[i].isPressed = false;
        }
    }
}
