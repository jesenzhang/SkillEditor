namespace DirectorEditor
{
    using System;
    using UnityEngine;

    public class DirectorBehaviourControlEventArgs : EventArgs
    {
        public UnityEngine.Behaviour Behaviour;
        public DirectorBehaviourControl Control;

        public DirectorBehaviourControlEventArgs(UnityEngine.Behaviour behaviour, DirectorBehaviourControl control)
        {
            this.Behaviour = behaviour;
            this.Control = control;
        }
    }
}

