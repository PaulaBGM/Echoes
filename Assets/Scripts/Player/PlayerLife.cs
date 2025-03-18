using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : BaseHealth, ITargeteable
{
    protected CharacterController ch_Controller;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
