using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 格子类型枚举
/// </summary>
public enum CellType
{
    Start,      // 起点
    Property,   // 地产
    Special,    // 特殊场所
    Card        // 道具卡格
}

/// <summary>
/// 特殊场所类型枚举
/// </summary>
public enum SpecialType
{
    Hospital,       // 医院
    PoliceStation,  // 警局
    Bank,           // 银行
    TaxOffice,      // 税务局
    Park,           // 公园
    Chance          // 机会
}

/// <summary>
/// 格子基类
/// </summary>
public class Cell
{
    public CellType type;
    public string name;
    public int index;
}

/// <summary>
/// 地产格子
/// </summary>
public class PropertyCell : Cell
{
    public int price;           // 购买价格
    public int rent;            // 基础租金
    public int upgradeCost;     // 升级费用
    public int level;           // 当前等级 (0-3)
    public Player owner;        // 拥有者
    public bool isMortgaged;    // 是否已抵押
    
    public PropertyCell()
    {
        type = CellType.Property;
        level = 0;
        owner = null;
        isMortgaged = false;
    }
}

/// <summary>
/// 特殊场所格子
/// </summary>
public class SpecialCell : Cell
{
    public SpecialType specialType;
    
    public SpecialCell()
    {
        type = CellType.Special;
    }
}

/// <summary>
/// 地图管理类 - 负责棋盘和格子逻辑
/// </summary>
public class Map : MonoBehaviour
{
    /// <summary>总格子数（动态获取）</summary>
    public int totalCells => cells.Count;
    
    /// <summary>地产数据配置</summary>
    public PropertyData[] propertyDatas;
    
    private List<Cell> cells = new List<Cell>();
    
    private void Awake()
    {
        Initialize();
    }
    
    /// <summary>
    /// 初始化地图
    /// </summary>
    public void Initialize()
    {
        cells.Clear();
        CreateMap();
        
        // 验证格子数量
        Debug.Log($"地图初始化完成，共 {cells.Count} 个格子");
    }
    
    /// <summary>
    /// 创建地图
    /// </summary>
    private void CreateMap()
    {
        CreateStartCell();          // 1 个
        CreatePropertyCells();      // 24 个
        CreateSpecialCells();       // 6 个
        CreateCardCells();          // 4 个
    }
    
    /// <summary>
    /// 创建起点格子
    /// </summary>
    private void CreateStartCell()
    {
        Cell startCell = new Cell();
        startCell.type = CellType.Start;
        startCell.name = "起点";
        startCell.index = 0;
        cells.Add(startCell);
    }
    
    /// <summary>
    /// 创建地产格子
    /// </summary>
    private void CreatePropertyCells()
    {
        // 优先使用ScriptableObject配置数据
        if (propertyDatas != null && propertyDatas.Length > 0)
        {
            foreach (PropertyData data in propertyDatas)
            {
                PropertyCell property = new PropertyCell();
                property.name = data.propertyName;
                property.price = data.price;
                property.rent = data.rent;
                property.upgradeCost = data.upgradeCost;
                property.index = cells.Count;
                cells.Add(property);
            }
        }
        else
        {
            // 使用硬编码数据作为后备
            string[] propertyNames = {
                "阳光小区", "幸福花园", "和平广场", "友谊大厦", "团结公寓",
                "希望田野", "梦想小镇", "未来城市", "科技园区", "文化中心",
                "体育场馆", "艺术殿堂", "商业中心", "金融大厦", "购物中心",
                "美食街", "娱乐城", "休闲广场", "度假酒店", "观光景点",
                "教育园区", "医疗中心", "工业园区", "农业基地"
            };
            
            int[] prices = {
                100, 150, 200, 250, 300,
                350, 400, 450, 500, 550,
                600, 650, 700, 750, 800,
                850, 900, 950, 1000, 1100,
                1200, 1300, 1400, 1500
            };
            
            int[] rents = {
                10, 15, 20, 25, 30,
                35, 40, 45, 50, 55,
                60, 65, 70, 75, 80,
                85, 90, 95, 100, 110,
                120, 130, 140, 150
            };
            
            for (int i = 0; i< propertyNames.Length; i++)
            {
                PropertyCell property = new PropertyCell();
                property.name = propertyNames[i];
                property.price = prices[i];
                property.rent = rents[i];
                property.upgradeCost = prices[i] / 2;
                property.index = cells.Count;
                cells.Add(property);
            }
        }
    }
    
    /// <summary>
    /// 创建特殊场所格子
    /// </summary>
    private void CreateSpecialCells()
    {
        SpecialCell hospital = new SpecialCell();
        hospital.name = "医院";
        hospital.specialType = SpecialType.Hospital;
        hospital.index = cells.Count;
        cells.Add(hospital);
        
        SpecialCell policeStation = new SpecialCell();
        policeStation.name = "警局";
        policeStation.specialType = SpecialType.PoliceStation;
        policeStation.index = cells.Count;
        cells.Add(policeStation);
        
        SpecialCell bank = new SpecialCell();
        bank.name = "银行";
        bank.specialType = SpecialType.Bank;
        bank.index = cells.Count;
        cells.Add(bank);
        
        SpecialCell taxOffice = new SpecialCell();
        taxOffice.name = "税务局";
        taxOffice.specialType = SpecialType.TaxOffice;
        taxOffice.index = cells.Count;
        cells.Add(taxOffice);
        
        SpecialCell park = new SpecialCell();
        park.name = "公园";
        park.specialType = SpecialType.Park;
        park.index = cells.Count;
        cells.Add(park);
        
        SpecialCell chance = new SpecialCell();
        chance.name = "机会";
        chance.specialType = SpecialType.Chance;
        chance.index = cells.Count;
        cells.Add(chance);
    }
    
    /// <summary>
    /// 创建道具卡格子
    /// </summary>
    private void CreateCardCells()
    {
        for (int i = 0; i < 4; i++)
        {
            Cell cardCell = new Cell();
            cardCell.type = CellType.Card;
            cardCell.name = "道具卡";
            cardCell.index = cells.Count;
            cells.Add(cardCell);
        }
    }
    
    /// <summary>
    /// 获取指定索引的格子
    /// </summary>
    public Cell GetCell(int index)
    {
        if (index >= 0 && index < cells.Count)
        {
            return cells[index];
        }
        Debug.LogWarning($"格子索引越界: {index}, 总格子数: {cells.Count}");
        return null;
    }
    
    /// <summary>
    /// 获取所有格子
    /// </summary>
    public List<Cell> GetAllCells()
    {
        return cells;
    }
    
    /// <summary>
    /// 获取所有地产格子
    /// </summary>
    public List<PropertyCell> GetAllProperties()
    {
        List<PropertyCell> properties = new List<PropertyCell>();
        foreach (Cell cell in cells)
        {
            if (cell is PropertyCell)
            {
                properties.Add(cell as PropertyCell);
            }
        }
        return properties;
    }
    
    /// <summary>
    /// 获取所有特殊场所格子
    /// </summary>
    public List<SpecialCell> GetAllSpecialCells()
    {
        List<SpecialCell> specialCells = new List<SpecialCell>();
        foreach (Cell cell in cells)
        {
            if (cell is SpecialCell)
            {
                specialCells.Add(cell as SpecialCell);
            }
        }
        return specialCells;
    }
}
