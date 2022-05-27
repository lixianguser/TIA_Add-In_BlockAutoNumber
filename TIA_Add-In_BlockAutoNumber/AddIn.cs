using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.HW;
using Siemens.Engineering.Online;
using Siemens.Engineering.SW.Blocks;
using TIA_Add_In_BlockAutoNumber_UI;

namespace TIA_Add_In_BlockAutoNumber
{
    public class AddIn : ContextMenuAddIn
    {
        private readonly TiaPortal _tiaPortal;

        public AddIn(TiaPortal tiaPortal) : base("BlockAutoNumber")
        {
            _tiaPortal = tiaPortal;
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            addInRootSubmenu.Items.AddActionItem<PlcBlock>("Number", Number_OnClick);
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("Number",
                menuSelectionProvider => { }, TextInfoStatus);
        }
        
        private static void Number_OnClick(MenuSelectionProvider<PlcBlock> menuSelectionProvider)
        {
            BlockAutoNumberForm numberForm = new BlockAutoNumberForm();
            if (numberForm.ShowDialog() != DialogResult.OK)
                return;
            int startingNumber = numberForm.StartingNumber;
            int increment = numberForm.Increment;
            DeviceItem deviceItem = FindDeviceItem(menuSelectionProvider.GetSelection());
            if (IsOffline(deviceItem))
            {
                try
                {
                    foreach (PlcBlock plcBlocks in menuSelectionProvider.GetSelection())
                    {
                        if (plcBlocks.AutoNumber)
                            plcBlocks.AutoNumber = false;
                        if (plcBlocks.Number != startingNumber)
                            plcBlocks.Number = startingNumber;
                        startingNumber += increment;
                    }
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("PLC device not offline", "Offline Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static DeviceItem FindDeviceItem(IEnumerable<object> menuSelectionProvider)
        {
            foreach (IEngineeringObject engineeringObject in menuSelectionProvider)
            {
                IEngineeringObject parent = engineeringObject.Parent;
                while (!(parent is DeviceItem))
                {
                    parent = parent.Parent;
                }

                return (DeviceItem)parent;
            }
            return null;
        }

        private static bool IsOffline(IEngineeringServiceProvider item)
        {
            OnlineProvider onlineProvider = item.GetService<OnlineProvider>();
            return (onlineProvider.State == OnlineState.Offline);
        }
        
        private static MenuStatus TextInfoStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            bool show = false;

            foreach (IEngineeringObject engineeringObject in menuSelectionProvider.GetSelection())
            {
                if (engineeringObject is PlcBlock) continue;
                show = true;
                break;
            }

            return show ? MenuStatus.Disabled : MenuStatus.Hidden;
        }
    }
}

