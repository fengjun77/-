using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerTrigger : Enemy_AnimationTrigger
{
    private Enemy_DeathBriner deathBriner => GetComponentInParent<Enemy_DeathBriner>();

    private void Relocate() => deathBriner.FindPosition();

    //��͸��
    private void MakeInvisible() => deathBriner.fx.MakeTransprent(true);

    //��ɼ�
    private void MakeVisible() => deathBriner.fx.MakeTransprent(false);
}
