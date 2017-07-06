using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public DrawLine drawLine;
	public GameObject tilePrefab;
	public List<Tile> tiles;
	public List<Tile> _tiles;
	public List<Tile> tilesEdge;
	public int begin = 0;
	public int end = 0;
	//	public Material material;
	public int x, y;
	private Tile tileA;
	private Tile tileB;
	private bool destroy;
	private Vector3 mousePos;
	private int timeCount = 1200;
	private int timeForHint = 1200;
	//用于自动提示
	private stepType _stepType;
	private bool isAutoCheck = false;
	public List<Tile> comparePointsA = new List<Tile> ();
	private Audio music;
	private enum stepType
	{
		one,
		two,
		three
	}

	// Use this for initialization
	IEnumerator  TimerTick (float waitTime)
	{
		while (timeCount > 0) {
			timeCount--;
			yield return new WaitForSeconds (waitTime);
			//print ("TimerTick is updating " + timeCount);
		}
	}

	void Start ()
	{
		//this.gameObject.transform.position = Vector3.zero;
		SpawneGnerate ();
		StartCoroutine (TimerTick (1));
		_stepType = stepType.one;
		music = this.GetComponent<Audio> ();
		//music = GetComponent<AudioSource> ();
		//musicVolum = .5f;
	}

	private void SpawneGnerate ()
	{
		float num = (x * y - ((x * 2 + y * 2) - 4)) * 0.5f;
		for (int i = 0; i < num; i++) {
			int idText = Random.Range (begin, end);
			GameObject obj_curr = Instantiate (tilePrefab) as GameObject;
			GameObject obj_oppo = Instantiate (tilePrefab) as GameObject;
			Tile tile_curr = obj_curr.GetComponent <Tile > ();
			Tile tile_oppo = obj_oppo.GetComponent <Tile > ();
			tile_curr.init (idText);
			tile_oppo.init (idText);
			tiles.Add (tile_curr);
			tiles.Add (tile_oppo);
		}
		for (int i = 0; i < ((x * 2 + y * 2) - 4); i++) {
			GameObject obj_edage = Instantiate (tilePrefab) as GameObject;
			obj_edage.name = "edage";
			Tile tile_edage = obj_edage.GetComponent <Tile > ();
			tilesEdge.Add (tile_edage);
		}
		createTiles ();
		for (int i = 0; i < _tiles.Count; i++) {
			_tiles [i].transform.name = i.ToString ();
			_tiles [i].id = i;
		}
		this.transform.position = new Vector3 (-(x / 2.0f - 0.5f), -(y / 2.0f - 0.5f), 0);
	}

	private void createTiles ()
	{
		float _y = 0.0f;
		for (int i = 0; i < y; i++) {
			float _x = 0.0f;
			for (int j = 0; j < x; j++) {
				if (i == 0 || j == 0 || i == y - 1 || j == x - 1) {
					tilesEdge [0].transform.position = new Vector3 (_x, _y, 0);
					tilesEdge [0].pos = new Vector2 (_x, _y);
					tilesEdge [0].transform.rotation = new Quaternion (0, 0, 180, 0);
					tilesEdge [0].transform.parent = this.gameObject.transform;
					_tiles.Add (tilesEdge [0]);
					tilesEdge [0].transform.localScale = Vector3.zero;
					tilesEdge [0].type = false;
					tilesEdge.RemoveAt (0);
				} else {
					int _id = Mathf.FloorToInt (Random.Range (0, tiles.Count));
					tiles [_id].transform.position = new Vector3 (_x, _y, 0);
					tiles [_id].pos = new Vector2 (_x, _y);
					tiles [_id].transform.rotation = new Quaternion (0, 0, 180, 0);
					tiles [_id].transform.parent = this.gameObject.transform;
					_tiles.Add (tiles [_id]);
					tiles.RemoveAt (_id);
				}
				_x += 1;
			}
			_y += 1;
		}
	}

	private void selectedTile ()
	{
		Ray ray = Camera.main.ScreenPointToRay (mousePos);
		RaycastHit hit;
		int mask = 1 << 8;

		if (Physics.Raycast (ray, out hit, mask)) {
			music.playClickEffect ();
			if (tileA == null) {
				tileA = hit.transform.GetComponent <Tile > ();
				tileA.setTileTexture (1);
				isAutoCheck = true;
				print ("tileA = hit.transform.GetComponent<Tile> ();" + tileA.transform.name);
			} else {
				tileB = hit.transform.GetComponent<Tile > ();
				tileB.setTileTexture (1);
				print ("tileB = hit.transform.GetComponent<Tile> ();" + tileB.transform.name);
				compareTiles (tileA, tileB);
				if (tileA == null && tileB == null) {
					//print ("both A and B is null !");
				}
			}
			timeForHint = timeCount;
		}
	}

	private void compareTiles2 (Tile tile_curr, Tile tile_oppo, bool autoCkeck = false)
	{
		_stepType = stepType.one;
		destroy = false;
		if (tile_curr.pos.x == tile_oppo.pos.x && tile_curr.pos.y == tile_oppo.pos.y) {
			tileA.setTileTexture (0);

			return;
		} else if (tile_curr.pos.x == tile_oppo.pos.x && tile_curr.pos.y != tile_oppo.pos.y) {
			destroy = checkPosY (tile_curr, tile_oppo);
		} else if (tile_curr.pos.x != tile_oppo.pos.x && tile_curr.pos.y == tile_oppo.pos.y) {
			destroy = checkPosX (tile_curr, tile_oppo);
		}
		if (!destroy) {
			_stepType = stepType.two;
			destroy = checkTheSecondStep (tile_curr, tile_oppo);
			if (!destroy) {
				_stepType = stepType.three;
				destroy = checkTheThirdStep (tile_curr, tile_oppo);
			}
		}
		if (destroy) {
			tile_curr.setTileTexture (1);
			tile_oppo.setTileTexture (1);
			drawLine.ClearPath ();
		} else {
			tileA.setTileTexture (0);
			drawLine.ClearPath ();
			return;
		}
	}

	private void compareTiles (Tile tile_curr, Tile tile_oppo, bool autoCkeck = false)
	{
		_stepType = stepType.one;
		drawLine.waypoints.Add (tile_curr.transform);
		drawLine.transform.position = tile_curr.transform.position;
		destroy = false;
		//print ("Comparing");
		if (tile_curr.pos.x == tile_oppo.pos.x && tile_curr.pos.y == tile_oppo.pos.y) {
			drawLine.ClearPath ();
			tileA.setTileTexture (0);
			//tileB.setTileTexture (0);
			tileA = tileB;
			//tileA.setTileTexture (1);
			tileB = null;
			tileA = null;
			return;
		} else if (tile_curr.pos.x == tile_oppo.pos.x && tile_curr.pos.y != tile_oppo.pos.y) {
			//print ("check Y");
			destroy = checkPosY (tile_curr, tile_oppo);
			if (destroy)
				drawLine.waypoints.Add (tile_oppo.transform);
		} else if (tile_curr.pos.x != tile_oppo.pos.x && tile_curr.pos.y == tile_oppo.pos.y) {
			//print ("check X");
			destroy = checkPosX (tile_curr, tile_oppo);
			if (destroy)
				drawLine.waypoints.Add (tile_oppo.transform);
		}
		if (!destroy) {
			_stepType = stepType.two;
			destroy = checkTheSecondStep (tile_curr, tile_oppo);
			//print ("Checking the second step");
			if (!destroy) {
				_stepType = stepType.three;
				destroy = checkTheThirdStep (tile_curr, tile_oppo);
				//print ("Check the third step and destroied " + destroy);
				print ("tile_curr.idText:" + tile_curr.idText + "tile_oppo.idText:" + tile_oppo.idText);
			}
		}
		if (destroy) {
			timeForHint = timeCount;
			tile_curr.transform.localScale = Vector3.zero;
			tile_oppo.transform.localScale = Vector3.zero;
			tile_curr.type = false;
			tile_oppo.type = false;
			isAutoCheck = false;
		
			music.playMatchEffect ();
//			_tiles.RemoveAt (tile_curr.id);
//			_tiles.RemoveAt (tile_oppo.id);
			//ClearTiles (tile_curr);
			//ClearTiles (_tiles[tile_oppo.id]);
			tileA = null;
			tileB = null;
			drawLine.MoveToWaypoint ();
		} else {
			drawLine.ClearPath ();
			tileA.setTileTexture (0);
			tileA = tileB;
			tileB = null;
			isAutoCheck = true;
			return;
		}
		
	}

	private bool checkPosX (Tile _tile_curr, Tile  _tile_oppo)
	{
		bool isCompared = true;
		int _min, _max;
		if (_tile_curr.idText == _tile_oppo.idText) {
			compareID (_tile_curr, _tile_oppo, out _min, out _max);
			_min += 1;
			if (_min == _max)
				return true;
			for (int i = _min; i < _max; i++) {
				if (_tiles [i].type == true) {
					isCompared = false;
					break;
				}
			}
			return isCompared;
		} else
			return false;
	}

	private bool checkPosY (Tile _tile_curr, Tile _tile_oppo)
	{
		bool isCompared = true;
		int _min, _max;
//				int idA = (int)(a.x * x + a.y);
//				int idB = (int)(b.x * x + b.y);
//				print (_tiles [idA].id.ToString () + "idA:" + idA);
//				print (_tiles [idB].id.ToString () + "idB:" + idB);
//				print ("a.idtex:" + a.idTex + "b.idtex:" + b.idTex);
		if (_tile_curr.idText == _tile_oppo.idText) {
			compareID (_tile_curr, _tile_oppo, out _min, out _max);
			_min += x;
			if (_min == _max)
				return true;
			for (int i = _min; i < _max; i += x) {
				//print ("This is still in step one ");
				if (_tiles [i].type == true) {
					isCompared = false;
					break;
				}
			}
			return isCompared;
		} else
			return false;
	}
	//step tow
	private bool checkTheSecondStep (Tile _tile_curr, Tile _tile_oppo)
	{
		if (_tile_curr.pos.x == _tile_oppo.pos.x || _tile_curr.pos.y == _tile_oppo.pos.y)
			return false;
		int index1 = (int)(_tile_curr.pos.y * x + _tile_oppo.pos.x);

		if (_tiles [index1].type == false) {
			_tiles [index1].idText = _tile_curr.idText;
			if (checkPosY (_tiles [index1], _tile_oppo)) {
				if (checkPosX (_tile_curr, _tiles [index1])) {
					if (_stepType == stepType.two) {
						drawLine.waypoints.Add (_tiles [index1].transform);
						drawLine.waypoints.Add (_tile_oppo.transform);
					} else if (_stepType == stepType.three) {
						drawLine.waypoints.Add (_tiles [index1].transform);
						//	print ("=====================:" + 1);
						//	print ("In this way ,we under the first way");
					}
					return true;
				}
			}
		}
		int index2 = (int)(_tile_oppo.pos.y * x + _tile_curr.pos.x);
		if (_tiles [index2].type == false) {
			_tiles [index2].idText = _tile_oppo.idText;
			if (checkPosY (_tile_curr, _tiles [index2])) {
				if (checkPosX (_tile_oppo, _tiles [index2])) {
					if (_stepType == stepType.two) {
						drawLine.waypoints.Add (_tiles [index2].transform);
						drawLine.waypoints.Add (_tile_oppo.transform);
					} else if (_stepType == stepType.three) {
						drawLine.waypoints.Add (_tiles [index2].transform);
//						print ("=====================:" + 2);
//						print ("In this way ,we under the third way");
					}
					return true;
				}
				
			}
		}
		return false;
	}
	//step three
	private bool checkTheThirdStep (Tile a, Tile b)
	{
		//print ("a:" + a.idText + "b:" + b.idText);
		//		if (a.pos.x == b.pos.x || a.pos.y == b.pos.y) return false;
		bool returnValue = false;
		//print ("returnValue:" + returnValue);
		List<Tile> _comparrPointsB;
		ComparePoints (b, out _comparrPointsB);
		for (int i = 0; i < _comparrPointsB.Count; i++) {
			returnValue = checkTheSecondStep (a, _comparrPointsB [i]);
			if (returnValue) {
				drawLine.waypoints.Add (_comparrPointsB [i].transform);
				drawLine.waypoints.Add (b.transform);
				return returnValue;
			}
		}
		if (!returnValue) {
			List<Tile> _comparrPointsA;
			ComparePoints (a, out _comparrPointsA);
			//print (a.name);
			//print (b.name);
			for (int i = 0; i < _comparrPointsA.Count; i++) {
				print ("--------------" + b.idText);
				returnValue = checkTheSecondStep (b, _comparrPointsA [i]);
				if (returnValue) {
					drawLine.waypoints.Add (_comparrPointsA [i].transform);
					drawLine.waypoints.Add (b.transform);
					return returnValue;
				}
			}

		}
		return returnValue;
	}

	private void ComparePoints (Tile a, out List<Tile>comparePoints)
	{
		//print ("a.idtex" + a.idText);
		comparePoints = new List<Tile> ();
		comparePoints.Clear ();
		//		for (int i = 0; i < y; i ++) {
		//			if (i != a.y) {
		//				int id = (int)(i * x + a.pos.x);
		//				if (_tiles [id].type == false) {
		//					comparePoints.Add (_tiles [id]);
		//					_tiles [id].idTex = a.idTex;
		//				}
		//			}
		//		}
		for (int i = (int)a.pos.y - 1; i > -1; i--) {
			int id = (int)(i * x + a.pos.x);
			//print ("three step :" + id);
			if (_tiles [id].type == false) {
				comparePoints.Add (_tiles [id]);
				_tiles [id].idText = a.idText;
				print ("_tiles [id].idTex = a.idTex; " + _tiles [id].idText);
			} else
				break;
		}
		for (int i = (int)a.pos.y + 1; i < y; i++) {
			int id = (int)(i * x + a.pos.x);
			//print ("three step :" + id);
			if (_tiles [id].type == false) {
				comparePoints.Add (_tiles [id]);
				_tiles [id].idText = a.idText;
				print ("_tiles [id].idTex = a.idTex; " + _tiles [id].idText);
			} else
				break;
		}
		for (int i = (int)a.pos.x - 1; i > -1; i--) {
			int id = (int)(a.pos.y * x + i);
			if (_tiles [id].type == false) {
				comparePoints.Add (_tiles [id]);
				_tiles [id].idText = a.idText;
				print ("_tiles [id].idTex = a.idTex; " + _tiles [id].idText);
			} else
				break;
		}
		for (int i = (int)a.pos.x + 1; i < x; i++) {
			int id = (int)(a.pos.y * x + i);
			if (_tiles [id].type == false) {
				comparePoints.Add (_tiles [id]);
				_tiles [id].idText = a.idText;
				print ("_tiles [id].idTex = a.idTex; " + _tiles [id].idText);
			} else
				break;
		}

//		comparePointsA = comparePoints;
//		for (int i = 0; i < x; i ++) {
//			if (i != a.x) {
//				int id = (int)(a.pos.y * x + i);
//				if (_tiles [id].type == false) {
//					comparePoints.Add (_tiles [id]);
//					_tiles [id].idText = a.idText;
//				}
//			}
//		}
	}

	private void compareID (Tile _tile_curr, Tile _tile_oppo, out int min, out int max)
	{
		if (_tile_curr.id < _tile_oppo.id) {
			min = _tile_curr.id;
			max = _tile_oppo.id;
		} else {
			min = _tile_oppo.id;
			max = _tile_curr.id;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonUp (0)) {
			mousePos = Input.mousePosition;
			selectedTile ();
		} else if (timeForHint - timeCount >= 2 && isAutoCheck == true) {
			print ("compareTiles2");
			HintAuto ();
		}
	}

	void HintAuto ()
	{//2seconds do not have any operation Auto found the next 
		print ("选中了有2秒没有操作");
		Ray ray = Camera.main.ScreenPointToRay (mousePos);
		RaycastHit hit;
		int mask = 1 << 8;
		if (Physics.Raycast (ray, out hit, mask)) {
			tileA = hit.transform.GetComponent <Tile > ();
			for (int i = 1; i < _tiles.Count; i++) {
				if (_tiles [i].type && _tiles [i] != null) {
					compareTiles2 (tileA, _tiles [i]);
				}
			}

		
//				for (int i = (int)tileA.pos.y - 1; i > -1; i--) {//下
//					int id = (int)(i * x + tileA.pos.x);
//					if (!_tiles [id].type) {//有空儿
//						print ("有空儿");
//
//						break;
//					} else if (_tiles [id].type) {//无空格
//						if (tileA.idText == _tiles [id].idText) {
//							_tiles [id].setTileTexture (1);
//							break;
//						} else {
////							
//						}
//					}
//				}
//				for (int i = (int)tileA.pos.y + 1; i < y; i++) {//上
//					int id = (int)(i * x + tileA.pos.x);
//					if (!_tiles [id].type) {
//						//_tiles [id] = tileA;
//					} else if (_tiles [id].type) {
//						if (tileA.idText == _tiles [id].idText) {
//							_tiles [id].setTileTexture (1);
//						}
//						break;
//					} 
//				}
//				for (int i = (int)tileA.pos.x - 1; i > -1; i--) {//左
//					int id = (int)(tileA.pos.y * x + i);
//					if (!_tiles [id].type) {
//						//_tiles [id] = tileA;
//					} else if (_tiles [id].type) {
//						if (tileA.idText == _tiles [id].idText) {
//							_tiles [id].setTileTexture (1);
//						}
//						break;
//					}
//				}
//				for (int i = (int)tileA.pos.x + 1; i < x; i++) {//右
//					int id = (int)(tileA.pos.y * x + i);
//					if (!_tiles [id].type) {
//						//_tiles [id] = tileA;	
//					} else if (_tiles [id].type) {
//						if (tileA.idText == _tiles [id].idText) {
//							_tiles [id].setTileTexture (1);
//						}
//						break;
//					}
//				}

			//selectedTile ();
			isAutoCheck = false;
		} 
//		for (int i = 0; i < _tiles.Count; i++) {
//			_tiles [i].transform.name = i.ToString ();
//			_tiles [i].id = i;
//		}
		//print ("gameObaject.transform.childcount = " + gameObject.transform.childCount);
//		for(int i = 0;i<_tiles.Count;i++){
//			for(int j = 0;j<i;j++){
//				
//				//compareTiles (_tiles [i], _tiles [j]);
//
//			}
//		}

	}
	//		Vector2 TexSize ()
	//		{
	//			Vector2 size = new Vector2 (1 / x, 1 / y);
	//			return size;
	//		}
	//		Vector2 TexOffset (int _idTex)
	//		{
	//			int a = (int)(_idTex / x);
	//			//		print (a + "a:" + _idTex);
	//			int b = (int)(_idTex % x);
	//			//		print (b + "b:" + _idTex);
	//			Vector2 offset = new Vector2 (b / x, (y - 1 - a) / y);
	//			return offset;
	//		}
	private void ClearTiles (List<Tile> tiles)
	{
	
		tiles.Clear ();
		this.gameObject.transform.DetachChildren ();
	}
}
