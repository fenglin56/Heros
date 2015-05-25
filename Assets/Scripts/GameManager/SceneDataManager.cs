using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SceneDataManager : MonoBehaviour {

    private const int TILEPOSX_WIDTH	=5	;			// Tile格子宽  (分米)
    private const int TILEPOSY_HEIGHT = 5;			// Tile格子高（分米）

    //public GameObject Block;
    public SceneConfigDataBase SceneConfigData;
    //场景动态阻挡配置区域
    public MapDynamicBlockDataBase MapDynamicBlockFile;

    private Dictionary<int, SceneConfigData> m_sceneConfigDict = new Dictionary<int, SceneConfigData>();
    private Dictionary<uint,TriggerAreaInfo[]> m_triggerAreaInfos;  //地图触发刷怪区域

    private BlockCell[,] m_blockCells;
    private Dictionary<int, BlockCell[]> m_dynamicBlockCell;
    private int m_heightBound, m_widthBound;
    //private List<BlockCell> m_blockCells = new List<BlockCell>();

    private static SceneDataManager m_instance;
    public static SceneDataManager Instance { get { return m_instance; } }
	public List<SMsgEctypeUpDateBlock> CacheInitBlocks{ get; private set; }
    void Awake()
    {
        m_instance = this;
        m_dynamicBlockCell = new Dictionary<int, BlockCell[]>();
        m_triggerAreaInfos = new Dictionary<uint, TriggerAreaInfo[]>();
        Load();
    }

    void OnDestroy()
    {
        m_instance = null;
    }
    public IEnumerator InitBlockCells(uint mapID)
    {
        var sceneConfigData = GetSceneData((int)mapID);
		string blockName = sceneConfigData._szMapFile + ".block";
		string dyblockName = sceneConfigData._szMapFile + ".dblock";
        var filePath = Path.Combine(Application.streamingAssetsPath, blockName);
        var dfilePath = Path.Combine(Application.streamingAssetsPath, dyblockName);
       

        if (filePath.Contains("://"))
        {
            var www = new WWW(filePath);
            yield return www;
			if(string.IsNullOrEmpty(www.error) && www.bytes.Length > 0)
			{
            	InitBlockCells(sceneConfigData,www.bytes);
			}
        }
        else
        {
			if (!File.Exists(filePath))
			{
				yield break;
			}

            yield return null;
            //TraceUtil.Log("Start Read:" + Time.realtimeSinceStartup);
            byte[] datas = File.ReadAllBytes(filePath);
            //TraceUtil.Log("Start Read:" + Time.realtimeSinceStartup);
            InitBlockCells(sceneConfigData, datas);
            //TraceUtil.Log("Start Read:" + Time.realtimeSinceStartup);
        }
       
        if (dfilePath.Contains("://"))
        {
            var www = new WWW(dfilePath);
            yield return www;
			if(string.IsNullOrEmpty(www.error) && www.bytes.Length > 0)
			{
				InitDynamicBlockCells(www.bytes);
			}
        }
        else
        {
			if (!File.Exists(dfilePath))
			{
				yield break;
			}
            yield return null;
            //TraceUtil.Log("Start Read:" + Time.realtimeSinceStartup);
            byte[] datas = File.ReadAllBytes(dfilePath);
            //TraceUtil.Log("Start Read:" + Time.realtimeSinceStartup);
            InitDynamicBlockCells(datas);
            //TraceUtil.Log("Start Read:" + Time.realtimeSinceStartup);
        }
    }
    public void ClearBlockCells()
    {
		GameManager.Instance.ClearBlockList();
        m_blockCells = null;
    }


    public bool IsPositionInBlock(Vector3 position)
    {
        bool flag = false;
        if (m_blockCells != null)
        {
            var blockCell = GetBlockCellOfCoor(position);
            if (blockCell != null)
            {
                flag = blockCell.IsBlock;
                if (!flag)
                {
                    //检查是否在动态阻挡中
                    foreach (var dBlock in m_dynamicBlockCell.Values)
                    {
                        foreach (var cell in dBlock)
                        {
                            if (blockCell.Index == cell.Index && cell.IsBlock)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return flag;
    }
    /// <summary>
    /// 改变动态阻挡状态
    /// </summary>
    /// <param name="groupId"></param>
    public IEnumerator ChangeDynamicBlockGroupState(int groupId, bool openFlag,float effctDelay)
    {
        if (m_dynamicBlockCell.ContainsKey(groupId))
        {
            yield return new WaitForSeconds(effctDelay/1000f);
            m_dynamicBlockCell[groupId].ApplyAllItem(P => P.BlockValue = (byte)(openFlag ? 2 : 1));

          //  m_dynamicBlockCell[groupId].ApplyAllItem(P => { if (P.IsBlock) Debug.Log(P.Index); });
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, "动态阻挡组不存在："+groupId);
        }
    }
    
	public enum ColideType
	{
		COLIDE_UPDOWN,
		COLIDE_LEFTRIGHT,
		
	}
	
	public ColideType GetColideType( Vector3 currentPos, Vector3 nextPos )
	{
		int h=Mathf.CeilToInt(nextPos.x / TILEPOSX_WIDTH);
        int v = Mathf.CeilToInt(nextPos.z / TILEPOSY_HEIGHT) * -1;
		float BlockCenterX = TILEPOSX_WIDTH*((float)h + 0.5f);
		float BlockCenterY = TILEPOSY_HEIGHT*((float)v + 0.5f);
		
		float deltaX = currentPos.x - BlockCenterX;
		float deltaY = (-currentPos.z) - BlockCenterY;
		
		if(deltaX <= 0.00001f && deltaX >= -0.00001f) 
		{
			return ColideType.COLIDE_UPDOWN;
		}
		else
		{
			float num = deltaY/deltaX;
			if(num >= -1 && num <= 1)
			{
				//TraceUtil.Log("@@@@@@@@@@@Colide Left Right@!!!!!");
				return ColideType.COLIDE_LEFTRIGHT;	
			}
			else
			{
				//TraceUtil.Log("@@@@@@@@@@@Colide Up Down@!!!!!");
				return ColideType.COLIDE_UPDOWN;	
			}
		}
	}
	
    /// <summary>
    /// 根据坐标定位到宫格
    /// </summary>
    private BlockCell GetBlockCellOfCoor(Vector3 position)
    {
        int h=Mathf.FloorToInt(position.x / TILEPOSX_WIDTH);
        int v = Mathf.CeilToInt(position.z / TILEPOSY_HEIGHT) * -1;
        if (v >= 0 && v < m_heightBound && h >= 0 && h < m_widthBound)
        {
            return this.m_blockCells[v, h];
        }
        else
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Not Found cell:" + h + "  " + v);
            return null;
        }
    }
    private void Load()
    {
        foreach (SceneConfigData data in SceneConfigData._dataTable)
        {
            m_sceneConfigDict.Add(data._lMapID, data);            
            if (data._TriggerAreaPoint != null && data._TriggerAreaPoint.Length > 0)
            {
                m_triggerAreaInfos.Add((uint)data._lMapID, new TriggerAreaInfo[data._TriggerAreaPoint.Length]);
                for (int i = 0; i < data._TriggerAreaPoint.Length; i++)
                {
                    m_triggerAreaInfos[(uint)data._lMapID][i]=new TriggerAreaInfo() { AreaId = i + 1, Center = data._TriggerAreaPoint[i], Radius = data._TriggerAreaRadius[i], CanBeTrigger = true };
                }
            }
        }
    }
    public int GetMapTriggerAreaId(uint mapId,Vector3 checkPos)
    {
        int triggerAreaId = -1;
        if (m_triggerAreaInfos.ContainsKey(mapId))
        {
            var triggerAreaInfos = m_triggerAreaInfos[mapId];
            foreach (var areaInfo in triggerAreaInfos)
            {
                if (areaInfo.InTriggerBound(checkPos))
                {
                    triggerAreaId = areaInfo.AreaId;
                    break;
                }
            }
        }
        
        return triggerAreaId;
    }
    /// <summary>
    /// 退出副本时,重置触发区域的可触发属性
    /// </summary>
    public void ResetMapTridderAreaInfo(uint mapId)
    {
        if(m_triggerAreaInfos.ContainsKey(mapId))
        {
            m_triggerAreaInfos[mapId].ApplyAllItem(P => P.CanBeTrigger = true);
        }
    }
    public TriggerAreaInfo GetTriggerAreaInfo(int areaId)
    {
        return m_triggerAreaInfos[GameManager.Instance.GetCurSceneMapID].SingleOrDefault(P => P.AreaId == areaId);
    }
    public SceneConfigData GetSceneData(int mapID)
    {
        SceneConfigData data = null;
        if (m_sceneConfigDict.ContainsKey(mapID))
        {
            m_sceneConfigDict.TryGetValue(mapID, out data);
        }
        return data;
    }
    public MapDynamicBlockData GetMapDynamicBlockData(int areaId,int groupId)
    {
        return MapDynamicBlockFile.Datas.SingleOrDefault(P => P.MapID == GameManager.Instance.GetCurSceneMapID &&P.AreaID==areaId && P.BlockGroup == groupId);
    }
    public MapDynamicBlockData GetMapDynamicBlockData(int groupId)
    {
        return MapDynamicBlockFile.Datas.FirstOrDefault(P => P.MapID == GameManager.Instance.GetCurSceneMapID && P.BlockGroup == groupId);
    }

    public void ClearDynamicBlockData()
	{
		if(m_dynamicBlockCell != null)
		{
			m_dynamicBlockCell.Clear();
		}

		if(CacheInitBlocks != null)
		{
			CacheInitBlocks.Clear();
			CacheInitBlocks = null;
		}
	}

    private void InitDynamicBlockCells(byte[] data)
    {
        int offset = 9, blockNum;
        byte dblockGroupNum,groudId;
        m_dynamicBlockCell.Clear();
        offset += PackageHelper.ReadData(data, out dblockGroupNum, offset); //动态阻挡组数
		CacheInitBlocks = new List<SMsgEctypeUpDateBlock> ();
        for (int i = 0; i < dblockGroupNum; i++)
        {
            offset += PackageHelper.ReadData(data, out groudId,offset); //动态阻挡组Id
            offset += PackageHelper.ReadData(data, out blockNum, offset); //动态阻挡组包含的动态阻挡点数量

            var dynamicData= GetMapDynamicBlockData(groudId);

            m_dynamicBlockCell.Add(groudId, new BlockCell[blockNum]);

            int nIndex;
            byte byBlock;
            for (int j = 0; j < blockNum; j++)
            {
                offset += PackageHelper.ReadData(data, out nIndex,offset); //动态阻挡点序号
                offset += PackageHelper.ReadData(data, out byBlock,offset); //动态阻挡点属性
                m_dynamicBlockCell[groudId][j] = new BlockCell() { Index = nIndex, BlockValue = (byte)(dynamicData.BlockState==0?2:1)};  //初始化所有动态阻挡都为开放状态

				//print dynamic block
				if(GameManager.Instance.ShowDynamicBlock)
				{
					int h = nIndex % m_widthBound;
					int v = Mathf.CeilToInt( nIndex/m_widthBound);
					Vector3 pos = new Vector3((float)(h + 0.5f)*TILEPOSX_WIDTH, 1, -(float)(v+0.5f)*TILEPOSY_HEIGHT);
					GameManager.Instance.AddBlockCell(pos);	
				}

			}

            if (dynamicData.BlockState != 0)
            {
                if (!CacheInitBlocks.Exists(P => P.dwblockGroupID == groudId))
                {
                    CacheInitBlocks.Add(new SMsgEctypeUpDateBlock() { dwareaId = dynamicData.AreaID, dwblockGroupID = groudId, byBlockState = 1 });


				}
            }
        }
    }

    private void InitBlockCells(SceneConfigData configData, byte[] data)
    {
        //Block文件的文件头，跳过指定字节数（char[5]+float = 9)//
        //struct SMapFileDataInfo
        //{
        //    char	fileHeader[5];
        //    float	Virsion;
        //    int		dwMapHeight;   //高度格子数量
        //    int		dwMapWidth;	   //宽度格子数量

        //    SMapFileDataInfo()
        //    {
        //        //TileBlock = NULL;
        //        Virsion = 0;
        //        dwMapHeight = 0;
        //        dwMapWidth = 0;
        //        memset(fileHeader, 0, sizeof(fileHeader));
        //    }	
        //};
        int offset = 9, dwMapHeight, dwMapWidth;
        offset += PackageHelper.ReadData(data.Skip(offset).ToArray(), out dwMapWidth);
        offset += PackageHelper.ReadData(data.Skip(offset).ToArray(), out dwMapHeight);
        //计算地图上的宫格数
        //TraceUtil.Log(dwMapHeight + "  " + dwMapWidth);
        m_heightBound = dwMapHeight;
        m_widthBound = dwMapWidth;
        m_blockCells = new BlockCell[m_heightBound, m_widthBound];
        int blockIndex = 0;
        for (int v = 0; v < dwMapHeight; v++)
        {
            for (int h = 0; h < dwMapWidth; h++)
            {
                m_blockCells[v, h] = new BlockCell() { Index=blockIndex++, VIndex = v, HIndex = h, BlockValue = data[offset] };
                //if (m_blockCells[v, h].IsBlock)
                //{
                //    Debug.Log(blockIndex + "   " + v + "  " + h);
                    //GameObject.Instantiate(Block, new Vector3(h * TILEPOSX_WIDTH / 2, 1, v * TILEPOSY_HEIGHT / 2), Quaternion.identity);
                //}
                //this.m_blockCells.Add(new BlockCell() { VIndex = v, HIndex = h, BlockValue = data[offset] });
                offset++;
				if(GameManager.Instance.m_showBlock)
				{
					if(m_blockCells[v, h].IsBlock)
					{
						Vector3 pos = new Vector3((float)(h + 0.5f)*TILEPOSX_WIDTH, 1, -(float)(v+0.5f)*TILEPOSY_HEIGHT);
						GameManager.Instance.AddBlockCell(pos);	
					}
				}
            }
        }
    }
    public class BlockCell
    {
        public int Index;  //阻挡点序号，从0开始，以横排递增。为与动态阻挡匹配而新增
        public int VIndex;
        public int HIndex;
        public byte BlockValue; // 小于等于1 表示该宫格为阻挡

        public bool IsBlock
        {
            get { return BlockValue <= 1; }
        }
    }

    public class TriggerAreaInfo
    {
        //public uint MapId;
        public int AreaId;
        public bool CanBeTrigger;
        public Vector3 Center;
        public float Radius;

        public bool InTriggerBound(Vector3 playerPos)
        {
            bool flag = false;
            if (CanBeTrigger)
            {
                flag = Vector3.Distance(playerPos, new Vector3(Center.x, playerPos.y, Center.z)) <= Radius;
            }
            if (flag)
            {
                CanBeTrigger = false;
            }
            return flag;
        }
    }
}
