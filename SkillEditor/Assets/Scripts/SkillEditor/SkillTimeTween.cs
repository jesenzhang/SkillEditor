using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TweenState{
	IDLE,
	STARTED,
	STOPED,
	CANCELED,
}

[ExecuteInEditMode]
public abstract class SkillTimeTween : MonoBehaviour {
	public int startFrame = 0 ; 

	public int duration = -1; 

	private TweenState state; 


	public virtual void Simulate(SkillCaller caller,Skill skill,int time){

		switch (state) {
		case TweenState.IDLE:
			if (time >= startFrame) {
				StartAction (caller,skill,time);
			}
			break;
		case TweenState.STARTED:
			OnUpdate (caller, skill, time - startFrame);
			if (duration >= 0 && time >= startFrame + duration) {
				StopAction (caller,skill,time);
			}
			break; 
		}
	}

	public void ResetAction(SkillCaller caller,Skill skill){
		state = TweenState.IDLE;
		gameObject.SetActive (false); 
	}

	void StartAction(SkillCaller caller,Skill skill,int time){
		state = TweenState.STARTED;
		gameObject.SetActive (true); 
		OnStart (caller,skill,time - startFrame);
	}

	void StopAction(SkillCaller caller,Skill skill,int time){
		state = TweenState.STOPED;
		gameObject.SetActive (false); 
		OnStop (caller, skill, time - startFrame);
		OnClean (caller,skill);
	}

	public void CancelAction(SkillCaller caller,Skill skill,int time,int reason){
		state = TweenState.CANCELED;
		gameObject.SetActive (false); 
		OnCancel (caller, skill, time, reason);
		OnClean (caller,skill);
	}

	protected virtual void OnStart(SkillCaller caller,Skill skill,int time){

	}

	protected virtual void OnStop(SkillCaller caller,Skill skill,int time){

	}

	protected virtual void OnUpdate(SkillCaller caller,Skill skill,int time){
	}

	protected virtual void OnCancel(SkillCaller caller,Skill skill,int time,int reason){
	}

	protected virtual void OnClean(SkillCaller caller,Skill skill){
		gameObject.SetActive (false); 
	}

	public bool IsRunning{
		get { 
			return state == TweenState.STARTED;
		} 
	}

	public bool IsOver{
		get { 
			return state == TweenState.CANCELED || state == TweenState.STOPED;
		} 
	}

	#region Editor Support

	void Update(){
		#if UNITY_EDITOR
		OnEditorUpdate ();
		#endif
	}

	protected virtual void OnEditorUpdate(){ 
		name = displayName;  
	}	 

	public virtual int GetEndTime(SkillCaller caller,Skill skill){		 
		if (duration <= 0)
			return startFrame;

		return startFrame + duration; 
	}

	public virtual string displayName{
		get { 
			//return string.Format ("{0}-{1}", startFrame, frameName);
			return name;
		}
	}

	#endregion

}
