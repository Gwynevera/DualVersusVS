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
    KeyCode grabP1keyboard = KeyCode.J;
    KeyCode parryP1keyboard = KeyCode.K;
    KeyCode teleportP1keyboard = KeyCode.Q;

    // Player 2 - Keyboard
    KeyCode leftP2keyboard = KeyCode.LeftArrow;
    KeyCode rightP2keyboard = KeyCode.RightArrow;
    KeyCode upP2keyboard = KeyCode.UpArrow;
    KeyCode downP2keyboard = KeyCode.DownArrow;
    KeyCode jumpP2keyboard = KeyCode.Keypad5;
    KeyCode dashP2keyboard = KeyCode.RightControl;
    KeyCode grabP2keyboard = KeyCode.Keypad4;
    KeyCode parryP2keyboard = KeyCode.Keypad6;
    KeyCode teleportP2keyboard = KeyCode.Keypad7;

    // Player 1 - Gamepad
    KeyCode jumpP1gamepad = KeyCode.Joystick1Button0;   // X (Ekis)
    KeyCode dashP1gamepad = KeyCode.Joystick1Button1;   // Circulo
    KeyCode grabP1gamepad = KeyCode.Joystick1Button3;   // Triangulo
    KeyCode parryP1gamepad = KeyCode.Joystick1Button2;  // Cuadrado
    KeyCode teleportP1gamepad = KeyCode.Joystick1Button4; // L1

    // Player 2 - Gamepad
    KeyCode jumpP2gamepad = KeyCode.Joystick2Button0;
    KeyCode dashP2gamepad = KeyCode.Joystick2Button1;
    KeyCode grabP2gamepad = KeyCode.Joystick2Button3;
    KeyCode parryP2gamepad = KeyCode.Joystick2Button2;
    KeyCode teleportP2gamepad = KeyCode.Joystick2Button4;

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

    public void SetPlayerKeys(ref Player.MyPlayerKeys _p)
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
                _p.grabKey = grabP1keyboard;
                _p.parryKey = parryP1keyboard;
                _p.teleportKey = teleportP1keyboard;
                break;
            case PlayerKeysType.Player2_Keyboard:
                _p.rightKey = rightP2keyboard;
                _p.leftKey = leftP2keyboard;
                _p.upKey = upP2keyboard;
                _p.downKey = downP2keyboard;

                _p.jumpKey = jumpP2keyboard;
                _p.dashkey = dashP2keyboard;
                _p.grabKey = grabP2keyboard;
                _p.parryKey = parryP2keyboard;
                _p.teleportKey = teleportP2keyboard;
                break;
            case PlayerKeysType.Player1_Gamepad:
                _p.jumpKey = jumpP1gamepad;
                _p.dashkey = dashP1gamepad;
                _p.grabKey = grabP1gamepad;
                _p.parryKey = parryP1gamepad;
                _p.teleportKey = teleportP1gamepad;
                break;
            case PlayerKeysType.Player2_Gamepad:
                _p.jumpKey = jumpP2gamepad;
                _p.dashkey = dashP2gamepad;
                _p.grabKey = grabP2gamepad;
                _p.parryKey = parryP2gamepad;
                _p.teleportKey = teleportP1gamepad;
                break;
        }
    }
}
