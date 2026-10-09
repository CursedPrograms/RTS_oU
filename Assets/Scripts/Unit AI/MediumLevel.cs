using UnityEngine;
using System.Collections.Generic;

public class MediumLevel : MonoBehaviour
{
    public List<GameObject> Units = new List<GameObject>();

    int UnitLimit;

    Vector3 AverageLocation;

    public enum Action
    {
        explore,
        patrol,
        attack,
        defend,
        nothing,
    };

    public Action action = Action.nothing;

    public bool OrderGiven = true;
    public bool DfaultOrder = false;
    LastOrder LastOrder;

    Vector3 AreaToDefend;
    GameObject Target;
    ThreatInfo TargetInfo;

    GameObject[] Waypoints;
    GameObject targetWayPoint;
    bool WaypointSet = false;
    int CurrentWaypoint = 0;
    float PatrolCounter = 0;
    bool GoForAttack = false;
    bool GoToTarget = false;
    GameObject AttackTarget;

    void Update()
    {
        Units.RemoveAll(u => u == null);
        if (Units.Count == 0) return;

        int counter = 0;
        AverageLocation = new Vector3();
        foreach (GameObject g in Units)
        {
            AverageLocation += g.transform.position;
            counter++;
        }

        AverageLocation /= counter;
        if (action == Action.defend)
        {

        }
        else if (action == Action.patrol)
        {
            bool OkToProceed = true;
            foreach (GameObject g in Units)
            {
                if (g.GetComponent<Unit_Core>().getObjectsInRange() != null && g.GetComponent<Unit_Core>().getObjectsInRange().Length > 0)
                {
                    OkToProceed = false;
                }
            }

            if (OkToProceed)
            {
                if (!WaypointSet)
                {
                    targetWayPoint = Waypoints[CurrentWaypoint];
                    setDestination(targetWayPoint.transform.position, 2);
                    WaypointSet = true;
                    CurrentWaypoint++;
                    if (CurrentWaypoint >= Waypoints.Length) CurrentWaypoint = 0;
                }

                if (Vector3.Distance(AverageLocation, targetWayPoint.transform.position) < 5.0f)
                {
                    PatrolCounter += Time.deltaTime;

                    if (PatrolCounter > 8.0f)
                    {
                        WaypointSet = false;
                        PatrolCounter = 0;
                    }
                }

            }
        }
        else if (action == Action.explore)
        {

        }
        else if (action == Action.attack)
        {
            if (Units.Count >= UnitLimit)
            {
                GoForAttack = true;
            }
            else
            {
                foreach (GameObject g in Units)
                {
                    if (!g.GetComponent<Unit_Logic>().getOrders())
                    {
                        setDestination(RTS_AI.temp.rangeCenter.transform.position, 4);
                    }
                }
            }

            if (GoForAttack)
            {
                if (AttackTarget == null)
                {
                    GoToTarget = false;
                }

                if (!GoToTarget)
                {
                    bool attackBuilding;
                    AttackTarget = findClosestTarget(out attackBuilding);

                    if (AttackTarget != null)
                    {
                        GoToTarget = true;
                        setAttackTarget(AttackTarget, attackBuilding, 4);
                    }
                }
            }
        }
        else if (action == Action.nothing)
        {

        }
    }

    // Nearest player unit or building to the group. Units are preferred unless a building is much closer.
    GameObject findClosestTarget(out bool isBuilding)
    {
        GameObject bestUnit = closestWithTag("Player");
        GameObject bestBuilding = closestWithTag("fBuilding");
        isBuilding = false;

        if (bestUnit == null && bestBuilding == null) return null;
        if (bestUnit == null) { isBuilding = true; return bestBuilding; }
        if (bestBuilding == null) return bestUnit;

        float unitDist = Vector3.Distance(AverageLocation, bestUnit.transform.position);
        float buildingDist = Vector3.Distance(AverageLocation, bestBuilding.transform.position);
        if (buildingDist * 2f < unitDist)
        {
            isBuilding = true;
            return bestBuilding;
        }
        return bestUnit;
    }

