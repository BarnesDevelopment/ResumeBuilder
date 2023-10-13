using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Builders;

public static class ResumeBuilder
{
  public static string Build(this ResumeTreeNode tree)
  {
    var html = new TagBuilder("html");
    html.InnerHtml.AppendHtml(BuildStyle());
    html.InnerHtml.AppendHtml(BuildBody(tree));
    return html.Write();
  }

  private static TagBuilder BuildStyle()
  {
    var head = new TagBuilder("head");
    var style = new TagBuilder("style");
    StreamReader sr = new StreamReader("./CSS/DefaultCss.css");
    var css = sr.ReadToEnd();
    sr.Close();
    style.InnerHtml.AppendHtml(css);
    head.InnerHtml.AppendHtml(style);
    return head;
  }

  private static TagBuilder BuildBody(ResumeTreeNode tree)
  {
    var body =  new TagBuilder("body");
    return body;
  }
}
