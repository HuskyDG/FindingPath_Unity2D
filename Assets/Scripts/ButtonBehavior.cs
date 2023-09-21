using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonBehavior : MonoBehaviour
{
    public MainGameObject game_player;

    private void ResolveButton(MainGameObject.resolve_method mask)
    {

        game_player.gameObject.SetActive(false);
        game_player.gameObject.SetActive(true);
        game_player.resolve_mask = mask;
        game_player.state = 1;
    }

    public void BFSButton()
    {
        ResolveButton(MainGameObject.resolve_method.BFS_RESOLVE);
    }

    public void DFSButton()
    {
        ResolveButton(MainGameObject.resolve_method.DFS_RESOLVE);
    }

    public void AstarButton()
    {
        ResolveButton(MainGameObject.resolve_method.ASTAR_RESOLVE);
    }

    public void GreedyButton()
    {
        ResolveButton(MainGameObject.resolve_method.GREEDY_RESOLVE);
    }

}
