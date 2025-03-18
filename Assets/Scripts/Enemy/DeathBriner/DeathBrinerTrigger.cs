using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerTrigger : Enemy_AnimationTrigger
{
    private Enemy_DeathBriner deathBriner => GetComponentInParent<Enemy_DeathBriner>();

    private void Relocate() => deathBriner.FindPosition();

    //变透明
    private void MakeInvisible() => deathBriner.fx.MakeTransprent(true);

    //变可见
    private void MakeVisible() => deathBriner.fx.MakeTransprent(false);
}
