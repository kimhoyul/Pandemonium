using UnityEditor;
using UnityEngine;

public class GUIStyles
{
    public GUIStyle entranceNodeStyle = new();
    public GUIStyle entranceNodeSelectedStyle = new();

    public GUIStyle roomNodeStyle = new();
    public GUIStyle roomNodeSelectedStyle = new();

    public GUIStyle bossRoomNodeStyle = new();
    public GUIStyle bossRoomNodeSelectedStyle = new();

    public GUIStyle corridorNodeStyle = new();
    public GUIStyle corridorNodeSelectedStyle = new();

    public GUIStyle chestRoomNodeStyle = new();
    public GUIStyle chestRoomNodeSelectedStyle = new();

    public void Initialize()
    {
        SetupEntranceNodeStyle();
        SetupRoomNodeStyle();
        SetupBossRoomNodeStyle();
        SetupCorridorNodeStyle();
        SetupChestRoomNodeStyle();

        void SetupEntranceNodeStyle()
        {
            entranceNodeStyle = new GUIStyle();
            entranceNodeStyle.normal.background = EditorGUIUtility.Load("node3") as Texture2D;
            entranceNodeStyle.padding = new RectOffset(Settings.nodePadding, Settings.nodePadding, Settings.nodePadding, Settings.nodePadding);
            entranceNodeStyle.border = new RectOffset(Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder);

            entranceNodeSelectedStyle = new GUIStyle();
            entranceNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node3 on") as Texture2D;
            entranceNodeSelectedStyle.padding = entranceNodeStyle.padding;
            entranceNodeSelectedStyle.border = entranceNodeStyle.border;
        }

        void SetupRoomNodeStyle()
        {
            roomNodeStyle = new GUIStyle();
            roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            roomNodeStyle.padding = new RectOffset(Settings.nodePadding, Settings.nodePadding, Settings.nodePadding, Settings.nodePadding);
            roomNodeStyle.border = new RectOffset(Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder);

            roomNodeSelectedStyle = new GUIStyle();
            roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
            roomNodeSelectedStyle.padding = roomNodeStyle.padding;
            roomNodeSelectedStyle.border = roomNodeStyle.border;
        }

        void SetupBossRoomNodeStyle()
        {
            bossRoomNodeStyle = new GUIStyle();
            bossRoomNodeStyle.normal.background = EditorGUIUtility.Load("node6") as Texture2D;
            bossRoomNodeStyle.padding = new RectOffset(Settings.nodePadding, Settings.nodePadding, Settings.nodePadding, Settings.nodePadding);
            bossRoomNodeStyle.border = new RectOffset(Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder);

            bossRoomNodeSelectedStyle = new GUIStyle();
            bossRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node6 on") as Texture2D;
            bossRoomNodeSelectedStyle.padding = bossRoomNodeStyle.padding;
            bossRoomNodeSelectedStyle.border = bossRoomNodeStyle.border;
        }

        void SetupCorridorNodeStyle()
        {
            corridorNodeStyle = new GUIStyle();
            corridorNodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            corridorNodeStyle.padding = new RectOffset(Settings.nodePadding, Settings.nodePadding, Settings.nodePadding, Settings.nodePadding);
            corridorNodeStyle.border = new RectOffset(Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder);

            corridorNodeSelectedStyle = new GUIStyle();
            corridorNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node0 on") as Texture2D;
            corridorNodeSelectedStyle.padding = corridorNodeStyle.padding;
            corridorNodeSelectedStyle.border = corridorNodeStyle.border;
        }

        void SetupChestRoomNodeStyle()
        {
            chestRoomNodeStyle = new GUIStyle();
            chestRoomNodeStyle.normal.background = EditorGUIUtility.Load("node4") as Texture2D;
            chestRoomNodeStyle.padding = new RectOffset(Settings.nodePadding, Settings.nodePadding, Settings.nodePadding, Settings.nodePadding);
            chestRoomNodeStyle.border = new RectOffset(Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder, Settings.nodeBorder);

            chestRoomNodeSelectedStyle = new GUIStyle();
            chestRoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node4 on") as Texture2D;
            chestRoomNodeSelectedStyle.padding = chestRoomNodeStyle.padding;
            chestRoomNodeSelectedStyle.border = chestRoomNodeStyle.border;
        }

    }
}