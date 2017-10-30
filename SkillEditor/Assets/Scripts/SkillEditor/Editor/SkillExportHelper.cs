using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SkillExportHelper  {

	public static Dictionary<string,int> skillIDS = new Dictionary<string, int> ();

    public static void ExportTimelineSkill(TimeLine.TimelineManager skill)
    {
        Skill sss = skill.gameObject.GetComponent<Skill>();
        if(sss!=null)
            ExportTimelineSkill(skill, "Export/Skills/Skill_" + sss.skillID + ".bytes");

    }

    public static void ExportSkill(Skill skill){
		ExportSkill (skill, "Export/Skills/Skill_" + skill.skillID + ".bytes");
	}


    public static void ExportTimelineSkill(TimeLine.TimelineManager skill, string path)
    {
        Example.Skill skillData = new Example.Skill();
        List<Example.SkillAction> skillActions = new List<Example.SkillAction>();
        List<Example.SkillParticleEmitter> particleEmitters = new List<Example.SkillParticleEmitter>();
        List<Example.SkillEvent> skillEvents = new List<Example.SkillEvent>();

        TimeLine.TrackGroup[] groups = skill.GetTrackGroups();
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] is SkillGroup)
            {
                SkillGroup skg = (SkillGroup)groups[i];
                Transform Actor = skg.Actor;
                TimeLine.TimelineTrack[] tracks = skg.GetTracks();
                for (int j= 0; j < tracks.Length; j++)
                {
                    if (tracks[j] is SkillTrack)
                    {
                        SkillTrack skt = (SkillTrack)tracks[j];
                        TimeLine.ActorAction[] acts = skt.ActorActions;
                        for (int k = 0; k < acts.Length; k++)
                        {
                            if (acts[k] is SkillParticleEmitterItem)
                            {
                                ExportParticleEmitter((SkillParticleEmitterItem)acts[k], particleEmitters);
                            }
                            else if (acts[k] is SkillActionItem)
                            {
                                ExportSkillAction((SkillActionItem)acts[k], skillActions);
                            }
                        }
                        TimeLine.ActorEvent[] events = skt.ActorEvents;
                        for (int k = 0; k < events.Length; k++)
                        {
                            if (events[k] is SkillEventItem)
                            {
                                ExportSkillAction((SkillActionItem)acts[k], skillActions);
                            }
                        }
                    }
                }
            }
        }

        Skill skilll = skill.gameObject.GetComponent<Skill>();
        if (skilll == null)
        {
            return;
        }
        int targetMask = 0;
        foreach (var mask in skilll.targetTypes)
        {
            targetMask |= 1 << (int)mask;
        }

        skillData.Id = ConvertSkillID(skilll.skillID);
        skillData.skillType = skilll.skillType;
        skillData.TargetMask = targetMask;
        skillData.SkillCD = skilll.skillCD;
        skillData.SkillTime = Mathf.FloorToInt(skill.Duration*1000);
        skillData.CastingDistance = skilll.castingDistance;
        skillData.Actions = skillActions;
        skillData.Emitters = particleEmitters;
        skillData.Events = skillEvents;

        var data = Example.Skill.SerializeToBytes(skillData);
        File.WriteAllBytes(path, data);

        Debug.LogFormat("Export Skill {0} to {1}", skill.name, path);
    }


    public static void ExportSkill(Skill skill,string path){
		Example.Skill skillData = new Example.Skill();


		List<Example.SkillAction> skillActions = new List<Example.SkillAction> ();
		List<Example.SkillParticleEmitter> particleEmitters = new List<Example.SkillParticleEmitter> ();
		List<Example.SkillEvent> skillEvents = new List<Example.SkillEvent> ();


		var actions = SkillManager.GetSkillActionRoot(skill).GetComponentsInChildren<SkillAction> ();
		foreach (var action in actions) {
			ExportSkillAction (action,skillActions); 
		}

		var emitters = skill.GetComponentsInChildren<SkillParticleEmitter> ();
		foreach (var emitter in emitters) {
			ExportParticleEmitter (emitter,particleEmitters); 
		} 

		var events = skill.GetComponentsInChildren<SkillEvent> ();
		foreach (var skillEvent in events) {
			ExportSkillEvent (skillEvent,skillEvents); 
		} 


		//use uint better
		int targetMask = 0;
		foreach (var mask in skill.targetTypes) {
			targetMask |= 1 << (int)mask;
		}

		skillData.Id = ConvertSkillID(skill.skillID);
		skillData.skillType = skill.skillType;
		skillData.TargetMask = targetMask;
		skillData.SkillCD = skill.skillCD;
		skillData.SkillTime = skill.skillTime;
		skillData.CastingDistance = skill.castingDistance; 
		skillData.Actions = skillActions;
		skillData.Emitters = particleEmitters; 
		skillData.Events = skillEvents;

		var data = Example.Skill.SerializeToBytes (skillData);
		File.WriteAllBytes (path, data);

		Debug.LogFormat ("Export Skill {0} to {1}", skill.name, path);
	}
    private static Example.SkillParticle ExportSkillParticle(SkillParticleActionItem particle, List<Example.SkillParticle> particles)
    {
        Example.SkillParticle skillParticle = new Example.SkillParticle();
        List<Example.SkillAction> skillActions = new List<Example.SkillAction>();

        var actions = particle.GetComponentsInChildren<SkillActionItem>();
        foreach (var action in actions)
        {
            ExportSkillAction(action, skillActions);
        }

        Example.SkillPath skillPath = new Example.SkillPath();
        skillPath.pathType = particle.path.pathType;
        skillPath.Args = ArrayToList(particle.path.arguments);

        Example.SkillShapeNew skillShape = ExportSkillShape(particle.hitShape);

        skillParticle.Id = particles.Count;
        skillParticle.StartTime = Mathf.FloorToInt(particle.Firetime * 1000);
        skillParticle.Duration = Mathf.FloorToInt(particle.Duration * 1000);
        skillParticle.Effect = particle.effectName;
        skillParticle.HitShape = skillShape;
        skillParticle.Path = skillPath;
        skillParticle.Actions = skillActions;
        particles.Add(skillParticle);
        return skillParticle;
    }

    private static Example.SkillParticle ExportSkillParticle(SkillParticle particle,List<Example.SkillParticle> particles){
		Example.SkillParticle skillParticle = new Example.SkillParticle ();
		List<Example.SkillAction> skillActions = new List<Example.SkillAction> ();

		var actions = particle.GetComponentsInChildren<SkillAction> ();
		foreach (var action in actions) {
			ExportSkillAction (action,skillActions);
		}

		Example.SkillPath skillPath = new Example.SkillPath ();
		skillPath.pathType = particle.path.pathType;
		skillPath.Args = ArrayToList(particle.path.arguments);

		Example.SkillShapeNew skillShape = ExportSkillShape (particle.hitShape);

		skillParticle.Id = particles.Count;
		skillParticle.StartTime = particle.startFrame;
		skillParticle.Duration = particle.duration;
		skillParticle.Effect = particle.effectName;
		skillParticle.HitShape = skillShape;
		skillParticle.Path = skillPath; 
		skillParticle.Actions = skillActions;
		particles.Add (skillParticle);
		return skillParticle;
	}
    private static Example.SkillParticleEmitter ExportParticleEmitter(SkillParticleEmitterItem emitter, List<Example.SkillParticleEmitter> emitters)
    {
        Example.SkillParticleEmitter particleEmitter = new Example.SkillParticleEmitter();
        List<Example.SkillParticle> skillParticles = new List<Example.SkillParticle>();
        var particles = emitter.GetComponentsInChildren<SkillParticleActionItem>();
        foreach (var particle in particles)
        {
            ExportSkillParticle(particle, skillParticles);
        }
        if (emitter is AutoSkillParticleEmitterItem)
        {
            AutoSkillParticleEmitterItem autoEmitter = (AutoSkillParticleEmitterItem)emitter;
            particleEmitter.emitterType = autoEmitter.emitterType;
            particleEmitter.StartTime = Mathf.FloorToInt(autoEmitter.Firetime * 1000);
            particleEmitter.Duration = Mathf.FloorToInt(autoEmitter.Duration * 1000);
            particleEmitter.EmitterShape = ExportSkillShape(autoEmitter.emitterShape);
            particleEmitter.EmitterPosition = MathUtil.ToVector3f(autoEmitter.emitterOffset);
            particleEmitter.Template = skillParticles.Count > 0 ? skillParticles[0] : null;
            particleEmitter.Particles = skillParticles;
        }else if(emitter is ManualSkillParticleEmitterItem)
        {
             ManualSkillParticleEmitterItem autoEmitter = (ManualSkillParticleEmitterItem)emitter;
            particleEmitter.emitterType = autoEmitter.emitterType;
            particleEmitter.StartTime = Mathf.FloorToInt(autoEmitter.Firetime * 1000);
            particleEmitter.Duration = Mathf.FloorToInt(autoEmitter.Duration * 1000);
            particleEmitter.Template = skillParticles.Count > 0 ? skillParticles[0] : null;
            particleEmitter.Particles = skillParticles;
        }
        emitters.Add(particleEmitter);
        return particleEmitter;
    }

    private static Example.SkillParticleEmitter ExportParticleEmitter(SkillParticleEmitter emitter,List<Example.SkillParticleEmitter> emitters){
		Example.SkillParticleEmitter particleEmitter = new Example.SkillParticleEmitter ();


		List<Example.SkillParticle> skillParticles = new List<Example.SkillParticle> ();
		var particles = emitter.GetComponentsInChildren<SkillParticle> ();
		foreach (var particle in particles) {
			ExportSkillParticle (particle,skillParticles);
		} 

		particleEmitter.emitterType = emitter.emitterType;
		particleEmitter.StartTime = emitter.startFrame;
		particleEmitter.Duration = emitter.duration; 
		particleEmitter.EmitterShape = ExportSkillShape (emitter.EmitterShape);
		particleEmitter.EmitterPosition = MathUtil.ToVector3f (emitter.EmitterPosition);
		particleEmitter.Template = skillParticles.Count>0?skillParticles [0]:null;
		particleEmitter.Particles = skillParticles;

		emitters.Add (particleEmitter);
		return particleEmitter;
	}
    private static Example.SkillEvent ExportSkillEvent(SkillEventItem evt, List<Example.SkillEvent> events)
    {
        Example.SkillEvent skillEvent = new Example.SkillEvent();

        List<Example.SkillAction> skillActions = new List<Example.SkillAction>();

        var actions = evt.GetComponentsInChildren<SkillAction>();
        foreach (var action in actions)
        {
            ExportSkillAction(action, skillActions);
        }
        skillEvent.Id = events.Count;
        skillEvent.eventType = evt.eventType;
        skillEvent.Conditions = ArrayToList(evt.conditions);
        skillEvent.Actions = skillActions;
        events.Add(skillEvent);
        return skillEvent;
    }
    private static Example.SkillEvent ExportSkillEvent(SkillEvent evt,List<Example.SkillEvent> events){
		Example.SkillEvent skillEvent = new Example.SkillEvent ();

		List<Example.SkillAction> skillActions = new List<Example.SkillAction> ();

		var actions = evt.GetComponentsInChildren<SkillAction> ();
		foreach (var action in actions) {
			ExportSkillAction (action,skillActions);
		}
		skillEvent.Id = events.Count;
		skillEvent.eventType = evt.eventType; 
		skillEvent.Conditions = ArrayToList(evt.conditions);
		skillEvent.Actions = skillActions;
		events.Add (skillEvent);
		return skillEvent;
	}
    private static Example.SkillAction ExportSkillAction(SkillActionItem action, List<Example.SkillAction> actions)
    {
        Example.SkillAction skillAction = new Example.SkillAction();
        skillAction.actionType = action.actionType;
        skillAction.StartTime = Mathf.FloorToInt(action.Firetime*1000);
        skillAction.Duration = Mathf.FloorToInt(action.Duration * 1000);
        skillAction.Args = ArrayToList(action.arguments);
        actions.Add(skillAction);
        return skillAction;
    }
    private static Example.SkillAction ExportSkillAction(SkillAction action,List<Example.SkillAction> actions){
		Example.SkillAction skillAction = new Example.SkillAction ();
		skillAction.actionType = action.actionType;
		skillAction.StartTime = action.startFrame;
		skillAction.Duration = action.duration;
		skillAction.Args = ArrayToList(action.arguments);
		actions.Add (skillAction);
		return skillAction;
	}

	private static Example.SkillShapeNew ExportSkillShape(SkillShape shape){
		if (shape == null)
			return null;
		
		Example.SkillShapeNew skillShape = new Example.SkillShapeNew ();
		skillShape.shapeType = shape.shapeType;
		skillShape.Width = shape.Width;
		skillShape.Height = shape.Height;
		skillShape.Radius = shape.Radius;
		skillShape.Angle = shape.Angle;
		return skillShape;
	}

	private static int ConvertSkillID(string strID){
		if (skillIDS.ContainsKey (strID)) {
			return skillIDS [strID];
		}
		return 0;
	}

	private static List<Example.ContentValue> ArrayToList(ContentValue[] array){
		List<Example.ContentValue> list = new List<Example.ContentValue> ();
		foreach (var element in array) {
			Example.ContentValue value = new Example.ContentValue ();
			value.IntValue = element.IntValue;
			value.FloatValue = element.FloatValue;
			value.StrValue = element.StrValue;
			value.Vector3Value = MathUtil.ToVector3f (element.Vector3Value);
			list.Add(value);
		}
		return list;
	}
}
