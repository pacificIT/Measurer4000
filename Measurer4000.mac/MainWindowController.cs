﻿﻿using System;

using Foundation;
using AppKit;
using Measurer4000.Core.Services.Interfaces;
using Measurer4000.Core.Services;
using Measurer4000.mac.Services;
using Measurer4000.Core.Models;
using Measurer4000.Core.Utils;
using System.Globalization;

namespace Measurer4000.mac
{
    public partial class MainWindowController : NSWindowController
    {
        private readonly IDialogService _fileDialogService = ServiceLocator.Get<FileDialogService>();
        private readonly MeasureService _measureService = ServiceLocator.Get<MeasureService>();
        private Solution _currentSolution;
        public MainWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MainWindowController(NSCoder coder) : base(coder)
        {
        }

        public MainWindowController() : base("MainWindow")
        {
        }

        public override void WindowWillLoad()
        {
            base.WindowWillLoad();
        }
        public override void WindowDidLoad()
        {
            ButtonShareLink.Enabled = true;
            base.WindowDidLoad();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new MainWindow Window
        {
            get { return (MainWindow)base.Window; }
        }

        partial void ButtonOpenFileClick(Foundation.NSObject sender)
        {
            _fileDialogService.OpenFileDialog(
                        (solutionPath) => OpenSolutionPath(solutionPath),
                        (error) => ShowError(error));
        }

        partial void ButtonShareLinkClick(NSObject sender)
        {
            NSUrl shareUrl = new NSUrl("https://docs.google.com/forms/d/e/1FAIpQLSe1CMNFNnAh_GoZ3z9PD7d5a07CUd9zOVk3sywURY__zHMytA/viewform?" +
                                       "entry.187884389=" + ShareCodeIniOS.StringValue +
                                       "&entry.1649909781=" + ShareCodeIniOS.StringValue +
                                       "&entry.1981106453=" + ShareUIIniOS.StringValue +
                                       "&entry.827223542=" + ShareUIInAndroid.StringValue +
                                       "&entry.1102590771=" + TotalLOC.StringValue +
                                       "&entry.2068980640&entry.767914800&entry.1673597349&entry.1583496322");
            NSWorkspace.SharedWorkspace.OpenUrl(shareUrl);
        }

        private void ShowError(Exception e)
        {
            _fileDialogService.OpenError(e, string.Empty);
        }

        private void OpenSolutionPath(string solutionPath)
        {
            ButtonOpenFile.Enabled = false;
            ProgressBar.StartAnimation(this);
            _currentSolution = _measureService.GetProjects(solutionPath);
            ButtonOpenFile.Enabled = true;
            ProgressBar.StopAnimation(this);
            MeasureSolution(_currentSolution);
        }

        private void MeasureSolution(Solution solution)
        {
            ButtonOpenFile.Enabled = false;
            ProgressBar.StartAnimation(this);
            _currentSolution = _measureService.Measure(solution);
            _currentSolution.Stats = MeasureUtils.CalculateStats(_currentSolution);
            AmountOfFiles.StringValue = _currentSolution.Stats.AmountOfFiles.ToString();
            AndroidFiles.StringValue = _currentSolution.Stats.AndroidFiles.ToString();
            AndroidLOC.StringValue = _currentSolution.Stats.TotalLinesInAndroid.ToString();
            CodeFiles.StringValue = _currentSolution.Stats.CodeFiles.ToString();
            CoreLOC.StringValue = _currentSolution.Stats.TotalLinesCore.ToString();
            iOSFiles.StringValue = _currentSolution.Stats.iOSFiles.ToString();
            iOSLOC.StringValue = _currentSolution.Stats.TotalLinesIniOS.ToString();
            ShareCodeInAndroid.StringValue = _currentSolution.Stats.ShareCodeInAndroid.ToString("F2", CultureInfo.InvariantCulture);
            ShareCodeIniOS.StringValue = _currentSolution.Stats.ShareCodeIniOS.ToString("F2", CultureInfo.InvariantCulture);
            ShareUIInAndroid.StringValue = _currentSolution.Stats.AndroidSpecificCode.ToString("F2", CultureInfo.InvariantCulture);
            ShareUIIniOS.StringValue = _currentSolution.Stats.iOSSpecificCode.ToString("F2", CultureInfo.InvariantCulture);
            TotalLOC.StringValue = _currentSolution.Stats.TotalLinesOfCode.ToString();
            TotalUILines.StringValue = _currentSolution.Stats.TotalLinesOfUI.ToString();
            UIFiles.StringValue = _currentSolution.Stats.UIFiles.ToString();
            ButtonOpenFile.Enabled = true;
            ProgressBar.StopAnimation(this);
        }
    }
}
