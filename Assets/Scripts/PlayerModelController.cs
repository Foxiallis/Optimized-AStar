using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelController : MonoBehaviour
{
    public static PlayerModelController Instance;

    [SerializeField] private Vector3 tileOffset;

    [SerializeField] private float speed;
    [SerializeField] private float animationSpeed; //speed for Animator component
    [SerializeField] private float accelerationSeconds; //time to reach 100% speed

    private float currentSpeed;
    private float currentAnimationSpeed;
    private Animator animator;

    private static float DESTINATION_CUTOFF_POINT = 0.001f;

    public void OnFootstep() { } //empty function so that starter animator doesn't raise errors

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Instance = this;
    }

    public void ResetPosition(MapTile tile)
    {
        StopAllCoroutines();
        transform.position = tile == null ? MapManager.Instance.mapTiles[0].transform.position + tileOffset : tile.transform.position + tileOffset;
        currentSpeed = 0;
        currentAnimationSpeed = 0;
        animator.SetFloat("Speed", currentAnimationSpeed);
    }

    public void StartWalking(List<MapTile> path)
    {
        StartCoroutine(WalkToDestination(path));
    }

    IEnumerator WalkToDestination(List<MapTile> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0) StartCoroutine(ChangeSpeed(true));
            else if (i == path.Count - 1) StartCoroutine(ChangeSpeed(false));

            yield return WalkToWayPoint(path[i]);
        }
    }

    IEnumerator WalkToWayPoint(MapTile destinationTile)
    {
        Vector3 destination = destinationTile.transform.position;
        destination.y = transform.position.y;
        transform.LookAt(destination);

        while (Vector3.Distance(transform.position, destination) > DESTINATION_CUTOFF_POINT)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator ChangeSpeed(bool accelerate)
    {
        float targetSpeed = accelerate ? speed : 0;
        float targetAnimationSpeed = accelerate ? animationSpeed : 0;
        float startingSpeed = currentSpeed;
        float startingAnimationSpeed = currentAnimationSpeed;
        float capturedTime = 0;

        while (capturedTime < accelerationSeconds)
        {
            capturedTime += Time.deltaTime;
            currentSpeed = Mathf.Lerp(startingSpeed, targetSpeed, capturedTime / accelerationSeconds);
            currentAnimationSpeed = Mathf.Lerp(startingAnimationSpeed, targetAnimationSpeed, capturedTime / accelerationSeconds);
            animator.SetFloat("Speed", currentAnimationSpeed);
            yield return null;
        }
    }
}
