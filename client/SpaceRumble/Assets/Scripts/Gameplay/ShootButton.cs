using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootButton : MonoBehaviour
{
    public bool isPressed = false;
    private int shootCoolDown = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown() {
        isPressed = true;
        StartCoroutine(ShootingCooldown());
    }

    public void OnPointerUp()
    {
        isPressed = false;
    }

    IEnumerator ShootingCooldown()
    {
        yield return new WaitForSeconds(shootCoolDown);
        OnPointerUp();
    }
}
