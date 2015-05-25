using UnityEngine;
using System;

public class JoyStick : MonoBehaviour
{
	private bool _isActiceMove = false;
	private int _fingerId = -1;
	private Vector3 _position = Vector3.zero;
	protected Vector3 _guiCenter = Vector3.zero;
	protected Transform _transform;
	protected Camera _uiCamera;
	private UISprite _sprite;
    private float _radious;
	protected BoxCollider selfCollider ;

	private static int _zPos = 200;
	
	public delegate void PadPress(bool isPress, Vector3 dir);
	public PadPress PadPressAction;

    public delegate void DoubleClickDelegate();
    public DoubleClickDelegate DoubleClick;
	
	public void AddPressDelegate(PadPress pp)
	{
		PadPressAction += pp;
	}

  
    public void SetJoyStickActive()
    {
        _isActiceMove = true;
    }

    public void AddDoubleClickDelegate(DoubleClickDelegate dc)
    {
        DoubleClick += dc;
    }

	private bool _tutorial = false;
	private Vector3 _screenPos = Vector3.zero;

    public void SetGUICenter(Vector3 center, float radious)
    {
        _radious = radious;
        _guiCenter = center;
    }
	
	public virtual void MovePad(bool isPress, Vector3 dir)
	{	
		if(_zPos < 0)
		{
			return;
		}
		
		if(isPress){
			
		}
		
	
		if(isPress)
		{
			
		}
		
		
		if(PadPressAction != null)
		{
			Vector3 speed = Camera.main.transform.TransformDirection (dir);
			speed.y = 0.0f;
			speed.Normalize (); 
			PadPressAction(isPress,speed);
		}
	}
	
	
	void OnDestroy()
	{
		PadPressAction = null;	
        DoubleClick = null;
	}
	
	public void MoveIn()
	{
		_zPos = 200;
	}
	
	public void MoveOut()
	{
		IsActiveMove = false;
		_position = Vector3.zero;
		MovePad(false,_position);
		_zPos = -200;
	}
	
	
	public Vector3 Position {
		get { return _position; }
		set{
			_position = value;	
		}
	}

	public void Awake ()
	{
		_transform = transform;
		
		IsActiveMove = false;
		_tutorial = false;
		_zPos = 200;
		PlayerManager.Instance.ConnectHeroJoyStick(this);
		
	}
	
	public bool IsActiveMove
	{
		get{
			return _isActiceMove;	
		}
		
		set{
			_isActiceMove = value;	
			if(_isActiceMove)
			{
                ShowDragPanel();
				//_sprite.enabled = true;
			}else
			{
                HideDragPanel();
				//_sprite.enabled = false;
			}
		}
	}

    public virtual void ShowDragPanel()
    { 
    }

    public virtual void HideDragPanel()
    { 
    }
	
	void validCenter()
	{
        float offset = -1f;//Screen.width/10;
		
		if(_guiCenter.x < offset )
		{
			_guiCenter.x = offset;
		}
		
		if(_guiCenter.y < offset)
		{
			_guiCenter.y = offset;
		}
	}

