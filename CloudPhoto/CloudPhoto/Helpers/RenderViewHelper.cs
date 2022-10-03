using RazorEngine;

namespace CloudPhoto.Helpers;

public static class RenderViewHelper
{
    public static string RenderPartialToString(string viewPath, object model)
    {
        string viewAbsolutePath = MapPath(viewPath);
        var viewSource = File.ReadAllText(viewAbsolutePath);
        string renderedText = Razor.Parse(viewSource, model);
        return renderedText;
    }

    public static string MapPath(string filePath)
    {
        return string.Format("{0}{1}", Environment.CurrentDirectory+'/',
            filePath.Replace("~", string.Empty).TrimStart('/'));
    }
}