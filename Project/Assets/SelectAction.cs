using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAction : MonoBehaviour {

	public float myProb;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float Selected() {
		float reward = 0f;
		if (myProb > Random.Range (0.0f, 1.0f)) {
			GameObject diamond = (GameObject)GameObject.Instantiate (Resources.Load ("diamond"));
			diamond.transform.position = this.transform.position;
			diamond.name = "diamond";
			reward = 1.0f;
			foreach (GameObject chest in GameObject.Find("Main Camera").GetComponent<BanditEnvironment>().chests) {
				if (chest == this.gameObject) {
					chest.gameObject.GetComponent<MeshRenderer> ().enabled = false;
				}
				chest.gameObject.GetComponent<Collider> ().enabled = false;
			}
		} else {
			GameObject slime = (GameObject)GameObject.Instantiate (Resources.Load ("slime"));
			slime.transform.position = this.transform.position;
			slime.name = "slime";
			reward = -1.0f;
			foreach (GameObject chest in GameObject.Find("Main Camera").GetComponent<BanditEnvironment>().chests) {
				if (chest == this.gameObject) {
					chest.gameObject.GetComponent<MeshRenderer> ().enabled = false;
				}
				chest.gameObject.GetComponent<Collider> ().enabled = false;
			}
		}
		StartCoroutine (Example ());
		return reward;
	}

	void OnMouseDown() {
		float r = Selected ();
		GameObject.Find ("Main Camera").GetComponent<BanditEnvironment> ().totalRewards += r;
	}

	IEnumerator Example() {
		yield return new WaitForSeconds(GameObject.Find("Main Camera").GetComponent<BanditEnvironment>().actSpeed);
		GameObject.Find ("Main Camera").GetComponent<BanditEnvironment> ().LoadTrial ();
	}


}
