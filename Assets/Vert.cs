using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections ;
[ExecuteInEditMode]

//https://docs.unity3d.com/ScriptReference/Graphics.RenderPrimitives.html
public class Vert : MonoBehaviour
{
    // public Texture texture;
    GraphicsBuffer vertBuffer;
    GraphicsBuffer dataBuffer;
    RenderParams rp;

    const int SQAURE_DATA_SIZE = 8;
    const int GRID_SIZE = 100;
    const int SQAURE_NUMS = GRID_SIZE*GRID_SIZE;

    const int VERT_COUNT = 6;
    
    float[] dataArr = new float [SQAURE_NUMS*SQAURE_DATA_SIZE];
    void Start()
    
    {   
        Debug.Log(SystemInfo.maxGraphicsBufferSize);
         // note: remember to check "Read/Write" on the mesh asset to get access to the geometry data
        Shader shader = Shader.Find("testVert");
        Material material = new Material(shader);
        //https://docs.unity3d.com/ScriptReference/Shader.PropertyToID.html  && https://forum.unity.com/threads/propertytoid-multiple-shaders.662770/
        int mainTexID = Shader.PropertyToID("_MainTex");


        // Texture2D texture = new Texture2D(2, height: 2,TextureFormat . RGBA32 , false);
        Texture2D texture = LoadFromFile("Assets/texture/bg.png");
        material.SetTexture(mainTexID, texture);
        rp = new RenderParams(material);

        vertBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, VERT_COUNT, 2 * sizeof(float));
        NativeArray<float> bufferArray =  new NativeArray<float>(12, Allocator.Temp){
            [0] = 0, [1] = 1,
            [2] = 1, [3] = 1,
            [4] = 1, [5] = 0,
            [6] = 1, [7] = 0,
            [8] = 0, [9] = 0,
            [10] = 0, [11] = 1,
        };

        vertBuffer.SetData(bufferArray);

        //float3 index [col,row, Z-index], float4 boundary
        dataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, SQAURE_NUMS * sizeof(float), SQAURE_DATA_SIZE);
        for(int i=0; i<dataArr.Length/SQAURE_DATA_SIZE;i++){
            int index = i*SQAURE_DATA_SIZE;
            int col = i%GRID_SIZE;
            int row = (int) i/GRID_SIZE;
            dataArr[index] = col;  dataArr[index+1] = row;
            int randCol = (int) UnityEngine.Random.Range(1.0f, 10.0f);
            int randRow = (int) UnityEngine.Random.Range(1.0f, 10.0f);
            Debug.Log(randCol);
            Debug.Log(randRow);
            dataArr[index+2] = randCol; dataArr[index+3] = randRow; dataArr[index+4] = 1; dataArr[index+5] = 1;dataArr[index+6] = 0;dataArr[index+7] = 0;

        }
        string result = "List contents: ";
        foreach (var item in dataArr)
        {
            result += item.ToString() + ", ";
        }
        // Debug.Log(result);
        // Debug.Log(dataArr.Length);

        NativeArray<float> dataBufferArr =  new NativeArray<float>(dataArr, Allocator.Temp);
        // {
        //     [0] = 0,    [1] = 0,//col, row 
        //     [2] = 1,    [3] = 0,
        //     [4] = 2,    [5] = 0,
        //     [6] = 3,    [7] = 0,
        //     [8] = 4,    [9] = 0,
        //     [10] = 5,   [11] = 0,
        //     [12] = 6,    [13] = 0,//col, row
        //     [14] = 7,    [15] = 0,//col, row


        //     // //dstCol, dstRow,           boundary = [col,row,sizex,sizey]
        //     // [0] = 0,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 1,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 2,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 3,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 4,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 5,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 6,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,
        //     // [0] = 7,  [0] = 0,      [0] = 0,[0] = 0,[0] = 1,[0] = 1,

        // };
        dataBuffer.SetData(dataBufferArr);
    }
    void OnDisable(){
    vertBuffer?.Release();
    dataBuffer?.Release();


    }

    // Update is called once per frame
    void Update()
    {

        rp.worldBounds = new Bounds(Vector3.zero, 1000*Vector3.one); // use tighter bounds
        rp.matProps = new MaterialPropertyBlock();

        rp.matProps.SetBuffer("verticeBuff", vertBuffer);
        rp.matProps.SetBuffer("gridIndexBuff", dataBuffer);
        Graphics.RenderPrimitives(rp, MeshTopology.Triangles, (int)VERT_COUNT, SQAURE_NUMS);  
    }
     private Texture2D LoadFromFile(string path)
    {
        Texture2D m_Tex = new Texture2D(1, 1);
        m_Tex.LoadImage(ReadPNG(path));
        return m_Tex;
    }
    private byte[] ReadPNG(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read);

        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] binary = new byte[fileStream.Length]; //创建文件长度的buffer
        fileStream.Read(binary, 0, (int)fileStream.Length);

        fileStream.Close();

        fileStream.Dispose();

        fileStream = null;

        return binary;
    }

}
