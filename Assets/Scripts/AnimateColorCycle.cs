using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateColorCycle : MonoBehaviour
{
    [SerializeField] private float scaleSpeed = 10.0f;
    [SerializeField] private float scaleAmount = 0.05f;
    [SerializeField] private bool scaleUp = false;
    [SerializeField] private float scaleFrom = 1.5f;
    [SerializeField] private float scaleTo = 1.8f;

    void Update() => ScaleUpAndDown();

    private void ScaleUpAndDown()
    {
        if (transform.localScale.x <= scaleFrom)
            scaleUp = true;
        if (transform.localScale.x >= scaleTo)
            scaleUp = false;
        
        if (scaleUp)
            transform.localScale += new Vector3(scaleAmount, scaleAmount, scaleAmount) * Time.deltaTime * scaleSpeed;
        else
            transform.localScale -= new Vector3(scaleAmount, scaleAmount, scaleAmount) * Time.deltaTime * scaleSpeed;
    }
}
