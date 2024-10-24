using UnityEngine;

public static class InputManager
{
    public static float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public static bool JumpPressed()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public static bool JumpReleased()
    {
        return Input.GetKeyUp(KeyCode.W);
    }

    public static bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
}
