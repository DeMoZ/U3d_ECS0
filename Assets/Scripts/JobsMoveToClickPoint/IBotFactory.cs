using Unity.Collections;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    public interface IBotFactory
    {
        Transform[] GenerateBots(int numberOfBots);
    }
}