// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Catel.IoC;

/// <summary>
/// Class that gets called as soon as the module is loaded.
/// </summary>
/// <remarks>
/// This is made possible thanks to Fody.
/// </remarks>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;
    }
}