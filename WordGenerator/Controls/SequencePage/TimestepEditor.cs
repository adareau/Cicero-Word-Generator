using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DataStructures;

namespace WordGenerator.Controls
{
    public partial class TimestepEditor : UserControl
    {

        // This function makes use of some picture boxes with previews of UI elements, to save drawing time when
        // many of these timestep editors are first created.  The previews are swapped out for real
        // elements when the mouse moves over them.


        private bool marked;

        public bool Marked
        {
            get { return marked; }
            set { 
                marked = value;
                updateBackColor(false);
            }
        }

        /// <summary>
        /// Call this function on each timestep editor after a change in the sequence mode, to have the editor update its enabled/disabled and show/hide
        /// </summary>
        public void refreshButtonsAndGroupIndicator()
        {
            bool nowEnabled = (this.enabledButton.Text == "Enabled");

            if (nowEnabled != stepData.StepEnabled)
            {
                layoutEnableButton();
            }


            layoutShowhideButton();

            layoutGroupIndicatorLabel();
        }

        public void layoutGroupIndicatorLabel()
        {
            if (stepData.UsesTimestepGroup)
            {
                
                timestepGroupIndicatorLabel.Visible = true;
                timestepGroupLoopIndicatorLabel.Visible = false;

                if (stepData.MyTimestepGroup.LoopTimestepGroup)
                {
                    if (stepData.MyTimestepGroup.LoopCountInt > 1)
                    {
                        timestepGroupIndicatorLabel.Visible = false;
                        timestepGroupLoopIndicatorLabel.Visible = true;
                        timestepGroupLoopIndicatorLabel.Text = "L" + stepData.MyTimestepGroup.LoopCountInt;                      
                    }
                }
                

            }
            else
            {
                timestepGroupIndicatorLabel.Visible = false;
                timestepGroupLoopIndicatorLabel.Visible = false;
            }
        }

        public void updateBackColor(bool currentlyOutput)
        {
            if (Marked)
            {
                this.BackColor = Color.Salmon;
            }
            else
            {
                this.BackColor = Color.Transparent;
            }

            if (currentlyOutput)
            {
                this.BackColor = Color.DarkGray;
            }

            if (currentlyOutput && Marked)
            {
                this.BackColor = Color.DarkRed;
            }

        }

        public const int TimestepEditorWidth = 86;
        public const int TimestepEditorHeight = 215;

        private TimeStep stepData;

        public TimeStep StepData
        {
            get { return stepData; }
        }

        public event EventHandler updateGUI;

        public EventHandler messageLog;

        /// <summary>
        /// This is the number DISPLAYED above the timestep. Note that the actualy timestep index is this number MINUS ONE.
        ///
        /// </summary>
        private int stepNumber;

