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
    public partial class EngineListEditor : Form
    {
        /// <summary>
        /// The Truck object we are modifying this list for
        /// </summary>
        protected Truck Truck { get; set; }

        public EngineListEditor(Truck truck)
        {
            // Create controls and setup the styling
            InitializeComponent();
            headerPanel.BackColor = Color.FromArgb(51, 53, 53);
            shadowLabel1.Text = $"Engine List for {truck.Name}";

            // Fix scroll bars
            engineListView1.Columns[2].Width -= SystemInformation.VerticalScrollBarWidth;
            engineListView2.Columns[2].Width -= SystemInformation.VerticalScrollBarWidth;

            // Set power headers
            var imp = (Program.Config.UnitSystem == UnitSystem.Imperial);
            engineListView1.Columns[2].Text = (imp) ? "Torque" : "N·m";
            engineListView2.Columns[2].Text = (imp) ? "Torque" : "N·m";

            // Great 2 group lists, one for installed, and one for non-installed
            Dictionary<int, ListViewGroup> groups1 = new Dictionary<int, ListViewGroup>();
            Dictionary<int, ListViewGroup> groups2 = new Dictionary<int, ListViewGroup>();

            // Set internals
            Truck = truck;
            ListViewGroup group;
            int index = 0;

            // Load engines from the database
            using (AppDatabase db = new AppDatabase())
            {
                // Grab engine series and create a group for each one
                foreach (var series in db.EngineSeries.OrderBy(x => x.ToString()))
                {
                    // Fetch name once
                    string name = series.ToString();

                    // Group 1 (not installed)
                    group = new ListViewGroup(name);
                    group.Tag = index;
                    groups1.Add(series.Id, group);
                    engineListView1.Groups.Add(group);
                    // Group 2 (installed)
                    group = new ListViewGroup(name);
                    group.Tag = index;
                    groups2.Add(series.Id, group);
                    engineListView2.Groups.Add(group);

                    // Increment index
                    index++;
                }

                // Get installed engines
                var listItems = truck.TruckEngines.Select(x => x.EngineId).ToList();

                // Fill in trucks
                foreach (Engine eng in db.Engines.OrderByDescending(x => x.Horsepower))
                {
                    // Create the list view row
                    ListViewItem item = new ListViewItem();
                    item.Tag = eng;
                    item.Text = eng.Name;
                    item.SubItems.Add(eng.Horsepower.ToString());

                    // Add torque / Nm
                    if (Program.Config.UnitSystem == UnitSystem.Imperial)
                        item.SubItems.Add(eng.Torque.ToString());
                    else
                        item.SubItems.Add(eng.NewtonMetres.ToString());

                    // Switch list depending on if the engine is installed
                    if (listItems.Contains(eng.Id))
                    {
                        groups2[eng.SeriesId].Items.Add(item);
                        engineListView2.Items.Add(item);
                    }
                    else
                    {
                        groups1[eng.SeriesId].Items.Add(item);
                        engineListView1.Items.Add(item);
                    }
                }
            }
        }

        private void engineListView2_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (engineListView2.SelectedItems.Count == 0) return;

            engineListView2.DoDragDrop(engineListView2.SelectedItems, DragDropEffects.Move);
        }

        private void engineListView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // If we have no selected trucks, skimp out here
            if (engineListView1.SelectedItems.Count == 0) return;

            engineListView1.DoDragDrop(engineListView1.SelectedItems, DragDropEffects.Move);
        }

        private void engineListView2_DragDrop(object sender, DragEventArgs e)
        {
            Type accpetedType = typeof(ListView.SelectedListViewItemCollection);
            var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(accpetedType);
            foreach (var listItem in items)
            {
                var item = (ListViewItem)listItem;
                int groupId = (int)item.Group.Tag;

                // Remove the engine from that list view
                engineListView1.Groups[groupId].Items.Remove(item);
                engineListView1.Items.Remove(item);

                // Add the engine to this listview
                engineListView2.Groups[groupId].Items.Add(item);
                engineListView2.Items.Add(item);
            }
        }

        private void engineListView1_DragDrop(object sender, DragEventArgs e)
        {
            Type accpetedType = typeof(ListView.SelectedListViewItemCollection);
            var items = (ListView.SelectedListViewItemCollection)e.Data.GetData(accpetedType);
            foreach (var listItem in items)
            {
                var item = (ListViewItem)listItem;
                int groupId = (int)item.Group.Tag;

                // Remove the engine from that list view
                engineListView2.Groups[groupId].Items.Remove(item);
                engineListView2.Items.Remove(item);

                // Add the engine to this listview
                engineListView1.Groups[groupId].Items.Add(item);
                engineListView1.Items.Add(item);
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
                    foreach (TruckEngine eng in Truck.TruckEngines)
                    {
                        db.TruckEngines.Remove(eng);
                    }

                    // Add all engines in the list
                    foreach (ListViewItem item in engineListView2.Items)
                    {
                        Engine engine = item.Tag as Engine;
                        TruckEngine truckEng = new TruckEngine()
                        {
                            Engine = engine,
                            Truck = Truck
                        };
                        db.TruckEngines.Add(truckEng);
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
