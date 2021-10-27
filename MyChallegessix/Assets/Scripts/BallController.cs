using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //Audio manager
    //public GameObject Audio;

    public Rigidbody rb;
    public float speed = 15;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    public Vector2 swipePosLastFrame;
    public Vector2 swipePosCurrentFrame;
    public Vector2 currentSwipe;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }


    private void FixedUpdate()
    {   
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }


        
        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position,nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isTraveling)
            return;

        if(Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                // UP/Down
                if(currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    // GO UP/Down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);

                    //Adding Sound here
                    //Load the Audio Script here
                    FindObjectOfType<AudioManager>().bounceSound.Play();
                }

                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);

                    FindObjectOfType<AudioManager>().bounceSound.Play();

                    //FindObjectOfType<AudioManager>.Sound + variable + name.Play(); //Another Sound here
                }
            }

            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }
}
