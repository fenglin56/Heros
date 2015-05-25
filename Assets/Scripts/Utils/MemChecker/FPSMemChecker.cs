using UnityEngine;
using System.Collections;

public class FPSMemChecker : MonoBehaviour
{
    public float UpdateInterval = 0.5f;
    
    private float _lastFPSTime;
    private int _frameCount;
    private int _fps;
	private int _lowFps = 60;
    private int _curMemory;

    private const float TEXT_SCALE_COEFF = 2.0f;
    private Vector3 _scaleVector;
    
    private const float RECT_WIDTH  = 64.0f;
    private const float RECT_HEIGHT = 72.0f;
    private Texture2D _texture;
    private Rect _rect;
	
	public enum AlignHorz
	{
		LEFT,
		CENTER,
		RIGHT
	}
	public enum AlignVert
	{
		UP,
		CENTER,
		DOWN
	}
	
	public AlignHorz _alighHorz = AlignHorz.LEFT;
	public AlignVert _alignVert = AlignVert.DOWN;
	
	private float _xPos;
	private float _yPos;
	
    void Awake()
    {
		_xPos = 0;
		switch(_alighHorz)
		{
		case AlignHorz.CENTER:
			_xPos = 0.5f * (Screen.width - RECT_WIDTH);
			break;
		case AlignHorz.RIGHT:
			_xPos = Screen.width - RECT_WIDTH;
			break;
		}		

		_yPos = 0;
		switch(_alignVert)
		{
		case AlignVert.CENTER:
			_yPos = 0.5f * (Screen.height - RECT_HEIGHT);
			break;
		case AlignVert.DOWN:
			_yPos = Screen.height - RECT_HEIGHT - RECT_HEIGHT;
			break;
		}		
		
        _scaleVector = new Vector3(TEXT_SCALE_COEFF, TEXT_SCALE_COEFF, 1.0f);                
        _rect = new Rect(_xPos, _yPos, RECT_WIDTH, RECT_HEIGHT);
        
        _texture = new Texture2D(1, 1);
        _texture.SetPixel(0, 0, Color.black);
        _texture.Apply();
    }
    
    void Start ()
    {
		//AppManager.Instance.LogoutMem += LogoutMem;
        _lastFPSTime = Time.realtimeSinceStartup;
        _frameCount = 0;
        _fps = 0;
        
        _curMemory = 0;               
    }

    void Update ()
    {
        _frameCount += 1;
        
        float currentTime = Time.realtimeSinceStartup;
        
        if (currentTime > _lastFPSTime + UpdateInterval)
        {
            _fps = (int)(_frameCount / (currentTime - _lastFPSTime));
			
			if(_lowFps > _fps)
			{
				_lowFps = _fps;	
			}
            _lastFPSTime = currentTime;
            _frameCount = 0;
            
            _curMemory = (NativeUtils.GetCurrentMemoryBytes () >> 20);
        }
    }
///#DEBUG_ONLY
    void OnGUI ()
    {
        GUI.DrawTexture(_rect, _texture);
        
        Color lastcolor = GUI.contentColor;
        GUI.contentColor = Color.magenta;

        Matrix4x4 lastMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS (new Vector3(_xPos, _yPos, 0), Quaternion.identity, _scaleVector);
        
        string str = _fps.ToString ("00") + " -- " + _lowFps.ToString("00") + "\n" + _curMemory.ToString("000");
        GUILayout.Label (str);
        
        GUI.matrix = lastMatrix;
        GUI.contentColor = lastcolor;
    }
///#
	
	public void LogoutMem(string message)
	{
		//TraceUtil.Log("MEM: "+ message + " "+NativeUtils.GetCurrentMemoryBytes ()/1024);	
	}
	
	public void ResetLowMem()
	{
		_lowFps = _fps;	
	}
}
