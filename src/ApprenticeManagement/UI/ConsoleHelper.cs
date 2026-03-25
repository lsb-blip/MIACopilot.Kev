namespace ApprenticeManagement.UI;

/// <summary>
/// Centralises all console I/O helpers: coloured output, input prompts, and
/// validated number/date reading.
/// </summary>
public static class ConsoleHelper
{
    // ─── Output ───────────────────────────────────────────────────────────────

    public static void PrintHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('═', 60));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('═', 60));
        Console.ResetColor();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✔  {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✖  {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"   {message}");
        Console.ResetColor();
    }

    public static void PrintLine(string message = "") => Console.WriteLine(message);

    /// <summary>Writes a dimmed separator line.</summary>
    public static void PrintSeparator()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('─', 60));
        Console.ResetColor();
    }

    // ─── Input ────────────────────────────────────────────────────────────────

    /// <summary>Prompts the user and returns the entered string (never null).</summary>
    public static string ReadString(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            string input = Console.ReadLine() ?? string.Empty;
            if (allowEmpty || !string.IsNullOrWhiteSpace(input))
                return input.Trim();

            PrintError("Value cannot be empty. Please try again.");
        }
    }

    /// <summary>Prompts for a string but returns <paramref name="current"/> if the user presses Enter.</summary>
    public static string ReadStringOrDefault(string prompt, string current)
    {
        Console.Write($"  {prompt} [{current}]: ");
        string input = Console.ReadLine() ?? string.Empty;
        return string.IsNullOrWhiteSpace(input) ? current : input.Trim();
    }

    /// <summary>Prompts for an integer within an optional range.</summary>
    public static int ReadInt(string prompt, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            string input = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(input, out int value))
            {
                if (min.HasValue && value < min.Value)
                {
                    PrintError($"Value must be at least {min.Value}.");
                    continue;
                }
                if (max.HasValue && value > max.Value)
                {
                    PrintError($"Value must be at most {max.Value}.");
                    continue;
                }
                return value;
            }

            PrintError("Invalid number. Please enter a whole number.");
        }
    }

    /// <summary>Prompts for a double within an optional range.</summary>
    public static double ReadDouble(string prompt, double? min = null, double? max = null)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            string input = Console.ReadLine() ?? string.Empty;

            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
                if (min.HasValue && value < min.Value)
                {
                    PrintError($"Value must be at least {min.Value}.");
                    continue;
                }
                if (max.HasValue && value > max.Value)
                {
                    PrintError($"Value must be at most {max.Value}.");
                    continue;
                }
                return value;
            }

            PrintError("Invalid number. Please enter a numeric value (e.g. 7.5).");
        }
    }

    /// <summary>
    /// Prompts for a double but returns <paramref name="current"/> when the user presses Enter.
    /// </summary>
    public static double ReadDoubleOrDefault(string prompt, double current, double? min = null, double? max = null)
    {
        while (true)
        {
            Console.Write($"  {prompt} [{current:F2}]: ");
            string input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input)) return current;

            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
                if (min.HasValue && value < min.Value)
                {
                    PrintError($"Value must be at least {min.Value}.");
                    continue;
                }
                if (max.HasValue && value > max.Value)
                {
                    PrintError($"Value must be at most {max.Value}.");
                    continue;
                }
                return value;
            }

            PrintError("Invalid number — keeping the original value.");
            return current;
        }
    }

    /// <summary>Prompts for a date in the format <c>yyyy-MM-dd</c>.</summary>
    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write($"  {prompt} (yyyy-MM-dd): ");
            string input = Console.ReadLine() ?? string.Empty;

            if (DateTime.TryParseExact(input, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None, out DateTime date))
                return date;

            PrintError("Invalid date. Please use the format yyyy-MM-dd (e.g. 2024-03-15).");
        }
    }

    /// <summary>
    /// Prompts for a date but returns <paramref name="current"/> when the user presses Enter.
    /// </summary>
    public static DateTime ReadDateOrDefault(string prompt, DateTime current)
    {
        Console.Write($"  {prompt} (yyyy-MM-dd) [{current:yyyy-MM-dd}]: ");
        string input = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input)) return current;

        if (DateTime.TryParseExact(input, "yyyy-MM-dd",
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   System.Globalization.DateTimeStyles.None, out DateTime date))
            return date;

        PrintError("Invalid date — keeping the original value.");
        return current;
    }

    /// <summary>Displays a numbered list of options and returns the chosen 1-based index.</summary>
    public static int ChooseFromList(string prompt, IReadOnlyList<string> options)
    {
        PrintLine();
        for (int i = 0; i < options.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.WriteLine($" {options[i]}");
        }

        return ReadInt(prompt, 1, options.Count);
    }

    /// <summary>Pauses until the user presses Enter.</summary>
    public static void Pause(string message = "Press Enter to continue…")
    {
        PrintLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  {message}");
        Console.ResetColor();
        Console.ReadLine();
    }
}
