using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeys
{
    // Player 1 - Keyboard
    KeyCode leftP1keyboard = KeyCode.A;
    KeyCode rightP1keyboard = KeyCode.D;
    KeyCode upP1keyboard = KeyCode.W;
    KeyCode downP1keyboard = KeyCode.S;
    KeyCode jumpP1keyboard = KeyCode.Space; // J
    KeyCode dashP1keyboard = KeyCode.LeftShift;
    KeyCode attackP1keyboard = KeyCode.H;

    // Player 2 - Keyboard
    KeyCode leftP2keyboard = KeyCode.LeftArrow;
    KeyCode rightP2keyboard = KeyCode.RightArrow;
    KeyCode upP2keyboard = KeyCode.UpArrow;
    KeyCode downP2keyboard = KeyCode.DownArrow;
    KeyCode jumpP2keyboard = KeyCode.Keypad5;
    KeyCode dashP2keyboard = KeyCode.RightControl;
    KeyCode attackP2keyboard = KeyCode.Keypad4;

    // Player 1 - Gamepad
    KeyCode jumpP1gamepad = KeyCode.Joystick1Button0;   // X (Ekis)
    KeyCode dashP1gamepad = KeyCode.Joystick1Button4;   // L1
    KeyCode attackP1gamepad = KeyCode.Joystick1Button2; // Cuadrao

    // Player 2 - Gamepad
    KeyCode jumpP2gamepad = KeyCode.Joystick2Button0;
    KeyCode dashP2gamepad = KeyCode.Joystick2Button4;
    KeyCode attackP2gamepad = KeyCode.Joystick2Button2;

    // Axis names
    public string g1PadX = "DPad1_X";
    public string g1PadY = "DPad1_Y";
    public string g2PadX = "DPad2_X";
    public string g2PadY = "DPad2_Y";

    public string g1StickX = "Joystick1_X";
    public string g1StickY = "Joystick1_Y";
    public string g2StickX = "Joystick2_X";
    public string g2StickY = "Joystick2_Y";

    public float minStickValue = 0.5f;

    #region Controles de Play
    /*
    X        = Joystick Button 0
    Circle   = Joystick Button 1
    Square   = Joystick Button 2
    Triangle = Joystick Button 3
    L1       = Joystick Button 4
    R1       = Joystick Button 5
    L2       = Joystick Button 6
    R2       = Joystick Button 7
    Share    = Joystick Button 8
    Options  = Joystick Button 9
    L3       = Joystick Button 10
    R3       = Joystick Button 11
    PS       = Joystick Button 12
    
    JoyStick Left Right is the "X-axis". JoyStick Up Down is the "Y-axis"
    DPad Left Right is the "7th-axis". DPad Up Down is the "8th-axis"
    */
    #endregion

    public enum PlayerKeysType
    {
        Player1_Keyboard,
        Player2_Keyboard,
        Player1_Gamepad,
        Player2_Gamepad,

        None
    }

    public void SetPlayerKeys(ref Player2.MyPlayerKeys _p)
    {
        switch (_p.playerKeys)
        {
            case PlayerKeysType.Player1_Keyboard:
                _p.rightKey = rightP1keyboard;
                _p.leftKey = leftP1keyboard;
                _p.upKey = upP1keyboard;
                _p.downKey = downP1keyboard;

                _p.jumpKey = jumpP1keyboard;
                _p.dashkey = dashP1keyboard;
                _p.attackKey = attackP1keyboard;
                break;
            case PlayerKeysType.Player2_Keyboard:
                _p.rightKey = rightP2keyboard;
                _p.leftKey = leftP2keyboard;
                _p.upKey = upP2keyboard;
                _p.downKey = downP2keyboard;

                _p.jumpKey = jumpP2keyboard;
                _p.dashkey = dashP2keyboard;
                _p.attackKey = attackP2keyboard;
                break;
            case PlayerKeysType.Player1_Gamepad:
                // Axis

                _p.jumpKey = jumpP1gamepad;
                _p.dashkey = dashP1gamepad;
                _p.attackKey = attackP1gamepad;
                break;
            case PlayerKeysType.Player2_Gamepad:
                // Axis

                _p.jumpKey = jumpP2gamepad;
                _p.dashkey = dashP2gamepad;
                _p.attackKey = attackP2gamepad;
                break;
        }
    }
}
