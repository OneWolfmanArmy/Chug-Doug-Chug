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
    public ControlPoint[] Children;

    #endregion


    #region Properties

    private Rigidbody2D mRigidbody2D;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;
    
    private bool bDrifting;
    private float mDriftTime;
    private bool bDragging;
    private float mDragDistance;

    private System.Action mSelectCallback;
    private System.Action mReleaseCallback;

    #endregion


    #region MonoBehavior    

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

    public void OnCreate()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        OriginalPos = transform.localPosition;
        OriginalRot = Quaternion.Euler(StartRotation * Vector3.forward);
        
        DragCollider.enabled = Draggable;     
    }

    public void OnGameBegin()
    {
        ResetNode();        
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
    
    public void SetCallbacks(System.Action Select, System.Action Release)
    {
        mSelectCallback = Select;
        mReleaseCallback = Release;
    }

    public void Destabilize(float Delay, float MaxTime)
    {
        Invoke("OnDriftEnter", Delay);
        Invoke("OnDriftExit", Delay + MaxTime);
    }

    public bool IsMobile()
    {
        if (mRigidbody2D != null)
        {
            return mRigidbody2D.bodyType == RigidbodyType2D.Dynamic;
        }
        return false;
    }

    public void Mobilize()
    {
        if (mRigidbody2D != null)
        {
            mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void Immobilize()
    {
        if (mRigidbody2D != null)
        {
            mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            mRigidbody2D.velocity = Vector2.zero;
            mRigidbody2D.angularVelocity = 0;
            DisableDragCollider();
        }
    }

    #endregion


    #region Private Class Methods

    private void EnableDragCollider()
    {
        if (DragCollider != null)
        {
            DragCollider.enabled = true;
        }
    }

    private void DisableDragCollider()
    {
        if (DragCollider != null && !Draggable)
        {
            DragCollider.enabled = false;
        }
    }

    private void SetSpriteColor(Color Col)
    {
        if (MySprite != null)
        {
            MySprite.color = Col;
        }
    }

    private void OnDriftEnter()
    {
        bDrifting = true;
        mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        mRigidbody2D.velocity = Vector2.down * Random.Range(MinDriftSpeed, MaxDriftSpeed);
        
        if (Mathf.Abs(mRigidbody2D.centerOfMass.x) <= .05f)
        {
            mRigidbody2D.angularVelocity = 20.0f * Mathf.Pow(-1, Random.Range(0, 2));
        }

        SetSpriteColor(Color.red);

        EnableDragCollider();

        if (mSelectCallback != null)
        {
            mSelectCallback();
        }
    }

    private void OnDriftExit()
    {
        if (bDrifting)
        {
            bDrifting = false;
            SetSpriteColor(Color.white);
        }
    }

    private void OnDragEnter()
    {
        bDragging = true;
        if (Draggable)
        {
            mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

            if (mSelectCallback != null)
            {
                mSelectCallback();
            }
        }
        else
        {
            OnDriftExit();
        }
        
        SetSpriteColor(Color.blue);
    }

    private void OnDragExit()
    {
        bDragging = false;
        mDragDistance = 0;
        SetSpriteColor(Color.white);       
        DisableDragCollider();

        if (mReleaseCallback != null)
        {
            mReleaseCallback();
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
            Vector2 Delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * UIManager.CanvasWidth/Screen.width;
            mDragDistance += Delta.magnitude;
            mRigidbody2D.velocity = Delta * DragSpeed;
        }
    }

    private void OnRelease()
    {
        if (bDragging)
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

        Immobilize();     
        DisableDragCollider();
        SetSpriteColor(Color.white);
    }


    #endregion    
}