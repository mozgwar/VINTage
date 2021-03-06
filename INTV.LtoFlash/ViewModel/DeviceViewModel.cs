﻿// <copyright file="DeviceViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for Locutus device.
    /// </summary>
    public partial class DeviceViewModel : INTV.Shared.ViewModel.ViewModelBase, INTV.Core.Model.Device.IPeripheral
    {
        #region Constants

        public const string IsConfigurablePropertyName = "IsConfigurable";
        public const string DisplayNamePropertyName = "DisplayName";

        #endregion // Constants

        public static readonly string NoDevice = Resources.Strings.StatusBar_NoDevice;
        public static readonly string NoConnectedDevices = ResourceHelpers.CreatePackedResourceString(typeof(DeviceViewModel), "Resources/Images/disconnected_16x16.png");
        public static readonly string ConnectedDevices = ResourceHelpers.CreatePackedResourceString(typeof(DeviceViewModel), "Resources/Images/connected_16x16.png");
        public static readonly string PowerStatusTip = Resources.Strings.ConsolePowerState_Tip;

        public static readonly DeviceViewModel InvalidDevice = new DeviceViewModel(null);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of DeviceViewModel.
        /// </summary>
        /// <param name="device">The device to be a ViewModel for.</param>
        public DeviceViewModel(Device device)
        {
            _device = device;
            if (_device == null)
            {
                _ecsCompatibility = EcsStatusFlags.Default;
                _intvIICompatibility = IntellivisionIIStatusFlags.Default;
                _showTitleScreen = ShowTitleScreenFlags.Default;
                _saveMenuPosition = SaveMenuPositionFlags.Default;
                _backgroundGC = true;
                _keyclicks = false;
                _displayName = NoDevice;
            }
            else
            {
                _ecsCompatibility = device.EcsCompatibility;
                _intvIICompatibility = device.IntvIICompatibility;
                _showTitleScreen = device.ShowTitleScreen;
                _saveMenuPosition = device.SaveMenuPosition;
                _backgroundGC = device.BackgroundGC;
                _keyclicks = device.Keyclicks;
                _device.ErrorHandler = ErrorHandler;
                _device.PropertyChanged += DevicePropertyChanged;
                UpdateDisplayName();
            }
            UpdatePowerState();
            UpdateCompatibilityMode(DeviceStatusCategory.Ecs, (byte)EcsCompatibility);
            UpdateCompatibilityMode(DeviceStatusCategory.IntvII, (byte)IntvIICompatibility);
            UpdateShowTitleScreen(ShowTitleScreen);
            UpdateSaveMenuPosition(SaveMenuPosition);
            UpdateBackgroundGC(BackgroundGC);
            UpdateKeyclicks(Keyclicks);
        }

        #endregion // Constructors

        #region Properties

        private static Dictionary<EcsStatusFlags, string> EcsCompatiblityInfoTable
        {
            get
            {
                if (_ecsCompatiblityInfoTable == null)
                {
                    _ecsCompatiblityInfoTable = new Dictionary<EcsStatusFlags, string>()
                    {
                        { EcsStatusFlags.None, Resources.Strings.EcsCompatibilityMode_Enabled_ToolTipDescription },
                        { EcsStatusFlags.EnabledForRequiredAndOptional, Resources.Strings.EcsCompatibilityMode_Limited_ToolTipDescription },
                        { EcsStatusFlags.EnabledForRequired, Resources.Strings.EcsCompatibilityMode_Strict_ToolTipDescription },
                        { EcsStatusFlags.Disabled, Resources.Strings.EcsCompatibilityMode_Disabled_ToolTipDescription }
                    };
                }
                return _ecsCompatiblityInfoTable;
            }
        }
        private static Dictionary<EcsStatusFlags, string> _ecsCompatiblityInfoTable;

        private static Dictionary<IntellivisionIIStatusFlags, string> IntvIICompatiblityInfoTable
        {
            get
            {
                if (_intvIICompatiblityInfoTable == null)
                {
                    _intvIICompatiblityInfoTable = new Dictionary<IntellivisionIIStatusFlags, string>()
                        {
                            { IntellivisionIIStatusFlags.None, Resources.Strings.IntellivisionIICompatibilityMode_Disabled_ToolTipDescription },
                            { IntellivisionIIStatusFlags.Conservative, Resources.Strings.IntellivisionIICompatibilityMode_Limited_ToolTipDescription },
                            { IntellivisionIIStatusFlags.Aggressive, Resources.Strings.IntellivisionIICompatibilityMode_Full_ToolTipDescription }
                        };
                }
                return _intvIICompatiblityInfoTable;
            }
        }
        private static Dictionary<IntellivisionIIStatusFlags, string> _intvIICompatiblityInfoTable;

        private static Dictionary<ShowTitleScreenFlags, string> ShowTitleScreenInfoTable
        {
            get
            {
                if (_showTitleScreenInfoTable == null)
                {
                    _showTitleScreenInfoTable = new Dictionary<ShowTitleScreenFlags, string>()
                        {
                            { ShowTitleScreenFlags.Always, Resources.Strings.ShowTitleScreen_Always_ToolTipDescription },
                            { ShowTitleScreenFlags.OnPowerUp, Resources.Strings.ShowTitleScreen_OnPowerUp_ToolTipDescription },
                            { ShowTitleScreenFlags.Never, Resources.Strings.ShowTitleScreen_Never_ToolTipDescription }
                        };
                }
                return _showTitleScreenInfoTable;
            }
        }
        private static Dictionary<ShowTitleScreenFlags, string> _showTitleScreenInfoTable;

        private static Dictionary<SaveMenuPositionFlags, string> SaveMenuPositionInfoTable
        {
            get
            {
                if (_saveMenuPositionInfoTable == null)
                {
                    _saveMenuPositionInfoTable = new Dictionary<SaveMenuPositionFlags, string>()
                        {
                            { SaveMenuPositionFlags.Always, Resources.Strings.SaveMenuPosition_Always_ToolTipDescription },
                            { SaveMenuPositionFlags.DuringSessionOnly, Resources.Strings.SaveMenuPosition_SessionOnly_ToolTipDescription },
                            { SaveMenuPositionFlags.Reserved, Resources.Strings.SaveMenuPosition_SessionOnly_ToolTipDescription },
                            { SaveMenuPositionFlags.Never, Resources.Strings.SaveMenuPosition_Never_ToolTipDescription }
                        };
                }
                return _saveMenuPositionInfoTable;
            }
        }
        private static Dictionary<SaveMenuPositionFlags, string> _saveMenuPositionInfoTable;

        /// <summary>
        /// Gets a value indicating whether this ViewModel is associated with a valid device.
        /// </summary>
        public bool IsValid
        {
            get { return (Device != null) && Device.IsValid; }
        }

        /// <summary>
        /// Gets the actual device this is a ViewModel for.
        /// </summary>
        public Device Device
        {
            get { return _device; }
        }
        private Device _device;

        /// <summary>
        /// Gets the display name to show for the device, which may include additional information.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            private set { AssignAndUpdateProperty(DisplayNamePropertyName, value, ref _displayName); }
        }
        private string _displayName;

        /// <summary>
        /// Gets or sets the name of the owner of the device.
        /// </summary>
        public string Owner
        {
            get { return _owner; }
            set { AssignAndUpdateProperty(Device.OwnerPropertyName, value, ref _owner, (p, o) => UpdateOwner(o)); }
        }
        private string _owner;

        /// <summary>
        /// Gets the device-specific ID.
        /// </summary>
        public string UniqueId
        {
            get { return (Device == null) ? NoDevice : Device.UniqueId; }
        }

        #region ECS Setting

        /// <summary>
        /// Gets or sets a value indicating how the Locutus device treats programs with known ECS compatibility problems.
        /// </summary>
        public EcsStatusFlags EcsCompatibility
        {
            get { return (Device == null) ? _ecsCompatibility : Device.EcsCompatibility; }
            set { AssignAndUpdateProperty(Device.EcsCompatibilityPropertyName, value, ref _ecsCompatibility, (p, v) => UpdateCompatibilityMode(DeviceStatusCategory.Ecs, (byte)v)); }
        }
        private EcsStatusFlags _ecsCompatibility;

        /// <summary>
        /// Gets the tool tip info title for ECS compatibility mode.
        /// </summary>
        public string EcsCompatibilityInfoTitle
        {
            get { return _ecsCompatibilityInfoTitle; }
            private set { AssignAndUpdateProperty("EcsCompatibilityInfoTitle", value, ref _ecsCompatibilityInfoTitle); }
        }
        private string _ecsCompatibilityInfoTitle;

        /// <summary>
        /// Gets the detailed information for ECS compatibility mode tool tip.
        /// </summary>
        public string EcsCompatibilityInfo
        {
            get { return _ecsCompatibilityInfo; }
            private set { AssignAndUpdateProperty("EcsCompatibilityInfo", value, ref _ecsCompatibilityInfo); }
        }
        private string _ecsCompatibilityInfo;

        #endregion // ECS Setting

        #region Intellivision II Setting

        /// <summary>
        /// Gets or sets a value indicating how the Locutus device treats programs with known Intellivision II compatibility problems.
        /// </summary>
        public IntellivisionIIStatusFlags IntvIICompatibility
        {
            get { return (Device == null) ? _intvIICompatibility : Device.IntvIICompatibility; }
            set { AssignAndUpdateProperty(Device.IntvIICompatibilityPropertyName, value, ref _intvIICompatibility, (p, v) => UpdateCompatibilityMode(DeviceStatusCategory.IntvII, (byte)v)); }
        }
        private IntellivisionIIStatusFlags _intvIICompatibility;

        /// <summary>
        /// Gets the tool tip info title for Intellivision II compatibility mode.
        /// </summary>
        public string IntvIICompatibilityInfoTitle
        {
            get { return _intvIICompatibilityInfoTitle; }
            private set { AssignAndUpdateProperty("IntvIICompatibilityInfoTitle", value, ref _intvIICompatibilityInfoTitle); }
        }
        private string _intvIICompatibilityInfoTitle;

        /// <summary>
        /// Gets the detailed information for Intellivision II compatibility mode tool tip.
        /// </summary>
        public string IntvIICompatibilityInfo
        {
            get { return _intvIICompatibilityInfo; }
            private set { AssignAndUpdateProperty("IntvIICompatibilityInfo", value, ref _intvIICompatibilityInfo); }
        }
        private string _intvIICompatibilityInfo;

        #endregion // Intellivision II Setting

        #region Show Title Screen Setting

        /// <summary>
        /// Gets or sets the behavior of showing the title screen when resetting the console with Locutus plugged in.
        /// </summary>
        public ShowTitleScreenFlags ShowTitleScreen
        {
            get { return (Device == null) ? _showTitleScreen : Device.ShowTitleScreen; }
            set { AssignAndUpdateProperty(Device.ShowTitleScreenPropertyName, value, ref _showTitleScreen, (p, v) => UpdateShowTitleScreen(v)); }
        }
        private ShowTitleScreenFlags _showTitleScreen;

        /// <summary>
        /// Gets the tool tip title for Show title screen at launch mode.
        /// </summary>
        public string ShowTitleScreenInfoTitle
        {
            get { return _showTitleScreenInfoTitle; }
            private set { AssignAndUpdateProperty("ShowTitleScreenInfoTitle", value, ref _showTitleScreenInfoTitle); }
        }
        private string _showTitleScreenInfoTitle;

        /// <summary>
        /// Gets the detailed information for Show title screen at launch mode tool tip.
        /// </summary>
        public string ShowTitleScreenInfo
        {
            get { return _showTitleScreenInfo; }
            private set { AssignAndUpdateProperty("ShowTitleScreenInfo", value, ref _showTitleScreenInfo); }
        }
        private string _showTitleScreenInfo;

        #endregion // Show Title Screen Setting

        #region Save Menu Position Setting

        /// <summary>
        /// Gets or sets whether LTO FLash! remembers its previous menu position when reset.
        /// </summary>
        public SaveMenuPositionFlags SaveMenuPosition
        {
            get { return (Device == null) ? _saveMenuPosition : Device.SaveMenuPosition; }
            set { AssignAndUpdateProperty(Device.SaveMenuPositionPropertyName, value, ref _saveMenuPosition, (p, v) => UpdateSaveMenuPosition(v)); }
        }
        private SaveMenuPositionFlags _saveMenuPosition;

        /// <summary>
        /// Gets the tool tip title for Show title screen at launch mode.
        /// </summary>
        public string SaveMenuPositionInfoTitle
        {
            get { return _saveMenuPositionInfoTitle; }
            private set { AssignAndUpdateProperty("SaveMenuPositionInfoTitle", value, ref _saveMenuPositionInfoTitle); }
        }
        private string _saveMenuPositionInfoTitle;

        /// <summary>
        /// Gets the detailed information for Show title screen at launch mode tool tip.
        /// </summary>
        public string SaveMenuPositionInfo
        {
            get { return _saveMenuPositionInfo; }
            private set { AssignAndUpdateProperty("SaveMenuPositionInfo", value, ref _saveMenuPositionInfo); }
        }
        private string _saveMenuPositionInfo;

        #endregion // Save Menu Position Setting

        /// <summary>
        /// Gets or sets a value indicating whether Locutus does background file system garbage collection when at the menu on the console.
        /// </summary>
        public bool BackgroundGC
        {
            get { return (Device == null) ? _backgroundGC : Device.BackgroundGC; }
            set { AssignAndUpdateProperty(Device.BackgroundGCPropertyName, value, ref _backgroundGC, (p, v) => UpdateBackgroundGC(v)); }
        }
        private bool _backgroundGC;

        /// <summary>
        /// Gets or sets a value indicating whether the menu program on Locutus emits key click sounds when navigating.
        /// </summary>
        public bool Keyclicks
        {
            get { return (Device == null) ? _keyclicks : Device.Keyclicks; }
            set { AssignAndUpdateProperty(Device.KeyclicksPropertyName, value, ref _keyclicks, (p, v) => UpdateKeyclicks(v)); }
        }
        private bool _keyclicks;

        /// <summary>
        /// Gets a value indicating whether the device is in a state that allows configuration changes to be applied.
        /// </summary>
        public bool IsConfigurable
        {
            get { return Device.CanExecuteCommand(DeviceCommandGroup.SetIntellivisionIICompatibilityCommand); }
        }

        /// <summary>
        /// Gets a string describing the power state of the console a Locutus is plugged into, if possible.
        /// </summary>
        public string PowerState
        {
            get { return _powerState; }
            private set { AssignAndUpdateProperty("PowerState", value, ref _powerState, (p, v) => RaisePropertyChanged("IsPowerOn")); }
        }
        private string _powerState;

        /// <summary>
        /// Gets a value indicating whether a Locutus is plugged into a powered-up Intellivision. If <c>true</c>, power is on.
        /// If <c>false</c>, the power state is unknown.
        /// </summary>
        public bool IsPowerOn
        {
            get { return IsValid && (Device != null) && Device.IsConnectedToIntellivision; }
        }

        #region IPeripheral Properties

        /// <inheritdoc />
        public string Name
        {
            get { return (Device == null) ? NoDevice : Device.CustomName; }
            set { UpdateProperty("Name", value, _name, (p, n) => UpdateName(n)); }
        }
        private string _name;

        /// <inheritdoc />
        public IEnumerable<INTV.Core.Model.Device.IConnection> Connections
        {
            get { return (Device == null) ? Enumerable.Empty<INTV.Core.Model.Device.IConnection>() : _device.Connections; }
        }

        #endregion // IPeripheral Properties

        #endregion // Properties

        /// <summary>
        /// Factory function to create new DeviceViewModel instances.
        /// </summary>
        /// <param name="device">The device for which a ViewModel is needed.</param>
        /// <returns>A new instance of DeviceViewModel.</returns>
        public static DeviceViewModel Factory(Device device)
        {
            return new DeviceViewModel(device);
        }

        #region IPeripheral

        /// <inheritdoc />
        public bool IsRomCompatible(IProgramDescription programDescription)
        {
            var isCompatible = true;
            if (isCompatible)
            {
                var isLtoOnly = programDescription.Rom.IsLtoFlashOnlyRom();
                if (IsValid)
                {
                    isCompatible = programDescription.Rom.CanExecuteOnDevice(UniqueId);
                }
                else
                {
                    isCompatible = !isLtoOnly;
                }
            }
            return isCompatible;
        }

        #endregion // IPeripheral

        private bool ErrorHandler(DeviceStatusFlagsLo deviceStatusFlags, ProtocolCommandId commandId, string errorMessage, System.Exception exception)
        {
            bool handled = false;
            var title = string.Empty;
            var messageFormat = string.Empty;
            var showMessageBox = false;
            switch (commandId)
            {
                case ProtocolCommandId.SetConfiguration:
                    handled = true;
                    showMessageBox = handled;
                    switch (deviceStatusFlags)
                    {
                        case DeviceStatusFlagsLo.IntellivisionIIStatusMask:
                            title = Resources.Strings.SetConfigurationCommand_IntellivisionII_Failed_Title;
                            messageFormat = Resources.Strings.SetConfigurationCommand_IntellivisionII_Failed_Message_Format;
                            break;
                        case DeviceStatusFlagsLo.EcsStatusMask:
                            title = Resources.Strings.SetConfigurationCommand_Ecs_Failed_Title;
                            messageFormat = Resources.Strings.SetConfigurationCommand_Ecs_Failed_Message_Format;
                            break;
                        case DeviceStatusFlagsLo.ShowTitleScreenMask:
                            title = Resources.Strings.SetConfigurationCommand_ShowTitleScreen_Failed_Title;
                            messageFormat = Resources.Strings.SetConfigurationCommand_ShowTitleScreen_Failed_Message_Format;
                            break;
                        case DeviceStatusFlagsLo.SaveMenuPositionMask:
                            title = Resources.Strings.SetConfigurationCommand_SaveMenuPosition_Failed_Title;
                            messageFormat = Resources.Strings.SetConfigurationCommand_SaveMenuPosition_Failed_Message_Format;
                            break;
                        case DeviceStatusFlagsLo.BackgroundGC:
                            title = Resources.Strings.SetConfigurationCommand_BackgroundGC_Failed_Title;
                            messageFormat = Resources.Strings.SetConfigurationCommand_BackgroundGC_Failed_Message_Format;
                            break;
                        case DeviceStatusFlagsLo.Keyclicks:
                            title = Resources.Strings.SetConfigurationCommand_Keyclicks_Failed_Title;
                            messageFormat = Resources.Strings.SetConfigurationCommand_Keyclicks_Failed_Message_Format;
                            break;
                    }
                    break;
                case ProtocolCommandId.DownloadCrashLog:
                    handled = true;
                    showMessageBox = handled;
                    title = Resources.Strings.GetErrorAndCrashLogs_Failed_Title;
                    messageFormat = Resources.Strings.GetErrorAndCrashLogs_Failed_Message_Format;
                    break;
                case ProtocolCommandId.UnknownCommand:
                    handled = true;
                    var application = SingleInstanceApplication.Instance;
                    if ((application != null) && (application.MainWindow != null))
                    {
                        var portName = "<null>";
                        if ((Device != null) && (Device.Port != null))
                        {
                            portName = Device.Port.Name;
                        }
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceValidation_Failed_Message_Format, portName);
                        var dialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.DeviceValidation_Failed_Title, message);
                        dialog.ReportText = errorMessage;
                        dialog.ShowSendEmailButton = false;
                        dialog.ShowCopyToClipboardButton = false;
                        dialog.BeginInvokeDialog(Resources.Strings.OK, null);
                    }
                    break;
                default:
                    break;
            }
            if (showMessageBox && handled)
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, messageFormat, errorMessage);
                OSMessageBox.Show(message, title, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, (r) => { });
            }
            return handled;
        }

        private void UpdateDisplayName()
        {
            var port = _device.Port;
            var displayName = "<Unknown>";
            if (port is INTV.Shared.Model.Device.SerialPortConnection)
            {
                if (Device.IsValid)
                {
                    displayName = string.Format("{0} ({1})", Device.Name, port);
                }
                else
                {
                    displayName = Device.Name;
                }
            }
            else if (port is INTV.Shared.Model.Device.NamedPipeConnection)
            {
                displayName = "Simulated";
            }
            DisplayName = displayName;
        }

        private void UpdateName(string name)
        {
            if (Device != null)
            {
                Device.UpdateDeviceName(name, DeviceCommandGroup.SetDeviceNameCommandErrorHandler);
            }
        }

        private void UpdateOwner(string owner)
        {
            if (Device != null)
            {
                Device.UpdateDeviceOwner(owner, DeviceCommandGroup.SetDeviceOwnerCommandErrorHandler);
            }
        }

        private void UpdateCompatibilityMode(DeviceStatusCategory which, byte compatibilityMode)
        {
            switch (which)
            {
                case DeviceStatusCategory.Ecs:
                    var ecsFlags = (EcsStatusFlags)compatibilityMode;
                    EcsCompatibilityInfo = EcsCompatiblityInfoTable[ecsFlags];
                    EcsCompatibilityInfoTitle = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.EcsCompatibilityMode_ToolTipTitleFormat, ecsFlags.ToDisplayString());
                    if (Device != null)
                    {
                        Device.EcsCompatibility = ecsFlags;
                    }
                    break;
                case DeviceStatusCategory.IntvII:
                    var intvIIFlags = (IntellivisionIIStatusFlags)compatibilityMode;
                    IntvIICompatibilityInfo = IntvIICompatiblityInfoTable[intvIIFlags];
                    IntvIICompatibilityInfoTitle = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.IntellivisionIICompatibilityMode_ToolTipTitleFormat, intvIIFlags.ToDisplayString());
                    if (Device != null)
                    {
                        Device.IntvIICompatibility = intvIIFlags;
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateShowTitleScreen(ShowTitleScreenFlags showTitleScreen)
        {
            if (Device != null)
            {
                Device.ShowTitleScreen = showTitleScreen;
            }
            ShowTitleScreenInfo = ShowTitleScreenInfoTable[showTitleScreen];
            ShowTitleScreenInfoTitle = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ShowTitleScreen_ToolTipTitleFormat, showTitleScreen.ToDisplayString());
        }

        private void UpdateSaveMenuPosition(SaveMenuPositionFlags saveMenuPosition)
        {
            if (Device != null)
            {
                Device.SaveMenuPosition = saveMenuPosition;
            }
            SaveMenuPositionInfo = SaveMenuPositionInfoTable[saveMenuPosition];
            SaveMenuPositionInfoTitle = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.SaveMenuPosition_ToolTipTitleFormat, saveMenuPosition.ToDisplayString());
        }

        private void UpdateBackgroundGC(bool backgroundGC)
        {
            if (Device != null)
            {
                Device.BackgroundGC = backgroundGC;
            }
        }

        private void UpdateKeyclicks(bool keyclicks)
        {
            if (Device != null)
            {
                Device.Keyclicks = keyclicks;
            }
        }

        private void UpdatePowerState()
        {
            var powerState = Resources.Strings.ConsolePowerState_Unknown;
            var powerOn = false;
            if (IsValid)
            {
                powerOn = Device.IsConnectedToIntellivision;
                powerState = Device.IsConnectedToIntellivision ? Resources.Strings.ConsolePowerState_On : Resources.Strings.ConsolePowerState_Off;
            }
            PowerState = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ConsolePowerState_Format, powerState);
            INTV.Shared.Model.Program.ProgramCollection.Roms.CanEditElements = !powerOn;
        }

        private void DevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var device = (Device)sender;
            bool invalidateRequerySuggested = true;
            switch (e.PropertyName)
            {
                case Device.CustomNamePropertyName:
                    _name = Device.CustomName;
                    RaisePropertyChanged("Name");
                    break;
                case Device.NamePropertyName:
                    UpdateDisplayName();
                    RaisePropertyChanged(e.PropertyName);
                    break;
                case Device.PortPropertyName:
                    UpdateDisplayName();
                    break;
                case Device.IsValidPropertyName:
                case Device.IsIntellivisionConnectedPropertyName:
                    RaisePropertyChanged(e.PropertyName);
                    RaisePropertyChanged(IsConfigurablePropertyName);
                    UpdatePowerState();
                    UpdateDisplayName();
                    if (device.IsValid && (e.PropertyName == Device.IsValidPropertyName))
                    {
                        OnDeviceHardwareStatusFlagsChanged();
                    }
                    break;
                case Device.EcsCompatibilityPropertyName:
                    _ecsCompatibility = Device.EcsCompatibility;
                    RaisePropertyChanged(e.PropertyName);
                    UpdateCompatibilityMode(DeviceStatusCategory.Ecs, (byte)_ecsCompatibility);
                    break;
                case Device.IntvIICompatibilityPropertyName:
                    _intvIICompatibility = Device.IntvIICompatibility;
                    RaisePropertyChanged(e.PropertyName);
                    UpdateCompatibilityMode(DeviceStatusCategory.IntvII, (byte)_intvIICompatibility);
                    break;
                case Device.ShowTitleScreenPropertyName:
                    _showTitleScreen = Device.ShowTitleScreen;
                    RaisePropertyChanged(e.PropertyName);
                    UpdateShowTitleScreen(_showTitleScreen);
                    break;
                case Device.SaveMenuPositionPropertyName:
                    _saveMenuPosition = Device.SaveMenuPosition;
                    RaisePropertyChanged(e.PropertyName);
                    UpdateSaveMenuPosition(_saveMenuPosition);
                    break;
                case Device.BackgroundGCPropertyName:
                    _backgroundGC = Device.BackgroundGC;
                    RaisePropertyChanged(e.PropertyName);
                    break;
                case Device.KeyclicksPropertyName:
                    _keyclicks = Device.Keyclicks;
                    RaisePropertyChanged(e.PropertyName);
                    break;
                case Device.UniqueIdPropertyName:
                case Device.FileSystemPropertyName:
                case Device.FirmwareRevisionsPropertyName:
                case Device.FileSystemStatisticsPropertyName:
                    RaisePropertyChanged(e.PropertyName);
                    break;
                case Device.FileSystemStatusPropertyName:
                    ReportFileSystemInconsistencies(Device.FileSystem, null);
                    break;
                case Device.OwnerPropertyName:
                    _owner = Device.Owner;
                    RaisePropertyChanged(e.PropertyName);
                    break;
                case Device.ConnectionStatePropertyName:
                    INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                    break;
                case Device.DeviceStatusUpdatePeriodPropertyName:
                    invalidateRequerySuggested = false;
                    break;
                case Device.HardwareStatusPropertyName:
                    OnDeviceHardwareStatusFlagsChanged();
                    break;
                default:
                    break;
            }

            if (invalidateRequerySuggested)
            {
                // This works around a bug in the .NET ribbon control library, in which RibbonButtons (and others)
                // do not correctly update their enabled state when bound to commands whose CanExecute result changes.
                INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OnDeviceHardwareStatusFlagsChanged()
        {
            if (IsValid && (Device.HardwareStatus.HasFlag(HardwareStatusFlags.NewErrorLogAvailable) || Device.HardwareStatus.HasFlag(HardwareStatusFlags.NewCrashLogAvailable)))
            {
                Device.GetErrorAndCrashLogs(GetDeviceErrorAndCrashLogsComplete, (m, e) => ErrorHandler(DeviceStatusFlagsLo.None, ProtocolCommandId.DownloadCrashLog, m, e));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The binary writer class leaves the stream open, so we need ensure these streams are properly disposed.")]
        private void GetDeviceErrorAndCrashLogsComplete(ErrorLog errorLog, CrashLog crashLog)
        {
            var hasErrorLog = (errorLog != null) && !errorLog.IsEmpty;
            var hasCrashLog = crashLog != null;
            if (hasErrorLog || hasCrashLog)
            {
                var reportText = (new System.Text.StringBuilder(Resources.Strings.CrashReporter_DetailHeader)).AppendLine();
                if (Device != null)
                {
                    reportText.AppendLine("Device Primary Firmware Version: " + FirmwareRevisions.FirmwareVersionToString(Device.FirmwareRevisions.Primary, false));
                    reportText.AppendLine("Device Secondary Firmware Version: " + FirmwareRevisions.FirmwareVersionToString(Device.FirmwareRevisions.Secondary, false));
                    reportText.AppendLine("Device Current Firmware Version: " + FirmwareRevisions.FirmwareVersionToString(Device.FirmwareRevisions.Current, false));
                }
                else
                {
                    reportText.AppendLine("Device firmware versions unavailable");
                }
                reportText.AppendLine(Resources.Strings.CrashReporter_Detail_DeviceId + " " + Device.UniqueId);
                var timestamp = PathUtils.GetTimeString(true);
                reportText.AppendLine(Resources.Strings.CrashReporter_Detail_Timestamp + " " + timestamp);
                var errorReportFiles = new List<string>();
                if (hasCrashLog)
                {
                    var crashLogFileName = string.Empty;
                    try
                    {
                        System.IO.Directory.CreateDirectory(Configuration.Instance.ErrorLogDirectory);
                        crashLogFileName = PathUtils.EnsureUniqueFileName(System.IO.Path.Combine(Configuration.Instance.ErrorLogDirectory, CrashLog.CrashLogFileName));
                        using (var crashLogFile = System.IO.File.Create(crashLogFileName))
                        {
                            using (var writer = new ASCIIBinaryWriter(crashLogFile))
                            {
                                crashLog.Serialize(writer);
                            }
                        }
                        errorReportFiles.Add(crashLogFileName);
                    }
                    catch (System.Exception)
                    {
                        // We got an error trying to write the error log...
                    }
                    finally
                    {
                        if (!System.IO.File.Exists(crashLogFileName))
                        {
                            crashLogFileName = Resources.Strings.CrashReporter_Detail_CrashLogUnavailable;
                        }
                    }
                    reportText.AppendLine().AppendFormat(Resources.Strings.CrashReporter_Detail_FirmwareCrashFormat, crashLogFileName);
                }
                if (hasErrorLog)
                {
                    if (hasCrashLog)
                    {
                        reportText.AppendLine();
                    }
                    var errorLogFileName = string.Empty;
                    try
                    {
                        System.IO.Directory.CreateDirectory(Configuration.Instance.ErrorLogDirectory);
                        errorLogFileName = PathUtils.EnsureUniqueFileName(System.IO.Path.Combine(Configuration.Instance.ErrorLogDirectory, "ErrorLog.txt"));
                        using (var errorLogFile = System.IO.File.Create(errorLogFileName))
                        {
                            using (var writer = new ASCIIBinaryWriter(errorLogFile))
                            {
                                errorLog.SerializeToTextFile(Device.FirmwareRevisions.Current, writer);
                            }
                        }
                        errorReportFiles.Add(errorLogFileName);
                    }
                    catch (System.Exception)
                    {
                        // TODO Really? All exceptions? Perhaps be a bit more selective here.
                    }
                    finally
                    {
                        if (!System.IO.File.Exists(errorLogFileName))
                        {
                            errorLogFileName = Resources.Strings.CrashReporter_Detail_ErrorLogUnavailable;
                        }
                    }
                    var errorLogText = errorLog.GetDetailedErrorReport(Device.FirmwareRevisions.Current);
                    reportText.AppendLine().AppendFormat(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.CrashReporter_Detail_ErrorLogFormat, errorLogText, errorLogFileName);
                }
                var title = Resources.Strings.CrashReporter_Title;
                var message = Resources.Strings.CrashReporter_Message;
                var reportDialog = ReportDialog.Create(title, message);
                reportDialog.TextWrapping = OSTextWrapping.Wrap;
                reportDialog.ReportText = reportText.ToString();
                errorReportFiles.ForEach(f => reportDialog.Attachments.Add(f));
                if (hasErrorLog || hasCrashLog)
                {
                    var cc = new List<string>();
                    cc.Add("joe.zbiciak@LeftTurnOnly.info");
                    reportDialog.EmailCCRecipients = cc;
                }
                reportDialog.ShowDialog(Resources.Strings.Close);
            }
        }

        /// <summary>
        /// Determines whether there is a need to report file system inconsistencies.
        /// </summary>
        /// <param name="fileSystem">File system to check.</param>
        /// <param name="corruptFileSystemException">If non-<c>null</c>, <paramref name="fileSystem"/> is corrupt and cannot be used.</param>
        /// <returns><c>true</c>, if file system inconsistencies are found, <c>false</c> otherwise.</returns>
        internal static bool ReportFileSystemInconsistencies(FileSystem fileSystem, System.Exception corruptFileSystemException)
        {
            var reportProblems = (fileSystem == null) || (fileSystem.Status != LfsDirtyFlags.None) || (corruptFileSystemException != null);
            if (corruptFileSystemException is InconsistentFileSystemException)
            {
                corruptFileSystemException = null;
            }
            if (reportProblems)
            {
                // TODO Should the message content be localizable?
                var hadAny = false;
                var messageStringBuilder = new System.Text.StringBuilder();
                if (fileSystem != null)
                {
                    fileSystem.Status = LfsDirtyFlags.None;
                    if (corruptFileSystemException != null)
                    {
                        messageStringBuilder.AppendLine("The file system appears to be corrupt. If you are unable to send your menu layout, reformat your LTO Flash! and try again.").AppendLine();
                        messageStringBuilder.AppendFormat("Error message:\n\n{0}", corruptFileSystemException.Message).AppendLine();
                    }
                    else
                    {
                        var orphanedEntries = fileSystem.GetOrphanedFileSystemEntries();
                        var culture = System.Globalization.CultureInfo.CurrentCulture;

                        messageStringBuilder.AppendLine(Resources.Strings.FileSystem_Inconsistent).AppendLine();

                        hadAny = orphanedEntries.OrphanedForks.Any() || orphanedEntries.OrphanedFiles.Any() || orphanedEntries.OrphanedDirectories.Any();
                        if (hadAny)
                        {
                            messageStringBuilder.AppendLine(Resources.Strings.FileSystem_OrphanElements_Message).AppendLine();
                        }

                        var numOrphans = orphanedEntries.OrphanedDirectories.Count();
                        if (numOrphans == 1)
                        {
                            messageStringBuilder.Append("\t\t").AppendFormat(culture, Resources.Strings.FileSystem_OrphanElement_Format, Resources.Strings.DirectoryInSentence).AppendLine();
                        }
                        else if (numOrphans > 1)
                        {
                            messageStringBuilder.Append("\t\t").AppendFormat(culture, Resources.Strings.FileSystem_OrphanElements_Format, numOrphans, Resources.Strings.DirectoriesInSentence).AppendLine();
                        }

                        numOrphans = orphanedEntries.OrphanedFiles.Count();
                        if (numOrphans == 1)
                        {
                            messageStringBuilder.Append("\t\t").AppendFormat(culture, Resources.Strings.FileSystem_OrphanElement_Format, Resources.Strings.FileInSentence).AppendLine();
                        }
                        else if (numOrphans > 1)
                        {
                            messageStringBuilder.Append("\t\t").AppendFormat(culture, Resources.Strings.FileSystem_OrphanElements_Format, numOrphans, Resources.Strings.FilesInSentence).AppendLine();
                        }

                        numOrphans = orphanedEntries.OrphanedForks.Count();
                        if (numOrphans == 1)
                        {
                            messageStringBuilder.Append("\t\t").AppendFormat(culture, Resources.Strings.FileSystem_OrphanElement_Format, Resources.Strings.ForkInSentence).AppendLine();
                        }
                        else if (numOrphans > 1)
                        {
                            messageStringBuilder.Append("\t\t").AppendFormat(culture, Resources.Strings.FileSystem_OrphanElements_Format, numOrphans, Resources.Strings.ForksInSentence).AppendLine();
                        }

                        if (hadAny)
                        {
                            messageStringBuilder.AppendLine();
                        }
                        messageStringBuilder.AppendLine(Resources.Strings.FileSystem_InconsistentEnd);
                    }
                }
                else
                {
                    messageStringBuilder.Append(Resources.Strings.FileSystem_InconsistencyError_NoFileSystem_Message);
                }
                string reportText = null;
                if ((corruptFileSystemException != null) && !SingleInstanceApplication.SharedSettings.ShowDetailedErrors)
                {
                    corruptFileSystemException = null;
                }
                else if ((corruptFileSystemException != null) && !SingleInstanceApplication.SharedSettings.ShowDetailedErrors)
                {
                    reportText = corruptFileSystemException.Message;
                }
                INTV.Shared.View.OSMessageBox.Show(messageStringBuilder.ToString(), Resources.Strings.FileSystem_Inconsistent_Title, corruptFileSystemException, reportText, (r) => { });
            }
            return reportProblems;
        }

        private enum DeviceStatusCategory
        {
            /// <summary>
            /// No category.
            /// </summary>
            None,

            /// <summary>
            /// Hardware category.
            /// </summary>
            Hardware,

            /// <summary>
            /// ECS device category.
            /// </summary>
            Ecs,

            /// <summary>
            /// Intellivision II device category.
            /// </summary>
            IntvII
        }
    }
}
