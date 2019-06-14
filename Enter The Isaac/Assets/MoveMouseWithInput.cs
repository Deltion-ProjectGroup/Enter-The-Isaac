using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MoveMouseWithInput : MonoBehaviour {

    [DllImport ("user32.dll")]
    public static extern bool SetCursorPos (int X, int Y);
    [DllImport ("user32.dll")]
    [
        return :MarshalAs (UnmanagedType.Bool)
    ]
    public static extern bool GetCursorPos (out Point pos);

    [SerializeField] string moveMouseHor = "PauseHor";
    [SerializeField] string moveMouseVert = "PauseVert";
    [SerializeField] float speed = 15;
    void Update () {
        Point p;
        GetCursorPos (out p);
        SetCursorPos (p.X + (int) (Input.GetAxis (moveMouseHor) * speed), p.Y + (int) (Input.GetAxis (moveMouseVert) * speed));
    }

    public void SetMouseToCenter(){
        SetCursorPos(Screen.width / 2,Screen.height / 2);
    }
}

[StructLayout (LayoutKind.Sequential)]
public struct Point {
    public int X;
    public int Y;
    public static implicit operator Vector2 (Point p) {
        return new Vector2 (p.X, p.Y);
    }
}