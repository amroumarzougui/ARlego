using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/*[Flags]
public enum BlockCollision
{
    Left=1,
    Right=2,
    Forward=4,
    Backward=8,
    Up=16,
    Down=32
}*/

public class Block
{
    public Transform blockTransform;
    public BlockColor color;
}

public enum BlockColor
{
    white=0,
    red=1,
    green=2,
    blue=3
}
public struct BlockAction
{
    public bool delete;
    public Vector3 index;
    public BlockColor color;
}

public class GameManager : MonoBehaviour
{
    private EventSystem es;
    public static GameManager Instance { set; get; }

    public float blocksize = 0.5f;
    public Block[,,] blocks = new Block[20, 20, 20];
    private GameObject foundationObject;

    public GameObject blockPrefab;
    public Material[] blockMaterials;

    public BlockColor selectedColor;
    private bool isDeleting;

    private BlockAction previewAction;

    public Button deletButton;
    public Sprite[] DeleteButtons;


    private Vector3 blockOffset;
    private Vector3 foundationCenter = new Vector3(2.5f, 0, 2.5f);
    private void Start()
    {
        Instance = this;
        foundationObject = GameObject.Find("Foundation");
        blockOffset = (Vector3.one*1.0f)/4;
        selectedColor = BlockColor.white;
        es = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (es.IsPointerOverGameObject())
                return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 30.0f))
            {
               /* if (hit.transform.gameObject != foundationObject)
                {
                    Vector3 blockPostion = hit.transform.position;
                    BlockCollision c = 0;

                    if (hit.point.x - blockPostion.x == 0.5f)
                        c |= BlockCollision.Right;
                    else if (hit.point.x - blockPostion.x == -0.5f)
                        c |= BlockCollision.Left;
                    else if (hit.point.z - blockPostion.x == 0.5f)
                        c |= BlockCollision.Forward;
                    else if (hit.point.z - blockPostion.x == -0.5f)
                        c |= BlockCollision.Backward;
                    else if (hit.point.y - blockPostion.x == 0.5f)
                        c |= BlockCollision.Up;
                    else c |= BlockCollision.Down;
                }*/
                
                //go.transform.position = hit.point;
               

                if (isDeleting )
                {
                    if (hit.transform.name != "Foundation")
                    {
                        Vector3 oldCubeIndex = BlockPosition(hit.point - (hit.normal * blocksize));
                        BlockColor previousColor = blocks[(int)oldCubeIndex.x, (int)oldCubeIndex.y, (int)oldCubeIndex.z].color;
                        Destroy(blocks[(int)oldCubeIndex.x, (int)oldCubeIndex.y, (int)oldCubeIndex.z].blockTransform.gameObject);
                        blocks[(int)oldCubeIndex.x, (int)oldCubeIndex.y, (int)oldCubeIndex.z] = null;

                        previewAction = new BlockAction
                        {
                            delete = true,
                            index = oldCubeIndex,
                            color = previousColor
                        };

                    }
                   
                    return;
                }
                Vector3 index = BlockPosition(hit.point);

                int x = (int)index.x
                    , y = (int)index.y
                    , z = (int)index.z;

                if (blocks[x, y, z] == null)
                {
                    GameObject go = CreateBlock();
               
                    PositionBlock(go.transform, index);
                    blocks[x, y, z] = new Block
                    {
                        blockTransform = go.transform,
                        color= selectedColor
                    };

                    previewAction = new BlockAction
                    {
                        delete = false,
                        index = new Vector3(x,y,z) ,
                        color = selectedColor
                    };

                }
                else
                {
                    //Debug.Log("Eroor: clicking inside of a cube at position" + index.ToString());
                    GameObject go = CreateBlock();
                 
                    Vector3 newIndex = BlockPosition(hit.point + (hit.normal)*blocksize);

                    blocks[(int)newIndex.x, (int)newIndex.y, (int)newIndex.z] = new Block
                    {
                        blockTransform = go.transform,
                        color = selectedColor
                    };

                    PositionBlock(go.transform, newIndex);

                    previewAction = new BlockAction
                    {
                        delete = false,
                        index = newIndex,
                        color = selectedColor
                    };

                }
            }
        }
    }
    private GameObject CreateBlock()

    {
        GameObject go = Instantiate(blockPrefab) as GameObject;
        go.GetComponent<Renderer>().material = blockMaterials[(int)selectedColor];
        go.transform.localScale = Vector3.one * blocksize;
        return go;
    }

    public GameObject CreateBlock(int x, int y, int z,Block b)
    {
        GameObject go = Instantiate(blockPrefab) as GameObject;
        go.GetComponent<Renderer>().material = blockMaterials[(int)b.color];
        go.transform.localScale = Vector3.one * blocksize;

        b.blockTransform = go.transform;
        blocks[x, y, z] = b;

        PositionBlock(b.blockTransform, new Vector3(x, y, z));
        return go;
    }

    private Vector3 BlockPosition(Vector3 hit)
    {
        int x = (int)(hit.x / blocksize);
        int y = (int)(hit.y / blocksize);
        int z = (int)(hit.z / blocksize);
        return new Vector3(x, y, z);
    }

    public void PositionBlock(Transform t,Vector3 index)
    {
        t.position = ((index*blocksize) + blockOffset) + (foundationObject.transform.position - foundationCenter);
    }
    public void ChangeBlockColor(int color)
    {
        selectedColor = (BlockColor)color;

     
       
    }
    public void TuggleDelet()
    {
        isDeleting = !isDeleting;
        deletButton.image.sprite = (!isDeleting) ? DeleteButtons[0] : DeleteButtons[1];
    }

    public void undo()
    {
        if (previewAction.delete)
        {
            GameObject go = CreateBlock();
            
            blocks[(int)previewAction.index.x, (int)previewAction.index.y, (int)previewAction.index.z] = new Block
            {
                blockTransform = go.transform,
                color = selectedColor
            };

            PositionBlock(go.transform, previewAction.index);

            previewAction = new BlockAction
            {
                delete = false,
                index = previewAction.index,
                color = previewAction.color
            };
        }
        else
        {
           
                
                Destroy(blocks[(int)previewAction.index.x, (int)previewAction.index.y, (int)previewAction.index.z].blockTransform.gameObject);
                blocks[(int)previewAction.index.x, (int)previewAction.index.y, (int)previewAction.index.z] = null;

                previewAction = new BlockAction
                {
                    delete = true,
                    index = previewAction.index,
                    color = previewAction.color
                };

            

        }
    }

    public void RefreshGrid()
    {
        for(int i=0;i<20;i++)
            for (int j = 0; j < 20; j++)
                for (int k = 0; k < 20; k++)
                {
                    if (blocks[i, j, k] == null)
                        continue;
                    Destroy(blocks[i, j, k].blockTransform.gameObject);
                    blocks[i, j, k] = null;
                }
    }
}
