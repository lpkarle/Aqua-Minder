using UnityEngine;

public class Timer
{
    private float duration;
    private float elapsedTime;
    private bool isRunning;
    private bool isFinished;

    public float Duration => duration;
    public float ElapsedTime => elapsedTime;
    public bool IsRunning => isRunning;
    public bool IsFinished => isFinished;

    public Timer(float duration)
    {
        this.duration = duration;
        elapsedTime = 0f;
        isRunning = false;
        isFinished = false;
    }

    public void Start()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= duration)
            {
                Stop();
                isFinished = true;
            }
        }
    }

    public void Stop()
    {
        elapsedTime = duration;
        isRunning = false;
    }

    public void Reset()
    {
        elapsedTime = 0f;
        isRunning = false;
        isFinished = false;
    }
}