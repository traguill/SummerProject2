using UnityEngine;
using System.Collections;

public class offsetAnim : MonoBehaviour {

	private Vector2 offset = new Vector2 (0,0);

	void Start () {
		this.gameObject.GetComponent<MeshRenderer> ().material.SetTextureOffset ("_DetailAlbedoMap",offset);
		this.gameObject.GetComponent<MeshRenderer> ().material.SetTextureOffset ("_MainTex",offset);
	}
	
	void Update () {
		offset.x = offset.x+.0001f;
		offset.y = offset.y+.0003f;

		this.gameObject.GetComponent<MeshRenderer> ().material.SetTextureOffset ("_DetailAlbedoMap",offset);
		this.gameObject.GetComponent<MeshRenderer> ().material.SetTextureOffset ("_MainTex",-offset*.5f);

	    Color c = this.gameObject.GetComponent<MeshRenderer> ().material.GetColor ("_EmissionColor");
		this.gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_EmissionColor",Color.white * Mathf.LinearToGammaSpace((Mathf.PingPong(Time.time,3f)/110)+.015f));
		}
}
