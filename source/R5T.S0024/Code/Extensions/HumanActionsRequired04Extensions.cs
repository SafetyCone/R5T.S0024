using System;


namespace R5T.S0024
{
    public static class HumanActionsRequired04Extensions
    {
        public static bool Any(this HumanActionsRequired04 humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewDuplicateExtensionMethodBaseNames
                ;

            return output;
        }

        public static bool AnyMandatory(this HumanActionsRequired04 humanActionsRequired)
        {
            // None are mandatory.
            var output = false
                || humanActionsRequired.ReviewDuplicateExtensionMethodBaseNames
                ;

            return output;
        }

        public static void UnsetNonMandatory(this HumanActionsRequired04 _)
        {
            // Do nothing, all are mandatory.
        }
    }
}
