using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ResumeAPI.Helpers;

public static class TagWriter
{
    public static string Write(this TagBuilder builder)
    {
        using (var writer = new System.IO.StringWriter())
        {        
            builder.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        } 
    }
}