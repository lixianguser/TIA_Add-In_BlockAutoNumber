using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Base class for projects
        /// can be use in multi-user environment
        /// </summary>
        private ProjectBase _projectBase;

        public AddIn(TiaPortal tiaPortal) : base("块自动编号")
        {
            _tiaPortal = tiaPortal;
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            addInRootSubmenu.Items.AddActionItem<PlcBlock>("编号", Number_OnClick);
            addInRootSubmenu.Items.AddActionItem<IEngineeringObject>("如需编号，请选中程序块",
                menuSelectionProvider => { }, TextInfoStatus);
        }

        private void Number_OnClick(MenuSelectionProvider<PlcBlock> menuSelectionProvider)
        {
            try
            {
                // Multi-user support
                // If TIA Portal is in multi user environment (connected to project server)
                if (_tiaPortal.LocalSessions.Any())
                {
                    _projectBase = _tiaPortal.LocalSessions
                        .FirstOrDefault(s => s.Project != null && s.Project.IsPrimary)?.Project;
                }
                else
                {
                    // Get local project
                    _projectBase = _tiaPortal.Projects.FirstOrDefault(p => p.IsPrimary);
                }
                
                //打开自动编号窗口
                BlockAutoNumberForm numberForm = new BlockAutoNumberForm();
                if (numberForm.ShowDialog() != DialogResult.OK)
                    return;
                //获取输入信息
                int startingNumber = numberForm.StartingNumber;
                int increment      = numberForm.Increment;

                //检测PLC是否为离线模式
                DeviceItem deviceItem = FindDeviceItem(menuSelectionProvider.GetSelection());
                if (!IsOffline(deviceItem))
                {
                    MessageBox.Show("PLC设备不是离线状态", "离线错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //写入编号
                using (ExclusiveAccess exclusiveAccess = _tiaPortal.ExclusiveAccess("编号中……"))
                {
                    using (Transaction transaction = exclusiveAccess.Transaction(_projectBase, "程序块自动编号"))
                    {
                        foreach (PlcBlock plcBlock in menuSelectionProvider.GetSelection())
                        {
                            if (exclusiveAccess.IsCancellationRequested)
                            {
                               return; 
                            }
                            exclusiveAccess.Text = "编号中-> " + plcBlock.Name + "设置为 " + startingNumber;
                            if (plcBlock.AutoNumber)
                                plcBlock.AutoNumber = false;
                            if (plcBlock.Number != startingNumber)
                                plcBlock.Number = startingNumber;
                            startingNumber += increment;
                        }

                        if (transaction.CanCommit)
                        {
                            transaction.CommitOnDispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
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