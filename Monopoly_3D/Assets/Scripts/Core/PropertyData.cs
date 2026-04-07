using UnityEngine;

[CreateAssetMenu(fileName = "NewPropertyData", menuName = "Monopoly/Property Data")]
public class PropertyData : ScriptableObject
{
    public string propertyName;
    public int price;
    public int rent;
    public int upgradeCost;
    public Color color;
}
