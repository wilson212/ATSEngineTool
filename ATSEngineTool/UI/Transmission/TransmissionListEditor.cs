using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public partial class TransmissionListEditor : Form
    {
        /// <summary>
        /// The Truck object we are modifying this list for
        /// </summary>
        protected Truck Truck { get; set; }

        public TransmissionListEditor(Truck truck)
        {
            // Create controls and setup the styling
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);
            shadowLabel1.Text = $"Transmission List for {truck.Name}";

            // Fix scroll bars
            listView1.Columns[0].Width -= SystemInformation.VerticalScrollBarWidth;
            listView2.Columns[0].Width -= SystemInformation.VerticalScrollBarWidth;

            // Great 2 group lists, one for installed, and one for non-installed
            Dictionary<int, ListViewGroup> groups1 = new Dictionary<int, ListViewGroup>();
            Dictionary<int, ListViewGroup> groups2 = new Dictionary<int, ListViewGroup>();

            Truck = truck;
            ListViewGroup group = new ListViewGroup();
            int index = 0;

            // Load engines from the database
            using (AppDatabase db = new AppDatabase())
            {
                // Grab trans series and create a group for each one
                foreach (var series in db.TransmissionSeries.OrderBy(x => x.ToString()))
                {
                    // Fetch name once
                    string name = series.ToString();

                    // Group 1 (not installed)
                    group = new ListViewGroup(name);
                    group.Tag = index;
                    groups1.Add(series.Id, group);
                    listView1.Groups.Add(group);
                    // Group 2 (installed)
                    group = new ListViewGroup(name);
                    group.Tag = index;
                    groups2.Add(series.Id, group);
                    listView2.Groups.Add(group);

                    // Increment index
                    index++;
                }

                // Grab installed transmission id's
                var listItems = truck.TruckTransmissions.Select(x => x.TransmissionId).ToList();

                // Fill in trucks
                foreach (var trans in db.Transmissions.OrderByDescending(x => x.Price))
                {
                    // Setup a ListView row
                    ListViewItem item = new ListViewItem();
                    item.Tag = trans;
                    item.Text = trans.Name;
                    item.SubItems.Add(trans.DifferentialRatio.ToString());

                    // Switch list depending on if the engine is installed
                    if (listItems.Contains(trans.Id))
                    {
                        groups2[trans.SeriesId].Items.Add(item);
                        listView2.Items.Add(item);
                    }
                    else
                    {
                        groups1[trans.SeriesId].Items.Add(item);
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void listView2_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (listView2.SelectedItems.Count == 0) return;

            listView2.DoDragDrop(listView2.SelectedItems, DragDropEffects.Move);
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (listView1.SelectedItems.Count == 0) return;

            listView1.DoDragDrop(listView1.SelectedItems, DragDropEffects.Move);
        }

        private void listView2_DragDrop(object sender, DragEventArgs e)
        {
            Type accpetedType = typeof(ListView.SelectedListViewItemCollection);
            var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(accpetedType);
            foreach (var listItem in items)
            {
                var item = (ListViewItem)listItem;
                int groupId = (int)item.Group.Tag;

                // Remove the engine from that list view
                listView1.Groups[groupId].Items.Remove(item);
                listView1.Items.Remove(item);

                // Add the engine to this listview
                listView2.Groups[groupId].Items.Add(item);
                listView2.Items.Add(item);
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            Type accpetedType = typeof(ListView.SelectedListViewItemCollection);
            var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(accpetedType);
            foreach (var listItem in items)
            {
                var item = (ListViewItem)listItem;
                int groupId = (int)item.Group.Tag;

                // Remove the engine from that list view
                listView2.Groups[groupId].Items.Remove(item);
                listView2.Items.Remove(item);

                // Add the engine to this listview
                listView1.Groups[groupId].Items.Add(item);
                listView1.Items.Add(item);
            }
        }

        /// <summary>
        /// Event for both ListViews in which we determine whether the
        /// data type for the item being dropped in a <see cref="ListViewItem"/>,
        /// and also that we are not dropping on the originator list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            // Grab out ListView object sending the file
            ListView view = (ListView)sender;
            DragDropEffects effect = DragDropEffects.Move;
            Type accpetedType = typeof(ListView.SelectedListViewItemCollection);

            // If this is an ListViewItemCollection
            if (e.Data.GetDataPresent(accpetedType))
            {
                var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(accpetedType);
                foreach (var item in items)
                {
                    // Ensure that each item is a ListViewItem, and has
                    // an engine for its tag
                    if (!(item is ListViewItem))
                    {
                        var listItem = (ListViewItem)item;
                        if (!(listItem.Tag is Engine) || !view.Items.Contains(listItem))
                        {
                            effect = DragDropEffects.None;
                            break;
                        }
                    }
                }
            }

            e.Effect = effect;
        }

        private void confirmButton_Click(object sender, System.EventArgs e)
        {
            // Load engines from the database
            using (AppDatabase db = new AppDatabase())
            using (SQLiteTransaction trans = db.BeginTransaction())
            {
                try
                {
                    // Remove all truck engines in the database
                    foreach (var t in Truck.TruckTransmissions)
                    {
                        db.TruckTransmissions.Remove(t);
                    }

                    // Add all engines in the list
                    foreach (ListViewItem item in listView2.Items)
                    {
                        var transmission = item.Tag as Transmission;
                        var truckTrans = new TruckTransmission()
                        {
                            Transmission = transmission,
                            Truck = Truck
                        };
                        db.TruckTransmissions.Add(truckTrans);
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    // Tell the user about the failed validation error
                    MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
