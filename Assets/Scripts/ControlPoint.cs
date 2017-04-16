﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlPoint : MonoBehaviour, IGameLoop
{
    #region Editor

    public SpriteRenderer MySprite;
    public float Influence;
    public float StartRotation;
    public float DragSpeed;
    public float MinGravity;
    public float MaxGravity;

    #endregion


    #region Properties

    private Rigidbody2D mRigidbody2D;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;
    private Vector3 OriginalScale;
    
    private bool bDrifting;
    private float mDriftTime;
    private bool bDragging;

    private System.Action UpdateControlPointManager;

    #endregion


    #region MonoBehavior

    // Use this for initialization
    void Start()
    {       
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        OriginalPos = transform.localPosition;
        OriginalRot = Quaternion.Euler(StartRotation * Vector3.forward);
    }

    /*void Update()
    {
        Vector3 inv = new Vector3(1f / transform.parent.localScale.x, 1f / transform.parent.localScale.y, 1f / transform.parent.localScale.z);
        if (inv != Vector3.one)
        {
            //transform.localScale = Vector3.one; //Vector3.Scale(transform.localScale, transform.parent.localScale);
            Vector3 bounds = mSpriteRenderer.bounds.size;
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(OriginalScale.x / bounds.x, OriginalScale.y / bounds.y, OriginalScale.z / bounds.z));

            Debug.Log(gameObject.name + " " + mSpriteRenderer.bounds.size + " " + transform.localScale);

        }
    }*/


    /* void OnDrawGizmos()
     {
         UnityEditor.Handles.color = Color.blue;
         UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, NodeRadius);
         //UnityEditor.Handles.color = Color.yellow;
         //UnityEditor.Handles.DrawWireDisc(Target.transform.position, Vector3.forward, NodeRadius);
     }*/

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnSelect();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseMove();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnRelease();
        }       
    }

    void OnMouseExit()
    {
        OnRelease();
    }

    #endregion


    #region IGameLoop

    public void OnGameBegin()
    {
        if (mRigidbody2D != null)
        {
            ResetNode();
        }
    }

    public void OnFrame()
    {
        
    }

    #endregion


    #region Public Class Methods


    public void SetRigidBodyType(RigidbodyType2D Type)
    {
        //mRigidbody2D.constraints = RigidbodyConstraints2D.None;
        mRigidbody2D.bodyType = Type;
    }    

    public void Destabilize(System.Action Callback, float Delay, float MaxTime)
    {
        UpdateControlPointManager = Callback;
        Invoke("OnDriftEnter", Delay);
        Invoke("OnDriftExit", Delay + MaxTime);
    }

    #endregion


    #region Private Class Methods

    private void OnDriftEnter()
    {
        bDrifting = true;
        mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        mRigidbody2D.gravityScale = Random.Range(MinGravity, MaxGravity);
        float ang = -10f;// * Mathf.Pow(-1, (int)Random.Range(0, 2));
        Debug.Log(ang);
        mRigidbody2D.AddTorque(ang);
        ChangeSpriteColor(Color.red);
    }

    private void OnDriftExit()
    {
        if (bDrifting)
        {
            bDrifting = false;
            ChangeSpriteColor(Color.white);
            mRigidbody2D.gravityScale = 0;
            UpdateControlPointManager();
        }
    }

    private void OnDragEnter()
    {
        bDragging = true;
        ChangeSpriteColor(Color.blue);
    }

    private void OnDragExit()
    {
        bDragging = false;
        MySprite.color = Color.white;
        mRigidbody2D.velocity = Vector2.zero;
        mRigidbody2D.gravityScale = 0;
        UpdateControlPointManager();
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
            Vector2 Delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mRigidbody2D.AddForce(Delta * DragSpeed);
        }
    }

    private void OnRelease()
    {
        if (mRigidbody2D != null && bDragging)
        {
            OnDragExit();        
        }
    }  

    public void ResetNode()
    {
        transform.localPosition = OriginalPos;
        transform.localRotation = OriginalRot;
        bDrifting = false;
        bDragging = false;
        if (mRigidbody2D != null)
        {
            mRigidbody2D.velocity = Vector2.zero;
            mRigidbody2D.angularVelocity = 0;
            mRigidbody2D.gravityScale = 0;
            mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }
        ChangeSpriteColor(Color.white);
    }

    private void ChangeSpriteColor(Color Col)
    {
        if (MySprite != null)
        {
            MySprite.color = Col;
        }
    }

    #endregion
    
}
