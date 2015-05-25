using UnityEngine;
using System.Collections;

public class PlayerMake : MonoBehaviour {

    private GameObject root;
    private string animName;
    public GameObject WeaponPrefab;
	// Use this for initialization
	void Start () {
        animName = string.Empty;

        MakeRole("daoke", "daoke01", "idle", "attack01", "attack02", "idle", "idleattack", "run", "dead");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
   
    void OnGUI()
    {
        animName = string.Empty;
        if (GUILayout.Button("Avatar1",GUILayout.Width(100)))
        {
            MakeRole("daoke", "daoke01",  "idle", "attack01", "attack02", "idle", "idleattack", "run", "dead");
        }
        if (GUILayout.Button("Avatar2", GUILayout.Width(100)))
        {
            MakeRole("daoke", "daoke02", "idle", "attack01", "attack02", "idle", "idleattack", "run", "dead");
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("attack01")) animName = "attack01";
        if (GUILayout.Button("attack02")) animName = "attack02";
        if (GUILayout.Button("idle")) animName = "idle";
        if (GUILayout.Button("idleattack")) animName = "idleattack";
        if (GUILayout.Button("run")) animName = "run";
        if (GUILayout.Button("dead")) animName = "dead";

        if (!string.IsNullOrEmpty(this.animName))
        {
            root.animation.CrossFade(this.animName);
        }
        GUILayout.EndHorizontal();
            
    }

    private void MakeRole(string roleName, string avatarName, string defaultAnim, params string[] anims)
    {
        Object.DestroyImmediate(root);
        root = null;
        root = RoleGenerate.GenerateRole(roleName, avatarName);
        root.transform.Rotate(0, 180, 0);

        RoleGenerate.AttachAnimation(root, roleName, defaultAnim, anims);

        RoleGenerate.AttachWeapon(root, AttachPoint.RHWeapon, WeaponPrefab,null);

        root.animation.wrapMode = WrapMode.Loop;
    }
}
