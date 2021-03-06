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
    public partial class PulseEditor : UserControl
    {
        public Pulse pulse;
        public event EventHandler pulseDeleted;

        public PulseEditor()
        {
            InitializeComponent();

            startCondition.Items.Clear();
            endCondition.Items.Clear();
            foreach (Pulse.PulseTimingCondition cond in Enum.GetValues(typeof(Pulse.PulseTimingCondition)))
            {
                startCondition.Items.Add(cond);
                endCondition.Items.Add(cond);
            }
           
        }

        public PulseEditor(Pulse pulse)
            : this()
        {
            setPulse(pulse);
        }

        public void setPulse(Pulse pulse)
        {
            if (this.pulse == pulse)
                return; // if already set corrently, return immediately

            if (pulse != null)
            {
                this.pulse = pulse;

                this.startDelayTime.setParameterData(pulse.startDelay);
                this.endDelayTime.setParameterData(pulse.endDelay);
                this.pulseDuration.setParameterData(pulse.pulseDuration);

                this.startDelayEnabled.Checked = pulse.startDelayEnabled;
                this.endDelayEnabled.Checked = pulse.endDelayEnabled;

                this.startDelayed.Checked = pulse.startDelayed;
                this.endDelayed.Checked = pulse.endDelayed;

                this.pulseValue.Checked = pulse.PulseValue;

                this.pulseNameTextBox.Text = pulse.PulseName;
                this.pulseDescriptionTextBox.Text = pulse.PulseDescription;

                this.startCondition.SelectedItem = pulse.startCondition;
                this.endCondition.SelectedItem = pulse.endCondition;

                this.getValueFromVariableCheckBox.Checked = pulse.ValueFromVariable;

            }

            updateElements();
        }

        private void updateElements()
        {
            if (pulse != null)
            {
                if (pulse.startCondition == Pulse.PulseTimingCondition.Duration)
                {
                    this.startDelayTime.Enabled = false;
                    this.startDelayEnabled.Enabled = false;
                    this.startDelayed.Enabled = false;
                }
                else
                {
                    this.startDelayTime.Enabled = true;
                    this.startDelayEnabled.Enabled = true;
                    if (this.startDelayEnabled.Checked)
                    {
                        this.startDelayed.Enabled = true;
                        this.startDelayTime.Enabled = true;
                    }
                    else
                    {

                        this.startDelayed.Enabled = false;
                        this.startDelayTime.Enabled = false;
                    }
                }

                if (pulse.endCondition == Pulse.PulseTimingCondition.Duration)
                {
                    this.endDelayTime.Enabled = false;
                    this.endDelayEnabled.Enabled = false;
                    this.endDelayed.Enabled = false;
                }
                else
                {
                    this.endDelayTime.Enabled = true;
                    this.endDelayEnabled.Enabled = true;
                    if (endDelayEnabled.Checked)
                    {
                        endDelayTime.Enabled = true;
                        endDelayed.Enabled = true;
                    }
                    else
                    {
                        endDelayTime.Enabled = false;
                        endDelayed.Enabled = false;
                    }
                }

                if (pulse.endCondition == Pulse.PulseTimingCondition.Duration || pulse.startCondition == Pulse.PulseTimingCondition.Duration)
                {
                    this.pulseDuration.Enabled = true;
                }
                else
                {
                    this.pulseDuration.Enabled = false;
                }

                string dataValidityText = pulse.dataInvalidUICue();

                if (dataValidityText==null)
                {
                    this.validityLabel.Text = "Data valid";
                }
                else
                {
                    this.validityLabel.Text = dataValidityText;
                }

            }
            else
            {
                foreach (Control con in this.Controls)
                {
                    con.Enabled = false;
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pulseNameTextBox_TextChanged(object sender, EventArgs e)
        {
            pulse.PulseName = pulseNameTextBox.Text;
        }

        private void pulseDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            pulse.PulseDescription = pulseDescriptionTextBox.Text;
        }

        private void startCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            pulse.startCondition = (Pulse.PulseTimingCondition) startCondition.SelectedItem;
            updateElements();
        }

        private void startDelayEnabled_CheckedChanged(object sender, EventArgs e)
        {
            pulse.startDelayEnabled = startDelayEnabled.Checked;
            updateElements();
        }

        private void startDelayed_CheckedChanged(object sender, EventArgs e)
        {
            pulse.startDelayed = startDelayed.Checked;
            updateElements();
        }

        private void endCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            pulse.endCondition = (Pulse.PulseTimingCondition) endCondition.SelectedItem;
            updateElements();
        }

        private void endDelayEnabled_CheckedChanged(object sender, EventArgs e)
        {
            pulse.endDelayEnabled = endDelayEnabled.Checked;
            updateElements();
        }

        private void endDelayed_CheckedChanged(object sender, EventArgs e)
        {
            pulse.endDelayed = endDelayed.Checked;
            updateElements();
        }

        private void pulseValue_CheckedChanged(object sender, EventArgs e)
        {
            pulse.PulseValue = pulseValue.Checked;
            updateElements();
        }

        private void deletebutton_Click(object sender, EventArgs e)
        {
            foreach (TimeStep step in Storage.sequenceData.TimeSteps)
            {
                foreach (int digID in step.DigitalData.Keys)
                {
                    if (step.DigitalData[digID].DigitalPulse == pulse)
                    {
                        MessageBox.Show("Cannot delete this pulse, it is used in timestep [" + step.ToString() + "] in digital ID " + digID);
                        return;
                    }
                }
            }

            Storage.sequenceData.DigitalPulses.Remove(pulse);
            if (pulseDeleted == null)
                WordGenerator.mainClientForm.instance.RefreshSequenceDataToUI(Storage.sequenceData); // the slow way to delete pulse from UI
            else
                pulseDeleted(this, null);   // the fast way, if the hook exists.
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            int currentIndex = Storage.sequenceData.DigitalPulses.IndexOf(this.pulse);
            if (currentIndex != 0)
            {
                int newIndex = currentIndex - 1;
                Storage.sequenceData.DigitalPulses.Remove(this.pulse);
                Storage.sequenceData.DigitalPulses.Insert(newIndex, this.pulse);
                WordGenerator.mainClientForm.instance.pulsesPage1.layout();
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            int currentIndex = Storage.sequenceData.DigitalPulses.IndexOf(this.pulse);
            if (currentIndex != Storage.sequenceData.DigitalPulses.Count - 1)
            {
                int newIndex = currentIndex + 1;
                Storage.sequenceData.DigitalPulses.Remove(this.pulse);
                Storage.sequenceData.DigitalPulses.Insert(newIndex, pulse);
                WordGenerator.mainClientForm.instance.pulsesPage1.layout();
            }
        }

        private void duplicateButton_Click(object sender, EventArgs e)
        {
            Pulse newPulse = new Pulse(pulse);
            int currentIndex = Storage.sequenceData.DigitalPulses.IndexOf(pulse);
            Storage.sequenceData.DigitalPulses.Insert(currentIndex + 1, newPulse);
            WordGenerator.mainClientForm.instance.RefreshSequenceDataToUI(Storage.sequenceData);
        }

        private bool ignoreValueVariableComboBoxEvents = false;

        private void getValueFromVariableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.pulse.ValueFromVariable = getValueFromVariableCheckBox.Checked;

            if (this.pulse.ValueFromVariable)
            {


                populateValueVariableComboBox();

                if (pulse.ValueVariable != null)
                {
                    valueVariableComboBox.SelectedItem = pulse.ValueVariable;
                }

                valueVariableComboBox.Visible = true;
            }
            else
            {
                valueVariableComboBox.Visible = false;
            }
        }

        private void populateValueVariableComboBox()
        {
            ignoreValueVariableComboBoxEvents = true;
            valueVariableComboBox.Items.Clear();
            foreach (Variable var in Storage.sequenceData.Variables)
            {
                valueVariableComboBox.Items.Add(var);
            }
            ignoreValueVariableComboBoxEvents = false;
        }

        private void valueVariableComboBox_DropDown(object sender, EventArgs e)
        {
            populateValueVariableComboBox();
        }

        private void valueVariableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ignoreValueVariableComboBoxEvents)
            {
                pulse.ValueVariable = valueVariableComboBox.SelectedItem as Variable;
            }
        }

    }
}
