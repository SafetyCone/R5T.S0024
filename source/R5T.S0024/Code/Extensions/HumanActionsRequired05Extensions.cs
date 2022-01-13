using System;


namespace R5T.S0024
{
    public static class HumanActionsRequired05Extensions
    {
        public static bool Any(this HumanActionsRequired05 humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewDepartedSelectedNames
                || humanActionsRequired.ReviewNewSelectedNames
                ;

            return output;
        }

        public static bool AnyMandatory(this HumanActionsRequired05 _)
        {
            // None are mandatory.
            var output = false;
            return output;
        }

        public static void UnsetNonMandatory(this HumanActionsRequired05 humanActionsRequired)
        {
            humanActionsRequired.ReviewDepartedSelectedNames = false;
            humanActionsRequired.ReviewNewSelectedNames = false;
        }
    }
}
