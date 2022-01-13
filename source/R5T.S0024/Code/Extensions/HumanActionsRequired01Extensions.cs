using System;


namespace R5T.S0024
{
    public static class HumanActionsRequired01Extensions
    {
        public static bool Any(this HumanActionsRequired01 humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewDepartedExtensionMethodBases
                || humanActionsRequired.ReviewNewExtensionMethodBases
                ;

            return output;
        }

        public static bool AnyMandatory(this HumanActionsRequired01 _)
        {
            // None are mandatory.
            var output = false;
            return output;
        }

        public static void UnsetNonMandatory(this HumanActionsRequired01 humanActionsRequired)
        {
            humanActionsRequired.ReviewDepartedExtensionMethodBases = false;
            humanActionsRequired.ReviewNewExtensionMethodBases = false;
        }
    }
}
