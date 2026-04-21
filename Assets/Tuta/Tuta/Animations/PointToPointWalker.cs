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

    private Transform currentTarget;
    private Transform previousPoint;

    private bool isMoving = true;
    private bool isDancing = false;
    private bool isSad = false;

    private Coroutine currentActionRoutine;

    void Start()
    {
        transform.position = pointA.position;
        currentTarget = pointB;
        previousPoint = pointA;

        StartWalking();
    }

    void Update()
    {
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

        Transform temp = previousPoint;
        previousPoint = currentTarget;
        currentTarget = temp;

        StartWalking();
        isMoving = true;
    }

    // =======================
    // ACTION SYSTEM (INTERRUPTIBLE)
    // =======================

    public void PlayDance()
    {
        StartAction(DanceRoutine());
    }

    public void PlaySad()
    {
        StartAction(SadRoutine());
    }

    void StartAction(IEnumerator routine)
    {
        if (currentActionRoutine != null)
        {
            StopCoroutine(currentActionRoutine);
        }

        isDancing = false;
        isSad = false;
        isMoving = false;

        currentActionRoutine = StartCoroutine(routine);
    }

    // =======================
    // DANCE
    // =======================

    IEnumerator DanceRoutine()
    {
        isDancing = true;
        isSad = false;

        StopWalking();

        yield return RotateToTarget();

        animator.ResetTrigger("SadTrigger");
        animator.ResetTrigger("DanceTrigger");

        int randomDance = Random.Range(0, danceCount);
        animator.SetFloat("DanceIndex", randomDance);
        animator.SetTrigger("DanceTrigger");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsTag("Dance"))
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        StartWalking();
        isMoving = true;
        isDancing = false;
    }

    // =======================
    // SAD
    // =======================

    IEnumerator SadRoutine()
    {
        isSad = true;
        isDancing = false;

        StopWalking();

        yield return RotateToTarget();

        animator.ResetTrigger("DanceTrigger");
        animator.ResetTrigger("SadTrigger");

        animator.SetTrigger("SadTrigger");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsTag("Sad"))
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        StartWalking();
        isMoving = true;
        isSad = false;
    }

    // =======================
    // SHARED ROTATION
    // =======================

    IEnumerator RotateToTarget()
    {
        if (danceFacingTarget == null)
            yield break;

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
    }

    // =======================
    // ANIMATION HELPERS
    // =======================

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