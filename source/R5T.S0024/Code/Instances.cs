using System;

using R5T.T0034;
using R5T.T0039;
using R5T.T0040;
using R5T.T0041;
using R5T.T0044;
using R5T.T0057;
using R5T.T0062;
using R5T.T0070;
using R5T.T0098;


namespace R5T.S0024
{
    public static class Instances
    {
        public static IExtensionMethodBaseOperator ExtensionMethodBaseOperator { get; } = T0039.ExtensionMethodBaseOperator.Instance;
        public static IFileSystemOperator FileSystemOperator { get; } = T0044.FileSystemOperator.Instance;
        public static IHost Host { get; } = T0070.Host.Instance;
        public static IIgnoredValuesOperator IgnoredValuesOperator { get; } = T0057.IgnoredValuesOperator.Instance;
        public static IOperation Operation { get; } = T0098.Operation.Instance;
        public static INamespacedTypeName NamespacedTypeName { get; } = T0034.NamespacedTypeName.Instance;
        public static IPathOperator PathOperator { get; } = T0041.PathOperator.Instance;
        public static IProjectPathsOperator ProjectPathsOperator { get; } = T0040.ProjectPathsOperator.Instance;
        public static IServiceAction ServiceAction { get; } = T0062.ServiceAction.Instance;
    }
}
