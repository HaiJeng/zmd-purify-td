namespace GameCore.Core;

public class BacklashSystem
{
    private bool isBacklashActive = false;
    private float backlashTimer = 0f;

    public void CheckAndTriggerBacklash(float purificationProgress)
    {
        if (purificationProgress >= 0.5f && !isBacklashActive)
        {
            TriggerBacklash();
        }
    }

    void TriggerBacklash()
    {
        isBacklashActive = true;
        backlashTimer = 10f;
    }

    public void UpdateBacklash(float dt)
    {
        if (!isBacklashActive) return;

        backlashTimer -= dt;
        if (backlashTimer <= 0)
        {
            isBacklashActive = false;
        }
    }
}