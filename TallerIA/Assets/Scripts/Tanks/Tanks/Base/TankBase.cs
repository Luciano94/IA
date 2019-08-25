using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankBase : MonoBehaviour 
{
	public float VisionRange = 20.0f;
	public float AttackRange = 10.0f;

	public float Health = 100.0f;

	[SerializeField]
	private GameObject bulletPrefab;
	[SerializeField]
	private float bulletSpeed = 10.0f;

	[SerializeField]
	private float fireRate = 1.0f;

	[SerializeField]
	private Transform fireTransform;


	[SerializeField]
	private float speed = 10.0f;
	[SerializeField]
	private float moveAcceleration = 1.0f;


	[SerializeField]
	private float rotSpeed = 100.0f;
	[SerializeField]
	private float rotAcceleration = 1.0f;

	protected bool drawGizmos = true;

	private Rigidbody rb;
	private float axisH = 0.0f;
	private float axisV = 0.0f;
	private bool isRotating = false;
	private bool isMoving = false;
	private float lastFire = 0.0f;

	private Transform currentTargetInSight;
	private Transform currentTargetInAttackRange;
	private Vector3 targetPosition;

	// Use this for initialization
	protected void Awake () {
		rb = GetComponent<Rigidbody>();
	}

	public void LookTarget(Vector3 target)
	{
		this.targetPosition = target;
	}

	public bool IsLookingToTarget(Vector3 target)
	{
		Vector3 dir = target - this.transform.position;
		dir.y = 0.0f;
		dir.Normalize();

		return Vector3.Dot(this.transform.forward, dir) >= 0.9f && Vector3.Dot(this.transform.forward, dir) <= 1.1f;
	}

	public void MoveForward()
	{
		isMoving = true;
		if (axisV < 1.0f)
		{
			axisV += Time.deltaTime * moveAcceleration;
			axisV = Mathf.Clamp(axisV, -1.0f, 1.0f);
		}
	}

	public void MoveBackwards()
	{
		isMoving = true;
		if (axisV > -1.0f)
		{
			axisV -= Time.deltaTime * moveAcceleration;
			axisV = Mathf.Clamp(axisV, -1.0f, 1.0f);
		}
	}

	public void TurnLeft()
	{
		isRotating = true;
		if (axisH > -1.0f)
		{
			axisH -= Time.deltaTime * moveAcceleration;
			axisH = Mathf.Clamp(axisH, -1.0f, 1.0f);
		}
	}

	public void TurnRight()
	{
		isRotating = true;
		if (axisH < 1.0f)
		{
			axisH += Time.deltaTime * moveAcceleration;
			axisH = Mathf.Clamp(axisH, -1.0f, 1.0f);
		}
	}

	public void SetAxisH(float axis)
	{
		isRotating = true;
		axisH = axis;
		axisH = Mathf.Clamp(axisH, -1.0f, 1.0f);
	}

	public void SetAxisV(float axis)
	{
		isMoving = true;
		axisV = axis;
		axisV = Mathf.Clamp(axisV, -1.0f, 1.0f);
	}

	private void LookTowards()
	{
		if (targetPosition == Vector3.zero)
			return;

		Vector3 dir = targetPosition - this.transform.position;
		dir.y = 0.0f;
		dir.Normalize();

		Quaternion rt = Quaternion.LookRotation(dir, Vector3.up);
		Quaternion lerp = Quaternion.Lerp(this.transform.rotation, rt, Time.deltaTime * rotSpeed);
		
		this.rb.MoveRotation(lerp);
	}

	private void Move()
	{
		this.rb.MovePosition(this.transform.position + axisV * this.transform.forward * Time.deltaTime * speed);
	}

	private void Turn()
	{
		Quaternion rot = Quaternion.Euler(0, axisH * Time.deltaTime * rotSpeed, 0);
		
		this.rb.MoveRotation(rb.rotation * rot);
	}
	
	public float GetDistanceToTarget(Vector3 pos)
	{
		return Vector3.Distance(pos, this.transform.position);
	}

	public void Damage(float dmg)
	{
		Health -= dmg;

		if (Health <= 0.0f)
		{
			TanksManager.Instance.Respawn();
		}
	}

	protected void Update() {
		isRotating = false;
		isMoving = false;

		OnUpdate();

		if (!isRotating)
		{
			if (axisH != 0.0f)
			{
				if (axisH < 0.0f)
				{
					axisH += Time.deltaTime * rotAcceleration;
					if (axisH > 0.0f)
						axisH = 0.0f;
				}
				else if (axisH > 0.0f)
				{
					axisH -= Time.deltaTime * rotAcceleration;
					if (axisH < 0.0f)
						axisH = 0.0f;
				}
			}
		}

		if (!isMoving)
		{
			if (axisV != 0.0f)
			{
				if (axisV < 0.0f)
				{
					axisV += Time.deltaTime * moveAcceleration;
					if (axisV > 0.0f)
						axisV = 0.0f;
				}
				else if (axisV > 0.0f)
				{
					axisV -= Time.deltaTime * moveAcceleration;
					if (axisV < 0.0f)
						axisV = 0.0f;
				}
			}
		}
	}

	public void Fire()
	{
		if (Time.time - lastFire < fireRate)
			return;

		lastFire = Time.time;

		GameObject go = Instantiate(bulletPrefab, fireTransform.position, Quaternion.LookRotation(this.transform.forward));
		Rigidbody rb = go.GetComponent<Rigidbody>();
		
		rb.velocity = fireTransform.forward * bulletSpeed;
	}


	// Update is called once per frame
	protected void FixedUpdate () 
	{
		Turn();
		LookTowards();
		Move();

		targetPosition = Vector3.zero;
	}
	public void EnemyInSight(Transform enemy)
	{
		if (enemy != null)
		{
			currentTargetInSight = enemy;
			OnEnemyInSight(enemy.position);
		}
		else if (currentTargetInSight != null)
		{
			OnEnemyOutOfSight(currentTargetInSight.position);
			currentTargetInSight = null;
		}
	}

	public void EnemyInAttackRange(Transform enemy)
	{
		if (enemy != null)
		{
			currentTargetInAttackRange = enemy;
			OnEnemyInAttackRange(enemy.position);
		}
		else if (currentTargetInAttackRange != null)
		{
			OnEnemyOutOfAttackRange(currentTargetInAttackRange.position);
			currentTargetInAttackRange = null;
		}
	}

	abstract protected void OnUpdate();
	
	virtual protected void OnEnemyInSight(Vector3 enemyPosition)
	{

	}

	virtual protected void OnEnemyOutOfSight(Vector3 lastPosition)
	{

	}

	virtual protected void OnEnemyInAttackRange(Vector3 enemyPosition)
	{
		
	}
	virtual protected void OnEnemyOutOfAttackRange(Vector3 lastPosition)
	{
		
	}


	private void OnDrawGizmos() 
	{
		if (drawGizmos)
		{
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, AttackRange);

			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, VisionRange);
		}
	}
}
