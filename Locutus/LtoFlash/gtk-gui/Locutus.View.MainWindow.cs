
// This file has been generated by the GUI designer. Do not modify.
namespace Locutus.View
{
	public partial class MainWindow
	{
		private global::Gtk.UIManager UIManager;
		
		private global::Gtk.VBox _mainVBox;
		
		private global::Gtk.MenuBar _menubar;
		
		private global::Gtk.Toolbar _toolbar;
		
		private global::Gtk.HPaned _mainHPaned;
		
		private global::Gtk.Button _child1;
		
		private global::Gtk.Notebook _layoutsNotebook;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Statusbar _statusbar;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Locutus.View.MainWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "Locutus.View.MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("LTO Flash!");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("LtoFlash.Resources.LTOFlashApplicationIcon.ico");
			this.DefaultWidth = 1024;
			this.DefaultHeight = 768;
			// Container child Locutus.View.MainWindow.Gtk.Container+ContainerChild
			this._mainVBox = new global::Gtk.VBox ();
			this._mainVBox.Name = "_mainVBox";
			this._mainVBox.Spacing = 6;
			// Container child _mainVBox.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><menubar name='_menubar'/></ui>");
			this._menubar = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/_menubar")));
			this._menubar.Name = "_menubar";
			this._mainVBox.Add (this._menubar);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this._mainVBox [this._menubar]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child _mainVBox.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='_toolbar'/></ui>");
			this._toolbar = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/_toolbar")));
			this._toolbar.Name = "_toolbar";
			this._toolbar.ShowArrow = false;
			this._mainVBox.Add (this._toolbar);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this._mainVBox [this._toolbar]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			// Container child _mainVBox.Gtk.Box+BoxChild
			this._mainHPaned = new global::Gtk.HPaned ();
			this._mainHPaned.CanFocus = true;
			this._mainHPaned.Name = "_mainHPaned";
			this._mainHPaned.Position = 123;
			// Container child _mainHPaned.Gtk.Paned+PanedChild
			this._child1 = new global::Gtk.Button ();
			this._child1.CanFocus = true;
			this._child1.Name = "_child1";
			this._child1.UseUnderline = true;
			this._child1.Label = global::Mono.Unix.Catalog.GetString ("placeholder");
			this._mainHPaned.Add (this._child1);
			global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this._mainHPaned [this._child1]));
			w4.Resize = false;
			// Container child _mainHPaned.Gtk.Paned+PanedChild
			this._layoutsNotebook = new global::Gtk.Notebook ();
			this._layoutsNotebook.CanFocus = true;
			this._layoutsNotebook.Name = "_layoutsNotebook";
			this._layoutsNotebook.CurrentPage = 0;
			this._layoutsNotebook.TabPos = ((global::Gtk.PositionType)(3));
			this._layoutsNotebook.ShowTabs = false;
			// Notebook tab
			global::Gtk.Label w5 = new global::Gtk.Label ();
			w5.Visible = true;
			this._layoutsNotebook.Add (w5);
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("page1");
			this._layoutsNotebook.SetTabLabel (w5, this.label1);
			this.label1.ShowAll ();
			this._mainHPaned.Add (this._layoutsNotebook);
			this._mainVBox.Add (this._mainHPaned);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this._mainVBox [this._mainHPaned]));
			w7.Position = 2;
			// Container child _mainVBox.Gtk.Box+BoxChild
			this._statusbar = new global::Gtk.Statusbar ();
			this._statusbar.Name = "_statusbar";
			this._statusbar.Spacing = 6;
			this._mainVBox.Add (this._statusbar);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this._mainVBox [this._statusbar]));
			w8.Position = 3;
			w8.Expand = false;
			w8.Fill = false;
			this.Add (this._mainVBox);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}
