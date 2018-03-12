
using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using TimeLine;

/// <summary>
/// Actor Curve Track Control
/// </summary>
[CutsceneTrackAttribute(typeof(SkillTrack))]
public class SkillTrackControl : GenericTrackControl
{
    private int controlSelection;

    protected int MINIMUM_ROWS_SHOWING = 3;
    protected int rowsShowing = 5;
    protected SkillTrack track;
    protected SkillTrack Track {
        get {
            if (track == null)
                return this.Behaviour.gameObject.GetComponent<SkillTrack>();
            else
                return track;
        }
    }

   
    public override void calculateHeight()
    {
        int maxlevel = 1;
        TimelineItem[] items = Track.TimelineItems;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] is SkillParticleEmitterItem)
            {
                SkillParticleEmitterItem item = (SkillParticleEmitterItem)items[i];
                maxlevel = Mathf.Max(maxlevel, item.GetLevel());
            }
        }
        rowsShowing = maxlevel;
        if (base.isExpanded)
        {
            this.trackArea.height = (34f * rowsShowing);
        }
        else
        {
            this.trackArea.height = (17f);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        isExpanded = true;
    }
 
    private void pasteItem(object userData)
    {
        PasteContext data = userData as PasteContext;
        if (data != null)
        {
            float firetime = (data.mousePosition.x - state.Translation.x) / state.Scale.x;
            GameObject clone = DirectorCopyPaste.Paste(data.track.transform);

            ItemClipCurve clipCurve = clone.GetComponent<ItemClipCurve>();
            clipCurve.TranslateCurves(firetime - clipCurve.Firetime);

            Undo.RegisterCreatedObjectUndo(clone, "Pasted " + clone.name);
        }
    }

    private class PasteContext
    {
        public Vector2 mousePosition;
        public SkillTrack track;

        public PasteContext(Vector2 mousePosition, SkillTrack track)
        {
            this.mousePosition = mousePosition;
            this.track = track;
        }
    }
}
