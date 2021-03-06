﻿/**
 * Copyright (c) 2017 Alphonso Turner
 * All Rights Reserved
 */

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE80;
using EnvDTE;
using ProjectHero2.Core;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectHero2
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(ProjectHeroCommandPackage.PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(ProjectHeroToolWindow))]
    public sealed class ProjectHeroCommandPackage : AsyncPackage
    {
        /// <summary>
        /// ProjectHeroCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d9d3ef03-fa6c-4fd8-a3dd-3bdbf5b86cc5";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectHeroCommand"/> class.
        /// </summary>
        public ProjectHeroCommandPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            DTE2 appObject = await GetServiceAsync(typeof(SDTE)) as DTE2;
            IVsSolution ptrSolution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            VSEventManager.SharedManager.Setup(appObject, ptrSolution);

            ProjectHeroSettingManager.Manager.PreLoadSettings(ServiceProvider.GlobalProvider);
            ProjectHeroFactory.SharedInstance.InitPluginPackage(this);
            ProjectHeroFactory.SharedInstance.InitApplicationObject(appObject);

            ProjectHeroCommand.Initialize(this);
            ProjectHeroToolWindowCommand.Initialize(this);
        }

        #endregion
    }
}