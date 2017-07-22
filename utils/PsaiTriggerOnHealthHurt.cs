using UnityEngine;
using System.Collections;
using psai.net;
/*

public class PsaiTriggerOnHealthHurt: PsaiSynchronizedTrigger
{
    public KeyCode triggerKeyCode = KeyCode.Mouse0;
    public float minimumIntensity = 0.1f;
    public float maximumIntensity = 1.0f;
    public float intensityGainPerTick = 0.1f;

    public void OnStart()
    {
        Peripheral.onHealthChanged += onHealthChanged;
        Peripheral.onWaveStart += onWaveStart;
    }

    public PsaiTriggerOnHealthHurt()
    {
        this.triggerEvaluationNeedsUpdateMethod = true;
    }

    public override bool EvaluateTriggerCondition()
    {
        return Input.GetKey(this.triggerKeyCode);
    }

    void onWaveStart(int i)
    {
        onHealthChanged(i);

    }
    public void onHealthChanged(float i)
    {
        if (!fireContinuously)
        {
            if (Input.GetKeyDown(triggerKeyCode))
            {
                TryToFireSynchronizedShotTrigger();
            }
        }
        else
        {
            base.Update();
        }
    }


	public override float CalculateTriggerIntensity() 
    {
        if (PsaiCore.IsInstanceInitialized())
        {
            PsaiInfo psaiInfo = PsaiCore.Instance.GetPsaiInfo();
            float newIntensity = Mathf.Min(psaiInfo.currentIntensity + intensityGainPerTick, maximumIntensity);
            if (newIntensity < minimumIntensity)
            {
                newIntensity = minimumIntensity;
            }

            Debug.Log("CalculateTriggerInstensity() returns " + newIntensity);

            return newIntensity;
        }

        return 0;
	}

}
*/