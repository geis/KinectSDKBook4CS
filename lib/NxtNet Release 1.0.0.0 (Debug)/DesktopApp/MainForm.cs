using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.IO.Ports;
using System.Windows.Forms;

using NxtNet;
using System.Diagnostics.CodeAnalysis;

namespace NxtNet.DesktopApp
{
	public partial class MainForm : Form
	{
		private Nxt _nxt;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load( object sender, EventArgs e )
		{
			this.cboPort.DataSource = SerialPort.GetPortNames();
			this.cboPort.Focus();
		}

		[SuppressMessage( "Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions" ), SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
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

				this.UpdateStatus();
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




		private void UpdateStatus()
		{
			Cursor.Current = Cursors.WaitCursor;

			// Retrieving version information.
			this.lblVersion.SetText( "Retrieving..." );
			this.lblVersion.Text = this._nxt.GetVersion().ToString();

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
			this.lblFreeFlash.Text = String.Format( CultureInfo.CurrentCulture, "{0:N0} bytes", deviceInfo.FreeUserFlash );

			// Retrieving and displaying the keep alive time.
			this.lblKeepAlive.SetText( "Retrieving..." );
			ulong keepAliveTime = this._nxt.KeepAlive();
			this.lblKeepAlive.Text = String.Format( CultureInfo.CurrentCulture, "{0:N0} msec", keepAliveTime );

			Cursor.Current = Cursors.Default;
		}



		[SuppressMessage( "Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions" )]
		private void btnRename_Click( object sender, EventArgs e )
		{
			if( !String.IsNullOrEmpty( this.txtName.Text ) )
			{
				Cursor.Current = Cursors.WaitCursor;
				this._nxt.SetBrickName( this.txtName.Text );
				Cursor.Current = Cursors.Default;

				MessageBox.Show( "NXT renamed successfully.", "NXT.NET", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
		}

		private void btnUpdateStatus_Click( object sender, EventArgs e )
		{
			this.UpdateStatus();
		}


		private void btnUpdateSensors_Click( object sender, EventArgs e )
		{
			this.ctrlTouchSensor.Nxt = this._nxt;
			this.ctrlTouchSensor.UpdateValues();

			this.ctrlSoundSensor.Nxt = this._nxt;
			this.ctrlSoundSensor.UpdateValues();

			this.ctrlLightSensor.Nxt = this._nxt;
			this.ctrlLightSensor.UpdateValues();

			this.ctrlUltrasonicSensor.Nxt = this._nxt;
			//this.ctrlUltrasonicSensor.UpdateValues();
		}

	}
}
