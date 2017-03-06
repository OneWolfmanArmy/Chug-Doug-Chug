﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlPoint : MonoBehaviour
{
    #region Editor

    public GameObject Target;
    public Transform Node;
    public float NodeRadius;
    public float DragSpeed;
    public float MinGravity;
    public float MaxGravity;

    #endregion


    #region Properties

    private Rigidbody2D mRigidbody2D;
    private SpriteRenderer mSpriteRenderer;

    private bool bDrifting;
    private bool bDragging;
    
    private float mDriftForce;

    private System.Action UpdateManager;

    #endregion


    #region MonoBehavior

    // Use this for initialization
    void Start ()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        Target.transform.position = Node.position;

        ResetNode();
    }

    void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(Node.position, Vector3.forward, NodeRadius);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(Target.transform.position, Vector3.forward, NodeRadius);
    }

    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnSelect();
        }
        else if(Input.GetMouseButton(0))
        {
            OnMouseMove();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            OnRelease();
        }
    }

    void OnMouseExit()
    {
        OnRelease();
    }

    #endregion

    #region Public Class Methods

    public void Destabilize(System.Action Callback, float Delay)
    {
        UpdateManager = Callback;
        Invoke("OnDriftEnter", Delay);
    }

    #endregion


    #region Private Class Methods

    private void OnDriftEnter()
    {
        bDrifting = true;
        mRigidbody2D.gravityScale = Random.Range(MinGravity, MaxGravity);
        mSpriteRenderer.color = Color.red;
    }

    private void OnDriftExit()
    {
        bDrifting = false;
        mSpriteRenderer.color = Color.white;
        mRigidbody2D.gravityScale = 0;
    }

    private void OnDragEnter()
    {
        bDragging = true;
        mSpriteRenderer.color = Color.blue;
    }

    private void OnDragExit()
    {
        bDragging = false;
        mSpriteRenderer.color = Color.white;
        mRigidbody2D.velocity = Vector2.zero;
        UpdateManager();
        ResetNode();       
    }

    private void OnSelect()
    {
        if (bDrifting)
        {
            OnDriftExit();
            OnDragEnter();            
        }
    }

    private void OnMouseMove()
    {
        if(bDragging)
        {
            // mRigidbody2D.gravityScale = 0;
            Vector2 Delta= new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mRigidbody2D.velocity = Delta * DragSpeed;
        }
    }

    private void OnRelease()
    {
        if (mRigidbody2D != null && bDragging)
        {
            OnDragExit();        
        }
    }  
    
    private bool IsOnTarget()
    {
        return false;// Vector2.SqrMagnitude(Node.position - Target.position) <= mNodeRadiusSqr;
    } 

    private void ResetNode()
    {
        bDrifting = false;
        bDragging = false;
        mRigidbody2D.velocity = Vector2.zero;
        mRigidbody2D.angularVelocity = 0;
        mRigidbody2D.gravityScale = 0;
    }

    #endregion
    
}