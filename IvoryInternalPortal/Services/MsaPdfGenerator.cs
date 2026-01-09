using Microsoft.VisualBasic;
using NPoco.FluentMappings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace IvoryInternalPortal.Services
{
    public static class MsaPdfGenerator
    {
        public static byte[] Generate(
            string agreementDate,
            string providerDetails,
            string confidentialInfo,
            string serviceProviderText,
            string ivorySignatureBase64,
            string signatureBase64)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            byte[] signatureImage = null;

            byte[] ivorySignature = null;

            if (!string.IsNullOrWhiteSpace(ivorySignatureBase64))
            {
                ivorySignatureBase64 = ivorySignatureBase64.Replace("data:image/png;base64,", "");
                ivorySignature = Convert.FromBase64String(ivorySignatureBase64);
            }

            if (!string.IsNullOrEmpty(signatureBase64))
            {
                signatureBase64 = signatureBase64.Replace("data:image/png;base64,", "");
                signatureImage = Convert.FromBase64String(signatureBase64);
            }

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    //page.Margin(25);
                    page.MarginTop(0);
                    page.MarginLeft(40);
                    page.MarginRight(40);

                    // ================= HEADER =================

                    page.Header()
                    .PaddingTop(0)
                    .PaddingBottom(3)
                    .PaddingLeft(0)
                    .Row(row =>
                    {
                        row.AutoItem()
                            .TranslateX(-45)
                            .Height(160)
                            .Width(250)
                            .Image("wwwroot/images/IvoryLogo-03.jpg")
                            .FitHeight();

                        row.RelativeItem();

                        row.AutoItem()
                            .AlignMiddle()
                            .Text(text =>
                            {
                                text.DefaultTextStyle(
                                    TextStyle.Default
                                        .FontSize(12)
                                        .SemiBold()
                                );
                                text.Span("www.ivorytechnolab.com");
                            });
                    });
                              

                    // ================= FOOTER =================
                   

                    var footerStyle = TextStyle.Default
                    .FontSize(8)
                    .FontColor(Colors.Grey.Darken1);

                    page.Footer().PaddingTop(5).Row(row =>
                    {
                        // Left spacer
                        row.RelativeItem((float)0.5);

                        // Center text
                        row.RelativeItem(9).AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(footerStyle);

                            text.Line("Registered Office Address : Isquare Corporate Park, Science City Rd, Panchamrut Bunglows II, Sola, Ahmedabad, Gujarat 380060");
                            text.Line("www.ivorytechnolab.com");
                        });

                        row.RelativeItem((float)0.5);

                        // Page number (RIGHT)
                        row.AutoItem().AlignMiddle().Text(text =>
                        {
                            text.DefaultTextStyle(footerStyle);
                            text.CurrentPageNumber();
                        });
                    });

                    // ================= CONTENT =================
                    page.Content().Column(col =>
                    {
                        col.Spacing(4);
                        // ---------- PAGE 1 ----------

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1); // left spacer

                            row.RelativeItem(6).AlignCenter().Column(c =>
                            {
                                c.Item().AlignCenter()
                                    .Text("MASTER SERVICES AGREEMENT")
                                    .Bold()
                                    .FontSize(25);

                            });

                            row.RelativeItem(1); // right spacer
                        });


                        col.Item().Row(row =>
                        {
                            row.RelativeItem(2); // left spacer

                            row.RelativeItem(6).AlignCenter().Column(c =>
                            {
                                c.Item().AlignCenter()
                                    .Text($"({serviceProviderText})")
                                    .Bold()
                                    .FontSize(10);

                            });

                            row.RelativeItem(3); // right spacer
                        });

                        //col.Item().Height(2);
                        col.Item().Text($"This Master Services Agreement (“Agreement”) is entered into on {agreementDate}");

                        //col.Item().Height(2);
                        col.Item().Text("BETWEEN").Bold();

                        col.Item().Text(
                            "Ivory Technolab Private Limited,\n" +
                            "a company incorporated under the Companies Act, 2013,\n" +
                            "having its registered office at 807-Isquare Corporate Park, Science City Rd, Panchamrut Bunglows II, Sola, Ahmedabad, Gujarat 380060, India"
                        );

                        col.Item().Text("(hereinafter referred to as “Client”, which expression shall include its successors and permitted assigns)");

                        //col.Item().Height(2);
                        col.Item().Text("AND").Bold();

                        col.Item().Text(providerDetails);

                        col.Item().Text("(hereinafter referred to as “Service Provider”, which expression shall include its successors and permitted assigns)");

                        col.Item().Text("Client and Service Provider are individually referred to as a “Party” and collectively as the “Parties”.");

                        col.Item().PaddingTop(8).Text("1. DEFINITIONS").Bold().FontSize(18);
                        col.Item().PaddingTop(3).Text("1.1 “Confidential Information”").Bold().FontSize(13);

                        col.Item().PaddingLeft(20).Text(
                            "Means any and all information disclosed by Client to Service Provider, whether oral, written, electronic, visual or otherwise, including but not limited to:"
                        );

                        col.Item().Column(inner =>
                        {
                            inner.Spacing(0);

                            inner.Item().PaddingLeft(40).Text("● source code, object code, system architecture");
                            inner.Item().PaddingLeft(40).Text("● product roadmaps, designs, APIs");
                            inner.Item().PaddingLeft(40).Text("● customer data, personal data, business data");
                            inner.Item().PaddingLeft(40).Text("● credentials, keys, access tokens");
                            inner.Item().PaddingLeft(40).Text("● financial, commercial, marketing, operational information");
                        });


                        //Bullet(col, "source code, object code, system architecture");
                        //Bullet(col, "product roadmaps, designs, APIs");
                        //Bullet(col, "customer data, personal data, business data");
                        //Bullet(col, "credentials, keys, access tokens");
                        //Bullet(col, "financial, commercial, marketing, operational information");

                        col.Item().PageBreak();

                        // ---------- PAGE 2 ----------

                        col.Item().Text("All Confidential Information shall be presumed confidential unless expressly stated otherwise in writing by Client.");

                        col.Item().Text("1.2 Client Systems").Bold().FontSize(13);
                        col.Item().Text(
                            "Means all software, platforms, infrastructure, databases, documentation, AI models, prompts, datasets, workflows, credentials, cloud accounts, repositories, and related materials owned, licensed, or controlled by Client, whether developed before or during the engagement."
                        );

                        col.Item().Text("1.3 Project").Bold().FontSize(13);
                        col.Item().Text(
                            "Means software development, engineering, consulting, maintenance, support, or related services provided by by Service Provider to Client under this Agreement and any Work Order."
                        );

                        col.Item().PaddingTop(4);

                        col.Item().Text("2. TERM").Bold().FontSize(20);
                        col.Item().Text(
                            "This Agreement shall commence on the Effective Date and continue for an initial term of three (3) months, and shall automatically renew for successive periods of three (3) months unless terminated by either Party by giving thirty (30) days’ prior written notice."
                        );

                        col.Item().Text(
                            "Termination shall not affect Client’s ownership of IP, confidentiality rights, or data rights, which shall survive indefinitely."
                        );

                        col.Item().PaddingTop(4);

                        col.Item().Text("3. DUTIES & RELATIONSHIP").Bold().FontSize(20);

                        Bullet(col, "Service Provider shall provide services strictly in accordance with Client instructions.");
                        Bullet(col, "Service Provider acts solely as an independent contractor.");
                        Bullet(col, "No agency, partnership, joint venture, or employment relationship is created.");
                        Bullet(col, "Service Provider shall ensure only pre-approved personnel work on the Project.");
                        Bullet(col, "No subcontracting or delegation is permitted without Client’s prior written consent.");

                        col.Item().PageBreak();

                        col.Spacing(10);
                        // ---------- PAGE 3 ----------
                        col.Item().Text("4. COMPENSATION").Bold().FontSize(20);
                        col.Item().PaddingLeft(20).Text("Fees shall be as defined in applicable Work Orders.");

                        col.Item().Column(inner =>
                        {
                            inner.Spacing(0);

                            inner.Item().PaddingLeft(40).Text("● Payments are milestone-based or monthly as agreed.");
                            inner.Item().PaddingLeft(40).Text("● Client reserves the right to withhold payment for defective, non-compliant, or delayed deliverables");
                            inner.Item().PaddingLeft(40).Text("● No payment shall be deemed acceptance of work.");
                            inner.Item().PaddingLeft(40).Text("● derivative works");
                            inner.Item().PaddingLeft(40).Text("● source code");

                        });

                        //Bullet(col, "Payments are milestone-based or monthly as agreed.");
                        //Bullet(col, "Client reserves the right to withhold payment for defective, non-compliant, or delayed");
                        //col.Item().PaddingLeft(30).PaddingTop(0).Text("deliverables.");
                        //Bullet(col, "No payment shall be deemed acceptance of work.");

                        col.Item().PaddingTop(4);

                        col.Item().Text("5. PAYMENT TERMS").Bold().FontSize(20);
                        Bullet(col, "Invoices payable within 20 days from receipt by the end of the working month.");
                        Bullet(col, "The client may set off any damages, penalties, or losses against invoices.");
                        Bullet(col, "No advance payments unless expressly agreed in writing.");

                        col.Item().PaddingTop(4);

                        col.Item().Text("6. SOFTWARE & TOOLS").Bold().FontSize(20);
                        Bullet(col, "All tools, repositories, credentials, and licenses provided by Client remain Client");
                        col.Item().PaddingLeft(30).PaddingTop(0).Text("property.");
                        Bullet(col, "Service Provider shall not reuse, replicate, or retain access post-termination.");
                        Bullet(col, "Any open-source usage must be pre-approved in writing.");

                        col.Item().PaddingTop(4);

                        col.Item().Text("7. TAXES (INDIA)").Bold().FontSize(20);
                        Bullet(col, "Fees are exclusive of GST");
                        Bullet(col, "GST shall be charged as applicable under Indian law");
                        Bullet(col, "TDS shall be deducted as per the Income-tax Act, 1961.");

                        col.Item().PageBreak();

                        // ---------- PAGE 4 ----------
                        col.Item().Text("8. NON-SOLICITATION & NON-CIRCUMVENTION (STRICT)").Bold().FontSize(20);
                        col.Item().PaddingLeft(20).Text("During the Agreement and for three (3) years thereafter, Service Provider shall not:");

                        col.Item().Column(inner =>
                        {
                            inner.Spacing(0);

                            inner.Item().PaddingLeft(40).Text("● solicit or hire Client’s employees, contractors, or consultants");
                            inner.Item().PaddingLeft(40).Text("● approach Client’s customers, vendors, or partners directly or indirectly");
                            inner.Item().PaddingLeft(40).Text("● bypass Client to provide similar services to any Client lead or customer");
                            
                        });


                        //Bullet(col, "solicit or hire Client’s employees, contractors, or consultants");
                        //Bullet(col, "approach Client’s customers, vendors, or partners directly or indirectly");
                        //Bullet(col, "bypass Client to provide similar services to any Client lead or customer");

                        col.Item().PaddingLeft(20).Text("Liquidated damages:  INR 1Cr per breach, without prejudice to injunctive relief.");

                        col.Item().PaddingTop(0);

                        col.Item().Text("9. CONFIDENTIALITY & DATA PROTECTION (HARD CLAUSE)").Bold().FontSize(20);
                        Bullet(col, "Confidentiality obligations survive perpetually.");
                        Bullet(col, "Service Provider shall comply with:");

                        col.Item().PaddingLeft(40).Text(" ○ Information Technology Act, 2000");
                        col.Item().PaddingLeft(40).Text(" ○ DPDP Act, 2023");

                        Bullet(col, "No data may leave India without consent.");
                        Bullet(col, "Any breach must be reported within 24 hours.");

                        col.Item().PaddingTop(0);

                        col.Item().Text("10. INTELLECTUAL PROPERTY OWNERSHIP (ABSOLUTE)").Bold().FontSize(20);
                        col.Item().PaddingLeft(10).Text("10.1 Ownership").Bold().FontSize(13);
                        col.Item().PaddingLeft(20).Text("All work product, including:");

                        col.Item().Column(inner =>
                        {
                            inner.Spacing(0);

                            inner.Item().PaddingLeft(40).Text("● source code");
                            inner.Item().PaddingLeft(40).Text("● documentation");
                            inner.Item().PaddingLeft(40).Text("● designs");
                            inner.Item().PaddingLeft(40).Text("● AI models, prompts, datasets");
                            inner.Item().PaddingLeft(40).Text("● derivative works");
                        });


                        col.Item().PaddingLeft(20).Text("shall be deemed “work made for hire” and shall vest exclusively and irrevocably in Client.");


                        col.Item().PageBreak();

                        // ---------- PAGE 5 ----------
                        col.Item().Text("10.2 Assignment").Bold().FontSize(13);
                        col.Item().Text("Service Provider hereby assigns all worldwide IP rights to Client, including moral rights, in\r\nperpetuity.");

                        col.Item().Text("10.3 No Retained Rights").Bold().FontSize(13);
                        col.Item().Text("Service Provider shall have zero ownership, license, or reuse rights");

                        col.Spacing(15);

                        col.Item().Text("11. TERMINATION").Bold().FontSize(20);
                        col.Item().PaddingLeft(20).Text("Client may terminate:");

                        col.Item().Column(inner =>
                        {
                            inner.Spacing(0);

                            inner.Item().PaddingLeft(40).Text("● for convenience with 30 days’ notice");
                            inner.Item().PaddingLeft(40).Text("● immediately for breach, misconduct, data risk, IP risk, or insolvency");

                        });

                        //Bullet(col, "for convenience with 30 days’ notice");
                        //Bullet(col, "immediately for breach, misconduct, data risk, IP risk, or insolvency");

                        col.Item().PaddingLeft(20).Text("Upon termination:");

                        col.Item().Column(inner =>
                        {
                            inner.Spacing(0);

                            inner.Item().PaddingLeft(40).Text("● All access shall be revoked");
                            inner.Item().PaddingLeft(40).Text("● All Client data shall be returned and deleted");
                            inner.Item().PaddingLeft(40).Text("● Written certification of deletion shall be provided");
                            
                        });

                        col.Spacing(15);

                        col.Item().Text("12. GOVERNING LAW & JURISDICTION").Bold().FontSize(20);
                        col.Item().Text("This Agreement shall be governed by and construed in accordance with the laws of India.");
                        col.Item().Text("Courts at Ahmedabad, Gujarat shall have exclusive jurisdiction.\r\n");

                        col.Spacing(15);

                        col.Item().Text("13. ARBITRATION (INDIA)").Bold().FontSize(20);
                        Bullet(col, "Arbitration under Arbitration & Conciliation Act, 1996");
                        Bullet(col, "Seat & venue: Ahmedabad");
                        Bullet(col, "Sole arbitrator appointed by Client");
                        Bullet(col, "Language: English.");

                        col.Item().PageBreak();

                        // ---------- PAGE 6 ----------
                        col.Item().Text("14. INDEMNITY (CLIENT-FAVORABLE)").Bold().FontSize(20);
                        col.Item().Text("Service Provider shall fully indemnify, defend, and hold harmless Client against all losses arising from:");
                        Bullet(col, "IP infringement.");
                        Bullet(col, "Data breach.");
                        Bullet(col, "confidentiality breach");
                        Bullet(col, "regulatory non-compliance.");
                        Bullet(col, "negligence or misconduct.");

                        col.Item().Text("No liability cap shall apply to IP, confidentiality, or data breaches.").Bold();

                        col.Item().PaddingTop(4);

                        col.Item().Text("15. LIMITATION OF LIABILITY").Bold().FontSize(20);
                        col.Item().Text("Except for IP, confidentiality, data protection, and fraud:");
                        Bullet(col, "Service Provider’s liability shall be capped at 100% of fees paid in the preceding 12 months");

                        col.Item().PaddingTop(4);

                        col.Item().Text("16. FORCE MAJEURE").Bold().FontSize(20);
                        col.Item().Text("Standard Indian force-majeure clause, excluding payment obligations.");

                        col.Item().PaddingTop(4);

                        col.Item().Text("17. ASSIGNMENT").Bold().FontSize(20);
                        //col.Item().Text("Service Provider may not assign or subcontract without Client’s written approval.");
                        col.Item().Text(text =>
                        {
                            text.Span("Service Provider ");
                            text.Span("may not assign or subcontract").Bold();
                            text.Span(" without Client’s written approval.");
                        });


                       col.Item().PaddingTop(4);

                        col.Item().Text("18. ENTIRE AGREEMENT").Bold().FontSize(20);
                        col.Item().Text("This Agreement supersedes all prior discussions and agreements.");

                        col.Item().PageBreak();

                        // ---------- PAGE 7 ----------

                        col.Item().Text("19. SIGNATURES").Bold();

                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // ================= ROW 1 =================
                            table.Cell().Border(1).Padding(10).Column(left =>
                            {
                                left.Item().Text("For Ivory Technolab Private Limited").Bold();
                                left.Item().Text("Name: Sunny Panchal");
                                left.Item().Text("Title: CEO & Co-Founder");
                                left.Item().Text("Email: sunny@ivorytechnolab.com");
                            });

                            //table.Cell().Border(1).Padding(10).AlignCenter().AlignMiddle().Element(cell =>
                            //{
                            //    if (ivorySignature != null)
                            //        cell.Image(ivorySignature).FitHeight();
                            //    else
                            //        cell.Text("Signature");
                            //});

                            table.Cell().Border(1).Padding(10).AlignCenter().AlignMiddle().Element(cell =>
                            {
                                if (ivorySignature != null)
                                {
                                    cell.Height(80)
                                        .Image(ivorySignature)
                                        .FitArea();
                                }
                                else
                                {
                                    cell.Text("Signature");
                                }
                            });

                            // ================= ROW 2 =================
                            table.Cell().Border(1).Padding(10).Column(left =>
                            {
                                left.Item().Text("For Tabdelta Solutions Pvt. Ltd.").Bold();
                                left.Item().Text("(Service Provider)");
                                left.Item().Text("Name: Sunil Patel");
                                left.Item().Text("Title: CTO & Co-Founder");
                                left.Item().Text("Email: sunil@tabdelta.com");
                            });

                            //table.Cell().Border(1).Padding(10).AlignCenter().AlignMiddle().Element(cell =>
                            //{
                            //    if (signatureImage != null)
                            //        cell.Image(signatureImage).FitHeight();
                            //    else
                            //        cell.Text("Signature");
                            //});

                            table.Cell().Border(1).Padding(10).AlignCenter().AlignMiddle().Element(cell =>
                            {
                                if (signatureImage != null)
                                {
                                    cell.Height(80)
                                        .Image(signatureImage)
                                        .FitArea();
                                }
                                else
                                {
                                    cell.Text("Signature");
                                }
                            });

                        });

                        //col.Item().Text("For Service Provider").Bold();

                        //if (signatureImage != null)
                        //{
                        //    col.Item().Height(80).Image(signatureImage);
                        //}

                        //col.Item().Text($"Date: {agreementDate}");
                    });
                });
            }).GeneratePdf();
        }

        private static void Bullet(ColumnDescriptor col, string text)
        {
            col.Item().Row(row =>
            {
                row.ConstantItem(20);   // ⬅ LEFT INDENT SPACE

                row.AutoItem()
                   .Text("● ")
                   .FontSize(10)
                .LineHeight((float?)0.5);

                row.ConstantItem(5);

                row.RelativeItem()
                   .Text(text)
                   .FontSize(12)
                   .LineHeight((float?)0.5);


            });
        }
    }
}
