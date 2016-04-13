using UnityEngine;

public class NeedleMan : MonoBehaviour
{
    private float canStartUngry;
    public float UngryTime = 4f;

    public Animator animator;

    public void Start()
    {
        canStartUngry = UngryTime;
    }

    public void Update()
    {
        if ((canStartUngry -= Time.deltaTime) > 0)
            return;

        animator.SetTrigger("Ungry");

        canStartUngry = UngryTime;
    }
}
