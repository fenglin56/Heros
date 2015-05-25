// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;

public class Unit : NaviUnit
{
	// ====================================================================================================================
	#region inspector properties

	public int playerSide = 1;			// player-1 and player-2
	public int maxMoves = 1;			// how far this unit can move per turn
	public int attackRange = 1;			// range it can attack at
	public int attackDamage = 1;		// damage caused by an attack
	public Vector3 targetingOffset = Vector3.zero; // where missile should hit it

	#endregion
	// ====================================================================================================================
	#region vars

	[HideInInspector]
	public int currMoves = 0; // how many moves this unit has left

	public bool didAttack { get; set; }

	private SampleWeapon weapon;

	// helpers during random movement demo
	private bool waitingToChooseMoveNode = false;
	private float timer = 0f;
	private int deadlockDetection = 0;				

	#endregion
	// ====================================================================================================================
	#region pub

	public override void Start()
	{
		base.Start();
		weapon = gameObject.GetComponent<SampleWeapon>();
		weapon.Init(OnAttackDone);
	}

	public override void Update()
	{
		base.Update();

		if (waitingToChooseMoveNode)
		{
			timer -= Time.deltaTime;
			if (timer <= 0.0f)
			{
				waitingToChooseMoveNode = false;
				int tries = 0;
				TileNode node = null;
				while (node == null)
				{
					int r = Random.Range(0, mapnav.Length);
					if (this.CanStandOn(mapnav[r], true))
					{
						node = mapnav[r];
					}
					tries++;
					if (tries > 10) break;
				}

				if (node != null)
				{
					this.MoveTo(node);
				}

				if (!isMoving)
				{	// try again in a bit
					waitingToChooseMoveNode = true;
					timer = 0.5f;
				}
			}
		}
	}

	/// <summary>Should be called right after unit was spawned</summary>
	public override void Init(UnitEventDelegate callback)
	{
		base.Init(callback);
		this.Reset();		
	}

	/// <summary>Reset some values</summary>
	public void Reset()
	{
		currMoves = maxMoves;
		didAttack = false;
	}

	/// <summary>Check if target can be attacked by this unit</summary>
	public bool CanAttack(Unit target)
	{
		if (didAttack) return false; // can't attack again in this turn
		if (target.playerSide == this.playerSide) return false; // can't attack a friend
		if (this.node.units.Contains(target)) return false; // can't shoot at unit on same tile, for example a flying unit over opponent land unit

		// finally check if target is in range
		return this.node.IsInRange(target.node, this.attackRange);
	}

	/// <summary>Makes an attack on the target. Unit event callback will be passed an eventCode of 2</summary>
	public bool Attack(Unit target)
	{
		if (!CanAttack(target)) return false;

		didAttack = true;

		// turn to face target
		Vector3 direction = target.transform.position - transform.position; direction.y = 0f;
		transform.rotation = Quaternion.LookRotation(direction);

		weapon.Play(target);

		return true;
	}

	public void ChooseRandomTileAndMove()
	{
		waitingToChooseMoveNode = true;
		timer = 0.3f;
	}

	/// <summary>called by the weapon when it is done doing its thing</summary>
	private void OnAttackDone(NaviUnit unit, int eventCode)
	{
		// tell whomever is listening that I am done with my attack. eventCode 2
		if (onUnitEvent != null) onUnitEvent(this, 2);
	}

	protected override void OnMovementDelayed() 
	{
		deadlockDetection++;
		if (deadlockDetection >= 5)
		{
			// seems to be deadlocked, try going to somewhere new
			deadlockDetection = 0;
			ChooseRandomTileAndMove();
		}
	}

	#endregion
	// ====================================================================================================================
}
