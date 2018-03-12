using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil  { 
	
	public static Vector3 Vector3fToVector3(Example.Vector3f v)
	{
		Vector3 temp = new Vector3();
		temp.x = v.X;
		temp.y = v.Y;
		temp.z = v.Z;

		return temp;
	}

	public static Example.Vector3f Vector3ToVector3f(Vector3 v)
	{
		Example.Vector3f temp = new Example.Vector3f();
		temp.X = v.x;
		temp.Y = v.y;
		temp.Z = v.z;

		return temp;
	}

	public static Example.Vector3f ToVector3f(Vector3 input)
	{
		var output = new Example.Vector3f();
		output.X = input.x;
		output.Y = input.y;
		output.Z = input.z;
		return output;
	} 
}
