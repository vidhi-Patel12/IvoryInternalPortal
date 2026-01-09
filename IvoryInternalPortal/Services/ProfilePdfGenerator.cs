using IvoryInternalPortal.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace IvoryInternalPortal.Services
{
    public static class ProfilePdfGenerator
    {
        public static byte[] Generate(Profiles p)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);

                        // =========================
                        // NAME (SHOW ONLY ON FIRST PAGE)
                        // =========================
                        col.Item()
                           .ShowOnce()
                           .Text(p.Name)
                           .FontSize(20)
                           .Bold();

                        // BLANK SPACE AFTER NAME
                        col.Item()
                           .ShowOnce()
                           .Height(10);

                        // =========================
                        // CONTENT
                        // =========================
                        AddSection(col, "Professional Summary", p.ProfessionalSummary);
                        AddCoreSkills(col, p.CoreSkills);
                        AddSection(col, "Professional Experience", p.ProfessionalExperience);

                        // PROJECTS (CAN FLOW TO PAGE 2)
                        AddProjects(col, p);

                        // OPTIONAL: FORCE NEW PAGE FOR KEY SKILLS
                        //col.Item().PageBreak();

                        AddKeySkills(col, p.KeySkills);

                    });
                });
            }).GeneratePdf();
        }

        // SIMPLE SECTION
        private static void AddSection(ColumnDescriptor col, string title, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            col.Item().Text(title).Bold().FontSize(14);
            col.Item().Text(value).FontSize(11);
        }

        // CORE SKILLS

        private static void AddCoreSkills(ColumnDescriptor col, string? coreSkills)
        {
            if (string.IsNullOrWhiteSpace(coreSkills)) return;

            col.Item().Text("Core Skills").Bold().FontSize(14);

            foreach (var line in coreSkills.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split(':', 2);

                col.Item().Row(row =>
                {
                    row.AutoItem().Text(parts[0] + ": ").Bold().FontSize(11);
                    if (parts.Length > 1)
                        row.RelativeItem().Text(parts[1]).FontSize(11);
                });
            }
        }

        // PROJECTS

        private static void AddProjects(ColumnDescriptor col, Profiles p)
        {
            if (string.IsNullOrWhiteSpace(p.ProjectNames))
                return;

            col.Item().Text("Project Experience").Bold().FontSize(14);

            var names = p.ProjectNames.Split("[PROJECT]", StringSplitOptions.RemoveEmptyEntries);
            var domains = p.ProjectDomains?.Split("[PROJECT]") ?? Array.Empty<string>();
            var techs = p.ProjectTechnologies?.Split("[PROJECT]") ?? Array.Empty<string>();
            var descs = p.ProjectDescriptions?.Split("[PROJECT]") ?? Array.Empty<string>();
            var resps = p.ProjectResponsibilities?.Split("[PROJECT]") ?? Array.Empty<string>();

            for (int i = 0; i < names.Length; i++)
            {
                col.Item().PaddingTop(6)
                    .Text(names[i].Trim())
                    .Bold()
                    .FontSize(12);

                if (i < domains.Length && !string.IsNullOrWhiteSpace(domains[i]))
                    AddKeyValue(col, "Domain", domains[i]);

                if (i < techs.Length && !string.IsNullOrWhiteSpace(techs[i]))
                    AddKeyValue(col, "Technologies", techs[i]);

                if (i < descs.Length && !string.IsNullOrWhiteSpace(descs[i]))
                {
                    // Description title
                    col.Item()
                        .PaddingTop(4)
                        .Text("Description")
                        .Bold()
                        .FontSize(11);

                    // Description content (indented)
                    col.Item()
                        .PaddingLeft(20)
                        .Text(descs[i].Trim())
                        .FontSize(11);
                }


                if (i < resps.Length && !string.IsNullOrWhiteSpace(resps[i]))
                {
                    col.Item()
                        .PaddingTop(4)
                        .Text("Responsibilities")
                        .Bold()
                        .FontSize(11);

                    // Split by double spaces OR new lines
                    var bullets = resps[i]
                        .Split(new[] { "  ", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var b in bullets)
                    {
                        col.Item()
                            .PaddingLeft(20)
                            .Text("● " + b.Trim())
                            .FontSize(11);
                    }
                }


            }
        }

        // KEY SKILLS (BULLETED)
        private static void AddKeySkills(ColumnDescriptor col, string? keySkills)
        {
            if (string.IsNullOrWhiteSpace(keySkills)) return;

            col.Item()
                .Text("Key Skills")
                .Bold()
                .FontSize(14);

            var bullets = keySkills
                .Split(new[] { "  ", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var skill in bullets)
            {
                col.Item()
                    .PaddingLeft(20)
                    .Text("● " + skill.Trim())
                    .FontSize(11);
            }
        }

        // KEY : VALUE ROW (Domain / Technologies)
        private static void AddKeyValue(ColumnDescriptor col, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            col.Item().Row(row =>
            {
                row.AutoItem()
                   .Text($"{key}: ")
                   .Bold()
                   .FontSize(11);

                row.RelativeItem()
                   .Text(value.Trim())
                   .FontSize(11);
            });
        }


    }
}
