namespace TFG.Models
{
    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; } // Indicates the current page

        public BreadcrumbItem(string text, string url = null, bool isActive = false)
        {
            Text = text;
            Url = url;
            IsActive = isActive;
        }
    }
}