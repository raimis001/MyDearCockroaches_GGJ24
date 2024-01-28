using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class Crouch : MonoBehaviour
{

    [SerializeField]
    NeedsKind kind;
    [SerializeField]
    Animator anim;

    [SerializeField]
    Light2D light2D;

    [SerializeField]
    AudioSource audioWalk;

    [SerializeField]
    AudioSource audioSleep;

    [SerializeField]
    AudioSource audioAuch;

    Vector2 destionation = Vector2.zero;
    float destinationAngle = 0;

    float speed = 1f;
    float time = 2;
    float currentTime = 0;
    float iddleTime = 0;


    private void Start()
    {
        destionation = Vector2.zero;
        transform.localPosition = destionation;
        //NewDestination();
    }

    void NewDestination()
    {

        destionation = Face.RandomPoint(kind);

        Vector2 dir = destionation - (Vector2)transform.localPosition;
        destinationAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    }


    private void Update()
    {
        if (Face.gameEnded)
            return;

        if (Face.Needs[kind].home)
            return;

        if (Face.Needs[kind].value <= 0)
        {
            destionation = Vector2.zero;
            SetLookAngle2D();
            MoveToDestination();

            if (ReachDestination())
            {
                Debug.Log("Crouch to home");

                Face.Needs[kind].home = true;

                transform.eulerAngles = Vector3.zero;
                light2D.intensity = 0f;

                audioWalk.Stop();
                audioSleep.Play();
            }

            return;
        }

        //Increase needs
        iddleTime += Time.deltaTime;
        if (iddleTime > 3)
        {
            Face.Needs[kind].value += 0.1f;
            if (Face.Needs[kind].value > 1)
                Face.Needs[kind].value = 1;

            iddleTime = 0;
        }

        if (currentTime > 0)
        {
            //TODO iddle
            anim.SetFloat("Speed", 0);
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                NewDestination();
                currentTime = 0;
                iddleTime = 0;
            }

            return;
        }

        if (Vector2.Distance(transform.localPosition, destionation) < Mathf.Epsilon)
        {
            currentTime = time * (1f + Random.value);
            return;
        }

        MoveToDestination();
    }

    void MoveToDestination()
    {
        if (!audioWalk.isPlaying)
            audioWalk.Play();

        light2D.intensity = 1f;
        anim.SetFloat("Speed", 1);

        transform.localPosition = Vector2.MoveTowards(transform.localPosition, destionation, Time.deltaTime * speed);
        LookAt2D();
    }

    bool ReachDestination()
    {
        return Vector2.Distance(transform.localPosition, destionation) < Mathf.Epsilon;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere((Vector3)destionation + transform.parent.position, 0.2f);
    }

    void LookAt2D()
    {
        float a = transform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(a, destinationAngle, 2);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    void SetLookAngle2D()
    {
        Vector2 dir = destionation - (Vector2)transform.localPosition;
        destinationAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void OnMouseDown()
    {
        iddleTime = 0;

        if (Face.Needs[kind].value <= 0)
            return;

        if (!audioAuch.isPlaying)
            audioAuch.Play();

        Face.Click(kind);

        Face.Needs[kind].value -= Random.value / 15f;
        if (Face.Needs[kind].value <= 0)
        {
            Face.Needs[kind].value = 0;

            destionation = Vector2.zero;
            Vector2 dir = destionation - (Vector2)transform.localPosition;
            destinationAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            currentTime = 0;
        }

        Pool.GetObject(transform.position);
    }
}
