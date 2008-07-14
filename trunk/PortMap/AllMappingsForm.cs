using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using TCMPortMapper;

namespace PortMap
{
	public partial class AllMappingsForm : Form
	{
		private List<ListViewItem> mappings;


		public AllMappingsForm()
		{
			InitializeComponent();
		}

		private void AllMappingsForm_Load(object sender, EventArgs e)
		{
			mappings = new List<ListViewItem>();

			localIPLabel.Text = PortMapper.SharedInstance.LocalIPAddress.ToString();

			PortMapper.SharedInstance.DidReceiveUPNPMappingTable += new PortMapper.PMDidReceiveUPNPMappingTable(PortMapper_DidReceiveUPNPMappingTable);
			DoRefresh();
		}

		private void AllMappingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			PortMapper.SharedInstance.DidReceiveUPNPMappingTable -= new PortMapper.PMDidReceiveUPNPMappingTable(PortMapper_DidReceiveUPNPMappingTable);
		}

		private void PortMapper_DidReceiveUPNPMappingTable(PortMapper sender, List<ExistingUPnPPortMapping> existingMappings)
		{
			mappingsListView.BeginUpdate();
			{
				mappings.Clear();

				foreach (ExistingUPnPPortMapping pm in existingMappings)
				{
					String protocol;
					if(pm.TransportProtocol == PortMappingTransportProtocol.UDP)
						protocol = "UDP";
					else
						protocol = "TCP";

					ListViewItem lvi = new ListViewItem(protocol);
					lvi.SubItems.Add(pm.ExternalPort.ToString());
					lvi.SubItems.Add(pm.LocalAddress.ToString());
					lvi.SubItems.Add(pm.LocalPort.ToString());
					lvi.SubItems.Add(pm.Description);

					mappings.Add(lvi);
				}

				mappingsListView.VirtualListSize = mappings.Count;
			}
			mappingsListView.EndUpdate();

			progressPictureBox.Visible = false;
			refreshButton.Enabled = true;
		}

		private void DoRefresh()
		{
			progressPictureBox.Visible = true;
			refreshButton.Enabled = false;
			PortMapper.SharedInstance.RequestUPnPMappingTable();
		}

		private void refreshButton_Click(object sender, EventArgs e)
		{
			DoRefresh();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void mappingsListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			e.Item = mappingsListView_RetrieveVirtualItem(e.ItemIndex);
		}

		private ListViewItem mappingsListView_RetrieveVirtualItem(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= mappings.Count)
			{
				return null;
			}
			else
			{
				return mappings[rowIndex];
			}
		}
	}
}