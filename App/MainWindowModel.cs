﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Input;
using WinMachine.Mvvm;

namespace WinMachine.App
{
    public class MainWindowModel : ViewModelBase, IDisposable
    {
        #region Constructor

        private MachineDevice machineDevice = new MachineDevice();
        private IGraphDrawing graphDrawing;
        private DispatcherTimer redrawTimer;
        private DispatcherTimer searchTimer;
        private WaveAnalyzer waveAnalyzer = new WaveAnalyzer();

        private bool isSerialOpen = false;
        private bool isMachineStarted = false;

        public MainWindowModel(IGraphDrawing graphDrawing)
        {
            this.BaudRates = SerialConnection.PopularBaudRates.ToList();
            RecallOptions();

            this.graphDrawing = graphDrawing;

            redrawTimer = new DispatcherTimer();
            redrawTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            redrawTimer.Tick += delegate { OnRedrawTimer(); };

            searchTimer = new DispatcherTimer();
            searchTimer.Interval = new TimeSpan(0, 0, 0, 1);
            searchTimer.Tick += delegate { OnSearchTimer(); };

            UpdateLabelsAfterModeChange();
        }

        public void OnWindowLoaded()
        {
            Log("Loaded.");

            // open device on startup
            InvokeOnUIThread(new Action(OnOpenClose));

            this.graphDrawing.DrawAxis();
        }

        public void OnWindowClosed()
        {
            if (machineDevice.IsOpen)
            {
                redrawTimer.Stop();
                machineDevice.CloseSerial();
            }

            SaveOptions();
        }

        private const int stepHz = 50;

        public void OnWindowKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.F5) {
                if (!isMachineStarted) return;
                int hz = FrequencyHz - stepHz;
                Log(machineDevice.SendPWM(hz));
                FrequencyHz = hz;
            }

