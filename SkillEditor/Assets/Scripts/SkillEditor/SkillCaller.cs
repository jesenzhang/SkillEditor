using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillCaller : SkillLife {

	public Animator animator;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	#if UNITY_EDITOR
		transform.position = animator.transform.position;
		transform.rotation = animator.transform.rotation;
		transform.localScale = animator.transform.localScale;
	#endif
	}

	public string _animationName;

	public void PlayAction(string action,int transitionDuration){  
		_animationName = action;
		if (transitionDuration <=1) {
			animator.Play (action);
		} else {
			animator.CrossFade (action, transitionDuration/1000.0f);
		}
	}

	public void PlayEffect(GameObject effect,string boneName){
	}

	public override bool IsSelf{
		get { 
			return true;
		}
	}
}
