// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

using System.CodeDom.Compiler;

namespace INTV.LtoFlash.View
{
	[Register ("SettingsPage")]
	partial class SettingsPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("SettingsPageController")]
	partial class SettingsPageController
	{
		[Action ("_reconcileDeviceMenuToLocalMenu:")]
		partial void _reconcileDeviceMenuToLocalMenu (NSObject sender);

		[Action ("_searchAtStartupAction:")]
		partial void _searchAtStartupAction (NSObject sender);

		[Action ("_validateMenuAtStartupAction:")]
		partial void _validateMenuAtStartupAction (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
