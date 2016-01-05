using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SG.Utilities.Forms {

    /// <summary>
    /// Class managing the tray icon
    /// </summary>
    public class TrayIcon {

        #region private fields

        /// <summary>
        /// 5 Seconds default time out for ballon messages
        /// </summary>
        private const int defaultBallonTimeOut = 5000;

        /// <summary>
        /// The icon element in the notification area
        /// </summary>
        private System.Windows.Forms.NotifyIcon icon;

        /// <summary>
        /// The user specified message object
        /// </summary>
        private object messageObj;

        /// <summary>
        /// Flag to skip the next click event
        /// </summary>
        private bool skipNextClick;

        /// <summary>
        /// Flag whether or not to open the context menu on click
        /// </summary>
        private bool contextMenuOnClick;

        #endregion

        #region common properties

        /// <summary>
        /// Gets or sets the icon to be visible or invisible
        /// </summary>
        public bool Visible {
            get { return this.icon.Visible; }
            set { this.icon.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the menu of the icon
        /// </summary>
        public ContextMenuStrip Menu {
            get { return this.icon.ContextMenuStrip; }
            set {
                if (this.icon.ContextMenuStrip != value) {
                    if (this.icon.ContextMenuStrip != null) {
                        try {
                            this.icon.ContextMenuStrip.Opening -= ContextMenuStrip_Opening;
                        } catch {
                        }
                    }
                    this.icon.ContextMenuStrip = value;
                    if (this.icon.ContextMenuStrip != null) {
                        this.icon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the option to show the context menu on click
        /// </summary>
        public bool ShowContextMenuOnClick {
            get { return this.contextMenuOnClick; }
            set { this.contextMenuOnClick = value; }
        }

        /// <summary>
        /// Gets the message icon
        /// </summary>
        /// <remarks>
        /// Answer 'none' if no message is currently visible
        /// </remarks>
        public ToolTipIcon MessageIcon { get; private set; }

        /// <summary>
        /// Gets or sets the icon of the trayicon
        /// </summary>
        public System.Drawing.Icon Icon {
            get { return this.icon.Icon; }
            set { this.icon.Icon = value; }
        }

        #endregion

        #region events

        /// <summary>
        /// Event fired when the message of the icon is clicked
        /// </summary>
        public event Action<TrayIcon, object> MessageClick;

        /// <summary>
        /// Event fired when the message of the icon is clicked
        /// </summary>
        public event EventHandler IconClick;

        #endregion

        #region Modal dialog management

        /// <summary>
        /// The modal dialog
        /// </summary>
        private Form modalDialog = null;

        /// <summary>
        /// Gets or sets the reference to the application modal dialog. When a
        /// modal dialog object is set, neither the context menu or the normal
        /// click event will be raised. Instead the modal form will be brought
        /// to foreground and will be focussed.
        /// </summary>
        public Form ModalDialog {
            get { return this.modalDialog; }
            set {
                if (this.modalDialog != value) {
                    if (this.modalDialog != null) {
                        this.modalDialog.FormClosed -= modalDialog_FormClosed;
                    }
                    this.modalDialog = value;
                    if (this.modalDialog != null) {
                        this.modalDialog.FormClosed += modalDialog_FormClosed;
                    }
                }
            }
        }

        /// <summary>
        /// remove the reference to the modal dialog when that form had been closed
        /// </summary>
        /// <param name="sender">The form of the modal dialog</param>
        /// <param name="e">not used</param>
        void modalDialog_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.modalDialog == sender) {
                this.ModalDialog = null;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Finds the tray icon area
        /// </summary>
        /// <param name="screen">The screen the tray icon area is on</param>
        /// <param name="location">The corner of the screen the tray icon area is at</param>
        public static void FindTrayAreaLocation(out Screen screen, out System.Drawing.Point location) {
            TrayAreaUtility.FindTrayAreaLocation(out screen, out location);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public TrayIcon() {
            this.icon = new NotifyIcon();
            this.icon.Text = Application.ProductName;
            this.icon.Visible = false;
            this.messageObj = null;
            this.icon.Click += icon_Click;
            this.icon.BalloonTipClicked += icon_BalloonTipClicked;
            this.icon.BalloonTipClosed += icon_BalloonTipClosed;
            this.skipNextClick = false;
            this.contextMenuOnClick = false;
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="timeout">The display timeout in milliseconds</param>
        /// <param name="icon">The message icon</param>
        /// <param name="obj">The message object relayed to the MessageClick callback</param>
        public void ShowMessage(string text, int timeout, ToolTipIcon icon, object obj) {
            if (icon == ToolTipIcon.Info) icon = ToolTipIcon.None;
            this.MessageIcon = (icon == ToolTipIcon.None) ? ToolTipIcon.Info : icon;
            this.messageObj = obj;
            this.icon.ShowBalloonTip(timeout, Application.ProductName, text, icon);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="timeout">The display timeout in milliseconds</param>
        /// <param name="obj">The message object relayed to the MessageClick callback</param>
        public void ShowMessage(string text, int timeout, object obj) {
            this.ShowMessage(text, timeout, ToolTipIcon.None, obj);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="icon">The message icon</param>
        /// <param name="obj">The message object relayed to the MessageClick callback</param>
        public void ShowMessage(string text, ToolTipIcon icon, object obj) {
            this.ShowMessage(text, defaultBallonTimeOut, icon, obj);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="obj">The message object relayed to the MessageClick callback</param>
        public void ShowMessage(string text, object obj) {
            this.ShowMessage(text, defaultBallonTimeOut, ToolTipIcon.None, obj);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="timeout">The display timeout in milliseconds</param>
        /// <param name="icon">The message icon</param>
        public void ShowMessage(string text, int timeout, ToolTipIcon icon) {
            this.ShowMessage(text, timeout, icon, null);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="timeout">The display timeout in milliseconds</param>
        public void ShowMessage(string text, int timeout) {
            this.ShowMessage(text, timeout, ToolTipIcon.None, null);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        /// <param name="icon">The message icon</param>
        public void ShowMessage(string text, ToolTipIcon icon) {
            this.ShowMessage(text, defaultBallonTimeOut, icon, null);
        }

        /// <summary>
        /// Shows a ballon message
        /// </summary>
        /// <param name="text">The message text</param>
        public void ShowMessage(string text) {
            this.ShowMessage(text, defaultBallonTimeOut, ToolTipIcon.None, null);
        }

        #endregion

        #region private methods

        /// <summary>
        /// Remove the message object when message is no longer shown
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void icon_BalloonTipClosed(object sender, EventArgs e) {
            this.MessageIcon = ToolTipIcon.None;
            this.messageObj = null;
        }

        /// <summary>
        /// Do not generate a click event when the context menu is to be opened
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            this.skipNextClick = true;
        }

        /// <summary>
        /// Called when the icon is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void icon_Click(object sender, EventArgs e) {
            if (this.skipNextClick) {
                this.skipNextClick = false;
                return;
            }

            try {
                if (this.modalDialog != null) {
                    this.modalDialog.BringToFront();
                    this.modalDialog.Focus();
                    this.modalDialog.Activate();
                } else if (this.contextMenuOnClick && (sender is NotifyIcon)) {
                    NotifyIcon notifyIcon = (NotifyIcon)sender;
                    notifyIcon.GetType().InvokeMember("ShowContextMenu",
                        System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.InvokeMethod,
                        null, notifyIcon, null);
                    this.skipNextClick = false;
                } else {
                    EventHandler ic = this.IconClick;
                    if (ic != null) {
                        try {
                            ic(sender, e);
                        } catch { }
                    }
                }
            } catch {
            }

        }

        /// <summary>
        /// Called when the ballon tip of the icon is clicked
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void icon_BalloonTipClicked(object sender, EventArgs e) {
            Action<TrayIcon, object> a = this.MessageClick;
            if (a != null) {
                try {
                    a(this, this.messageObj);
                } catch { }
            }
        }

        #endregion

    }

}
