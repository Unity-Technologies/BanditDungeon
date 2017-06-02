# Bandit Dungeon Demo
Simple Unity project demonstrating multi-armed bandit algorithm.


## In-game Settings

* **Bandit Type** - Stateless bandit contains only a single set of chests. Contextual bandit contains three sets of chests, each denoted by a different room color.
* **Difficulty** - How great the difference between the optimally rewarding chest and the other chests.
* **Bandit Arms** - How many chests are in each room.
* **Begin Optimistic** - Whether to initialize the agent's value estimates with high values (active) or low values (inactive).
* **Agent Speed** - How quickly the agent takes actions. Increase speed to learn faster. Decrease speed to more easily visualize.
* **Agent Confidence** - How narrow the probability distribution over actions is. Increasing this causes the agent to more frequently pick only chests with a high estimated value. Descreasing this causes the agent to pick chests more uniformly.
