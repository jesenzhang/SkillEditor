
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;

[TrackGroupAttribute("Skill Event Group", TimelineTrackGenre.SkillTrack)]
public class SkillEventGroup : ActorTrackGroup
{

    public List<SkillActionItem> Actions;

    public ContentValue[] conditions
    {
        get
        {
            ContentValue[] args = new ContentValue[0];
            return args;
        }
    }

    public Example.SkillEvent.EventType eventType
    {
        get
        {
            return Example.SkillEvent.EventType.SKILL_OVER;
        }
    }

    public void Trigger(GameObject Actor)
    {

    }

}
