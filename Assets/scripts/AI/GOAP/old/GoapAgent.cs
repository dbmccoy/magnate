﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public sealed class GoapAgent : MonoBehaviour {

    public float UpdateRate = 1f;

	private FSM stateMachine;

	private FSM.FSMState idleState; // finds something to do
	private FSM.FSMState moveToState; // moves to a target
	private FSM.FSMState performActionState; // performs an action
	
	public HashSet<GoapAction> availableActions { get; private set; } = new HashSet<GoapAction>();
	private Queue<GoapAction> currentActions = new Queue<GoapAction>();
    List<Queue<GoapAction>> allPlans = new List<Queue<GoapAction>>();

    public IGoap dataProvider { get; private set; } // this is the implementing class that provides our world data and listens to feedback on planning
    
	public GoapPlanner planner;


	void Awake () {
		stateMachine = new FSM ();
		planner = new GoapPlanner ();
		findDataProvider ();
		createIdleState ();
		createMoveToState ();
		createPerformActionState ();
		stateMachine.pushState (idleState);
		loadActions ();
	}

    private float sinceLastUpdate;

	void Update () {
        
        sinceLastUpdate += Time.deltaTime;
        sinceLastUpdate = 0f;
        loadActions();
        stateMachine.Update(this.gameObject);
        if (sinceLastUpdate > UpdateRate)
        {
            
        }

    }

    private GoapAction currAct;
    public GoapAction CurrentAction() {
        return currAct;
    }

	public void addAction(GoapAction a) {
		availableActions.Add (a);
	}

	public GoapAction getAction(Type action) {
		foreach (GoapAction g in availableActions) {
			if (g.GetType().Equals(action) )
			    return g;
		}
		return null;
	}

	public void removeAction(GoapAction action) {
		availableActions.Remove (action);
	}

	private bool hasActionPlan() {
		return currentActions.Count > 0;
	}

    public HashSet<KeyValuePair<string, object>> Goal;

	private void createIdleState() {
		idleState = (fsm, gameObj) => {
			// GOAP planning

			// get the world state and the goal we want to plan for
			HashSet<KeyValuePair<string,object>> worldState = dataProvider.getWorldState();
			List<HashSet<KeyValuePair<string,object>>> goals = dataProvider.getGoals();

            goals.Shuffle();

            foreach (var goal in goals) //goals aren't ranked by priority yet- will use first valid
            {
                Goal = goal;

                // Plan
                Queue<GoapAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
                if (plan != null)
                {
                    // we have a plan, hooray!
                    currentActions = plan;
                    var match = true;

                    foreach (var item in allPlans) {
                        if (item.SequenceEqual(plan)) {
                            match = false;
                        }
                    }

                    if (match) allPlans.Add(plan);

                    dataProvider.planFound(goal, plan);

                    fsm.popState(); // move to PerformAction state
                    fsm.pushState(performActionState);
                    return;
                }
                else
                {
                    // ugh, we couldn't get a plan
                    //Debug.Log(GetComponent<Person>().name + " <color=orange>Failed Plan:</color>" + prettyPrint(goal));
                    dataProvider.planFailed(goal);
                    continue;
                }
            }
            fsm.popState(); // move back to IdleAction state
            fsm.pushState(idleState);
        };
	}

	
	private void createMoveToState() {
		moveToState = (fsm, gameObj) => {
			// move the game object

			GoapAction action = currentActions.Peek();
			if (action.requiresInRange() && action.target == null) {
				Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			// get the agent to move itself
			if ( dataProvider.moveAgent(action) ) {
				fsm.popState();
			}

			/*MovableComponent movable = (MovableComponent) gameObj.GetComponent(typeof(MovableComponent));
			if (movable == null) {
				Debug.Log("<color=red>Fatal error:</color> Trying to move an Agent that doesn't have a MovableComponent. Please give it one.");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			float step = movable.moveSpeed * Time.deltaTime;
			gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);

			if (gameObj.transform.position.Equals(action.target.transform.position) ) {
				// we are at the target location, we are done
				action.setInRange(true);
				fsm.popState();
			}*/
		};
	}

    private int currentPlanInt = 0;

    Queue<GoapAction> NextPlan() {
        var q = allPlans[currentPlanInt];
        currentPlanInt++;
        if (currentPlanInt == allPlans.Count) currentPlanInt = 0;
        return q;
    }

    public GoapAction Peek() {
        return currentActions.Peek();
    }
	
	private void createPerformActionState() {

		performActionState = (fsm, gameObj) => {
			// perform the action

			if (!hasActionPlan()) {
				// no actions to perform
				Debug.Log("<color=red>Done actions</color>");
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
                currAct = null;
				return;
			}

            currentActions = NextPlan();

			GoapAction action = currentActions.Peek();
            currAct = action;
            //Debug.Log(prettyPrint(action));
            if ( action.isDone() ) {
                // the action is done. Remove it so we can perform the next one
				currentActions.Dequeue();
			}

			if (hasActionPlan()) {
				// perform the next action
				action = currentActions.Peek();
				bool inRange = action.requiresInRange() ? action.isInRange() : true;

				if ( inRange ) {
					// we are in range, so perform the action
					bool success = action.perform(gameObj);

					if (!success) {
                        // action failed, we need to plan again
                        //Debug.Log(action.ToString());
						fsm.popState();
						fsm.pushState(idleState);
						dataProvider.planAborted(action);
					}
				} else {
					// we need to move there first
					// push moveTo state
					fsm.pushState(moveToState);
				}

			} else {
                allPlans.Remove(currentActions);
				// no actions left, move to Plan state
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
			}

		};
	}

	private void findDataProvider() {
		foreach (Component comp in gameObject.GetComponents(typeof(Component)) ) {
			if ( typeof(IGoap).IsAssignableFrom(comp.GetType()) ) {
				dataProvider = (IGoap)comp;
				return;
			}
		}
	}

	private void loadActions ()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();
		foreach (GoapAction a in actions) {
			availableActions.Add (a);
		}
		//Debug.Log("Found actions: "+prettyPrint(actions));
	}

    private List<GoapAction> tempActions = new List<GoapAction>();

    public GoapAction addTempCommissionProjectAction() {
        var a = gameObject.AddComponent<CommissionProjectAction>();
        tempActions.Add(a);
        return a;
    }

	public static string prettyPrint(HashSet<KeyValuePair<string,object>> state) {
		String s = "";
		foreach (KeyValuePair<string,object> kvp in state) {
			s += kvp.Key + ":" + kvp.Value.ToString();
			s += ", ";
		}
		return s;
	}

	public static string prettyPrint(Queue<GoapAction> actions) {
		String s = "";
		foreach (GoapAction a in actions) {
			s += a.GetType().Name;
            if(a is CommissionProjectAction c) {
                s += c.tempProject.Deliverable.Name;
            }
			s += "-> ";
		}
		s += "GOAL";
		return s;
	}

	public static string prettyPrint(GoapAction[] actions) {
		String s = "";
		foreach (GoapAction a in actions) {
			s += a.GetType().Name;
			s += ", ";
		}
		return s;
	}

	public static string prettyPrint(GoapAction action) {
		String s = ""+action.GetType().Name;
		return s;
	}
}
