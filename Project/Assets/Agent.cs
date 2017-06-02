using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Agent {
	public float[][] value_table;
	float learning_rate = 0.005f;

	public Agent(int stateSize, int actionSize, bool optimistic) {
		value_table = new float[stateSize][];
		for (int i = 0; i < stateSize; i++) {
			value_table [i] = new float[actionSize];
			for (int j = 0; j < actionSize; j++) {
				if (!optimistic) {
					value_table [i] [j] = 0.0f;
				} else {
					value_table [i] [j] = 1.0f;
				}
			}
		}
	}

	public int PickAction(int state) {
		float confidence = 2.1f - GameObject.Find ("confidence_S").GetComponent<Slider> ().value;
		float[] probs = softmax (value_table [state], confidence);
		float cumulative = 0.0f;
		int selectedElement = 0;
		float diceRoll = Random.Range (0f, 1f);
		for (int i = 0; i < probs.Length; i++)
		{
			cumulative += probs[i];
			if (diceRoll < cumulative)
			{
				selectedElement = i;
				break;
			}
		}
		return selectedElement;
	}

	public void UpdatePolicy(int state, int action, float reward) {
		value_table [state][action] += learning_rate * (reward - value_table[state] [action]);
	}

	float[] softmax(float[] values, float temp) {
		float[] softmax_values = new float[values.Length];
		float[] exp_values = new float[values.Length];
		for (int i = 0; i < values.Length; i++) {
			exp_values [i] = Mathf.Exp (values [i] / temp);
		}

		for (int i = 0; i < values.Length; i++) {
			softmax_values[i] = exp_values[i] / exp_values.Sum();
		}
		return softmax_values;
	}
}
