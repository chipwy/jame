using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeClass : MonoBehaviour
{
    public float sliceResetCooldown;
    public float sliceCooldown;

    public float dashDistance;
    public float dashDuration;

    public byte maxSlices;

    public bool isBusy;

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        sliceCooldown = sliceResetCooldown;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        UpdateCooldowns(Time.deltaTime);
    }

    private void UpdateCooldowns(float time)
    {
        sliceCooldown = Mathf.Max(0, sliceCooldown - time);
    }

    private void HandleInput()
    {
        if (isBusy) return;

        // If Q is pressed while there is no cooldown on the attack, call SliceAttack
        if (Input.GetKeyDown(KeyCode.Q) && sliceCooldown == 0)
            StartCoroutine(SliceAttack());
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        isBusy = true;
        Vector2 dest = (Vector2)transform.position +
                        GetComponent<PlayerMovement>().GetVelocity().normalized * dashDistance;
        yield return DashMove(dest, dashDuration);
        isBusy = false;
    }

    private IEnumerator SliceAttack()
    {
        isBusy = true;
        // Starting line should be the current player position
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        Vector2[] path = new Vector2[maxSlices];
        for (int i = 0; i < maxSlices; ++i)
        {
            yield return WaitForMouseClick();
            // Returns the point that the mouse is on
            path[i] = Camera.main.ScreenToWorldPoint(Input.mousePosition);            
            lineRenderer.positionCount = i+2;
            lineRenderer.SetPosition(i+1, path[i]);
        }

        for (byte i = 0; i < maxSlices; ++i)
        {
            yield return DashMove(path[i], 0.3f);
            // Removing the line that was just dashed through
            --lineRenderer.positionCount;
            for (byte j = 0; j < lineRenderer.positionCount; ++j)
                lineRenderer.SetPosition(j, path[j + i]);
            yield return new WaitForSeconds(0.1f);
        }
        sliceCooldown = sliceResetCooldown;
        isBusy = false;
    }

    private IEnumerator WaitForMouseClick()
    {
        // Waits until left mouse button is clicked to continue returning
        do { yield return null; } while (!Input.GetMouseButtonUp(0));
            // Waits for condition to be met,
            // but only keeps running this loop after it
            // gives a single frame to the coroutine manager

            // Make sure to do this one last time or else you'll
            // have inputs being register multiple times
    }

    private IEnumerator DashMove(Vector2 dest, float duration)
    {
        Vector2 start = transform.position;
        float time = 0;
        
        // Quickly goes to the point in the path that was inputted
        while (time < duration)
        {
            Debug.Log(time / duration);
            // Creates a Smooth Step Lerp movement
            // Source: https://en.wikipedia.org/wiki/Smoothstep
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector2.Lerp(start, dest, t);

            // If using the line renderer (only in SliceAttack)
            lineRenderer.SetPosition(0, transform.position);

            time += Time.deltaTime;
            yield return null; // Gives control back to the coroutine manager
        }

        transform.position = dest;
        yield return null;
    }
}