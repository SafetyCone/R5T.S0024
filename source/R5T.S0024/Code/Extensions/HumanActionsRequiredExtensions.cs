using System;


namespace R5T.S0024
{
    public static class HumanActionsRequiredExtensions
    {
        public static bool Any(this HumanActionsRequired humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewDepartedExtensionMethodBases
                || humanActionsRequired.ReviewNewExtensionMethodBases
                || humanActionsRequired.ReviewUnignoredDuplicateExtensionMethodBaseTypeNames
                || humanActionsRequired.ReviewNewToProjectMappings
                || humanActionsRequired.ReviewOldToProjectMappings
                ;

            return output;
        }

        public static bool AnyMandatory(this HumanActionsRequired humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewUnignoredDuplicateExtensionMethodBaseTypeNames
                ;

            return output;
        }

        public static void UnsetNonMandatory(this HumanActionsRequired humanActionsRequired)
        {
            humanActionsRequired.ReviewDepartedExtensionMethodBases = false;
            humanActionsRequired.ReviewNewExtensionMethodBases = false;
            humanActionsRequired.ReviewNewToProjectMappings = false;
            humanActionsRequired.ReviewOldToProjectMappings = false;
        }
    }
}
