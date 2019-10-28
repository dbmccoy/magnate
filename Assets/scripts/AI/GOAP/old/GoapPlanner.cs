using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */
public class GoapPlanner
{
    bool deepDebug;
    public void ToggleDeepDebug(bool v) {
        deepDebug = v;
    }


    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */
    public Queue<GoapAction> plan(GameObject agent,
								  HashSet<GoapAction> availableActions, 
	                              HashSet<KeyValuePair<string,object>> worldState, 
	                              HashSet<KeyValuePair<string,object>> goal) 
	{
        
        

        if(goal.Count == 0)
        {
            Debug.Log("no goal, aborting plan");
            return null;
        }

		// reset the actions so we can start fresh with them
		foreach (GoapAction a in availableActions) {
			a.doReset ();
		}

		// check what actions can run using their checkProceduralPrecondition
		HashSet<GoapAction> usableActions = new HashSet<GoapAction> ();
		foreach (GoapAction a in availableActions) {
			if ( a.checkProceduralPrecondition(agent)) {
                usableActions.Add(a);
            }
        }
		
		// we now have all actions that can run, stored in usableActions

		// build up the tree and record the leaf nodes that provide a solution to the goal.
		List<Node> leaves = new List<Node>();

        // build graph

        Node start = new Node (null, 0, worldState, null);
        //bool success = reverseSearch(start, leaves, usableActions, goal, worldState, agent.GetComponent<GoapAgent>());
		bool success = buildGraph(start, leaves, usableActions, goal, agent.GetComponent<GoapAgent>());
        

		if (!success) {
            // oh no, we didn't get a plan
            if (deepDebug) {
                Debug.Log(agent.GetComponent<Person>().name + " NO PLAN " + GoapAgent.prettyPrint(goal));
            }
			
			return null;
		}

		// get the cheapest leaf
		Node cheapest = null;
		foreach (Node leaf in leaves) {
			if (cheapest == null)
				cheapest = leaf;
			else {
				if (leaf.runningCost < cheapest.runningCost)
					cheapest = leaf;
			}
		}

		// get its node and work back through the parents
		List<GoapAction> result = new List<GoapAction> ();
		Node n = cheapest;
		while (n != null) {
			if (n.action != null) {
				result.Insert(0, n.action); // insert the action in the front
			}
			n = n.parent;
		}
        // we now have this action list in correct order

        var id = (leaves.IndexOf(cheapest) + 1).ToString();

        Queue<GoapAction> queue = new Queue<GoapAction> ();
		foreach (GoapAction a in result) {
            queue.Enqueue(a);
            a.addToFinalPlan(id);
		}

        // hooray we have a plan!
        if (deepDebug) {
            Debug.Log("we have a plan! " + agent.GetComponent<Person>().name + queue.Count);
        }
        return queue;
	}
    
    /**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest cost will be the best action
	 * sequence.
	 */

    int loops = 0;

