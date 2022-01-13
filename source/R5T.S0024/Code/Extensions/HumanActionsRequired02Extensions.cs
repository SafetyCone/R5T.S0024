using System;


namespace R5T.S0024
{
    public static class HumanActionsRequired02Extensions
    {
        public static bool Any(this HumanActionsRequired02 humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewExtensionMethodBasesUnmappableToSingleProject
                ;

            return output;
        }

        public static bool AnyMandatory(this HumanActionsRequired02 humanActionsRequired)
        {
            var output = false
                || humanActionsRequired.ReviewExtensionMethodBasesUnmappableToSingleProject
                ;

            return output;
        }

        public static void UnsetNonMandatory(this HumanActionsRequired02 _)
        {
            // All are mandatory.
        }
    }
}
