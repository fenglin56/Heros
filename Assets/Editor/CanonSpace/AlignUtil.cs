using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(Transform))]
public class AlignUtil : Editor
{
	
    static public int GridSize;
    static public bool IsFolding = false;

    static public bool ShowLinesX = false;
    static public bool ShowLinesY = false;
    static public bool ShowLinesZ = false;
	
	public Vector3 MoveSnap;
	public float RotSnap;
	public float ScaleSnap;

    private Transform _myTransform;

    void OnEnable()
    {
        _myTransform = target as Transform;
        EditorApplication.modifierKeysChanged += Repaint;
		
		MoveSnap.x = EditorPrefs.GetFloat("MoveSnapX", 1);
		MoveSnap.y = EditorPrefs.GetFloat("MoveSnapY", 1);
		MoveSnap.z = EditorPrefs.GetFloat("MoveSnapZ", 1);
		RotSnap = EditorPrefs.GetFloat("RotationSnap", 15);
		ScaleSnap = EditorPrefs.GetFloat("ScaleSnap", 1);
    }
    

    public override void OnInspectorGUI()
    {
        Undo.SetSnapshotTarget(target, "Transform");
        _myTransform.localPosition = EditorGUILayout.Vector3Field("Position", _myTransform.localPosition);
        _myTransform.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", new Vector3(_myTransform.localEulerAngles.x, _myTransform.localEulerAngles.y, _myTransform.localEulerAngles.z));
        _myTransform.localScale = EditorGUILayout.Vector3Field("Scale", _myTransform.localScale);
        IsFolding = EditorGUILayout.Foldout(IsFolding, " Conan Space");

        if (IsFolding)
        {
            EditorGUI.indentLevel = 2;
            GridSize = EditorGUILayout.IntSlider("Grid Map", GridSize, 0, 50);
			
			EditorGUILayout.BeginVertical();
			MoveSnap = EditorGUILayout.Vector3Field("Move", MoveSnap);
			RotSnap = EditorGUILayout.FloatField("Rot", RotSnap);
			ScaleSnap = EditorGUILayout.FloatField("Scale", ScaleSnap);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			ShowLinesX = EditorGUILayout.Toggle("X", ShowLinesX);
            ShowLinesY = EditorGUILayout.Toggle("Y", ShowLinesY);
            ShowLinesZ = EditorGUILayout.Toggle("Z", ShowLinesZ);    
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			Vector3 worldPosition = _myTransform.position;
			if(GUILayout.Button("Align X"))
			{
				worldPosition.x = AlignToUnit(worldPosition.x, MoveSnap.x);
			}
			if(GUILayout.Button("Align Y"))
			{
				worldPosition.y = AlignToUnit(worldPosition.y, MoveSnap.y);
			}
			if(GUILayout.Button("Align Z"))
			{
				worldPosition.z = AlignToUnit(worldPosition.z, MoveSnap.z);
			}
			_myTransform.position = worldPosition;
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
			
            EditorGUI.indentLevel = 0;
        }

        if (GUI.changed)
        {
			EditorPrefs.SetFloat("MoveSnapX", MoveSnap.x);
			EditorPrefs.SetFloat("MoveSnapY", MoveSnap.y);
			EditorPrefs.SetFloat("MoveSnapZ", MoveSnap.z);
			EditorPrefs.SetFloat("RotationSnap", RotSnap);
			EditorPrefs.SetFloat("ScaleSnap", ScaleSnap);
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
            EditorUtility.SetDirty(target);
        }

        Undo.ClearSnapshotTarget();
    }
	
	void OnSceneGUI ()
	{
	    for (int i = -GridSize; i <= GridSize; i++)
	    {
			Vector3 alignedPos = Vector3AlignToUnit(_myTransform.position.x, _myTransform.position.y, _myTransform.position.z);
            if (ShowLinesX)
            {
                Handles.color = Color.red; 
				Vector3 lineStart = alignedPos, lineEnd = alignedPos;
				lineStart.x += GridSize*MoveSnap.x;
				lineStart.z += i*MoveSnap.z;
				lineEnd.x += -GridSize*MoveSnap.x;
				lineEnd.z += i*MoveSnap.z;
                Handles.DrawLine(lineStart, lineEnd);
				
				lineStart = alignedPos;
				lineEnd = alignedPos;
				lineStart.x += i*MoveSnap.x;
				lineStart.z += GridSize*MoveSnap.z;
				lineEnd.x += i*MoveSnap.x;
				lineEnd.z += -GridSize*MoveSnap.z;
                Handles.DrawLine(lineStart, lineEnd);
            }

            if (ShowLinesY)
            {
                Handles.color = Color.blue;
				Vector3 lineStart = alignedPos, lineEnd = alignedPos;
				lineStart.y += i*MoveSnap.y;
				lineStart.z += GridSize*MoveSnap.z;
				lineEnd.y += i*MoveSnap.y;
				lineEnd.z += -GridSize*MoveSnap.z;
                Handles.DrawLine(lineStart, lineEnd);
				
				lineStart = alignedPos;
				lineEnd = alignedPos;
				lineStart.y += -GridSize*MoveSnap.y;
				lineStart.z += i*MoveSnap.z;
				lineEnd.y += GridSize*MoveSnap.y;
				lineEnd.z += i*MoveSnap.z;
                Handles.DrawLine(lineStart, lineEnd);
            }
            if (ShowLinesZ)
            {
                Handles.color = Color.green;
				Vector3 lineStart = alignedPos, lineEnd = alignedPos;
				lineStart.y += i*MoveSnap.y;
				lineStart.x += GridSize*MoveSnap.x;
				lineEnd.y += i*MoveSnap.y;
				lineEnd.x += -GridSize*MoveSnap.x;
                Handles.DrawLine(lineStart, lineEnd);
				
				lineStart = alignedPos;
				lineEnd = alignedPos;
				lineStart.y += GridSize*MoveSnap.y;
				lineStart.x += i*MoveSnap.x;
				lineEnd.y += -GridSize*MoveSnap.y;
				lineEnd.x += i*MoveSnap.x;
                Handles.DrawLine(lineStart, lineEnd);
            }
	    }
        
	}

    Vector3 Vector3AlignToUnit(float fvx, float fvy, float fvz)
    {
		return new Vector3(AlignToUnit(fvx, MoveSnap.x), AlignToUnit(fvy, MoveSnap.y), AlignToUnit(fvz, MoveSnap.z));
    }
	
	// accurate to 0.0001
	float AlignToUnit(float val, float unit)
	{
		return Mathf.Round(val / unit) * unit;
	}

    void OnDisable()
    {
        EditorApplication.modifierKeysChanged -= Repaint;
    }
	
}
