using UnityEngine;

[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/플레이어/현재 플레이어")]
public class CurrentPlayerSO : ScriptableObject
{
    public PlayerDetailsSO playerDetails;
    public string playerName;


}