        public int StepNumber
        {
            get { return stepNumber; }
            set
            {
                stepNumber = value;
                redrawStepNumberLabel(stepData, stepNumber);
                
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
           base.OnPaint(e);
        }

        


        public TimestepEditor()
        {
            InitializeComponent();
            stepData = new TimeStep();
            analogSelector.Items.Add("Continue");
            analogSelector.SelectedItem = "Continue";

            gpibSelector.Items.Add("Continue");
            gpibSelector.SelectedItem = "Continue";

            rs232Selector.Items.Add("Continue");
            rs232Selector.SelectedItem = "Continue";

            // If we are using Windows 7,
            // then the combo boxes must have a non-default
            // style in order to support background colors
            if (WordGenerator.GlobalInfo.usingWindows7)
            {
                analogSelector.FlatStyle = FlatStyle.Popup;
                gpibSelector.FlatStyle = FlatStyle.Popup;
                rs232Selector.FlatStyle = FlatStyle.Popup;
            }

            durationEditor.setMinimumAllowableManualValue(0);

            this.Width = TimestepEditorWidth;
            this.Height = TimestepEditorHeight;

        }

        public TimestepEditor(TimeStep stepData, int timeStepNumber) : this()
        {
            if (stepData!=null)
                this.stepData = stepData;

            updateDescriptionTooltip(stepData);
            descriptionTextBox.Text = stepData.Description;

            this.stepNumber = timeStepNumber;

            timestepName.Text = stepData.StepName;

            this.durationEditor.setParameterData(stepData.StepDuration);
            redrawStepNumberLabel(stepData, timeStepNumber);

            analogSelector.Items.AddRange(Storage.sequenceData.AnalogGroups.ToArray());
            gpibSelector.Items.AddRange(Storage.sequenceData.GpibGroups.ToArray());
            rs232Selector.Items.AddRange(Storage.sequenceData.RS232Groups.ToArray());

            if (stepData.AnalogGroup != null)
            {
                this.analogSelector.SelectedItem = stepData.AnalogGroup;
                this.analogSelector.BackColor = Color.White;
                this.analogSelector.Visible = true;
            }
            else
            {
                this.analogSelector.SelectedItem = "Continue";
                this.analogSelector.BackColor = Color.Green;
            }


            if (stepData.GpibGroup != null)
            {
                this.gpibSelector.SelectedItem = stepData.GpibGroup;
                this.gpibSelector.BackColor = Color.White;
                this.gpibSelector.Visible = true;
            }
            else
            {
                this.gpibSelector.SelectedItem = "Continue";
                this.gpibSelector.BackColor = Color.Green;
            }

            if (stepData.rs232Group != null)
            {
                rs232Selector.SelectedItem = stepData.rs232Group;
                rs232Selector.BackColor = Color.White;
                this.rs232Selector.Visible = true;
            }
            else
            {
                rs232Selector.SelectedItem = "Continue";
                rs232Selector.BackColor = Color.Green;
            }


            layoutEnableButton();
            layoutShowhideButton();
            updatePulsesIndicator();
            layoutGroupIndicatorLabel();
            updateWaitForRetriggerIndicator();
        }

        private void redrawStepNumberLabel(TimeStep stepData, int timeStepNumber)
        {
            this.timeStepNumber.Text = timeStepNumber.ToString();

            if (stepData.HotKeyCharacter != 0)
                this.timeStepNumber.Text += " {" + char.ToUpper(stepData.HotKeyCharacter) + "}";
        }

        private void updateDescriptionTooltip(TimeStep stepData)
        {
            toolTip1.SetToolTip(this.timeStepNumber, stepData.Description);
            toolTip1.SetToolTip(this.timestepName, stepData.Description);
        }


        private void timestepName_TextChanged(object sender, EventArgs e)
        {
            stepData.StepName = timestepName.Text;
        }

        private void enabledButton_Click(object sender, EventArgs e)
        {
            stepData.StepEnabled = !stepData.StepEnabled;

            layoutEnableButton();
            if (updateGUI != null)
                updateGUI(sender, e);
        }

        private void layoutEnableButton()
        {
            if (stepData.StepEnabled)
            {
                enabledButton.Text = "Enabled";
                enabledButton.BackColor = Color.Green;
            }
            else
            {
                enabledButton.Text = "Disabled";
                enabledButton.BackColor = Color.Red;
            }
            enabledButton.Invalidate();
        }

        private void showHideButton_Click(object sender, EventArgs e)
        {
            stepData.StepHidden = !stepData.StepHidden;

            layoutShowhideButton();

            if (Storage.sequenceData.stepHidingEnabled)
            {
                WordGenerator.mainClientForm.instance.sequencePage1.showOrHideHiddenTimestepEditors();
                WordGenerator.mainClientForm.instance.sequencePage1.layoutTheRest();
            }
        }

        private void layoutShowhideButton()
        {
            if (stepData.StepHidden)
            {
                showHideButton.Text = "Hidden";
                showHideButton.BackColor = Color.DarkKhaki;
            }
            else
            {
                showHideButton.Text = "Visible";
                showHideButton.BackColor = Color.Transparent;
            }
        }

        private void analogSelector_DropDown(object sender, EventArgs e)
        {
            analogSelector.Items.Clear();
            analogSelector.BackColor = Color.White;
            analogSelector.Items.Add("Continue");
            analogSelector.Items.AddRange(Storage.sequenceData.AnalogGroups.ToArray());
        }

        private object analogSelectorBackupItem;

        private void analogSelector_SelectedValueChanged(object sender, EventArgs e)
        {
            if (analogSelector.SelectedItem.ToString() == "Continue")
            {
                analogSelector.BackColor = Color.Green;
                toolTip1.SetToolTip(analogSelector, "Continue previous analog group.");
            }
            else
                analogSelector.BackColor = Color.White;

            if (stepData != null)
            {
                AnalogGroup ag = analogSelector.SelectedItem as AnalogGroup;
                stepData.AnalogGroup = ag;
                if (updateGUI != null)
                {
                    updateGUI(sender, e);
                }
                if (ag != null)
                    toolTip1.SetToolTip(analogSelector, ag.GroupDescription);

            }
            analogSelectorBackupItem = analogSelector.SelectedItem;
        }

        private object gpibSelectorBackupItem;

        private void gpibSelector_SelectedValueChanged(object sender, EventArgs e)
        {
            if (gpibSelector.SelectedItem.ToString() == "Continue")
            {
                gpibSelector.BackColor = Color.Green;
                toolTip1.SetToolTip(gpibSelector, "Continue previous GPIB group.");
            }
            else
                gpibSelector.BackColor = Color.White;

            if (stepData != null)
            {
                GPIBGroup gg = gpibSelector.SelectedItem as GPIBGroup;
                stepData.GpibGroup = gg;
                if (updateGUI != null)
                    updateGUI(sender, e);
                if (gg != null)
                    toolTip1.SetToolTip(gpibSelector, gg.GroupDescription);
            }

            gpibSelectorBackupItem = gpibSelector.SelectedItem;
        }

        private void gpibSelector_DropDown(object sender, EventArgs e)
        {
            gpibSelector.Items.Clear();
            gpibSelector.BackColor = Color.White;
            gpibSelector.Items.Add("Continue");
            gpibSelector.Items.AddRange(Storage.sequenceData.GpibGroups.ToArray());
        }

        private void outputNowToolStripMenuItem_Click(object sender, EventArgs e)
        {

            outputTimestepNow(false, true);

        }

        public void unRegsiterHotkey()
        {
            if (stepData.HotKeyCharacter != 0)
            {
                WordGenerator.mainClientForm.instance.unregisterHotkey(stepData.HotKeyCharacter, this);
            }
        }

        public void registerHotkey()
        {
            if (stepData.HotKeyCharacter != 0)
            {
                WordGenerator.mainClientForm.instance.registerTimestepHotkey(stepData.HotKeyCharacter, this);
            }
        }

        public void updatePulsesIndicator()
        {
            if (stepData == null)
            {
                pulseIndicator.Visible = false;
                return;
            }

            if (stepData.usesPulses())
            {
                pulseIndicator.Visible = true;
            }
            else
            {
                pulseIndicator.Visible = false;
            }

        }

        public bool outputTimestepNow()
        {
            return outputTimestepNow(false, false);
        }

        /// <summary>
        /// outputs the editor's timestep. set silent to true if no message logs should be generated.
        /// </summary>
        /// <param name="silent"></param>
        /// <returns></returns>
        public bool outputTimestepNow(bool silent, bool showErrorDialog)
        {
            List<string> unconnectedServers = Storage.settingsData.unconnectedRequiredServers();

            if (!Storage.sequenceData.Lists.ListLocked)
            {
                WordGenerator.mainClientForm.instance.variablesEditor1.tryLockLists();
            }
            if (!Storage.sequenceData.Lists.ListLocked)
            {
                if (!silent)
                    messageLog(this, new MessageEvent("Unable to output timestep, lists not locked."));
                if (showErrorDialog)
                {
                    MessageBox.Show("Unable to output timestep, lists not locked.");
                }
                return false;
            }

            if (unconnectedServers.Count == 0)
            {

                WordGenerator.mainClientForm.instance.cursorWait();



                ServerManager.ServerActionStatus actionStatus = Storage.settingsData.serverManager.outputSingleTimestepOnConnectedServers(
                    Storage.settingsData,
                    Storage.sequenceData.getSingleOutputFrameAtEndOfTimestep(this.stepNumber - 1, Storage.settingsData, Storage.settingsData.OutputAnalogDwellValuesOnOutputNow),
                    messageLog);

                WordGenerator.mainClientForm.instance.cursorWaitRelease();

                if (actionStatus == ServerManager.ServerActionStatus.Success)
                {
                    if (!silent)
                        messageLog(this, new MessageEvent("Successfully output timestep " + stepData.ToString()));
                    WordGenerator.mainClientForm.instance.CurrentlyOutputtingTimestep = this.stepData;
                    return true;
                }
                else
                {
                   
                    if (!silent)
                        messageLog(this, new MessageEvent("Communication or server error attempting to output this timestep: " + actionStatus.ToString()));
                    if (showErrorDialog)
                    {
                        MessageBox.Show("Communication or server error attempting to output this timestep: " + actionStatus.ToString());
                    }
                }
            }
            else
            {
                string missingServerList = ServerManager.convertListOfServersToOneString(unconnectedServers);
                if (!silent)
                    messageLog(this, new MessageEvent("Unable to output this timestep. The following required servers are not connected: " + missingServerList));
            
                if (showErrorDialog) {
                    MessageBox.Show("Unable to output this timestep. The following required servers are not connected: " + missingServerList);
                }
            }
            return false;
        }

        private void insertTimestepBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeStep newStep = new TimeStep("New timestep.");
            Storage.sequenceData.TimeSteps.Insert(stepNumber-1, newStep );
            Storage.sequenceData.populateWithChannels(Storage.settingsData);
            Storage.sequenceData.timestepsInsertedOrMoved();

            TimestepEditor te = new TimestepEditor(newStep, stepNumber);

            WordGenerator.mainClientForm.instance.sequencePage1.insertTimestepEditor(te, stepNumber - 1);


        //    WordGenerator.mainClientForm.instance.RefreshSequenceDataToUI(Storage.sequenceData);
        //    WordGenerator.mainClientForm.instance.sequencePage1.scrollToTimestep(newStep);
        }

