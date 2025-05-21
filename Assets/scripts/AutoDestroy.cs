using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private int remainingFrames;

    public void Init(int frames)
    {
        remainingFrames = frames;
    }

    void Update()
    {
        remainingFrames--;
        if (remainingFrames <= 0)
        {
            Destroy(gameObject);
        }
    }
}
