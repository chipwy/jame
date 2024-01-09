using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    void FixedUpdate()
    {
        HandleMove();
    }

    private void HandleMove()
    {
        if (GetComponent<MeleeClass>().isBusy) return;
        Vector2 velocity = GetVelocity();
        transform.Translate(velocity.x, velocity.y, 0);
    }

    public Vector2 GetVelocity()
    {
        float v = Input.GetAxis("Vertical") * Time.deltaTime;
        float h = Input.GetAxis("Horizontal") * Time.deltaTime;
        return new Vector2(h, v);
    }
}
