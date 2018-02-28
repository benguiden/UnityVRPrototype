using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Unity_Editor
using UnityEditor;
#endif



	[CreateAssetMenu(fileName = "Path Database", menuName = "Database/Path", order = 0)]
	public class PathDatabase : ScriptableObject {

		public List<AIPath> paths;

	}
		
	[System.Serializable]
	public class AIPath{
        public string name;
        public Color color;
		public List<Vector3> points;

		public AIPath(){
            name = "New Path";
			points = new List<Vector3> ();
            color = Color.white;
		}
	}

#region Enums
    public enum PathReference{
        Index,
        Name
    }
#endregion




