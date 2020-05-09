using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    private Texture2D grassDensityMap;
    private int grassAmountPerTile = 1;//实际渲染时每个Tile内最多的草叶数量
    public int pregenerateGrassAmount = 1023;//预生成Patch草体总长度
    public int bladeSectionCount = 5;//草叶分段，5段12顶点，6段14顶点
    public Vector2 Height = new Vector2(0.5f, 1f);
    public Vector2 Width = new Vector2(0.3f, 0.4f);
    public Material grassMaterial;
    private float patchSize;
    private ComputeBuffer grassPatchBuffer;


    struct GrassData
    {
        //         public float height, density;
        //         public Vector4 rootDir;
        //         public GrassData(float height, float density, Vector4 rootDir) {
        //             this.height = height;this.density = density;this.rootDir = rootDir;
        //         }

        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;
        public Vector3 Vertex4;
        public Vector3 Vertex5;
        public Vector3 Vertex6;
        public Vector3 Vertex7;
        public Vector3 Vertex8;
        public Vector3 Vertex9;
        public Vector3 Vertex10;
        public Vector3 Vertex11;
        public Vector3 Vertex12;

        //12
        public GrassData(Vector3[] v)
        {
            Vertex1 = Vector3.zero;
            Vertex2 = Vector3.zero;
            Vertex3 = Vector3.zero;
            Vertex4 = Vector3.zero;
            Vertex5 = Vector3.zero;
            Vertex6 = Vector3.zero;
            Vertex7 = Vector3.zero;
            Vertex8 = Vector3.zero;
            Vertex9 = Vector3.zero;
            Vertex10 = Vector3.zero;
            Vertex11 = Vector3.zero;
            Vertex12 = Vector3.zero;

            for (int i =0; i < v.Length; i++)
            {
                switch(i)
                {
                    case 0: Vertex1 = v[i]; break;
                    case 1: Vertex2 = v[i]; break;
                    case 2: Vertex3 = v[i]; break;
                    case 3: Vertex4 = v[i]; break;
                    case 4: Vertex5 = v[i]; break;
                    case 5: Vertex6 = v[i]; break;
                    case 6: Vertex7 = v[i]; break;
                    case 7: Vertex8 = v[i]; break;
                    case 8: Vertex9 = v[i]; break;
                    case 9: Vertex10 = v[i]; break;
                    case 10: Vertex11 = v[i]; break;
                    case 11: Vertex12 = v[i]; break;
                    default: break;
                }
            }
        }
    }

    /*public void GenShadowMap() {

    }*/

    /// <summary>
    /// 预生成草地信息数组，传输给grassMaterial
    /// </summary>
    public void PregenerateGrassPatch() 
    {
        GrassData[] grassData = new GrassData[pregenerateGrassAmount];
        int bladeVertexCount = (bladeSectionCount + 1) * 2;

        //随机生成草根位置、方向、高度、密度索引
        for (int i = 0; i < pregenerateGrassAmount; i++) 
        {
            //             float deltaX = (float)random.NextDouble();
            //             float deltaZ = (float)random.NextDouble();
            //             Vector3 root = new Vector3(deltaX * patchSize, 0, deltaZ * patchSize);
            // 
            //             GrassData data = new GrassData(0.5f + 0.5f * (float)random.NextDouble(),
            //                 (float)random.NextDouble(),
            //                 new Vector4(root.x, root.y, root.z, (float)random.NextDouble()));
            //             grassData[i] = data;
            float height = Random.Range(Height.x, Height.y);
            float width = Random.Range(Width.x, Width.y);
            Vector3[] vertices = new Vector3[bladeVertexCount];

            for (int j = 0; j < bladeVertexCount; j++)
            {

                //赋予x坐标，为了使其作为索引在gpu中读取数组信息
                vertices[j] = new Vector3((j % 2 - 0.5f) * width,
                                        ((float)(j / 2)) / bladeSectionCount * height, 0);//0-63,0-11,0

            }

            GrassData data = new GrassData(vertices);

            grassData[i] = data;

        }
        grassPatchBuffer = new ComputeBuffer(pregenerateGrassAmount, sizeof(float) * 3 *12);
        grassPatchBuffer.SetData(grassData);
        //send to gpu
        grassMaterial.SetInt("_SectionCount", bladeSectionCount);
        Shader.SetGlobalFloat("_TileSize", patchSize);
        grassMaterial.SetBuffer("_patchData", grassPatchBuffer);

        //test
        Shader.SetGlobalInt("_SectionCount", bladeSectionCount);
        Shader.SetGlobalBuffer("_patchData", grassPatchBuffer);
    }


    /// <summary>
    /// generate a mesh with assigned numbers of points
    /// </summary>
    /// 生成一棵草
    public Mesh GenerateGrassMesh() {
        grassAmountPerTile = 1;
        Mesh result = new Mesh();
        int bladeVertexCount = (bladeSectionCount + 1) * 2;
        Vector3[] normals = new Vector3[grassAmountPerTile * bladeVertexCount];
        Vector3[] vertices = new Vector3[grassAmountPerTile * bladeVertexCount];
        Vector2[] uvs = new Vector2[grassAmountPerTile * bladeVertexCount];

        float height = Random.Range(Height.x, Height.y);
        float width = Random.Range(Width.x, Width.y);

        for (int i = 0; i < vertices.Length; i++) {
            //赋予x坐标，为了使其作为索引在gpu中读取数组信息
            //             vertices[i] = new Vector3((i % bladeVertexCount % 2 - 0.5f) * width, 
            //                                         ((float)(i % bladeVertexCount / 2)) / bladeSectionCount * height, 0);//0-63,0-11,0
            vertices[i] = new Vector3(0, i, 0);
            normals[i] = -Vector3.forward;
            uvs[i] = new Vector2(i % bladeVertexCount % 2,
                ((float)(i % bladeVertexCount / 2)) / bladeSectionCount);
        }
        result.vertices = vertices;

        int[] triangles = new int[6 * grassAmountPerTile * bladeSectionCount];
        int trii = 0;
        for (int blade = 0; blade < grassAmountPerTile; blade++) {
            for (int section = 0; section < bladeSectionCount; section++) {
                int start = blade * bladeVertexCount + section * 2;
                triangles[trii] = start;
                triangles[trii + 1] = start + 3;
                triangles[trii + 2] = start + 1;

                triangles[trii + 3] = start;
                triangles[trii + 4] = start + 2;
                triangles[trii + 5] = start + 3;
                trii += 6;
            }
        }
        result.triangles = triangles;
        result.normals = normals;
        result.uv = uvs;
        return result;
    }

    public Mesh GenerateGrassMesh(int num)
    {
        grassAmountPerTile = num;
        Mesh result = new Mesh();
        int bladeVertexCount = (bladeSectionCount + 1) * 2;
        Vector3[] normals = new Vector3[grassAmountPerTile * bladeVertexCount];
        Vector3[] vertices = new Vector3[grassAmountPerTile * bladeVertexCount];
        Vector2[] uvs = new Vector2[grassAmountPerTile * bladeVertexCount];

        for (int i = 0; i < grassAmountPerTile; i++)
        {
            float height = Random.Range(Height.x, Height.y);
            float width = Random.Range(Width.x, Width.y);

            for (int j = 0; j < bladeVertexCount; j++)
            {
                int index = i * bladeVertexCount + j;

                //赋予x坐标，为了使其作为索引在gpu中读取数组信息
                vertices[index] = new Vector3((j % 2 - 0.5f) * width,
                                        ((float)(j / 2)) / bladeSectionCount * height, 0);//0-63,0-11,0

                vertices[index] += new Vector3(i * width, 0, 0);

                normals[index] = -Vector3.forward;
                uvs[index] = new Vector2(j % 2,
                    ((float)(j / 2)) / bladeSectionCount);
            }
        }
        result.vertices = vertices;

        int[] triangles = new int[6 * grassAmountPerTile * bladeSectionCount];
        int trii = 0;
        for (int blade = 0; blade < grassAmountPerTile; blade++)
        {
            for (int section = 0; section < bladeSectionCount; section++)
            {
                int start = blade * bladeVertexCount + section * 2;
                triangles[trii] = start;
                triangles[trii + 1] = start + 3;
                triangles[trii + 2] = start + 1;

                triangles[trii + 3] = start;
                triangles[trii + 4] = start + 2;
                triangles[trii + 5] = start + 3;
                trii += 6;
            }
        }
        result.triangles = triangles;
        result.normals = normals;
        result.uv = uvs;
        return result;
    }

    public GrassGenerator(Texture2D densityMap, int grassAmountPTile, int pregenerateLen, Material grass, float patchSize) {
        grassDensityMap = densityMap;
        grassAmountPerTile = grassAmountPTile;
        pregenerateGrassAmount = pregenerateLen;
        grassMaterial = grass;
        this.patchSize = patchSize;
    }

    public Texture GetTerrainDensityTexture() { return grassDensityMap; }

    public void OnDestroy() {
        grassPatchBuffer.Release();
    }
    ///show grass
        /*GameObject grass = new GameObject("grass", typeof(MeshRenderer), typeof(MeshFilter));
        grass.transform.parent = transform;
        grass.GetComponent<MeshFilter>().mesh = grassMesh;
        grass.GetComponent<MeshRenderer>().sharedMaterial = grassMaterial;
        */

}