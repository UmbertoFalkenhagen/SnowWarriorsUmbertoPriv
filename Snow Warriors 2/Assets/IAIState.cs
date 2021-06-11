using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIState
{
    int actionstateweight { get; set; }
    IAIState DoState(UmbertoAINew npc);
}
