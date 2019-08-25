using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanksManager : MonoBehaviour
{
	[SerializeField]
	private CameraControl cameraControl;

	[SerializeField]
	private GameObject playerTankPrefab;
	[SerializeField]
	private GameObject enemyTankPrefab;

	[SerializeField]
	private Transform[] spawnPoints;
	public int resourcesCount = 0;
	const int enemiesCount = 1;

	private TankBase playerTank;
	private List<TankBase> enemyTanks = new List<TankBase>();

	private static TanksManager instance = null;

	public static TanksManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = FindObjectOfType<TanksManager>();
			}
			
			return instance;
		}
	}

	private void Awake ()
	{
		instance = this;
	}

	private void Start() 
	{
		SpanwTanks();
	}

	public void Respawn()
	{
		playerTank.gameObject.SetActive(false);
		if(resourcesCount > 0)
		{
			List<Transform> spawn = new List<Transform>(spawnPoints);
			Vector3 point = PopRandomSpawnPoint(spawn);
			GameObject tank = Instantiate(playerTankPrefab, point, Quaternion.identity);
			playerTank = tank.GetComponent<TankBase>();
			resourcesCount--;
		}
		else
		{
			enemyTanks[0].enabled = false;
		}
	}

	void SpanwTanks()
	{
		Clear();

		cameraControl.m_Targets = new Transform[enemiesCount + 1];

		List<Transform> spawn = new List<Transform>(spawnPoints);

		Vector3 point = PopRandomSpawnPoint(spawn);

		GameObject tank = Instantiate(playerTankPrefab, point, Quaternion.identity);
		playerTank = tank.GetComponent<TankBase>();

		for (int i = 0; i < enemiesCount; i++)
		{
			point = PopRandomSpawnPoint(spawn);

			tank = Instantiate(enemyTankPrefab, point, Quaternion.identity);
			
			enemyTanks.Add(tank.GetComponent<TankBase>());

			cameraControl.m_Targets[i + 1] = tank.transform;
		}
		cameraControl.m_Targets[0] = tank.transform;

	}

	Vector3 PopRandomSpawnPoint(List<Transform> spawn)
	{
		int rand = Random.Range(0, spawn.Count);	
		
		Transform point = spawn[rand];
		spawn.RemoveAt(rand);

		return point.position;
	}

	void Clear()
	{
		if (playerTank != null)
		{
			Destroy(playerTank.gameObject);
			playerTank = null;
		}

		for (int i = 0; i < enemyTanks.Count; i++)
		{
			if (enemyTanks[i] != null)
			{
				Destroy(enemyTanks[i].gameObject);
			}
		}

		enemyTanks.Clear();

		cameraControl.m_Targets = null;
	}

	private void Update() 
	{
		CheckForEnemiesInSight();
	}

	void CheckForEnemiesInSight()
	{
		for (int i = 0; i < enemiesCount; i++)
		{
			Vector3 pos = enemyTanks[i].transform.position;
			if (playerTank.GetDistanceToTarget(pos) < enemyTanks[i].VisionRange)
			{
				playerTank.EnemyInSight(enemyTanks[i].transform);
				enemyTanks[i].EnemyInSight(playerTank.transform);

				if (playerTank.GetDistanceToTarget(pos) < enemyTanks[i].AttackRange) {
					playerTank.EnemyInAttackRange(enemyTanks[i].transform);
					enemyTanks[i].EnemyInAttackRange(playerTank.transform);
				} else {
					playerTank.EnemyInAttackRange(null);
					enemyTanks[i].EnemyInAttackRange(null);
				}
			}
			else
			{
				playerTank.EnemyInSight(null);
				enemyTanks[i].EnemyInSight(null);

				playerTank.EnemyInAttackRange(null);
				enemyTanks[i].EnemyInAttackRange(null);
			}
		}
	}
}
