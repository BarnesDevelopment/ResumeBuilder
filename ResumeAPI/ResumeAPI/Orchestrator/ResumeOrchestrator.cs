﻿using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IResumeOrchestrator
{
    MemoryStream BuildResume(Resume resume);
    string BuildResumeHtml(Resume resume);
}

public class ResumeOrchestrator : IResumeOrchestrator
{
    private readonly IResumeService _service;
    
    public ResumeOrchestrator(IResumeService service)
    {
        _service = service;
    }
    public MemoryStream BuildResume(Resume header)
    {
        var stream = new MemoryStream();
        
        var body = CreateHtml(header);
        var html = body.Write();

        var properties = new ConverterProperties();
        var fontProvider = new DefaultFontProvider(false,false,false);
        fontProvider.AddDirectory("./Fonts/Roboto");
        properties.SetFontProvider(fontProvider);
        
        HtmlConverter.ConvertToPdf(html,stream,properties);
        
        return stream;
    }

    public string BuildResumeHtml(Resume resume)
    {
        var html = CreateHtml(resume).Write();
        return html;
    }

    private TagBuilder CreateHtml(Resume resume)
    {
        var body = new TagBuilder("body");
        body.InnerHtml.AppendHtml(BuildHeader(resume.Header));
        body.InnerHtml.AppendHtml(_service.BuildSummary(resume.Header));
        body.InnerHtml.AppendHtml(BuildEducation(resume.Education));
        var experience = BuildExperience(resume.Experience);
        for (int i = 0; i < experience.Count; i++)
        {
            body.InnerHtml.AppendHtml(experience[i]);
            if (resume.SplitResume && i == resume.SplitExperienceAfter)
            {
                var pageBreak = new TagBuilder("p");
                pageBreak.MergeAttribute("style", "page-break-before: always");
                body.InnerHtml.AppendHtml(pageBreak);
            }
        }
        
        body.InnerHtml.AppendHtml(BuildSkills(resume.Skills)); 
        
        var html = new TagBuilder("html");
        html.InnerHtml.AppendHtml(_service.BuildStyle());
        html.InnerHtml.AppendHtml(body);
        return html;
    }
    