	public void Update ()
	{
		#if ((!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR)
        if (_isActiceMove)
        {
			if (Input.GetMouseButtonUp (0)) {
				IsActiveMove = false;
				_position = Vector3.zero;
				MovePad(false,_position);
			} else if (Input.GetMouseButton (0)) {
				_position.x = Input.mousePosition.x - _guiCenter.x;
				_position.z = Input.mousePosition.y - _guiCenter.y;
				MovePad(true,_position);
			}
		} else {
			//强引导，不能进行点击事件//
			if(!TaskModel.Instance.ResponseClickEvent)
				return;
			if (Input.GetMouseButtonDown (0) 
                && Input.mousePosition.x >= (_guiCenter.x - _radious) 
                && Input.mousePosition.x <= _guiCenter.x + _radious
                && Input.mousePosition.y >= _guiCenter.y - _radious
                && Input.mousePosition.y <= _guiCenter.y + _radious) //&& Input.mousePosition.x < Screen.width / 2 && Input.mousePosition.y < Screen.height * 3/ 4) {
            {
				//jamfing add//
				Vector3 clickPoint = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
				if (UICamera.currentCamera != null)
				{
					var uiRay = UICamera.currentCamera.ScreenPointToRay(clickPoint);
					RaycastHit uiRaycastHit;

					if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
					{
						if(selfCollider == null || (selfCollider != null && uiRaycastHit.collider != selfCollider))
							return;
					}
				}
				/*if(UICamera.hoveredObject != null)
					return;*/
                //_guiCenter.x = Input.mousePosition.x;
				//_guiCenter.y = Input.mousePosition.y;
				//validCenter();
				_position = Vector3.zero;
				IsActiveMove = true;
			}
		}

		_position = Vector3.zero;
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
			_position.x = -1;
		}
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
			_position.x = 1;
		}
		if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
			_position.z = -1;
		}
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
			_position.z = 1;
		}

		if(_position != Vector3.zero)
		{
			MovePad(true,_position);
		}
		else
		{
			MovePad(false,_position);
		}
		#else
		int count = Input.touchCount;
		bool isPress = false;
		for (int i = 0; i < count; i++) 
		{
			Touch touch = Input.GetTouch (i);
            if (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height * 3/ 4)
			{
				isPress = true;
			}
			if(isPress)
			{
				if (_isActiceMove) 
				{
					if (touch.fingerId == _fingerId) 
					{
						if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
							IsActiveMove = false;
							_position = Vector3.zero;
							MovePad(false,_position);
						} 
						else 
						{
							_position.x = touch.position.x - _guiCenter.x;
							_position.z = touch.position.y - _guiCenter.y;
							MovePad(true,_position);
						}
					}
				
				}
				else 
				{
					//强引导，不能进行点击事件//
					if(!TaskModel.Instance.ResponseClickEvent)
						return;
                    if(touch.phase == TouchPhase.Began
                       &&touch.position.x >= (_guiCenter.x - _radious) 
                       && touch.position.x <= _guiCenter.x + _radious
                       && touch.position.y >= _guiCenter.y - _radious
                       && touch.position.y <= _guiCenter.y + _radious)
                    {
						//jamfing add//
						Vector3 clickPoint = new Vector3(touch.position.x,touch.position.y,0);
						if (UICamera.currentCamera != null)
						{
							var uiRay = UICamera.currentCamera.ScreenPointToRay(clickPoint);
							RaycastHit uiRaycastHit;
							
							if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
							{
								if(selfCollider == null || (selfCollider != null && uiRaycastHit.collider != selfCollider))
									return;
							}
						}
						/*if(UICamera.hoveredObject != null)
							return;*/
    					IsActiveMove = true;
    					_fingerId = touch.fingerId;
    					//_guiCenter.x = touch.position.x;
    					//_guiCenter.y = touch.position.y;
    					//validCenter();
    					_position = Vector3.zero;
                    }
					
				}
				break;
			}
			
		}
		if(!isPress)
		{
			_fingerId = -1;
			
			if(!_tutorial)
			{
				IsActiveMove = false;
			}
			_position = Vector3.zero;
			MovePad(false,_position);			
		}
		#endif
	}
	
	public void OnGamePause()
	{
		ReleaseMove();	
	}
	
	
	public void ReleaseMove()
	{
		_fingerId = -1;
		_position = Vector3.zero;
		MovePad(false,_position);
		IsActiveMove = false;
	}
	
	
	void LateUpdate ()
	{
		if (_uiCamera != null) {
			
			if(_isActiceMove)
			{
				if(_tutorial)
				{
					return;
				}
                _transform.position = _uiCamera.ScreenToWorldPoint(_guiCenter);
                _screenPos = _transform.localPosition;
				_screenPos.z = _zPos;
                _transform.localPosition= _screenPos; 
                //_transform.position = _uiCamera.ScreenToWorldPoint(_guiCenter);
			}
		}
	}
}