    GameObject closestWithTag(string tag)
    {
        GameObject best = null;
        float bestDist = float.MaxValue;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag(tag))
        {
            float d = (g.transform.position - AverageLocation).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = g;
            }
        }
        return best;
    }

    public Vector3 GetLocation()
    {
        return AverageLocation;
    }

    public void setDestination(Vector3 location, int command)
    {
        Target = null;
        foreach (GameObject g in Units)
        {
            g.GetComponent<Unit_Logic>().giveCommand(command, location);
        }

        if (command == 1)
        {
            DfaultOrder = true;
            OrderGiven = false;
        }
        else if (command == 4)
        {
            DfaultOrder = true;
            OrderGiven = false;
        }

        LastOrder = new LastOrder("travel", location, command);
    }

    public void setAttackTarget(GameObject target, bool building, int command)
    {
        if (target == null) return;
        this.Target = target;
        foreach (GameObject g in Units)
        {
            g.GetComponent<Unit_Logic>().giveCommand(command, target);
        }

        if (command == 1)
        {
            DfaultOrder = false;
            OrderGiven = true;
            TargetInfo = target.GetComponent<ThreatInfo>();
            if (TargetInfo != null) TargetInfo.addHandler(this);
        }

        LastOrder = new LastOrder("attack", target, command);
    }

    public void assignUnit(GameObject g)
    {
        g.GetComponent<Unit_Logic>().setController(this);

        if (LastOrder != null)
        {
            if (LastOrder.Name == "travel")
            {
                g.GetComponent<Unit_Logic>().giveCommand(LastOrder.Command, LastOrder.Location);
            }
            else if (LastOrder.Name == "attack")
            {
                g.GetComponent<Unit_Logic>().giveCommand(LastOrder.Command, LastOrder.Target);
            }
        }
        Units.Add(g);
    }

    public void removeUnit(GameObject g)
    {
        g.GetComponent<Unit_Logic>().removeController();
        RTS_AI.temp.RequestUnit(this);
        Units.Remove(g);

        // Only fall back and regroup once the wave has lost half its strength,
        // instead of retreating every time a single unit dies.
        if (Units.Count * 2 < UnitLimit)
        {
            GoForAttack = false;
            GoToTarget = false;
        }
    }

    public void giveOrder(int o)
    {
        if (o == 1)
        {
            action = Action.defend;
            UnitLimit = RTS_AI.temp.midDefenseLimit;
            setDestination(AreaToDefend, 1);
        }
        else if (o == 2)
        {
            action = Action.patrol;
            Waypoints = GameObject.FindGameObjectsWithTag("wp");
            UnitLimit = RTS_AI.temp.midPatrolLimit;
        }
        else if (o == 3)
        {
            action = Action.explore;
            UnitLimit = RTS_AI.temp.midExploreLimit;
        }
        else if (o == 4)
        {
            action = Action.attack;
            UnitLimit = RTS_AI.temp.midAttackLimit;
        }
    }

    public void setUnitLimit(int limit)
    {
        UnitLimit = limit;
    }

    public bool underOrders()
    {
        return OrderGiven;
    }

    public GameObject GetTarget()
    {
        return Target;
    }

    public Vector3 getAreaToDefend()
    {
        return AreaToDefend;
    }

    public void setAreaToDefend(Vector3 area)
    {
        AreaToDefend = area;
    }

    public void requestOrder(Unit_Logic uL)
    {
        if (LastOrder != null)
        {
            if (LastOrder.Name == "travel")
            {
                uL.giveCommand(LastOrder.Command, LastOrder.Location);
            }
            else if (LastOrder.Name == "attack")
            {
                uL.giveCommand(LastOrder.Command, LastOrder.Target);
            }
        }
    }

    public bool hasTarget()
    {
        if (TargetInfo == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void assignTargetInfo(ThreatInfo tI)
    {
        TargetInfo = tI;
    }

    public void removeTargetInfo()
    {
        TargetInfo = null;
    }
}

public class LastOrder
{
    public string Name;
    public Vector3 Location;
    public GameObject Target;
    public int Command;

    public LastOrder(string s, Vector3 loc, int command)
    {
        Name = s;
        Location = loc;
        Target = null;
        this.Command = command;
    }

    public LastOrder(string s, GameObject tar, int command)
    {
        Name = s;
        Location = Vector3.zero;
        Target = tar;
        this.Command = command;
    }
}
