using System;
using UnityEngine;

public class LavaGameManager : MonoBehaviour
{
    [Header("Game settings")]
    [SerializeField] private int touchLimit;
    [SerializeField] private int touchUpwardsFroce;

    [Header("Regular state")]
    [SerializeField] private int[] growthSteps = { 1, 1, 2, 2, 3, 3 };
    [SerializeField] private int[] intervalsBetweenSteps = { 2, 2, 3, 3, 4, 4 };

    [Space]
    [Header("Sudden death")]
    [SerializeField] private float suddenDeathGrowthPerSecond;
    [SerializeField] private float secondsToSuddenDeath;

    [Space]
    [Header("References")]
    [SerializeField] private RisingLava lavaPool;
    [SerializeField] private GameObject player;

    [Space]
    [Header("Heretic debug")]
    [SerializeField] private string timeString;
    [SerializeField] private float seconds;
    [SerializeField] private float miliseconds;
    
    private PlayerMovement pm;
    private CameraTargetFollower ctf;

    private int touchesSoFar = 0;
    private int sign = 1;
    private int currentStep = 0;

    private Clock cock;

    private void Awake()
    {
        if(player)
        {
            pm = player.GetComponent<PlayerMovement>();
        }

        ctf = FindObjectOfType<CameraTargetFollower>();
        cock = new();
        cock.ForwardClockBy(121);
    }

    void Start()
    {
        lavaPool.gameObject.SetActive(true);
        lavaPool.Active = false;

        CycleThroughLavaPoolState();

        lavaPool.OnLavaTrigger.AddListener(OnObjectLavaTrigger);

        Invoke(nameof(SuddenDeath), secondsToSuddenDeath);
    }

    void Update()
    {
        seconds = cock.GetElapsedTimeInSeconds();
        miliseconds = cock.GetElapsedTimeInMiliseconds();
        timeString = Clock.FormatToMinSec(cock);
    }

    void OnObjectLavaTrigger(GameObject collidedObject)
    {
        if (collidedObject != player || pm.Velocity.y > 0.0f)
        {
            return;
        }

        if(touchesSoFar >= touchLimit)
        {
            lavaPool.Active = false;
            CancelInvoke();

            if(ctf)
            {
                ctf.enabled = false;
            }

            lavaPool.OnLavaTrigger.RemoveListener(OnObjectLavaTrigger);
            cock.Restart();

            return;
        }

        pm.JumpWithHeight(touchUpwardsFroce);
        touchesSoFar++;
    }

    void SuddenDeath()
    {
        CancelInvoke();

        lavaPool.UpwardsGrowthPerSecond = suddenDeathGrowthPerSecond;
        lavaPool.Active = true;
    }

    void CycleThroughLavaPoolState()
    {
        lavaPool.UpwardsGrowthPerSecond = growthSteps[currentStep] * sign;
        lavaPool.Active = !lavaPool.Active;
        Invoke(nameof(CycleThroughLavaPoolState), intervalsBetweenSteps[currentStep]);

        if(lavaPool.Active)
        {
            cock.Start();
        }
        else
        {
            cock.Stop();
        }

        currentStep += sign;

        if (currentStep == -1 || currentStep == growthSteps.Length)
        {
            sign *= -1;
            currentStep += sign;
        }
    }
}
