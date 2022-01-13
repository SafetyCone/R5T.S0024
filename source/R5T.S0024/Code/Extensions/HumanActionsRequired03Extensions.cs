using System;


namespace R5T.S0024
{
    public static class HumanActionsRequired03Extensions
    {
        public static bool Any(this HumanActionsRequired03 humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewDepartedToProjectMappings
                || humanActionsRequired.ReviewNewToProjectMappings
                ;

            return output;
        }

        public static bool AnyMandatory(this HumanActionsRequired03 _)
        {
            // None are mandatory.
            var output = false;
            return output;
        }

        public static void UnsetNonMandatory(this HumanActionsRequired03 humanActionsRequired)
        {
            humanActionsRequired.ReviewDepartedToProjectMappings = false;
            humanActionsRequired.ReviewNewToProjectMappings = false;
        }
    }
}
