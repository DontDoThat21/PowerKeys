namespace PowerHotkeysWPF.Models;

public class Shortcut
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Application { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Keys { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCustom { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime LastModified { get; set; } = DateTime.Now;
}