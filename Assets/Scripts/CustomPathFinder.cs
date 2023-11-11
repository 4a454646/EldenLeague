using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using Pathfinding;

public class CustomPathFinder : AIPath {
    [SerializeField] public GameObject toControl;
    public override void OnTargetReached() {
        IWalkAnimation iwa = toControl.GetComponent<IWalkAnimation>();
        iwa?.OnTargetReached();
    }
}