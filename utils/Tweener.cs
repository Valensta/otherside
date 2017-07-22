using UnityEngine;
using System.Collections;

public class TweenerRetired : MonoBehaviour {
/*
	public GameObject mesh;
//	public GameObject tweenobj;
	Hashtable inArgs = new Hashtable();
	Hashtable outArgs = new Hashtable ();
	Hashtable colors = new Hashtable ();
	public float isActive;
	public float duration = 1.2f;
	public string target_material = null;
	Color InactiveColor = new Color (0.2f, 0.2f, 0.2f, 0.2f);
	Color OriginalColor = Color.magenta;
	public TweenType type;
	public TweenColor flavor;
	// Use this for initialization
	public enum TweenColor{Force, Scale, Speed, Collided, Select, Hint, Disabled};
	public enum TweenType{ColorLoop, ColorOnce, ColorFade, MakeSmall, Null, ColorChange};

	public void Awake(){
	//	Debug.Log ("Starting tweener " + this.name);
		Init ();
	}

	public void SetTargetMaterial(string s){
		target_material = s;
	}

	public void InitUITexOriginalColor(){
		UITexture tex = this.GetComponentInChildren<UITexture> ();
		
		if (tex) 				
			OriginalColor = tex.material.GetColor ("_TintColor");
		}

	public void Init () {
	//	mesh = m;
	//	tweenobj = me;
	//	Debug.Log ("tweener Init for " + this.name);
		isActive = 0.0f;
		colors.Add (TweenColor.Force.ToString(),Get.EffectColor (EffectType.Force));
		colors.Add (TweenColor.Scale.ToString(),Get.EffectColor (EffectType.Scale));
		colors.Add (TweenColor.Collided.ToString(), new Color (27f / 255f, 78f / 255f, 41f / 105f, 1f));
		colors.Add (TweenColor.Speed.ToString(),Get.EffectColor (EffectType.Speed));
		colors.Add (TweenColor.Select.ToString(),Color.cyan);
		colors.Add (TweenColor.Hint.ToString(),new Color(1f, 0f, 0f, 0.2f));
		colors.Add (TweenColor.Disabled.ToString(),Color.gray);


	
			UITexture tex;
	//	Debug.Log ("this is " + this.name);
		tex = this.GetComponent<UITexture> ();
		if (tex == null)tex = this.GetComponentInChildren<UITexture> ();

		if (tex) {
						target_material = tex.material.name;
						if (tex.material.GetColor ("_TintColor") == null)
								Debug.Log (this.transform.parent.name + " UITexture " + tex.material.name + " does not have _TintColor\n");		
					//	Debug.Log ("Try me smartass " + tex.material.name + " " + tex.material.GetColor ("_TintColor"));
						//OriginalColor = tex.material.GetColor ("_TintColor");
						OriginalColor = Color.white;
				} else {

						
						Material[] mats;

						Material m = new Material (Shader.Find ("Particles/Additive"));
						Color hi = new Color ();
						hi = m.GetColor ("_TintColor");
						hi.a = 0f;
						m.SetColor ("_TintColor", hi);		
						m.name = "Tweener";
						if (mesh != null){
							if (mesh.GetComponent<MeshRenderer>()){
								MeshRenderer mr = mesh.GetComponent<MeshRenderer> ();							
								mats	 = new Material[mr.materials.GetLength (0) + 1];
								System.Array.Copy (mr.materials, mats, mr.materials.GetLength (0));									
								mats [mr.materials.GetLength (0)] = m;
								mesh.GetComponent<MeshRenderer> ().materials = mats;
							}else {
								SkinnedMeshRenderer smr = mesh.GetComponent<SkinnedMeshRenderer>();
								if (smr != null){
									mats = new Material[smr.materials.GetLength (0) + 1];
									System.Array.Copy (smr.materials, mats, smr.materials.GetLength (0));									
									mats [smr.materials.GetLength (0)] = m;
									mesh.GetComponent<SkinnedMeshRenderer> ().materials = mats;
								}
							}
						}
		
								
								target_material = m.name + " (Instance)"; //don't ask
								OriginalColor = hi;
			
						//}
						
						

				}
		colors.Add ("original",OriginalColor);
	//	outArgs.Add("target_material", target_material);
	//	inArgs.Add("target_material",target_material);

	}


	IEnumerator Tween(){
	//	Debug.Log ("About to tween type: " + type + " mesh: " + mesh.name + " target: " + target_material);
		switch (type) {
				case TweenType.MakeSmall:
						iTween.ScaleTo (mesh, iTween.Hash ("x", mesh.transform.localScale.x / 2f, 
			                                     "y", mesh.transform.localScale.y / 2f, 
			                                     "time", duration,
			                                     "easetype", iTween.EaseType.easeInQuart));
						break;
				case TweenType.ColorFade:
			//	Debug.Log("Tweening pulse\n");
						iTween.FadeTo (mesh, iTween.Hash ("alpha", 0.2f,
			                                  "time", duration,			                                  
			                                   "looptype", "pingPong",
			                                   "NamedColorValue", "_TintColor"));
						break;
				case TweenType.ColorOnce:
						iTween.ColorTo (mesh, iTween.Hash("time", duration,
			                                  			  "easetype", "easeInQuad",
			                                  			  "NamedColorValue","_TintColor",
			                                  			  "color", inArgs["color"],			
			                                  			  "target_material", target_material));
		
						yield return new WaitForSeconds ((float)duration);	
						iTween.ColorTo (mesh, iTween.Hash("color",OriginalColor,
									    				  "time",duration,
												          "easetype","easeInQuad",
														  "NamedColorValue","_TintColor",
			                                 			  "target_material", target_material));
						yield return null;
						break;
			    case TweenType.ColorLoop:
			//			Debug.Log("duration is " + duration + " mesh name is " + mesh.name + " color is " + inArgs ["color"]);
						iTween.ColorTo (mesh, iTween.Hash ("color", inArgs ["color"],
			                                      "time", duration,			                                  
			                                      "looptype", "pingPong",
			                                   "easetype", iTween.EaseType.easeInQuad,
			                                      "name", mesh.name,
			                                   "NamedColorValue","_TintColor",
												  "target_material", target_material
			                                      ));
						break;
				case TweenType.ColorChange:		

					iTween.ColorTo (mesh, iTween.Hash ("color", inArgs ["color"],
			                                 //  "time", duration,			                                  
			                                  // "looptype", iTween.LoopType.none,
			                                   "name", mesh.name,
			                                   "NamedColorValue","_TintColor",
			                                   "target_material", target_material
			                                   ));
					break;
				}
				
	}

	public void TweenMe(TweenType newtype, string color){
		type = newtype;
		TweenMe (color);
	}

	 void TweenMe(string  color){

	//	Debug.Log ("Want to tween " + color + " " + type.ToString());
		if (type.ToString().Contains("Color")) {
		//	Debug.Log("Setting color\n");
			if (colors.Contains (color)){
			//	Debug.Log("Found color " + colors [color] + " for " + color);
				inArgs ["color"] = colors [color];
			}
			else{
				inArgs ["color"] = InactiveColor;
			//	Debug.Log(this.name  + " did not find color, setting to inactive\n");
			}
		}


		if (type == TweenType.ColorFade) {		
			duration = 1.2f;
		}
		if (type == TweenType.ColorLoop) {		
			duration = 1.2f;
		}
		if (color.ToLower() == "hint") {
		//	Debug.Log("Set duration for hint\n");
			duration = 0.5f;
		}
		if (type == TweenType.ColorOnce) {
			//	Debug.Log("Set duration for hint\n");
			duration = 0.4f;
		}
		if (mesh) {
//			Debug.Log ("want to tween " + type + " and got color " + inArgs ["color"]);
		//	Debug.Log("Coroutine Tweening");
			StartCoroutine(Tween());
		}
		isActive = duration;
	}

	public void StopMe(){
		isActive = 0f;
			iTween.Stop (mesh, true);

	//	Debug.Log ("Original color is " + OriginalColor);
			iTween.ColorTo (mesh, iTween.Hash ("color", OriginalColor,
			                                  "time", 0.3f,			                                  
				                              "name", mesh + "_stop",
			                                   "looptype",iTween.LoopType.none,
		                                 		  "target_material", target_material,
			                                   "NamedColorValue","_TintColor"
				                             ));

	}
		



	// Update is called once per frame
	void Update () {
		if (isActive > 0.05f) {
						isActive = - 0.05f;
				}
	}

*/
}
