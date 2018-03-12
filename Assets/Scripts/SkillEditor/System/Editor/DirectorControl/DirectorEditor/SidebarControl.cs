namespace DirectorEditor
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    public abstract class SidebarControl : DirectorBehaviourControl, IComparable
    {
        public int expandedSize = 2;
        public bool isExpanded;

        [field: CompilerGenerated]
        public event SidebarControlHandler DuplicateRequest;

        [field: CompilerGenerated]
        public event SidebarControlHandler SelectRequest;

        protected SidebarControl()
        {
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            SidebarControl control = obj as SidebarControl;
            if (control == null)
            {
                throw new ArgumentException("Comparison object is not of type SidebarControl.");
            }
            int num = 0;
            int num2 = Math.Min(this.Ordinal.Length, control.Ordinal.Length);
            for (int i = 0; i < num2; i++)
            {
                num = this.Ordinal[i] - control.Ordinal[i];
                if (num != 0)
                {
                    return num;
                }
            }
            return (this.Ordinal.Length - control.Ordinal.Length);
        }

        public void RequestDuplicate()
        {
            if (this.DuplicateRequest != null)
            {
                this.DuplicateRequest(this, new SidebarControlEventArgs(base.Behaviour, this));
            }
        }

        public void RequestSelect()
        {
            if (this.SelectRequest != null)
            {
                this.SelectRequest(this, new SidebarControlEventArgs(base.Behaviour, this));
            }
        }

        public void RequestSelect(SidebarControlEventArgs args)
        {
            if (this.SelectRequest != null)
            {
                this.SelectRequest(this, args);
            }
        }

        internal void Select()
        {
            GameObject[] objArray = Selection.gameObjects;
            ArrayUtility.Add<GameObject>(ref objArray, base.Behaviour.gameObject);
            Selection.objects=(objArray);
        }

        internal void SetExpandedFromEditorPrefs()
        {
            string isExpandedKey = this.IsExpandedKey;
            if (EditorPrefs.HasKey(isExpandedKey))
            {
                this.isExpanded = EditorPrefs.GetBool(isExpandedKey);
            }
            else
            {
                EditorPrefs.SetBool(isExpandedKey, this.isExpanded);
            }
        }

        internal string IsExpandedKey
        {
            get
            {
                SerializedObject obj2 = new SerializedObject(base.Behaviour);
                typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj2, (InspectorMode) 1, null);
                SerializedProperty property = obj2.FindProperty("m_LocalIdentfierInFile");
                return string.Format("Director.{0}.{1}", property.intValue, "isExpanded");
            }
        }

        public int[] Ordinal { get; set; }
    }
}

