using System;

using R5T.T0034;
using R5T.T0039;
using R5T.T0040;
using R5T.T0041;


namespace R5T.S0024.Library
{
    public static class Instances
    {
        public static IExtensionMethodBaseOperator ExtensionMethodBaseOperator { get; } = T0039.ExtensionMethodBaseOperator.Instance;
        public static INamespacedTypeName NamespacedTypeName { get; } = T0034.NamespacedTypeName.Instance;
        public static IPathOperator PathOperator { get; } = T0041.PathOperator.Instance;
        public static IProjectPathsOperator ProjectPathsOperator { get; } = T0040.ProjectPathsOperator.Instance;
    }
}
