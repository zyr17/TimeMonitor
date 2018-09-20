using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeMonitor
{
    public partial class NotifyIconComponent : Component
    {
        public NotifyIconComponent()
        {
            InitializeComponent();
            Initialize();
        }

        public NotifyIconComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            NotifyIconMain.Visible = true;
        }
        public string NotifyIconText
        {
            get
            {
                return NotifyIconMain.Text;
            }
            set
            {
                NotifyIconMain.Text = value;
            }
        }
        public System.Drawing.Icon NotifyIconIcon
        {
            get
            {
                return NotifyIconMain.Icon;
            }
            set
            {
                NotifyIconMain.Icon = value;
            }
        }
        public System.Windows.Forms.ToolStripItemCollection ContextMenuStripItems
        {
            get
            {
                return ContextMenuStripMain.Items;
            }
        }
        public bool NotifyIconVisibility
        {
            get
            {
                return NotifyIconMain.Visible;
            }
            set
            {
                NotifyIconMain.Visible = value;
            }
        }
        public MouseEventHandler NotifyIconDoubleClick
        {
            set
            {
                NotifyIconMain.MouseDoubleClick += value;
            }
        }
    }
}
