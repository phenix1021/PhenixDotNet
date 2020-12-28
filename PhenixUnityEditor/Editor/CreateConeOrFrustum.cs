using UnityEngine;
using UnityEditor;
using Phenix.Unity.Utilities;
 
// an Editor method to create a cone primitive (so far no end caps)
// the top center is placed at (0/0/0)
// the bottom center is placed at (0/0/length)
// if either one of the radii is 0, the result will be a cone, otherwise a truncated cone
// note you will get inevitable breaks in the smooth shading at cone tips
// note the resulting mesh will be created as an asset in Assets/Editor
// Author: Wolfram Kresse
public class CreateConeOrFrustum : ScriptableWizard
{ 
	public int numVertices = 10;
	public float radiusTop = 0f;
	public float radiusBottom = 1f;
	public float length = 1f;
	public float openingAngle = 0f; // if >0, create a cone with this angle by setting radiusTop to 0, and adjust radiusBottom according to length;
	public bool outside = true;
	public bool inside = false;
	public bool addCollider = false;
 
    [MenuItem ("GameObject/3D Object/Phenix/Cone or Frustum")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Cone/Frustum", typeof(CreateConeOrFrustum));
    }
 
	void OnWizardCreate()
    {
		GameObject shape = new GameObject();
		if( openingAngle > 0 && openingAngle < 180 )
        {
			radiusTop = 0;
			radiusBottom = length * Mathf.Tan(openingAngle * Mathf.Deg2Rad / 2);
		}

        Mesh mesh;
        /*string meshName = shape.name + numVertices + "v" + radiusTop + "t" + radiusBottom + "b" + length + "l" + length + 
            (outside ? "o" : "") + (inside ? "i" : "");
		string meshPrefabPath = "Assets/Editor/" + meshName + ".asset";
		mesh = (Mesh)AssetDatabase.LoadAssetAtPath(meshPrefabPath, typeof(Mesh));*/

        //if(mesh==null)
        //{
            mesh = MeshTools.MakeConeOrFrustum(numVertices, radiusTop, radiusBottom, length, openingAngle, outside, inside);			
			//AssetDatabase.CreateAsset(mesh, meshPrefabPath);
			//AssetDatabase.SaveAssets();
		//}
 
		MeshFilter mf = shape.AddComponent<MeshFilter>();
		mf.mesh = mesh;
 
		shape.AddComponent<MeshRenderer>();
 
		/*if(addCollider)
        {
			MeshCollider mc=newCone.AddComponent<MeshCollider>();
			mc.sharedMesh=mf.sharedMesh;
		}*/
 
        Selection.activeObject = shape;
	}
}
