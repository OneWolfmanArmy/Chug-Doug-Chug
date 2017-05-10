using UnityEngine;

public interface IGameLoop
{
    void OnCreate();
    void OnGameBegin();
    void OnFrame();
}