        private void insertTimestepAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeStep newStep = new TimeStep("New timestep.");
            Storage.sequenceData.TimeSteps.Insert(stepNumber, newStep);
            Storage.sequenceData.populateWithChannels(Storage.settingsData);
            Storage.sequenceData.timestepsInsertedOrMoved();

            WordGenerator.mainClientForm.instance.sequencePage1.insertTimestepEditor(
                new TimestepEditor(newStep, stepNumber + 1), stepNumber);
 

        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeStep newStep = new TimeStep(this.stepData);
            Storage.sequenceData.TimeSteps.Insert(stepNumber, newStep);
            Storage.sequenceData.timestepsInsertedOrMoved();

            WordGenerator.mainClientForm.instance.sequencePage1.insertTimestepEditor(
                new TimestepEditor(newStep, stepNumber + 1), stepNumber);

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Storage.sequenceData.TimeSteps.RemoveAt(stepNumber-1);

            WordGenerator.mainClientForm.instance.sequencePage1.removeTimestepEditor(this);
        }

        private void durationEditor_updateGUI(object sender, EventArgs e)
        {
            if (updateGUI != null)
                updateGUI(sender, e);
        }

        private void analogSelector_DropDownClosed(object sender, EventArgs e)
        {
            if (analogSelector.SelectedItem == null)
                analogSelector.SelectedItem = analogSelectorBackupItem;

        }

