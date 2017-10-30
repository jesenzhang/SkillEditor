using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public struct ContentValue{
	public int IntValue;
	public string StrValue;
	public float FloatValue;
	public Vector3 Vector3Value;
}

public enum SkillState{
	IDLE,
	STARTED,
	STOPED,
	CANCELED,
}

[ExecuteInEditMode]
public class Skill : MonoBehaviour {

	public string skillID;

	public Example.Skill.SkillType skillType;

	public Example.Skill.TargetType[] targetTypes;

	public int castingDistance = 5;  

	public int skillCD = 0;

	public int skillTime = -1;

	public SkillState state = SkillState.IDLE; 

	private int startTime = 0;

	private int cancelTime = 0;

	private List<SkillTimeTween> actions = new List<SkillTimeTween>(); 

	private List<SkillLimitAction> limits = new List<SkillLimitAction>(); 
	 

	public void Simulate(SkillCaller caller,int time){
		switch (state) {
		case SkillState.IDLE:
			StartSkill (caller, time);
			if (!UpdateSkill (caller, time)) {
				StopSkill (caller,time);
			}
			break;
		case SkillState.STARTED:
			if (!UpdateSkill (caller, time)) {
				StopSkill (caller,time);
			}
			break;  
		}
	}

	public void ResetSkill(SkillCaller caller){
		state = SkillState.IDLE;
		actions.Clear (); 
		var skillActions = this.GetComponentsInChildren<SkillTimeTween> ();
		foreach (var action in skillActions) {  
			action.ResetAction (caller, this);
			actions.Add (action);
		}
		limits.Clear ();
	}

	protected void StartSkill(SkillCaller caller,int time){ 
		state = SkillState.STARTED;
		startTime = time;
		OnStart (caller,time);
	}

	protected bool UpdateSkill(SkillCaller caller,int time){
		OnUpdate (caller,time-startTime);
		bool active = false;
		foreach (var action in actions) {  
			if (!action.IsOver) {
				action.Simulate (caller, this, time);
			}
			if (!action.IsOver) {
				active = true;
			}
		} 
		return active;
	}

	protected void StopSkill(SkillCaller caller,int time){
		state = SkillState.STOPED;
		OnStop (caller,time);
		OnClean ();
	}

	public void CancelSkill(SkillCaller caller,int time,int reason){
		state = SkillState.CANCELED;
		cancelTime = time;
		OnCancel (caller,time,reason);
		foreach (var action in actions) {  
			if (!action.IsOver) {
				action.CancelAction (caller, this, time, reason);
			}
		}
		OnClean ();
	}


	protected virtual void OnStart(SkillCaller caller,int time){
		startTime = time;
	}

	protected virtual void OnUpdate(SkillCaller caller,int time){
		
	}

	protected virtual void OnStop(SkillCaller caller,int time){

	}

	protected virtual void OnCancel(SkillCaller caller,int time,int reason){
		 
	}

	protected virtual void OnClean(){ 
		limits.Clear ();
	}

	public void AddLimit(SkillLimitAction limit){
		limits.Add (limit);
	}

	public void RemoveLimit(SkillLimitAction limit){
		limits.Remove (limit);
	}

	public bool IsRunning{
		get { 
			return state == SkillState.STARTED;
		} 
	}

	public bool CanCancel{
		get {  
			return state != SkillState.STARTED || limits.Count == 0;
		} 
	}

	#region Editor Support

	void OnDrawGizmos(){
		//Gizmos.color = Color.gray;
		//Gizmos.DrawWireSphere (transform.position, castingDistance);
	}

	void Update(){
		#if UNITY_EDITOR
		if(!Application.isPlaying){
			OnEditorUpdate ();
		}
		#endif
	}

	void OnEditorUpdate(){
		name = "Skill-" + skillType + "-" + skillID; 
		CaculateSkillTime();
	}

	public void CaculateSkillTime(){ 
		var actions = GetComponentsInChildren<SkillTimeTween> ();
		int skillTime = 0;
		foreach (var action in actions) {   
			skillTime = Mathf.Max (skillTime,action.GetEndTime (GetComponentInParent<SkillCaller>(), this));
		}
		this.skillTime = skillTime;
	}

	#endregion


}
