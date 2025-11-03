using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    [Range(0f,1f)]public float xSensitivity = 0.3f;
    [Range(0f, 1f)] public float ySensitivity = 0.3f;


    public void ProcessLook(Vector2 input)
    {
            float mouseX = input.x;
            float mouseY = input.y;

            // calculate camera rotation for looking up and down
            xRotation -= (mouseY) * ySensitivity;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            // apply this to our camera transform
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            // rotate player to look left and right
            transform.Rotate(Vector3.up * (mouseX) * xSensitivity);
    }
}
