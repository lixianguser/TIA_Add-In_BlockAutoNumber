# Enabling and disabling add-ins
## Requirements

- TIA Portal Openness is installed.

- The package files with the file extension "*.addin" were saved to "AddIns" folder in the TIA Portal installation directory. 

## Activating add-ins

To activate add-ins, follow these steps:

1. Open the "Add-ins" task card.

2. In the "Add-ins" pane, navigate to the add-in that you want to activate.

3. In the shortcut menu of the add-in or in the "Status" column, select the command "Activate".

4. You can identify activated add-ins by the <img width="16" alt="image" src="https://user-images.githubusercontent.com/36319693/170804854-8b20168b-e970-4e30-afb2-83ff99905bb4.png">
symbol in the "Status" column.

5. Repeat the procedure for all add-ins you want to activate.

6. The activated add-ins are displayed in the shortcut menus of the supported work areas.

## Deactivating add-ins

To deactivate add-ins, follow these steps:

1. Open the "Add-ins" task card.

2. In the "Add-ins" pane, navigate to the add-in that you want to deactivate.

3. In the shortcut menu of the add-in or in the "State" column, select the command "Deactivate".

4. You can identify deactivated add-ins by the <img width="18" alt="image" src="https://user-images.githubusercontent.com/36319693/170804869-91378cb8-1fb4-4d9c-968d-a13038d3b1b3.png">
symbol in the "State" column.

5. Repeat the procedure for all add-ins you want to deactivate.

6. The deactivated add-ins are displayed in the shortcut menus of the supported work areas.
 
# Running add-ins 
## Requirement

The add-ins were activated in the "Add-ins" task card.

## Procedure

To run add-ins, follow these steps:

1. Select the ***plcBlock*** in the project tree.

2. Open the shortcut menu.

3. In the shortcut menu under the "Add-Ins" entry, execute the command **"BlockAutoNumber" -> "Number"**.

4. Select or enter a ***"Starting number"*** and ***"increment"*** in the "Number" dialog，please enter a value between 1 and 59999.
 <img width="239" alt="image" src="https://user-images.githubusercontent.com/36319693/170805412-4052739a-8be4-476f-bd79-e419f2c0863c.png">

 
### Note 

Closing project via add-ins

Do not use any actions to close projects in add-ins. Closing projects via add-ins can result in an unexpected program crash.
