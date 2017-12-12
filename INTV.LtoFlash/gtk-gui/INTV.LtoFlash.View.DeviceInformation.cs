
// This file has been generated by the GUI designer. Do not modify.
namespace INTV.LtoFlash.View
{
	public partial class DeviceInformation
	{
		private global::Gtk.Notebook _informationPages;
		
		private global::INTV.LtoFlash.View.DeviceInformationPage _infoPage;
		
		private global::Gtk.Label _infoPageName;
		
		private global::INTV.LtoFlash.View.DeviceSettingsPage _settingsPage;
		
		private global::Gtk.Label _settingsPageName;
		
		private global::INTV.LtoFlash.View.DeviceFirmwarePage _firmwarePage;
		
		private global::Gtk.Label _firmwarePageName;
		
		private global::INTV.LtoFlash.View.DeviceFileSystemPage _fileSystemPage;
		
		private global::Gtk.Label _fileSystemPageName;
		
		private global::Gtk.HBox hbox2;
		
		private global::Gtk.Label label11;
		
		private global::Gtk.Label _deviceConnection;
		
		private global::Gtk.Button buttonClose;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget INTV.LtoFlash.View.DeviceInformation
			this.Name = "INTV.LtoFlash.View.DeviceInformation";
			this.Title = "LTO Flash! Device Information";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child INTV.LtoFlash.View.DeviceInformation.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this._informationPages = new global::Gtk.Notebook ();
			this._informationPages.CanFocus = true;
			this._informationPages.Name = "_informationPages";
			this._informationPages.CurrentPage = 0;
			// Container child _informationPages.Gtk.Notebook+NotebookChild
			this._infoPage = new global::INTV.LtoFlash.View.DeviceInformationPage ();
			this._infoPage.Events = ((global::Gdk.EventMask)(256));
			this._infoPage.Name = "_infoPage";
			this._informationPages.Add (this._infoPage);
			// Notebook tab
			this._infoPageName = new global::Gtk.Label ();
			this._infoPageName.Name = "_infoPageName";
			this._infoPageName.LabelProp = global::Mono.Unix.Catalog.GetString ("Information");
			this._informationPages.SetTabLabel (this._infoPage, this._infoPageName);
			this._infoPageName.ShowAll ();
			// Container child _informationPages.Gtk.Notebook+NotebookChild
			this._settingsPage = new global::INTV.LtoFlash.View.DeviceSettingsPage ();
			this._settingsPage.Events = ((global::Gdk.EventMask)(256));
			this._settingsPage.Name = "_settingsPage";
			this._informationPages.Add (this._settingsPage);
			global::Gtk.Notebook.NotebookChild w3 = ((global::Gtk.Notebook.NotebookChild)(this._informationPages [this._settingsPage]));
			w3.Position = 1;
			// Notebook tab
			this._settingsPageName = new global::Gtk.Label ();
			this._settingsPageName.Name = "_settingsPageName";
			this._settingsPageName.LabelProp = global::Mono.Unix.Catalog.GetString ("Settings");
			this._informationPages.SetTabLabel (this._settingsPage, this._settingsPageName);
			this._settingsPageName.ShowAll ();
			// Container child _informationPages.Gtk.Notebook+NotebookChild
			this._firmwarePage = new global::INTV.LtoFlash.View.DeviceFirmwarePage ();
			this._firmwarePage.Events = ((global::Gdk.EventMask)(256));
			this._firmwarePage.Name = "_firmwarePage";
			this._informationPages.Add (this._firmwarePage);
			global::Gtk.Notebook.NotebookChild w4 = ((global::Gtk.Notebook.NotebookChild)(this._informationPages [this._firmwarePage]));
			w4.Position = 2;
			// Notebook tab
			this._firmwarePageName = new global::Gtk.Label ();
			this._firmwarePageName.Name = "_firmwarePageName";
			this._firmwarePageName.LabelProp = global::Mono.Unix.Catalog.GetString ("Firmware");
			this._informationPages.SetTabLabel (this._firmwarePage, this._firmwarePageName);
			this._firmwarePageName.ShowAll ();
			// Container child _informationPages.Gtk.Notebook+NotebookChild
			this._fileSystemPage = new global::INTV.LtoFlash.View.DeviceFileSystemPage ();
			this._fileSystemPage.Events = ((global::Gdk.EventMask)(256));
			this._fileSystemPage.Name = "_fileSystemPage";
			this._informationPages.Add (this._fileSystemPage);
			global::Gtk.Notebook.NotebookChild w5 = ((global::Gtk.Notebook.NotebookChild)(this._informationPages [this._fileSystemPage]));
			w5.Position = 3;
			// Notebook tab
			this._fileSystemPageName = new global::Gtk.Label ();
			this._fileSystemPageName.Name = "_fileSystemPageName";
			this._fileSystemPageName.LabelProp = global::Mono.Unix.Catalog.GetString ("File System");
			this._informationPages.SetTabLabel (this._fileSystemPage, this._fileSystemPageName);
			this._fileSystemPageName.ShowAll ();
			w1.Add (this._informationPages);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(w1 [this._informationPages]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			w6.Padding = ((uint)(1));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Connection:");
			this.hbox2.Add (this.label11);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label11]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this._deviceConnection = new global::Gtk.Label ();
			this._deviceConnection.Name = "_deviceConnection";
			this._deviceConnection.LabelProp = global::Mono.Unix.Catalog.GetString ("label2");
			this.hbox2.Add (this._deviceConnection);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this._deviceConnection]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			w1.Add (this.hbox2);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox2]));
			w9.Position = 1;
			w9.Expand = false;
			w9.Fill = false;
			// Internal child INTV.LtoFlash.View.DeviceInformation.ActionArea
			global::Gtk.HButtonBox w10 = this.ActionArea;
			w10.Name = "dialog1_ActionArea";
			w10.Spacing = 10;
			w10.BorderWidth = ((uint)(5));
			w10.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonClose = new global::Gtk.Button ();
			this.buttonClose.CanFocus = true;
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.UseUnderline = true;
			this.buttonClose.Label = global::Mono.Unix.Catalog.GetString ("_Close");
			this.AddActionWidget (this.buttonClose, -7);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonClose]));
			w11.Expand = false;
			w11.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 568;
			this.DefaultHeight = 309;
			this.Show ();
			this._informationPages.SwitchPage += new global::Gtk.SwitchPageHandler (this.HandleSwitchPage);
		}
	}
}