    private bool buildGraph (Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, HashSet<KeyValuePair<string, object>> goal, GoapAgent agent)
	{
        bool foundOne = false;

        // go through each action available at this node and see if we can use it here

        foreach (GoapAction action in usableActions) {
            //Debug.Log("next action " + GoapAgent.prettyPrint(action) + " " + usableActions.Count);
            // if the parent state has the conditions for this action's preconditions, we can use it here

            if ( inState(action.Preconditions, parent.state) ) {
                // apply the action's effects to the parent state

                if (agent.GetComponent<Person>().name == "Richie") {
                    var db = GameObject.Find("PlannerDebugText").GetComponent<PlannerDebug>();
                    db.worldState = parent.state;
                    db.goal = goal;
                }

                if (inState(action.Effects, parent.state)) {

                    foreach (var item in action.Effects) {
                        //Debug.Log(item.Key + " : " + item.Value + " already in state");
                    }
                    if(action.Effects.Count == 0) {
                        //Debug.Log(GoapAgent.prettyPrint(action)+" no effects");
                    }
                    continue;
                }
                
                HashSet<KeyValuePair<string,object>> currentState = populateState (parent.state, action.Effects);

                Node node = new Node(parent, parent.runningCost+action.cost, currentState, action);

                string s = "";
                var n = node;
                while (n.parent != null){
                    if(n.parent.action != null) {
                        s += GoapAgent.prettyPrint(n.parent.action) + " > ";
                        foreach (var item in n.parent.action.Effects) {
                            s += "(" + item.Key + ":" + item.Value + ")";
                        }
                    }
                    n = n.parent;
                }
                if(action != null) {
                    s += GoapAgent.prettyPrint(action);
                    foreach (var item in action.Effects) {
                        s += "(" + item.Key + ":" + item.Value + ")";
                    }
                }

                if (inState(goal, currentState)) {
                    foreach (var item in currentState) {
                        //Debug.Log(item.Key + " : " + item.Value);
                    }
					// we found a solution!
                    if(action.addToPlan(leaves.Count.ToString()) == true) {
                        foundOne = true;
                        leaves.Add(node);
                        if(action is DevelopAction cp) {
                            Debug.Log(GoapAgent.prettyPrint(action) + leaves.Count + " adding leaf " + s);
                            var s2 = "leaf has state:\n";
                            foreach (var item in currentState) {
                                s2 += item.Key + ":" + item.Value + "\n";
                            }
                            s2 += "______";
                            //Debug.Log(s2);
                        }
                    }


                }
                else {
                    // not at a solution yet, so test all the remaining actions and branch out the tree
                    // if action is reusable, don't remove it from usableActions
                    HashSet<GoapAction> subset = usableActions;

                    if (action.isReusable) {
                        if (action is CommissionProjectAction cp) {
                            cp.checkProceduralPrecondition(cp.gameObject);
                            loops++;
                            if (cp.availableProjects.Count == 0) {
                                subset = actionSubset(usableActions, action);
                                Debug.Log("subset: no available projects");
                            }
                            else {
                                if(loops++ > 10) {
                                    subset = actionSubset(usableActions, action);
                                    //cp.doHardReset();
                                    Debug.Log("subset: time out");
                                    loops = 0;
                                }
                                
                            }
                        }
                    }
                    else {
                        subset = actionSubset(usableActions, action);
                    }
                    bool found = buildGraph(node, leaves, subset, goal, agent);
					if (found) {
                        if(action.addToPlan(leaves.Count.ToString()) == true) {
                            foundOne = true;
                            s = "";
                            n = node;
                            while (n.parent != null) {
                                if (n.parent.action != null) {
                                    s += GoapAgent.prettyPrint(n.parent.action) + " > ";
                                }
                                n = n.parent;
                            }
                        }


                        
                        //Debug.Log(action.ToString());
                        if (action is CommissionProjectAction cp) {
                            foreach (var kvp in cp.Effects) {
                                //Debug.Log(kvp.Key + " : " + kvp.Value);
                            }
                            if (cp.availableProjects.Count == 0) {
                                subset = actionSubset(usableActions, action);
                                Debug.Log("subset: no available projects");
                            }
                        }
                    }
                }
			}
            else {
               
            }
		}

		return foundOne;
	}

	/**
	 * Create a subset of the actions excluding the removeMe one. Creates a new set.
	 */
	private HashSet<GoapAction> actionSubset(HashSet<GoapAction> actions, GoapAction removeMe) {
		HashSet<GoapAction> subset = new HashSet<GoapAction> ();
		foreach (GoapAction a in actions) {
			if (!a.Equals(removeMe))
				subset.Add(a);
		}
		return subset;
	}

	/**
	 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
	 * then this returns false.
	 */
	public static bool inState(HashSet<KeyValuePair<string,object>> test, HashSet<KeyValuePair<string,object>> state) {
		bool allMatch = true;
		foreach (KeyValuePair<string,object> t in test) {
			bool match = false;
			foreach (KeyValuePair<string,object> s in state) {
				if (s.Equals(t)) {
					match = true;
					break;
				}
			}
			if (!match)
				allMatch = false;
		}
		return allMatch;
	}
	
	/**
	 * Apply the stateChange to the currentState
	 */
	private HashSet<KeyValuePair<string,object>> populateState(HashSet<KeyValuePair<string,object>> currentState, HashSet<KeyValuePair<string,object>> stateChange) {
		HashSet<KeyValuePair<string,object>> state = new HashSet<KeyValuePair<string,object>> ();
		// copy the KVPs over as new objects
		foreach (KeyValuePair<string,object> s in currentState) {
			state.Add(new KeyValuePair<string, object>(s.Key,s.Value));
		}

		foreach (KeyValuePair<string,object> change in stateChange) {
			// if the key exists in the current state, update the Value
			bool exists = false;

			foreach (KeyValuePair<string,object> s in state) {
				if (s.Equals(change)) {
					exists = true;
					break;
				}
			}

			if (exists) {
				state.RemoveWhere( (KeyValuePair<string,object> kvp) => { return kvp.Key.Equals (change.Key); } );
				KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key,change.Value);
				state.Add(updated);
			}
			// if it does not exist in the current state, add it
			else {
				state.Add(new KeyValuePair<string, object>(change.Key,change.Value));
			}
		}
		return state;
	}

	/**
	 * Used for building up the graph and holding the running costs of actions.
	 */
	private class Node {
		public Node parent;
		public float runningCost;
		public HashSet<KeyValuePair<string,object>> state;
		public GoapAction action;

		public Node(Node parent, float runningCost, HashSet<KeyValuePair<string,object>> state, GoapAction action) {
			this.parent = parent;
			this.runningCost = runningCost;
			this.state = state;
			this.action = action;
		}
	}

    
}


