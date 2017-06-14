using UnityEngine;

public class ControlPoint : MonoBehaviour, IGameLoop
{
    #region Editor

    public bool ShowVisualDebug;
    public SpriteRenderer MySprite;
    public Collider2D DragCollider;
    public bool AlwaysDraggable;
    public float Influence;
    public float StartRotation;
    public float MinDriftSpeed;
    public float MaxDriftSpeed;
    public float DragSpeed;
    public float MinDragDistance;
    public ControlPoint[] Children;

    #endregion


    #region Properties

    private TextMesh mDebugText;

    private Rigidbody2D mRigidbody2D;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    public bool IsDrifting { get { return bDrifting; } }
    private bool bDrifting;
    public bool IsDragging { get { return bDragging; } }
    private bool bDragging;
    private float mDragDistance;

    private System.Action mMoveCallback;
    private System.Action mStopCallback;

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
        if(ShowVisualDebug)
        {
            TextMesh ExistingText = GetComponentInChildren<TextMesh>();
            if (ExistingText == null)
            {
                mDebugText = ((GameObject)Instantiate(GameState.Instance.VisualDebugPrefab)).GetComponent<TextMesh>();
                mDebugText.transform.SetParent(transform, false);
            }
            else
            {
                mDebugText = ExistingText;
            }
            mDebugText.transform.position = MySprite.bounds.center - Vector3.forward;
            mDebugText.text = "";
        }

        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        OriginalPos = transform.localPosition;
        OriginalRot = Quaternion.Euler(StartRotation * Vector3.forward);
        
        DragCollider.enabled = AlwaysDraggable;     
    }

    public void OnGameBegin()
    {
        ResetNode();        
    }

    public void OnFrame()
    {
    }

    #endregion


    #region Public Class Methods
    
    public void SetCallbacks(System.Action Select, System.Action Release)
    {
        mMoveCallback = Select;
        mStopCallback = Release;
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
           // SetSpriteColor(Color.white);
           // Debug.Log(gameObject.name + " Mobilized");
            mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void Immobilize()
    {
        if (mRigidbody2D != null)
        {
            //SetSpriteColor(Color.green);
            //Debug.Log(gameObject.name + " Immobilized");
            mRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            mRigidbody2D.velocity = Vector2.zero;
            mRigidbody2D.angularVelocity = 0;
           // DisableDragging();
        }
    }

    #endregion


    #region Private Class Methods

    private void EnableDragging()
    {
        if (DragCollider != null)
        {
            DragCollider.enabled = true;
        }
    }

    private void DisableDragging()
    {
        if (DragCollider != null && !AlwaysDraggable)
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

    private void OnSelect()
    {
        //Player can only move Drifting ControlPoints and RightHand
        if (bDrifting || AlwaysDraggable)
        {
            OnDragEnter();
            OnDriftExit();
        }
    }

    private void OnDriftEnter()
    {
        GameState.Instance.DisplayVisualDebug(mDebugText, "ENTER DRIFT", Color.yellow, 2.0f);
        Debug.LogWarning("Entering Drift... " + gameObject.name);
        
        //Physics
        EnableDragging();
        mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        float RandomDriftSpeed = Random.Range(MinDriftSpeed, MaxDriftSpeed);
        mRigidbody2D.velocity = Vector2.down * RandomDriftSpeed;        
        if (Mathf.Abs(mRigidbody2D.centerOfMass.x) <= Mathf.Epsilon)
        {
            mRigidbody2D.angularVelocity = RandomDriftSpeed * Mathf.Pow(-1, Random.Range(0, 2));
        }

        //Visual FX
        SetSpriteColor(Color.red);

        //Notify ControlPointManager
        if (mMoveCallback != null) { mMoveCallback(); }

        bDrifting = true;
    }

    private void OnDriftExit()
    {
        if(!bDrifting)
        {
            return;
        }

        GameState.Instance.DisplayVisualDebug(mDebugText, "EXIT DRIFT", Color.black , 2.0f);
        Debug.LogWarning("Exiting Drift... " + gameObject.name);

        //Drift Timeout
        if (!bDragging) 
        { 
            //Visual FX
            SetSpriteColor(Color.white);

            //Notify ControlPointManager
            if (mStopCallback != null) { mStopCallback(); }
        }

        bDrifting = false;
    }

    private void OnDragEnter()
    {
        GameState.Instance.DisplayVisualDebug(mDebugText, "ENTER DRAG", Color.green, 2.0f);
        Debug.LogWarning("Entering Drag... " + gameObject.name);   
        
        if (AlwaysDraggable)
        {
            //Physics
            mRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

            //Notify ControlPointManager
            if (mMoveCallback != null) { mMoveCallback(); }
        }
                
        //Visual FX
        SetSpriteColor(Color.blue);

        bDragging = true;
    }

    private void OnDragExit()
    {
        GameState.Instance.DisplayVisualDebug(mDebugText, "EXIT DRAG", Color.black, 2.0f);
        Debug.LogWarning(" Exiting Drag... " + gameObject.name);

        mDragDistance = 0;   
        
        //Physics
        DisableDragging();

        //Visual FX
        SetSpriteColor(Color.white);

        //Notify ControlPointManager
        if (mStopCallback != null) { mStopCallback(); }

        bDragging = false;
    }


    private void OnMouseMove()
    {
        if(bDragging)
        {
            float DeltaX = Input.GetAxis("Mouse X") * UIManager.UNITS_PER_PIXEL.x;
            float DeltaY = Input.GetAxis("Mouse Y") * UIManager.UNITS_PER_PIXEL.y;
            Vector2 Delta = new Vector2(DeltaX, DeltaY);
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
        DisableDragging();
        SetSpriteColor(Color.white);
    }


    #endregion    
}