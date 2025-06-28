namespace PowerHotkeysWPF.Models;

public class Category
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#0078D4"; // Default blue
    public string Description { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}