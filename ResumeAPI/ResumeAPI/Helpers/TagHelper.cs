using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResumeAPI.Helpers;

public static class TagHelper
{
    public static string Write(this TagBuilder builder)
    {
        using (var writer = new System.IO.StringWriter())
        {        
            builder.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        } 
    }

    public static TagBuilder CreatTag(string tagType, string className = "", string innerText = "")
    {
        var tag = new TagBuilder(tagType);
        if(!string.IsNullOrEmpty(className)) tag.AddCssClass(className);
        tag.InnerHtml.Append(innerText);
        return tag;
    }
}