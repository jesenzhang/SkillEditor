namespace DirectorEditor
{
    using System;
    using UnityEngine;

    public class SidebarControlEventArgs : EventArgs
    {
        public UnityEngine.Behaviour Behaviour;
        public DirectorEditor.SidebarControl SidebarControl;

        public SidebarControlEventArgs(UnityEngine.Behaviour behaviour, DirectorEditor.SidebarControl control)
        {
            this.Behaviour = behaviour;
            this.SidebarControl = control;
        }
    }
}