        private void gpibSelector_DropDownClosed(object sender, EventArgs e)
        {
            if (gpibSelector.SelectedItem == null)
                gpibSelector.SelectedItem = gpibSelectorBackupItem;
        }

 

 

        private void removeTimestepHotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stepData.HotKeyCharacter != 0)
            {
                unRegsiterHotkey();
                stepData.HotKeyCharacter = (char) 0;
                WordGenerator.mainClientForm.instance.RefreshSequenceDataToUI(Storage.sequenceData);
            }
        }

        private void rs232Selector_DropDown(object sender, EventArgs e)
        {

            rs232Selector.Items.Clear();
            rs232Selector.BackColor = Color.White;
            rs232Selector.Items.Add("Continue");
            rs232Selector.Items.AddRange(Storage.sequenceData.RS232Groups.ToArray());

        }

        private void rs232Selector_DropDownClosed(object sender, EventArgs e)
        {
            if (rs232Selector.SelectedItem == null)
                rs232Selector.SelectedItem = rs232SelectorBackupItem;
        }

        Object rs232SelectorBackupItem;

        private void rs232Selector_SelectedValueChanged(object sender, EventArgs e)
        {
            if (rs232Selector.SelectedItem.ToString() == "Continue")
            {
                rs232Selector.BackColor = Color.Green;
                toolTip1.SetToolTip(rs232Selector, "Continue previous RS232 group.");
            }
            else
                rs232Selector.BackColor = Color.White;

            if (stepData != null)
            {
                RS232Group gg = rs232Selector.SelectedItem as RS232Group;
                stepData.rs232Group = gg;
                if (updateGUI != null)
                    updateGUI(sender, e);
                if (gg != null)
                    toolTip1.SetToolTip(rs232Selector, gg.GroupDescription);
            }

            rs232SelectorBackupItem = rs232Selector.SelectedItem;
        }

        private void hotkeyEntryTextBox_TextChanged(object sender, EventArgs e)
        {
            if (hotkeyEntryTextBox.Text.Length > 0)
            {
                char hotkeyChar = hotkeyEntryTextBox.Text[0];
                if (char.IsLetter(hotkeyChar))
                {
                    setHotkeyChar(hotkeyChar);
                    contextMenuStrip1.Close();
                }
                else
                {
                    hotkeyEntryTextBox.Text = "";
                }


            }
        }

        private void setHotkeyChar(char hChar)
        {
            if (hChar != 0)
            {
                foreach (TimeStep step in Storage.sequenceData.TimeSteps)
                {
                    if ((step != stepData) && (step.HotKeyCharacter == hChar))
                    {
                        MessageBox.Show("That hotkey is already in use.");
                        return;
                    }
                }

                if (stepData.HotKeyCharacter != 0)
                    unRegsiterHotkey();

                stepData.HotKeyCharacter = hChar;
                WordGenerator.mainClientForm.instance.RefreshSequenceDataToUI(Storage.sequenceData);
            }
        }

        private void moveToTimestepCombobox_DropDown(object sender, EventArgs e)
        {
            moveToTimestepCombobox.Items.Clear();
            int lastStep = Storage.sequenceData.NTimeSteps;
            for (int i = 0; i < lastStep; i++)
            {
                moveToTimestepCombobox.Items.Add(i + 1);
            }
            moveToTimestepCombobox.SelectedItem = stepNumber;
        }

        private void moveToTimestepCombobox_DropDownClosed(object sender, EventArgs e)
        {
            if (moveToTimestepCombobox.SelectedItem is int)
            {
                moveTimestep((int)moveToTimestepCombobox.SelectedItem);
                contextMenuStrip1.Close();
            }
        }

        private void moveTimestep(int destination)
        {
            if (destination > 0)
            {
                if (destination <= Storage.sequenceData.TimeSteps.Count)
                {
                    int destinationIndex = destination - 1;
                    int currentIndex = this.stepNumber - 1;

                    Storage.sequenceData.TimeSteps.RemoveAt(currentIndex);
                    Storage.sequenceData.TimeSteps.Insert(destinationIndex, this.stepData);
                    Storage.sequenceData.timestepsInsertedOrMoved();

                    WordGenerator.mainClientForm.instance.sequencePage1.moveTimestepEditor(currentIndex, destinationIndex);

                }
            }
        }

        private void agLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (stepData.AnalogGroup != null)
            {
                WordGenerator.mainClientForm.instance.activateAnalogGroupEditor(stepData.AnalogGroup);
            }
        }

        private void ggLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (stepData.GpibGroup != null)
            {
                WordGenerator.mainClientForm.instance.activateGPIBGroupEditor(stepData.GpibGroup);
            }
        }

        private void rgLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (stepData.rs232Group != null)
            {
                WordGenerator.mainClientForm.instance.activateRS232GroupEditor(stepData.rs232Group);
            }
        }

        private void descriptionTextChanged(object sender, EventArgs e)
        {
            stepData.Description = descriptionTextBox.Text;
            updateDescriptionTooltip(stepData);
        }

        private void viewDescMenuItem_Click(object sender, EventArgs e)
        {
            if (stepData.Description != "")
            {
                MessageBox.Show(stepData.Description, "Timestep description.");
            }
            else
            {
                MessageBox.Show("This timestep has no description.");
            }
        }

        private void TimestepEditor_Enter(object sender, EventArgs e)
        {
            // Together, these solve a former problem that when a partially concealed timestep was selected, it would be
            // scrolled to, but a scroll event would not be raised by stupid stupid windows, causing the 
            // horizontal scroll bars on the sequence page to become out of sync.

            WordGenerator.mainClientForm.instance.sequencePage1.timeStepsPanel.ScrollControlIntoView(this);
            WordGenerator.mainClientForm.instance.sequencePage1.forceUpdateAllScrollbars();
        }

        private void mark_Click(object sender, EventArgs e)
        {
            this.Marked = true;
        }

        private void unmark_Click(object sender, EventArgs e)
        {
            this.Marked = false;
        }

        private void markall_Click(object sender, EventArgs e)
        {
            WordGenerator.mainClientForm.instance.sequencePage1.markAllTimesteps();
        }

        private void unmarkall_Click(object sender, EventArgs e)
        {
            WordGenerator.mainClientForm.instance.sequencePage1.unmarkAllTimesteps();
        }

        private void waitForRetriggerMenuItem_Click(object sender, EventArgs e)
        {
            this.StepData.WaitForRetrigger = !this.StepData.WaitForRetrigger;
            updateWaitForRetriggerIndicator();
        }

        public void updateWaitForRetriggerIndicator()
        {
            if (this.StepData.WaitForRetrigger)
            {
                waitLabel.Visible = true;
                waitForRetriggerMenuItem.Text = "Disable Wait-for-retrigger.";
            }
            else
            {
                waitLabel.Visible = false;
                waitForRetriggerMenuItem.Text = "Enable Wait-for-retrigger.";
            }
        }

        private void timestepGroupToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            
            timestepGroupComboBox.Items.Clear();
            timestepGroupComboBox.Items.Add("None.");
            timestepGroupComboBox.Items.AddRange(Storage.sequenceData.TimestepGroups.ToArray());

            if (stepData.MyTimestepGroup == null)
            {
                timestepGroupComboBox.SelectedItem = "None.";
            }
            else
            {
                timestepGroupComboBox.SelectedItem = stepData.MyTimestepGroup;
            }
        }

        private void timestepGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            stepData.MyTimestepGroup = timestepGroupComboBox.SelectedItem as TimestepGroup;
            WordGenerator.mainClientForm.instance.sequencePage1.updateTimestepEditorsAfterSequenceModeOrTimestepGroupChange();
        }

        private void TimestepEditor_Layout(object sender, LayoutEventArgs e)
        {

        }



        private void analogPictureBox_MouseEnter(object sender, EventArgs e)
        {
            analogSelector.Visible = true;
            analogPictureBox.Visible = false;
        }

        private void rs232PictureBox_MouseEnter(object sender, EventArgs e)
        {
            rs232Selector.Visible = true;
            rs232PictureBox.Visible = false;
        }

        private void gpibPictureBox_MouseEnter(object sender, EventArgs e)
        {
            gpibSelector.Visible = true;
            gpibPictureBox.Visible = false;
        }


        // Optimization for faster loading of sequence files.
        // all of the UI elements are invisible when the timestep editor is created,
        // they are only made visible the first time the timestep editor is painted
        // (thus out-of-view timestep editors only have their sub-elements first
        // made visible when they are scrolled into view).
        // This, in combination with the picture-box previews of the analog, rs232, and gpib comboboxes
        // causes of sequence loading speedup by a factor of ~3.
        bool firstPaint = true;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (firstPaint)
            {
                this.SuspendLayout();
                firstPaint = false;
                timeStepNumber.Visible = true;
                timestepName.Visible = true;
                enabledButton.Visible = true;
                showHideButton.Visible = true;
                durationEditor.Visible = true;
                if (!analogSelector.Visible)
                    analogPictureBox.Visible = true;
                if (!gpibSelector.Visible)
                    gpibPictureBox.Visible = true;
                if (!rs232Selector.Visible)
                    rs232PictureBox.Visible = true;
                this.ResumeLayout();
            }
            base.OnPaintBackground(e);
        }

        private void setDigitalsToContinue_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will set all digital channels in this timestep to Continue. To set individual channels to Continue, CTRL-click on their entries in the digital grid. Do you want to proceed?", "Set all digitals to continue?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DigitalDataPoint dp in stepData.DigitalData.Values)
                {
                    dp.DigitalContinue = true;
                }
                WordGenerator.mainClientForm.instance.sequencePage1.digitalGrid1.forceRepaint();
            }
        }


    }
}
