using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CreateLobbyScene : ScriptableObject {

    [MenuItem("Tools/Lobby/Build Lobby")]
    public static void BuildLobby()
    {
        GameObject lobbyContainer = GameObject.Find("Lobby Container");
        GameObject backgroundContainer = GameObject.Find("Background Container");
        GameObject floorContainer = GameObject.Find("Floor Container");

        int floorTile = 8;
        int outlineTile = 9;
        int lobbyWall = 15;

        float sizeOfTile = .64f;
        
        ArrayList assets = new ArrayList();


        string[] dataFiles = Directory.GetFiles(Application.dataPath + "/Sprites/stage - lobby/", "*.png", SearchOption.AllDirectories);

        foreach (string asset in dataFiles)
        {
            string assetPath = "Assets" + asset.Replace(Application.dataPath, "").Replace('\\', '/');
            Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
            assets.Add(sprite);

        }


        foreach (Object asset in assets)
        {
            Debug.Log(asset);
        }

        Object tilePrototype = AssetDatabase.LoadAssetAtPath<Object>("Assets/Resources/Prefabs/Tile.prefab");

        int xTiles = 50;
        int yTiles = 12;
        for (int x = 0; x < xTiles; x++)
        {
            float posX = (x * sizeOfTile) - (sizeOfTile * xTiles / 2) + sizeOfTile;
            for (int y = 0; y < yTiles; y++)
            {
                float posY = (y * sizeOfTile) - (sizeOfTile * yTiles / 2) + sizeOfTile;

                Vector3 newPosition = new Vector3(posX, posY, 0f);

                Transform parent = (y == 0 && x != 0 && x != xTiles - 1) ? floorContainer.transform : lobbyContainer.transform;

                GameObject newBlock = (GameObject)Instantiate(tilePrototype, newPosition, Quaternion.identity, parent);

                newBlock.name = "Tile x: " + posX + " y: " + posY;

                newBlock.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                Sprite sprite = (Sprite)assets[lobbyWall];
                if (x == 0 || x == xTiles - 1 || y == yTiles - 1)
                {
                    sprite = (Sprite)assets[floorTile];

                    BoxCollider2D collider = newBlock.AddComponent<BoxCollider2D>();
                    

                    newBlock.layer = 8;
                }

                if (y == 0 && x != 0 && x != xTiles - 1)
                {
                    sprite = (Sprite)assets[outlineTile];
                    BoxCollider2D collider = newBlock.AddComponent<BoxCollider2D>();
                    newBlock.layer = 8;
                }

                newBlock.GetComponent<SpriteRenderer>().sprite = sprite;

                newBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Lobby";
                newBlock.GetComponent<SpriteRenderer>().sortingOrder = -1;

            }
        }
    }


    [MenuItem("Tools/Lobby/Delete Lobby Tiles")]
    public static void ClearLobbyTiles()
    {
        GameObject lobbyContainer = GameObject.Find("Lobby Container");
        Transform container = lobbyContainer.transform;

        while (container.childCount > 0)
        {
            Transform t = container.GetChild(0);
            t.SetParent(null);
            DestroyImmediate(t.gameObject);
        }

        GameObject floorContainer = GameObject.Find("Floor Container");
        container = floorContainer.transform;

        while (container.childCount > 0)
        {
            Transform t = container.GetChild(0);
            t.SetParent(null);
            DestroyImmediate(t.gameObject);
        }
    }

    [MenuItem("Tools/Lobby/Delete Floor Tiles")]
    public static void ClearFloorTiles()
    {

        GameObject floorContainer = GameObject.Find("Floor Container");
        Transform container = floorContainer.transform;

        while (container.childCount > 0)
        {
            Transform t = container.GetChild(0);
            t.SetParent(null);
            DestroyImmediate(t.gameObject);
        }
    }
}

#endif