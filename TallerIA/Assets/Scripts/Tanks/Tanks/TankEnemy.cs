using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : TankBase
{

    public enum States{
        Idle=0,
        Chase,
        Attack,
        Agony,
        Count
    }

    public enum Events{
        InSight=0,
        OutOfSight,
        InAttackRange,
        OutOfAttackRange,
        OnLowEnergy,
        Count
    }
    private FSM fsm;
    private Vector3 enemyPosition;
    // ===========================================================
    // Inicialización
    // ===========================================================
    private void Start() 
    {
        fsm=new FSM((int) States.Count, (int) Events.Count ,(int) States.Idle);

        fsm.SetRelacion((int) States.Idle,      (int) Events.InSight,           (int) States.Chase);
        fsm.SetRelacion((int) States.Chase,     (int) Events.InAttackRange,     (int) States.Attack);
        fsm.SetRelacion((int) States.Chase,     (int) Events.OutOfSight,        (int) States.Idle);
        fsm.SetRelacion((int) States.Attack,    (int) Events.OutOfAttackRange,  (int) States.Chase);
        fsm.SetRelacion((int) States.Attack,    (int) Events.OnLowEnergy ,      (int) States.Agony);
    }

    // ===========================================================
    // Métodos virtuales
    // ===========================================================
    override protected void OnUpdate()
    {
        Debug.Log((States)fsm.GetState());
        if(this.Health < 40.0f)
            OnLowEnergy();
        switch(fsm.GetState()){
            case (int)States.Idle:
                Idle();
            break;
            case (int)States.Chase:
                Chase();
            break;   
            case (int)States.Attack:
                Attack();
            break;
            case (int) States.Agony:
                Agony();
            break;
        }
    }

    private void Idle(){
    }

    private void Chase(){
        this.LookTarget(enemyPosition);
        MoveForward();
    }

    private void Attack(){
        LookTarget(enemyPosition);
        if(IsLookingToTarget(enemyPosition)){
            Fire();
        }else{
            MoveForward();
        }
    }

    private void Agony(){
        LookTarget(enemyPosition * -1);
        MoveForward();
    }

    // ===========================================================
    // Eventos
    // ===========================================================

    override protected void OnEnemyInSight(Vector3 enemyPosition)
    {
        this.enemyPosition = enemyPosition;
        fsm.SendEvent((int) Events.InSight);
	}

	override protected void OnEnemyOutOfSight(Vector3 lastPosition)
	{
        fsm.SendEvent((int) Events.OutOfSight);
	}

	override protected void OnEnemyInAttackRange(Vector3 enemyPosition)
	{
        this.enemyPosition = enemyPosition;
        fsm.SendEvent((int) Events.InAttackRange);
	}

	override protected void OnEnemyOutOfAttackRange(Vector3 lastPosition)
	{
        fsm.SendEvent((int) Events.OutOfAttackRange);
	}

    private void OnLowEnergy(){
        fsm.SendEvent((int) Events.OnLowEnergy);
    }
}