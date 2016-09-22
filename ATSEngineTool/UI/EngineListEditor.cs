using System;
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

            Truck = truck;
            ListViewGroup group1 = new ListViewGroup();
            ListViewGroup group2 = new ListViewGroup();
            int lastModelId = -1;
            int index = 0;

            // Load engines from the database
            using (AppDatabase db = new AppDatabase())
            {
                // Grab the trucks engines, ordered by Brand
                var engines = from x in db.Engines
                              orderby x.Series.ToString() ascending,
                                      x.Horsepower descending
                              select x;

                var listItems = truck.TruckEngines.Select(x => x.Engine.Id).ToList();

                // Fill in trucks
                foreach (Engine eng in engines)
                {
                    // Setup a new group?
                    if (lastModelId != eng.SeriesId)
                    {
                        lastModelId = eng.SeriesId;
                        string name = eng.Series.ToString();

                        group1 = new ListViewGroup(name);
                        group1.Tag = index;
                        engineListView1.Groups.Add(group1);

                        group2 = new ListViewGroup(name);
                        group2.Tag = index;
                        engineListView2.Groups.Add(group2);

                        index++;
                    }

                    ListViewItem item = new ListViewItem();
                    item.Tag = eng;
                    item.Text = eng.Name;
                    item.SubItems.Add(eng.Horsepower.ToString());
                    item.SubItems.Add(eng.Torque.ToString());

                    if (listItems.Contains(eng.Id))
                    {
                        engineListView2.Items.Add(item);
                        group2.Items.Add(item);
                    }
                    else
                    {
                        engineListView1.Items.Add(item);
                        group1.Items.Add(item);
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
