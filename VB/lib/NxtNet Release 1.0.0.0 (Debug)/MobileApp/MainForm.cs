using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Forms;

using NxtNet;
using System.Diagnostics.CodeAnalysis;

namespace NxtNet.PocketApp
{
	public partial class MainForm : Form
	{
		private Nxt _nxt;


		public MainForm()
		{
			this.InitializeComponent();
		}

		private void mnuExit_Click( object sender, EventArgs e )
		{
			if( this._nxt != null )
			{
				this._nxt.Disconnect();
				this._nxt.Dispose();
			}

			this.Close();
		}

		private void MainForm_Load( object sender, EventArgs e )
		{
			this.cboPort.DataSource = SerialPort.GetPortNames();
			this.cboPort.Focus();
		}


		[SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
		private void btnConnect_Click( object sender, EventArgs e )
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;

				this.lblStatus.SetText( "Connecting..." );

				// Connecting to the NXT.
				this._nxt = new Nxt();
				this._nxt.Connect( this.cboPort.SelectedValue.ToString() );

				this.lblStatus.Text = "Connected";

			}
			catch( Exception ex )
			{
				MessageBox.Show( ex.Message );
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void btnDisconnect_Click( object sender, EventArgs e )
		{
			Cursor.Current = Cursors.WaitCursor;

			this.lblStatus.SetText( "Disconnecting..." );

			if( this._nxt != null )
			{
				this._nxt.Disconnect();
			}

			this.lblStatus.Text = "Disconnected";
			this.lblVersion.Text = "N/A";
			this.txtName.Text = "N/A";

			Cursor.Current = Cursors.Default;
		}

		private void tabControl_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( this.tabControl.TabPages[ this.tabControl.SelectedIndex ] == this.tabStatus )
			{
				this.UpdateStatus();
			}
		}

		private void btnRename_Click( object sender, EventArgs e )
		{
			if( !String.IsNullOrEmpty( this.txtName.Text ) )
			{
				Cursor.Current = Cursors.WaitCursor;
				this._nxt.SetBrickName( this.txtName.Text );
				Cursor.Current = Cursors.Default;

				MessageBox.Show( "NXT renamed successfully." );
			}
		}

		private void btnUpdateStatus_Click( object sender, EventArgs e )
		{
			this.UpdateStatus();
		}



		private void UpdateStatus()
		{
			Cursor.Current = Cursors.WaitCursor;

			// Retrieving version information.
			this.lblVersion.SetText( "Retrieving..." );
			NxtNet.Version version = this._nxt.GetVersion();
			this.lblVersion.Text = String.Format( CultureInfo.InvariantCulture, "Fw: {0}, Proto: {1}", version.Firmware, version.Protocol );

			// Retrieving NXT brick name.
			this.txtName.SetText( "Retrieving..." );
			DeviceInfo deviceInfo = this._nxt.GetDeviceInfo();
			this.txtName.Text = deviceInfo.Name;

			// Retrieving battery level.
			ushort batteryLevel = this._nxt.GetBatteryLevel();
			this.pbBattery.Value = batteryLevel;
			this.lblBattery.Text = String.Format( CultureInfo.CurrentCulture, "{0} V", ( (decimal) batteryLevel ) / 1000 );

			// Displaying Bluetooth address.
			this.lblBluetoothAddress.Text = deviceInfo.BluetoothAddress.ToHexString();

			// Displaying available memory.
			this.lblFreeFlash.Text = deviceInfo.FreeUserFlash.ToString( "0 bytes", CultureInfo.CurrentCulture );

			Cursor.Current = Cursors.Default;
		}

	}

}