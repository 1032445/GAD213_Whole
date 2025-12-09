using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedMechanism : MonoBehaviour
{
    public bool isActive = false;

    private float timer = 0f;

    public void Activate(float duration)
    {
        isActive = true;
        timer = duration;
        OnActivated();
    }

    void Update()
    {
        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isActive = false;
                OnDeactivated();
            }
        }
    }

    protected virtual void OnActivated()
    {
        
    }

    protected virtual void OnDeactivated()
    {
        
    }
}
