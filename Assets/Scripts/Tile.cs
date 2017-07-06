using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public int id;
	public int idText;
	public Vector2 pos;//posation
	public bool type = true;
	public bool isActived = false;
	public float x, y;
	public Texture  textA,textB;
	public GameObject maskLayer;
	//public AudioSource music;
	//public float musicVolum ;  
	public void init(int _idText){
		idText = _idText;
		Vector2 offset = textOffset (_idText);
		this.GetComponent <Renderer> ().material.SetTextureOffset ("_MainTex", offset);
		this.GetComponent <Renderer> ().material.SetTextureScale ("_MainTex", new Vector2 (0.2f, 0.1f));
	}
	public void setTileTexture(int i){
		if (i == 0){
			this.GetComponent<Renderer> ().material.mainTexture = textA;
			maskLayer.transform.localScale = Vector3.zero;
			isActived = false;

		}
		if(i == 1){
			this.GetComponent<Renderer> ().material.mainTexture = textB;
			maskLayer.transform.localScale = new Vector3(0.1380835f,0.1380835f,0.1380835f);
			isActived = true;
		}
	}
	Vector2 textOffset(int _idText){
		int a = (int)(_idText / x);
		int b = (int)(_idText % x);
		Vector2 offset = new Vector2 (b / x, (y - a - 1) / y);
		return offset;
	}
	Vector2   textSize(){
		Vector2 size = new Vector2 (1 / x, 1 / y);
		return size;
	}
}
