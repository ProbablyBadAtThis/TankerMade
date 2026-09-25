using System.Globalization;
using System.Net;
using System.Text;
using TankerMade.Contracts.DTOs.ClientProgress;
using TankerMade.Core.Enums;

namespace TankerMade.ClientProgressHost;

public static class HostedClientPage
{
    public static string Render(ClientProgressSnapshotDto? snapshot, string token)
    {
        if (snapshot == null)
        {
            return Page("""
                <h1>This link is not available</h1>
                <p>It may have been revoked, or it was never published.</p>
                """);
        }

        var body = new StringBuilder();
        body.Append("<p class=\"studio-kicker\">Commission</p>");
        if (!string.IsNullOrWhiteSpace(snapshot.Title))
        {
            body.Append("<h1>").Append(Encode(snapshot.Title)).Append("</h1>");
        }

        body.Append("<p class=\"studio-stage\">").Append(Encode(StageLabel(snapshot.Stage))).Append("</p>");
        body.Append("<p class=\"studio-summary\">").Append(Encode(snapshot.Summary)).Append("</p>");
        body.Append("<p class=\"studio-meta\">Updated ")
            .Append(Encode(snapshot.LastUpdated.ToUniversalTime().ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture)))
            .Append(" UTC</p>");

        if (snapshot.Price.HasValue || snapshot.DueDate.HasValue)
        {
            body.Append("<dl class=\"studio-facts\">");
            if (snapshot.Price.HasValue)
            {
                body.Append("<div><dt>Price</dt><dd>")
                    .Append(Encode(snapshot.Price.Value.ToString("C", CultureInfo.GetCultureInfo("en-US"))))
                    .Append("</dd></div>");
            }

            if (snapshot.DueDate.HasValue)
            {
                body.Append("<div><dt>Expected</dt><dd>")
                    .Append(Encode(snapshot.DueDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)))
                    .Append("</dd></div>");
            }

            body.Append("</dl>");
        }

        if (snapshot.MaterialLines.Count > 0)
        {
            body.Append("<h2>Materials</h2><ul>");
            foreach (var line in snapshot.MaterialLines)
            {
                body.Append("<li>").Append(Encode(line.Name)).Append("</li>");
            }

            body.Append("</ul>");
        }

        if (snapshot.PhotoAssetIds.Count > 0)
        {
            body.Append("<h2>Photos</h2><div class=\"studio-photos\">");
            foreach (var assetId in snapshot.PhotoAssetIds)
            {
                body.Append("<img src=\"/client-progress/")
                    .Append(Uri.EscapeDataString(token))
                    .Append("/photos/")
                    .Append(assetId.ToString("D"))
                    .Append("\" alt=\"Published photo\" />");
            }

            body.Append("</div>");
        }

        body.Append("<h2>Next</h2><p>").Append(Encode(snapshot.NextStep)).Append("</p>");
        if (snapshot.Revisions.Count > 0)
        {
            body.Append("<h2>Revisions</h2><ol>");
            foreach (var revision in snapshot.Revisions)
            {
                body.Append("<li><span>")
                    .Append(Encode(revision.OccurredAt.ToUniversalTime().ToString("MMM d, yyyy", CultureInfo.InvariantCulture)))
                    .Append("</span>");
                if (!string.IsNullOrWhiteSpace(revision.Summary))
                {
                    body.Append("<span>").Append(Encode(revision.Summary)).Append("</span>");
                }

                if (revision.PriceChanged)
                {
                    body.Append("<span>Price ")
                        .Append(Encode(revision.PreviousPrice?.ToString("C", CultureInfo.GetCultureInfo("en-US"))))
                        .Append(" to ")
                        .Append(Encode(revision.NewPrice?.ToString("C", CultureInfo.GetCultureInfo("en-US"))))
                        .Append("</span>");
                }

                if (revision.DueDateChanged)
                {
                    body.Append("<span>Window ")
                        .Append(Encode(revision.PreviousDueDate?.ToString("MMM d", CultureInfo.InvariantCulture)))
                        .Append(" to ")
                        .Append(Encode(revision.NewDueDate?.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)))
                        .Append("</span>");
                }

                body.Append("</li>");
            }

            body.Append("</ol>");
        }

        return Page(body.ToString());
    }

    private static string StageLabel(CommissionStage value) => value switch
    {
        CommissionStage.InProgress => "In progress",
        _ => value.ToString()
    };

    private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

    private static string Page(string body) => $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>Commission</title>
          <style>
            body { margin: 0; background: #f4f1ec; }
            .studio-page { max-width: 40rem; margin: 3rem auto; padding: 2rem 1.5rem 4rem; color: #1c2430; font-family: "Segoe UI", sans-serif; }
            h1, h2 { font-weight: 500; }
            .studio-kicker { margin: 0; text-transform: uppercase; letter-spacing: 0.08em; font-size: 0.75rem; color: #5c6778; }
            .studio-stage { margin: 0.25rem 0 1rem; font-size: 1.25rem; }
            .studio-summary { font-size: 1.125rem; line-height: 1.5; }
            .studio-meta, dt { color: #5c6778; }
            .studio-facts { display: flex; gap: 2rem; }
            .studio-facts dd { margin: 0.25rem 0 0; font-size: 1.125rem; }
            .studio-photos img { max-width: 100%; margin: 0 0 1rem; }
            ol li span { display: block; }
          </style>
        </head>
        <body>
          <article class="studio-page">
            {{body}}
          </article>
        </body>
        </html>
        """;
}
