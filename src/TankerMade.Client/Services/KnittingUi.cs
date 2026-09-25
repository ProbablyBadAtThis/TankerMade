namespace TankerMade.Client.Services;

public static class KnittingUi
{
    public static string DifficultyLabel(int difficulty)
    {
        return difficulty switch
        {
            1 => "Beginner",
            2 => "Beginner+",
            3 => "Intermediate",
            4 => "Intermediate+",
            5 => "Advanced",
            6 => "Advanced+",
            _ => "Unspecified"
        };
    }

    public static string DifficultyCssClass(int difficulty)
    {
        return difficulty switch
        {
            1 => "difficulty-badge difficulty-beginner",
            2 => "difficulty-badge difficulty-beginner-plus",
            3 => "difficulty-badge difficulty-intermediate",
            4 => "difficulty-badge difficulty-intermediate-plus",
            5 => "difficulty-badge difficulty-advanced",
            6 => "difficulty-badge difficulty-advanced-plus",
            _ => "difficulty-badge difficulty-unspecified"
        };
    }

    public static string DifficultyCssClass(string? difficulty)
    {
        if (string.IsNullOrWhiteSpace(difficulty))
        {
            return "difficulty-badge difficulty-unspecified";
        }

        var normalized = difficulty.Trim().ToLowerInvariant();
        return normalized switch
        {
            "beginner" => "difficulty-badge difficulty-beginner",
            "beginner+" => "difficulty-badge difficulty-beginner-plus",
            "intermediate" => "difficulty-badge difficulty-intermediate",
            "intermediate+" => "difficulty-badge difficulty-intermediate-plus",
            "advanced" => "difficulty-badge difficulty-advanced",
            "advanced+" => "difficulty-badge difficulty-advanced-plus",
            _ => "difficulty-badge difficulty-unspecified"
        };
    }

    public static int ParseDifficulty(string? difficulty)
    {
        if (string.IsNullOrWhiteSpace(difficulty))
        {
            return 0;
        }

        return difficulty.Trim().ToLowerInvariant() switch
        {
            "beginner" => 1,
            "beginner+" => 2,
            "intermediate" => 3,
            "intermediate+" => 4,
            "advanced" => 5,
            "advanced+" => 6,
            _ => 0
        };
    }

    public static string FormatDurationCompact(long totalSeconds)
    {
        if (totalSeconds < 0)
        {
            totalSeconds = 0;
        }

        var span = TimeSpan.FromSeconds(totalSeconds);
        return span.TotalHours >= 1
            ? $"{(int)span.TotalHours:D2}:{span.Minutes:D2}:{span.Seconds:D2}"
            : $"{span.Minutes:D2}:{span.Seconds:D2}";
    }

    public static string FormatStepRange(int? start, int? end)
    {
        if (start == null && end == null)
        {
            return "-";
        }

        if (start == end || end == null)
        {
            return start?.ToString() ?? end?.ToString() ?? "-";
        }

        if (start == null)
        {
            return end.Value.ToString();
        }

        return $"{start}–{end}";
    }

    public static IEnumerable<int> ExpandStepRange(int? start, int? end)
    {
        if (start == null && end == null)
        {
            yield break;
        }

        var rangeStart = start ?? end!.Value;
        var rangeEnd = end ?? start!.Value;
        if (rangeStart > rangeEnd)
        {
            (rangeStart, rangeEnd) = (rangeEnd, rangeStart);
        }

        for (var value = rangeStart; value <= rangeEnd; value++)
        {
            yield return value;
        }
    }
}
