namespace UITManagerWebServer.Helpers
{
    public static class ViewHelpers
    {
        public static string GetHeaderClass(string currentSortOrder, string column)
        {
            return (currentSortOrder == column || currentSortOrder == $"{column}_desc") ? "bg-selected-header" : string.Empty;
        }

        public static string GetSortIcon(string currentSortOrder, string column)
        {
            return currentSortOrder switch
            {
                _ when currentSortOrder == column => "bi bi-sort-down",
                _ when currentSortOrder == $"{column}_desc" => "bi bi-sort-up",
                _ => "bi bi-arrow-down-up",
            };
        }

        public static string GetIsSolutionIcon(bool sol)
        {
            return sol ? "badge bg-success" : "badge bg-secondary";
        }

        public static string GetIsSolutionText(bool sol)
        {
            return sol ? "Solution" : "Follow-Up Note";
        }
        public static string GetBGColorForAttribution(string? attribution)
        {
            return string.IsNullOrEmpty(attribution) ? "bg-danger text-white" : "bg-success text-white";
        }
        public static string GetSeveryticolor(string currentSeverity)
        {
            switch (currentSeverity)
            {
                case ("Critical"):
                    return "text-danger";
                case ("High"):
                    return "text-High";
                case ("Medium"):
                    return "text-warning";
                case ("Low"):
                    return "text-low";
                case ("Warning"):
                    return "text-secondary";
            }

            return "text-secondary";
        }

        public static string GetPointerEvent(string actif, string expected) {
            return actif == expected ? "pointer-events-none" : "pointer-events-auto";
        }
    }
}