            if (e.Key == Key.F6) {
                if (!isMachineStarted) return;
                int hz = FrequencyHz + stepHz;
                Log(machineDevice.SendPWM(hz));
                FrequencyHz = hz;
            }
        }

        public void Dispose()
        {
        }

        #endregion
        #region ViewModel

        public RelayCommand OpenCloseCommand => new RelayCommand(OnOpenClose);
        public RelayCommand StartStopCommand => new RelayCommand(OnStartStop);

        public ObservableCollection<string> LogLines { get; private set; } 
            = new ObservableCollection<string>();

        private string _selectedLogLine;
        public string SelectedLogLine
        {
            get => _selectedLogLine;
            set
            {
                _selectedLogLine = value;
                RaisePropertyChanged(nameof(SelectedLogLine));
            }
        }

        private string _openCloseButtonText;
        public string OpenCloseButtonText
        {
            get => _openCloseButtonText;
            set
            {
                _openCloseButtonText = value;
                RaisePropertyChanged(nameof(OpenCloseButtonText));
            }
        }

        private string _frequencyText;
        public string FrequencyText
        {
            get => _frequencyText;
            set
            {
                _frequencyText = value;
                RaisePropertyChanged(nameof(FrequencyText));
            }
        }

        private string _dutyCycleText;
        public string DutyCycleText
        {
            get => _dutyCycleText;
            set
            {
                _dutyCycleText = value;
                RaisePropertyChanged(nameof(DutyCycleText));
            }
        }

        private string _numChopsText;
        public string DeadClocksText
        {
            get => _numChopsText;
            set
            {
                _numChopsText = value;
                RaisePropertyChanged(nameof(DeadClocksText));
            }
        }

        private int FrequencyHz
        {
            get => Int32.Parse(this.FrequencyText);
            set {
                FrequencyText = value.ToString();
            }
        }


        int Duty => Int32.Parse(this.DutyCycleText);
        int DeadClocks => Int32.Parse(this.DeadClocksText);

        private string _samplesPerPeriodValue;
        public string SamplesPerPeriodValue
        {
            get => _samplesPerPeriodValue;
            set
            {
                _samplesPerPeriodValue = value;
                RaisePropertyChanged(nameof(SamplesPerPeriodValue));
            }
        }

        private string _startStopButtonText;
        public string StartStopButtonText
        {
            get => _startStopButtonText;
            set
            {
                _startStopButtonText = value;
                RaisePropertyChanged(nameof(StartStopButtonText));
            }
        }

        private bool _isLogChecked;
        public bool IsLogChecked
        {
            get => _isLogChecked;
            set
            {
                _isLogChecked = value;
                RaisePropertyChanged(nameof(IsLogChecked));
            }
        }

        private bool _isSetupChecked;
        public bool IsSetupChecked
        {
            get => _isSetupChecked;
            set 
            {
                _isSetupChecked = value;
                RaisePropertyChanged(nameof(IsSetupChecked));
            }
        }

        private bool _isSearchChecked;
        public bool IsSearchChecked
        {
            get => _isSearchChecked;
            set
            {
                _isSearchChecked = value;
                if (value) searchTimer.Start();
                else searchTimer.Stop();
                RaisePropertyChanged(nameof(IsSearchChecked));
            }
        }

        public string SerialPortName { get; set; }
        public List<int> BaudRates { get; private set; }
        public int SelectedBaudRate { get; set; }

        #endregion
        #region Internals 

        private void OnOpenClose()
        {
            if (isSerialOpen)
            {
                if (isMachineStarted) OnStartStop();
                machineDevice.CloseSerial();
                Log("Closed device");
                isSerialOpen = false;
            }
            else
            {
                machineDevice.OpenSerial(SerialPortName, SelectedBaudRate);
                Log($"Opened device: {machineDevice.PortName} at {machineDevice.BaudRate}");
                Log($"Device: {machineDevice.Hello}");
                isSerialOpen = true;
            }

            UpdateLabelsAfterModeChange();
        }

        private void OnStartStop()
        {
            if (!isSerialOpen) return;

            try {
                if (isMachineStarted) DoStop();
                else DoStart();
            }
            catch (Exception e) {
                Log("Error: " + e.Message);
            }

            UpdateLabelsAfterModeChange();
        }
        
        private int lastUsedDeadClocks = 0;

        private void DoStart()
        {
            if (isMachineStarted) return;

            if (lastUsedDeadClocks != DeadClocks) {
                Log(machineDevice.SetDeadClocks(DeadClocks));
                lastUsedDeadClocks = DeadClocks;
            }

            Log(machineDevice.SendRUN(FrequencyHz, Duty));
            isMachineStarted = true;
            redrawTimer.Start();
        }

        private void DoStop()
        {
            if (isMachineStarted) {
                redrawTimer.Stop();
                Log(machineDevice.SendStop());
                SamplesPerPeriodValue = "--";
                isMachineStarted = false;
            }
        }

        private void UpdateLabelsAfterModeChange()
        {
            char square = '\u25A0';
            char triangle = '\u25BA';
            this.OpenCloseButtonText = isSerialOpen ? "Close" : "Open";
            this.StartStopButtonText = isMachineStarted ? $"Stop {square}" : (isSerialOpen ? $"Start {triangle}" : "--");
        }

        private void OnRedrawTimer()
        {
            RunADCAndDrawGraph();
        }

        private void RunADCAndDrawGraph()
        {
            if (isMachineStarted)
            {
                var samples = new List<int>();
                string logText;

                var timer = new MilliTimer();
                logText = machineDevice.RunADC(0, samples);
                SamplesPerPeriodValue = samples.Count() + "@" + timer;
                graphDrawing.DrawGraph(0, samples, AdcSampleProfile.DC8bit);
                MaybeLogAdc(0, logText, samples);

                logText = machineDevice.RunADC(1, samples);
                graphDrawing.DrawGraph(1, samples, AdcSampleProfile.ACS712_5A8bit);
                MaybeLogAdc(1, logText, samples);

                logText = machineDevice.RunADC(2, samples);
                graphDrawing.DrawGraph(2, samples, AdcSampleProfile.ACS712_30A8bit);
                MaybeLogAdc(2, logText, samples);
            }
        }

        private void OnSearchTimer()
        {
            if (isMachineStarted) {
                FrequencyHz = waveAnalyzer.Analyze(machineDevice, 2);
            }
        }

        private void MaybeLogAdc(int pin, string reply, List<int> samples)
        {
            if (this.IsLogChecked)
            {
                Log($"ADC{pin}: [{reply}] AVG={Math.Round(samples.Average(), 2)}");
            }
        }

        public void Log(string message)
        {
            var now = DateTime.Now;
            string text = $"[{now.Hour:00}:{now.Minute:00}:{now.Second:00}.{now.Millisecond:000}] {message}";
            LogLines.Add(text);
            SelectedLogLine = text;
        }

        private void RecallOptions()
        {
            try
            {
                var options = Options.Recall();
                SerialPortName = options.SerialPort;
                SelectedBaudRate = int.Parse(options.BaudRate);
                FrequencyText = options.Frequency;
                DutyCycleText = options.DutyCycle;
                DeadClocksText = options.DeadTimeCycles;
            }
            catch (Exception e)
            {
                Log("Failed to restore options: " + e.Message);
            }
        }

        private void SaveOptions()
        {
            try
            {
                var options = new Options();
                options.SerialPort = SerialPortName;
                options.BaudRate = SelectedBaudRate.ToString();
                options.Frequency = FrequencyText;
                options.DutyCycle = DutyCycleText;
                options.DeadTimeCycles = DeadClocksText;
                options.Save();
            }
            catch (Exception e)
            {
                Log("Failed to save options: " + e.Message);
            }
        }

        #endregion
    }
}
