using UnityEngine;
using System.Collections;

public class PointToPointWalker : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("Idle")]
    public float idleTime = 2f;

    [Header("Animation")]
    public Animator animator;

    [Header("Dance System")]
    public int danceCount = 5;
    public Transform danceFacingTarget;

    [Header("Debug")]
    public bool debugLogs = true;

    private Transform currentTarget;
    private Transform previousPoint;

    private bool isMoving = true;
    private bool isDancing = false;
    private bool isSad = false;

    private Coroutine currentActionRoutine;

    // =======================
    // DEBUG HELPER
    // =======================
    void Log(string msg)
    {
        if (debugLogs)
            Debug.Log("[Walker] " + msg);
    }

    void Start()
    {
        transform.position = pointA.position;
        currentTarget = pointB;
        previousPoint = pointA;

        StartWalking();
    }

    void Update()
    {
        if (isDancing || isSad) return;
        if (!isMoving) return;

        MoveToTarget();
    }

    void MoveToTarget()
    {
        Vector3 direction = (currentTarget.position - transform.position);
        float distance = direction.magnitude;

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        if (distance <= 0.05f)
        {
            StartCoroutine(HandleArrival());
        }
    }

    IEnumerator HandleArrival()
    {
        isMoving = false;

        transform.position = currentTarget.position;
        StopWalking();

        Vector3 lookDir = (previousPoint.position - transform.position);
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);

            while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.rotation = targetRot;
        }

        yield return new WaitForSeconds(idleTime);

        if (isDancing || isSad) yield break;

        Transform temp = previousPoint;
        previousPoint = currentTarget;
        currentTarget = temp;

        StartWalking();
        isMoving = true;
    }

    // =======================
    // ACTION SYSTEM
    // =======================

    public void PlayDance()
    {
        Log("PlayDance() called");
        StartAction(DanceRoutine());
    }

    public void PlaySad()
    {
        Log("PlaySad() called");
        StartAction(SadRoutine());
    }

    void StartAction(IEnumerator routine)
    {
        Log("StartAction()");

        if (currentActionRoutine != null)
        {
            Log("Stopping current coroutine");
            StopCoroutine(currentActionRoutine);
        }

        isDancing = false;
        isSad = false;

        isMoving = false;
        StopWalking();

        currentActionRoutine = StartCoroutine(routine);
    }

    // =======================
    // DANCE
    // =======================

    IEnumerator DanceRoutine()
    {
        isDancing = true;
        Log("DanceRoutine START");

        yield return RotateToTarget();

        Log("After rotation");

        if (animator == null)
        {
            Log("ERROR: Animator is NULL");
            yield break;
        }

        animator.ResetTrigger("SadTrigger");
        animator.ResetTrigger("DanceTrigger");

        int randomDance = Random.Range(0, danceCount);
        Log("Setting DanceIndex = " + randomDance);

        animator.SetFloat("DanceIndex", randomDance);
        animator.SetTrigger("DanceTrigger");

        Log("DanceTrigger SET");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        float timeout = 2f;
        float timer = 0f;

        while (!stateInfo.IsTag("Dance") && timer < timeout)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            timer += Time.deltaTime;
            yield return null;
        }

        if (!stateInfo.IsTag("Dance"))
        {
            Log("ERROR: Never entered Dance state");
            yield break;
        }

        Log("Entered Dance state");

        while (stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        Log("Dance finished");

        isDancing = false;
        ResumeMovement();
    }

    // =======================
    // SAD
    // =======================

    IEnumerator SadRoutine()
    {
        isSad = true;
        Log("SadRoutine START");

        yield return RotateToTarget();

        animator.ResetTrigger("DanceTrigger");
        animator.ResetTrigger("SadTrigger");

        animator.SetTrigger("SadTrigger");
        Log("SadTrigger SET");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        float timeout = 2f;
        float timer = 0f;

        while (!stateInfo.IsTag("Sad") && timer < timeout)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            timer += Time.deltaTime;
            yield return null;
        }

        if (!stateInfo.IsTag("Sad"))
        {
            Log("ERROR: Never entered Sad state");
            yield break;
        }

        Log("Entered Sad state");

        while (stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        Log("Sad finished");

        isSad = false;
        ResumeMovement();
    }

    // =======================
    // ROTATION
    // =======================

    IEnumerator RotateToTarget()
    {
        if (danceFacingTarget == null)
        {
            Log("WARNING: danceFacingTarget is NULL");
            yield break;
        }

        Log("Rotating to target");

        Vector3 lookDir = (danceFacingTarget.position - transform.position);
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);

            while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.rotation = targetRot;
        }

        Log("Rotation complete");
    }

    // =======================
    // HELPERS
    // =======================

    void ResumeMovement()
    {
        if (!isDancing && !isSad)
        {
            Log("Resuming movement");
            StartWalking();
            isMoving = true;
        }
    }

    void StartWalking()
    {
        if (animator != null)
            animator.SetBool("isWalking", true);
    }

    void StopWalking()
    {
        if (animator != null)
            animator.SetBool("isWalking", false);
    }
}