using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class BanditEnvironment : MonoBehaviour {

	public List<GameObject> chests; // List of chest objects.
	public List<GameObject> estimatedValues; // List of visualized value estimates (green orbs).
	public List<GameObject> trueValues; // List of visualized true values (clear orbs).
	public float totalRewards; // Total rewards obtained over the course of all trials.
	int trial; // Trial index.
	int num_arms; // Number of chests in a given trial.
	int totalStates; // Number of possible rooms with unique chest reward probabilties. 
	int state; // Index of current room.
	public float actSpeed; // Speed at which actions are chosen.
	float[][] armProbs; // True probability values for each chest in each room.
	Agent agent; // The agent which learns to pick actions.

	// Use this for initialization
	void Start () {
	}

	/// <summary>
	/// Initialized the bandit game. Called when "Start Learning" button in clicked.
	/// </summary>
	public void BeginLearning() {
		trial = 0;
		totalRewards = 0;
		int stateMode = GameObject.Find ("state_D").GetComponent<Dropdown> ().value;
		actSpeed = 0.5f - GameObject.Find ("speed_S").GetComponent<Slider> ().value;
		if (stateMode == 0) {
			totalStates = 1;
		} else {
			totalStates = 3;
		}
		num_arms = GameObject.Find ("arms_D").GetComponent<Dropdown> ().value + 2;
		bool optimistic = GameObject.Find ("optToggle").GetComponent<Toggle> ().isOn;

		agent = new Agent (totalStates, num_arms, optimistic);

		int diff = GameObject.Find ("diff_D").GetComponent<Dropdown> ().value;
		float adjust = ((float)diff) * 0.1f;
		armProbs = new float[totalStates] [];
		for (int i = 0; i < totalStates; i++) {
			armProbs [i] = new float[num_arms];
			int winner = Random.Range (0, num_arms);
			for (int j = 0; j < num_arms; j++) {
				if (j == winner) {
					armProbs [i] [j] = Random.Range (0.6f, 1.0f - adjust);
				} else {
					armProbs [i] [j] = Random.Range (0.0f + adjust, 0.4f);
				}
			}
		}
			
		GameObject[] startUI = (GameObject.FindGameObjectsWithTag ("loading"));
		foreach (GameObject obj in startUI) {
			Destroy (obj);
		}

		chests = new List<GameObject> ();
		estimatedValues = new List<GameObject> ();
		trueValues = new List<GameObject> ();

		GameObject.Find ("restartButton").GetComponent<Button> ().interactable = true;
		LoadTrial ();
	}
	
	// Update is called once per frame
	void Update () {
		actSpeed = 0.5f - GameObject.Find ("speed_S").GetComponent<Slider> ().value;

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	/// <summary>
	/// Gets an action from the agent, selects the chest accordingly,
	/// and updates the agent's value estimates based on the reward.
	/// </summary>
	IEnumerator Act() {
		yield return new WaitForSeconds(actSpeed);
		int action = agent.PickAction(state);
		float reward = chests [action].GetComponent<SelectAction> ().Selected ();
		totalRewards += reward;
		agent.UpdatePolicy (state, action, reward);
	}

	/// <summary>
	/// Resets chests for new trial.
	/// </summary>
	public void LoadTrial() {
		trial += 1;
		state = Random.Range (0, totalStates);

		if (totalStates == 1) {
			GameObject.Find ("Directional Light").GetComponent<Light> ().color = new Color (1f, 1f, 1f);
		} else {
			if (state == 0) {
				GameObject.Find ("Directional Light").GetComponent<Light> ().color = new Color (1f, 0f, 0f);
			}
			if (state == 1) {
				GameObject.Find ("Directional Light").GetComponent<Light> ().color = new Color (0f, 1f, 0f);
			}
			if (state == 2) {
				GameObject.Find ("Directional Light").GetComponent<Light> ().color = new Color (0f, 0f, 1f);
			}
		}

		GameObject.Find ("Text").GetComponent<Text> ().text = "Trial: " + trial.ToString() + "\nTotal Reward: " + totalRewards.ToString ();

		foreach (GameObject chest in chests) {
			DestroyImmediate (chest);
		}
		foreach (GameObject value in estimatedValues) {
			DestroyImmediate (value);
		}
		foreach (GameObject value in trueValues) {
			DestroyImmediate (value);
		}

		chests = new List<GameObject> ();
		estimatedValues = new List<GameObject> ();
		trueValues = new List<GameObject> ();


		DestroyImmediate (GameObject.Find ("slime"));
		DestroyImmediate (GameObject.Find ("diamond"));

		int total = num_arms + 1;
		for (int i = 0; i < num_arms; i++) {
			GameObject chest = (GameObject)GameObject.Instantiate (Resources.Load ("bandit_chest"));
			chest.transform.position = new Vector3 ((i+1) * (12.5f / (total-1)) - ((12.5f / (total-1))*total)/2, 0f, 1.5f);
			chest.GetComponent<SelectAction> ().myProb = armProbs [state][i];
			chests.Add (chest);

			GameObject valueSphere = (GameObject)GameObject.Instantiate (Resources.Load ("value"));
			valueSphere.transform.position = new Vector3 ((i+1) * (12.5f / (total-1)) - ((12.5f / (total-1))*total)/2, 3f, 1.5f);
			float inflation = 2 * agent.value_table [state][i] + 0.25f;
			
			inflation = Mathf.Clamp(inflation, 0, 2.5f);
			
			valueSphere.transform.localScale = new Vector3 (inflation, inflation, inflation);
			estimatedValues.Add (valueSphere);

			GameObject valueTrueSphere = (GameObject)GameObject.Instantiate (Resources.Load ("true_value"));
			valueTrueSphere.transform.position = new Vector3 ((i+1) * (12.5f / (total-1)) - ((12.5f / (total-1))*total)/2, 3f, 1.5f);
			float inflationTrue = 2 * armProbs [state][i] + 0.25f;
			if (inflationTrue > 2.5f) {
				inflationTrue = 2.5f;
			}
			valueTrueSphere.transform.localScale = new Vector3 (inflationTrue, inflationTrue, inflationTrue);
			trueValues.Add (valueTrueSphere);

		}
		StartCoroutine (Act ());
	}

	public void ReloadEnvironment() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