    private TagBuilder BuildHeader(ResumeHeader header)
    {
        var headerTag = new TagBuilder("div");
        headerTag.AddCssClass("header");
        if(!string.IsNullOrEmpty(header.Name)) headerTag.InnerHtml.AppendHtml(TagHelper.CreatTag("div", "name", header.Name));
        
        var details = new TagBuilder("div");
        details.AddCssClass("details");
        if (!string.IsNullOrEmpty(header.Email))
        {
            details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "email", header.Email));
            details.InnerHtml.AppendHtml(_service.VerticalSeparator());
        }

        if (header.Phone != null)
        {
            details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "phone", header.Phone.FormattedNumber));
            details.InnerHtml.AppendHtml(_service.VerticalSeparator());
        }
        if(!string.IsNullOrEmpty(header.Website)) details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "website", header.Website));

        headerTag.InnerHtml.AppendHtml(details);
        return headerTag;
    }

    private TagBuilder BuildSkills(List<string> skills)
    {
        var skillsTag = new TagBuilder("div");
        skillsTag.AddCssClass("skills");
        skillsTag.InnerHtml.AppendHtml(_service.AddSeparator("Skills"));
        var skillsContainer = new TagBuilder("div");
        skillsContainer.AddCssClass("container");
        var skillsRow = new TagBuilder("div");
        skillsRow.AddCssClass("row");
        var skillsSpacer = new TagBuilder("div");
        skillsSpacer.AddCssClass("spacer");
        var skillsCell = new TagBuilder("div");
        skillsCell.AddCssClass("cell");
        var skillsList = new TagBuilder("ul");
        foreach (var skill in skills)
        {
            var skillTag = new TagBuilder("li");
            skillTag.InnerHtml.Append(skill);
            skillsList.InnerHtml.AppendHtml(skillTag);
        }

        skillsCell.InnerHtml.AppendHtml(skillsList);
        skillsRow.InnerHtml.AppendHtml(skillsSpacer);
        skillsRow.InnerHtml.AppendHtml(skillsCell);
        skillsRow.InnerHtml.AppendHtml(skillsSpacer);
        skillsContainer.InnerHtml.AppendHtml(skillsRow);
        skillsTag.InnerHtml.AppendHtml(skillsContainer);
        return skillsTag;
    }

    private List<TagBuilder> BuildExperience(List<ResumeExperience> experience)
    {
        var experienceTags = new List<TagBuilder>
        {
            _service.AddSeparator("Experience")
        };

        foreach (var job in experience)
        {
            var jobTag = new TagBuilder("div");
            jobTag.AddCssClass("job");
            var jobHeader = new TagBuilder("div");
            jobHeader.AddCssClass("job-header");
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "title", job.JobTitle));
            jobHeader.InnerHtml.AppendHtml(_service.VerticalSeparator());
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "employer", job.Employer));
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "spacer", "-"));
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "city", job.City));
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "comma", ","));
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "state", job.State));
            jobHeader.InnerHtml.AppendHtml(_service.VerticalSeparator());
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "start", job.StartDate.ToString("MMM, yyyy")));
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "spacer", "-"));
            jobHeader.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "end", job.EndDate != null ? ((DateTime)job.EndDate!).ToString("MMM, yyyy") : "Present"));
            jobTag.InnerHtml.AppendHtml(jobHeader);

            var responsibilitiesTag = new TagBuilder("div");
            responsibilitiesTag.AddCssClass("responsibilities");
            var list = new TagBuilder("ul");
            list.AddCssClass("list");
            foreach (var responsibility in job.Responsibilities)
            {
                var item = new TagBuilder("li");
                item.InnerHtml.Append(responsibility);
                list.InnerHtml.AppendHtml(item);
            }

            responsibilitiesTag.InnerHtml.AppendHtml(list);
            jobTag.InnerHtml.AppendHtml(responsibilitiesTag);
            experienceTags.Add(jobTag);
        }
        
        return experienceTags;
    }

    private TagBuilder BuildEducation(List<ResumeEducation> education)
    {
        var educationTag = new TagBuilder("div");
        educationTag.AddCssClass("education");
        educationTag.InnerHtml.AppendHtml(_service.AddSeparator("Education"));
        
        foreach (var school in education)
        {
            BuildSchool(school, educationTag);
        }

        return educationTag;
    }

    private void BuildSchool(ResumeEducation school, TagBuilder educationTag)
    {
        var schoolTag = new TagBuilder("div");
        schoolTag.AddCssClass("school");

        if (school.Degree != null)
        {
            var degree = new TagBuilder("div");
            degree.AddCssClass("degree");
            degree.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "type", school.Degree.TypeOfDegree));
            degree.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "colon", ": "));
            degree.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "major", school.Degree.Major));
            degree.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "", ", "));
            degree.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "minor", school.Degree.Minor));
            schoolTag.InnerHtml.AppendHtml(degree);
        }

        var schoolNameTag = new TagBuilder("div");
        schoolNameTag.AddCssClass("school-name");
        schoolNameTag.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "name", school.School));
        schoolNameTag.InnerHtml.AppendHtml(_service.VerticalSeparator());
        schoolNameTag.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "city", school.City));
        schoolNameTag.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "", ", "));
        schoolNameTag.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "state", school.State));
        schoolNameTag.InnerHtml.AppendHtml(_service.VerticalSeparator());
        schoolNameTag.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "grad-year", school.GraduationYear));
        schoolTag.InnerHtml.AppendHtml(schoolNameTag);

        educationTag.InnerHtml.AppendHtml(schoolTag);
    }
}