using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{
    public UnityEvent CharacterClicked;

    [SerializeField] private Animator animator;

    [SerializeField] private Outline ot;

    private NavMeshAgent agent;

    public bool Moving => Animator.GetBool("IsWalking");

    public bool Idle => Animator.GetBool("IsIdle");

    public Animator Animator => animator;

    private Coroutine activeCoroutine;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void StopWalking() {
        if(activeCoroutine != null) {
            StopCoroutine(activeCoroutine);
        }
        agent.SetDestination(transform.position);
        animator.SetBool("IsWalking", false);
        animator.Play("MaleIdle");
    }

    public Coroutine SetNewDestination(Vector3 destination, Vector3? standing_position = null)
    {
        StopWalking();
        return activeCoroutine = StartCoroutine(HandleMovement(destination, standing_position));
    }

    private IEnumerator HandleMovement(Vector3 destination, Vector3? standing_position = null)
    {
        if(agent.SetDestination(standing_position ?? destination)) {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);

            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.15f);

            animator.SetBool("IsWalking", false);

            yield return new WaitUntil(() => animator.GetBool("IsIdle"));

        }
        StartCoroutine(FaceTarget(destination));
    }

    public IEnumerator FaceTarget(Vector3 destination)
    {
        bool isFacingTarget = false;
        while (!isFacingTarget)
        {
            Vector3 lookPos = destination - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);

            float angleDifference = Quaternion.Angle(transform.rotation, rotation);
            if(Mathf.Abs(angleDifference) < 1.0f) {
                isFacingTarget = true;
                continue;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
            yield return null; // Wait for the next frame before checking again
        }
    }
    public Coroutine PlayEmote(string name) {
        return StartCoroutine(HandleEmote(name));
    }

    private IEnumerator HandleEmote(string name) {
        Animator.Play(name);
        Animator.SetBool("IsIdle", false);

        while(!Idle) {
            yield return new WaitForSeconds(.5f);
        }
    }

    private void OnMouseDown() {
        if(Singleton<PlayerController>.Instance.CanInteractWithTransform(transform)) {
            CharacterClicked?.Invoke();
        }
    }
}
