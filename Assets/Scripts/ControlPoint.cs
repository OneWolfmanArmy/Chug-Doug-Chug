using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlPoint : MonoBehaviour, IGameLoop
{
    #region Editor

    public SpriteRenderer MySprite;
    public Collider2D DragCollider;
    public bool Draggable;
    public float Influence;
    public float StartRotation;
    public float MinDriftSpeed;
    public float MaxDriftSpeed;
    public float DragSpeed;
    public float MinDragDistance;

    #endregion


    #region Properties

    private Rigidbody2D mRigidbody2D;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;
    
    private bool bDrifting;
    private float mDriftTime;
    private bool bDragging;
    private float mDragDistance;

    private System.Action UpdateControlPointManager;

    #endregion


    #region MonoBehavior

    // Use this for initialization
    void Start()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.bodyType = Draggable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

        OriginalPos = transform.localPosition;
        OriginalRot = Quaternion.Euler(StartRotation * Vector3.forward);
    }

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
        if(bDrifting)
        {
            mDriftTime += Time.deltaTime;
        }
    }

    #endregion


    #region Public Class Methods


    public void SetRigidBodyType(RigidbodyType2D Type)
    {
        if (mRigidbody2D != null)
        {
            mRigidbody2D.bodyType = Type;
        }
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
        mRigidbody2D.velocity = Vector2.down * Random.Range(MinDriftSpeed, MaxDriftSpeed);
        
        if (Mathf.Abs(mRigidbody2D.centerOfMass.x) <= .05f)
        {
            mRigidbody2D.angularVelocity = 20.0f * Mathf.Pow(-1, Random.Range(0, 2));
        }

        ChangeSpriteColor(Color.red);

        DragCollider.enabled = true;
    }

    private void OnDriftExit()
    {
        if (bDrifting)
        {
            bDrifting = false;
            ChangeSpriteColor(Color.white);
        }
    }

    private void OnDragEnter()
    {
        OnDriftExit();
        bDragging = true;
        ChangeSpriteColor(Color.blue);
    }

    private void OnDragExit()
    {
        bDragging = false;
        mDragDistance = 0;
        MySprite.color = Color.white;       
        mRigidbody2D.velocity = Vector2.zero;
        if (!Draggable)
        {
            DragCollider.enabled = false;
        }
        if (UpdateControlPointManager != null)
        {
            UpdateControlPointManager();
        }
    }

    private void OnSelect()
    {
        if (bDrifting || Draggable)
        {
            OnDragEnter();            
        }
    }

    private void OnMouseMove()
    {
        if(bDragging)
        {
            Vector2 Delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mDragDistance += Delta.magnitude;
            mRigidbody2D.velocity = Delta * DragSpeed;
        }
    }

    private void OnRelease()
    {
        if (mRigidbody2D != null && bDragging)
        {
            if (mDragDistance >= MinDragDistance)
            {
                OnDragExit();
            } 
            else
            {
                OnDriftEnter();
            }
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
            if (!Draggable)
            {
                mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                DragCollider.enabled = false;
            }
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