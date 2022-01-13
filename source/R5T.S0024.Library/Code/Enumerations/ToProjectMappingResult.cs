using System;


namespace R5T.S0024.Library
{
    /// <summary>
    /// Describes the outcome of mapping an item to a single Visual Studio project file.
    /// </summary>
    public enum ToProjectMappingResult
    {
        Success = 0,

        NoProjectFound,
        MoreThanOneProjectFound,
    }
}
