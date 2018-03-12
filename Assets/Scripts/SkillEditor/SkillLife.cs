using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillLife : MonoBehaviour { 
	public virtual bool IsSelf{
		get { 
			return false;
		}
	}
}
