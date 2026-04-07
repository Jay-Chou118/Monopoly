using UnityEngine;
using System.Collections.Generic;

public enum CellType
{
    Start,
    Property,
    Special,
    Card
}

public enum SpecialType
{
    Hospital,
    PoliceStation,
    Bank,
    TaxOffice,
    Park,
    Chance
}

public class Cell
{
    public CellType type;
    public string name;
    public int index;
}

public class PropertyCell : Cell
{
    public int price;
    public int rent;
    public int upgradeCost;
    public int level;
    public Player owner;
    
    public PropertyCell()
    {
        type = CellType.Property;
        level = 0;
        owner = null;
    }
}

public class SpecialCell : Cell
{
    public SpecialType specialType;
    
    public SpecialCell()
    {
        type = CellType.Special;
    }
}

public class Map : MonoBehaviour
{
    public int totalCells = 36;
    private List<Cell> cells = new List<Cell>();
    
    private void Awake()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        cells.Clear();
        CreateMap();
    }
    
    private void CreateMap()
    {
        CreateStartCell();
        CreatePropertyCells();
        CreateSpecialCells();
        CreateCardCells();
    }
    
    private void CreateStartCell()
    {
        Cell startCell = new Cell();
        startCell.type = CellType.Start;
        startCell.name = "起点";
        startCell.index = 0;
        cells.Add(startCell);
    }
    
    private void CreatePropertyCells()
    {
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
    
    private void CreateCardCells()
    {
        for (int i = 0; i< 4; i++)
        {
            Cell cardCell = new Cell();
            cardCell.type = CellType.Card;
            cardCell.name = "道具卡";
            cardCell.index = cells.Count;
            cells.Add(cardCell);
        }
    }
    
    public Cell GetCell(int index)
    {
        if (index >= 0 && index< cells.Count)
        {
            return cells[index];
        }
        return null;
    }
    
    public List<Cell>GetAllCells()
    {
        return cells;
    }
    
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
