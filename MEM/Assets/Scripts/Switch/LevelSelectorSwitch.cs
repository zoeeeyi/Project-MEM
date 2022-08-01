using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorSwitch : SwitchController
{
    LevelSelectorSwitchParent parent;
    bool wasActivated;

    public string levelName;

    protected override void Start()
    {
        base.Start();
        parent = GetComponentInParent<LevelSelectorSwitchParent>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (activated && !wasActivated)
        {
            parent.currentlyActiveSwitch = this;
            parent.selectedLevel = levelName;
        }
        wasActivated = activated;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //Leave blank!
    }
}
