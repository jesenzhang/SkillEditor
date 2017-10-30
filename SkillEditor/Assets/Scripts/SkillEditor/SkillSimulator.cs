using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillSimulatorState{
	IDLE,
	STARTING,
	STARTED,
	PAUSED
} 
public class SkillSimulator : MonoBehaviour {

	public static int playingTime = 0;

	public SkillSimulatorState state = SkillSimulatorState.IDLE;

	public SkillCaller caller;

	public Skill skill;

	public bool loop = true; 

	public float simulateTimer;

	public float skillTime;
	 

	// Use this for initialization
	void Start () {
		StartSimulate ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case SkillSimulatorState.STARTING:
			StartSimulate (); 
			break;
		case SkillSimulatorState.STARTED:
			Simulate ();
			break;
		}
	}


	void StartSimulate(){
		foreach (var s in GetComponentsInChildren<Skill>()) {
			s.gameObject.SetActive (s==skill);
		}

		state = SkillSimulatorState.STARTED; 
		skill.CaculateSkillTime ();
		this.skillTime = skill.skillTime / 1000.0f;
		simulateTimer = 0;
		playingTime = Mathf.RoundToInt (simulateTimer * 1000); 
		skill.ResetSkill (caller);
		skill.Simulate (caller, playingTime);
	}

	void Simulate(){
		if (simulateTimer > this.skillTime) { 
			if (loop) {
				simulateTimer = 0;
			} else {
				state = SkillSimulatorState.IDLE;
			}
		}
		playingTime = Mathf.RoundToInt (simulateTimer * 1000);
		if (skill.IsRunning) {
			skill.Simulate (caller, playingTime);
		}
		simulateTimer += Time.deltaTime;
	}
}
