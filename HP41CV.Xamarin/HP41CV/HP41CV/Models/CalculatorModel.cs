using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using HP41CV.Annotations;
using HP41CV.Helpers;
using Xamarin.Forms;

namespace HP41CV.Models
{
    public class CalculatorModel : INotifyPropertyChanged
    {

        public enum Format
        {
            Fix,
            Sci,
            Eng,
            Hex,
            Bin
        }

        private readonly char[] printChar =
        {
            '˟', 'ẍ', '←', 'α','β', 'Г','↓', '∆', 'σ','♦','λ','μ','<','τ','Φ','θ','Ω','δ','Å','å','Ä','ä','Ö','ö','Ü','ü','Æ','æ','≠','₤','¥',' ','!',
            '\"','#', '$', '%','&','\'','(', ')', '*','+',',','-','.','/','0','1','2','3','4','5','6','7','8','9',':',';','<','=','>','?','@','A','B',
            'C', 'D', 'E', 'F','G', 'H','I', 'J', 'K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','[','\\',']','↑','_','ᵀ','a','b','c',
            'd', 'e', 'f', 'g','h', 'i','j', 'k', 'l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','π','|','→','Σ','Ⱶ'
        };

        private bool useTimer;

        private bool timerRestart;

        private TimeSpan slowTimerSpeed = TimeSpan.FromMilliseconds(1500);

        private TimeSpan timerSpeed = TimeSpan.FromMilliseconds(500);

        private const int maxValues = 99;

        ActionModel newOperator;

        ActionModel lastOperator;

        private bool isUpn = true;

        private bool isUnary = false;

        private bool printer = false;

        private bool robustStatistic = false;

        public int statisticRegister = 11;

        private SortedCollection<double>[] Values;

        private readonly bool[] _flag;

        private readonly Register _l;

        private readonly Register _t;

        private int pauseCount;

        private int promptSchift;

        private int programStep = 0;

        private int programLine = 0;

        private int programEndLine = 0;

        private double timePerStep = 10.0;

        private double timePause = 1000.0;

        private readonly Register _z;

        public Dictionary<string, ActionModel> Programs { get; private set; }

        private bool _dataError;

        private int _exponent;

        private bool _flagCleared;

        private bool _flagSet;

        private double _mantisse;

        private int _precision;

        private Register[] _r;

        private bool _rangeError;

        private StringBuilder _sbAlpha;

        private SelectActionModel _selectAction;

        private string formular;

        private double _value;

        private bool _view;

        private Register _x;

        private Register _y;

        private Dictionary<string, ActionModel>.ValueCollection.Enumerator actionModelEnumerator;

        private string lastAccessory;

//        private ProgramModel runningProgram;

//        private Collection<CallStackEntry> callStack;


        public string Foramula
        {
            get { return formular; }
            set
            {
                formular = value;
                OnPropertyChanged(nameof(Foramula));
            }
        }

        public string Display1
        {
            get
            {
                return ((this._value >= 0) || this._flag[50]) ? " " : this._flag[56] ? "#" : "-";
            }
        }

        public string Display2
        {
            get
            {
                if (this._flag[50])
                {
                    if (this._dataError)
                    {
                        return "DATA ERROR";
                    }

                    if (this._rangeError)
                    {
                        return "OUT OF RANGE";
                    }

                    if (this._flagSet)
                    {
                        return "YES";
                    }

                    if (this._flagCleared)
                    {
                        return "NO";
                    }

                    return "NONEXISTENT";
                }

                if (IsCatalog)
                {
                    return this.actionModelEnumerator.Current.TextD;
                }

                double value;
                var format = "F" + Precision;

                if (UsedFormat == Format.Hex)
                {
                    format = "X" + Precision * 2;
                    var help = (long)this._value;
                    if (this._value < 0)
                    {
                        help += (long)Math.Pow(2, 8 * Precision);
                    }

                    if (Precision == 8)
                    {
                        return help.ToString(format);
                    }

                    return "#" + help.ToString(format);
                }

                if ((UsedFormat == Format.Fix) && (this._exponent < 12) && (this._exponent >= (-1 * Precision)))
                {
                    value = Math.Abs(this._value);
                }
                else
                {
                    value = Math.Abs(this._mantisse);
                }

                var display = value.ToString(format, this._flag[28] ? new CultureInfo("de-DE") : CultureInfo.InvariantCulture);
                return (display.Length < 12) ? display : display.Substring(0, 12);
            }
        }

        public string Display3
        {
            get
            {
                return ((((UsedFormat != Format.Fix) && (!this._flag[56])) || (this._exponent < (-1 * Precision))) && (this._exponent < 0) && !this._flag[50]) ? "-" : string.Empty;
            }
        }

        public string Display4
        {
            get
            {
                return ((this._exponent == 0) || (this._exponent > 99) || (this._exponent < -99) ||
                        ((UsedFormat == Format.Fix) && (this._exponent < 12) && (this._exponent >= (-1 * Precision)) && !this._flag[50]) || this._flag[56])
                           ? string.Empty
                           : Math.Abs(this._exponent).ToString("00");
            }
        }

        public string DisplayA
        {
            get
            {
                if (IsPrompt && ((_sbAlpha.Length - promptSchift) < 16))
                    promptSchift = 0;
                return (this._sbAlpha.Length < 13) ? this._sbAlpha.ToString() :
                    IsPrompt ? this._sbAlpha.ToString().Substring(promptSchift, Math.Min(16, _sbAlpha.Length)) :
                    this._sbAlpha.ToString().Substring(this._sbAlpha.Length - 12, 12);
            }

            private set
            {
                IsPrompt = false;
                this._sbAlpha = new StringBuilder(value);
                OnPropertyChanged(nameof(DisplayA));
            }
        }

        public bool IncPromptSchift()
        {
            var result = (this._sbAlpha.Length - promptSchift) > 16;
            if (result)
                promptSchift++;
            return result;
        }

        public string DisplayP
        {
            get
            {
                if (IsPrompt)
                    return DisplayA;
                var blanc = "";
                for (var i = 0; i < programStep; i++)
                    blanc += " ";
                return blanc + "}";
            }
        }

        public bool IsCatalog { get; private set; }

        public bool IsCatalogRunning { get; private set; }

        public bool IsPrinting { get; private set; }

        public bool IsRunning { get; private set; }

        public bool IsProgramActiv { get { return IsRunning && (pauseCount < 1); } }

        public bool IsPrompt { get; private set; }

        public bool IsUpn
        {
            get { return isUpn; }
            private set
            {
                isUpn = value;
                OnPropertyChanged(nameof(IsUpn));
            }
        }

        public bool IsEqual { get; set; }

        public bool FlagSet { get { return _flagSet; } }

        private void PreviusEntry()
        {
        }

        private void ReSize(int v)
        {
            var help = new Register[v + 6];
            for (var i = 0; i <= v; i++)
            {
                if (i < MaxRegister)
                {
                    help[i] = this._r[i];
                }
                else
                {
                    help[i] = new Register(i.ToString());
                }
            }

            this._r = help;
            MaxRegister = v;
            this._r[MaxRegister + 1] = this._x;
            this._r[MaxRegister + 2] = this._y;
            this._r[MaxRegister + 3] = this._z;
            this._r[MaxRegister + 4] = this._t;
            this._r[MaxRegister + 5] = this._l;
        }

        public double LastX
        {
            get
            {
                return this._l.Value;
            }

            set
            {
                this._l.Value = value;
            }
        }

        public int MaxRegister { get; private set; }

        public int Precision
        {
            get
            {
                return this._precision;
            }

            private set
            {
                this._precision = value;
                this._flag[36] = (this._precision & 0x8) == 0x8;
                this._flag[37] = (this._precision & 0x4) == 0x4;
                this._flag[38] = (this._precision & 0x2) == 0x2;
                this._flag[39] = (this._precision & 0x1) == 0x1;
                X = this._x.Value;
            }
        }

        public Format UsedFormat
        {
            get
            {
                return IsFlagSet(57) ? Format.Bin : IsFlagSet(56) ? Format.Hex : IsFlagSet(40) ? Format.Fix : (IsFlagSet(41)) ? Format.Eng : Format.Sci;
            }

            private set
            {
                if (value == Format.Bin)
                {
                    SetFlag(57);
                }
                else
                {
                    ClearFlag(57);
                }

                if (value == Format.Hex)
                {
                    if (!IsFlagSet(56))
                    {
                        SetFlag(56);
                        OnPropertyChanged("KeyHex");
                    }
                }
                else
                {
                    if (IsFlagSet(56))
                    {
                        ClearFlag(56);
                        OnPropertyChanged("KeyHex");
                    }
                }

                this._flag[40] = value == Format.Fix;
                this._flag[41] = value == Format.Eng;
                OnPropertyChanged(nameof(Display1));
                OnPropertyChanged(nameof(Display2));
                OnPropertyChanged(nameof(Display3));
                OnPropertyChanged(nameof(Display4));
            }
        }

        public double X
        {
            get
            {
                return this._x.Value;
            }

            set
            {
                this._x.Value = value;
                NoStackLift = false;
                SetValue(this._x.Value);
            }
        }

        public double Y
        {
            get { return this._y.Value; }
        }

        private Collection<KeyAssignmentModel> KeyAssignment => IsFlagSet(56) ? keyAssignmentHex : keyAssignment;

        public SelectActionModel SelectAction
        {
            get
            {
                return this._selectAction;
            }

            set
            {
                this._selectAction = value;
                OnPropertyChanged(nameof(SelectAction));
            }
        }

        private ProgramModel runningProgram;

        private Collection<CallStackEntry> callStack;
        private Collection<KeyAssignmentModel> keyAssignment;
        private Collection<KeyAssignmentModel> keyAssignmentHex;

        public PrinterModel Printer { get; private set; }

        public CalculatorModel(bool useTimer = true)
        {
            this.useTimer = useTimer;

            CreateStandartAction();

            Printer = new PrinterModel(() =>
            {
                if (IsFlagSet(48))
                    GetActionModel("PRA").Execute();
                else GetActionModel("PRX").Execute();
            });
            this.Programs = new Dictionary<string, ActionModel>();
            callStack = new Collection<CallStackEntry>();

            var program = new ProgramModel(this, "HEX 8");
            program.AddStep(GetActionModel("HEX"), 8);
            this.Programs.Add(program.Text, program);
            program = new ProgramModel(this, "BIN 8");
            program.AddStep(GetActionModel("BIN"), 8);
            this.Programs.Add(program.Text, program);
            program = new ProgramModel(this, "FIX 4");
            program.AddStep(GetActionModel("FIX"), 4);
            this.Programs.Add(program.Text, program);

            Values = new SortedCollection<double>[maxValues];
            for (int index = 0; index < maxValues; index++)
                Values[index] = new SortedCollection<double>();
            MaxRegister = 99;
            this._x = new Register("X");
            this._y = new Register("Y");
            this._z = new Register("Z");
            this._t = new Register("T");
            this._l = new Register("L");
            this._r = new Register[MaxRegister + 6];
            this._dataError = false;
            this._rangeError = false;
            this._flagSet = false;
            this._flagCleared = false;
            this._view = false;
            for (var i = 0; i <= MaxRegister; i++)
            {
                this._r[i] = new Register(i.ToString());
            }

            this._r[MaxRegister + 1] = this._x;
            this._r[MaxRegister + 2] = this._y;
            this._r[MaxRegister + 3] = this._z;
            this._r[MaxRegister + 4] = this._t;
            this._r[MaxRegister + 5] = this._l;
            this._flag = new bool[58];
            this._sbAlpha = new StringBuilder(string.Empty);
            Precision = 4;
            UsedFormat = Format.Fix;

            keyAssignment = new Collection<KeyAssignmentModel>();
            keyAssignmentHex = new Collection<KeyAssignmentModel>();

            IsRunning = false;
            IsCatalog = false;
            IsCatalogRunning = false;
            IsPrinting = false;
            if (MarketPlace.IsAvtivated("Printer"))
            {
                _flag[21] = _flag[55] = true;
            }

            StartUserKeys();

            Device.StartTimer(new TimeSpan(1000),() =>
            {
                OnPropertyChanged("Reset");
                return false;
            });

        }

        public void EndCatalog()
        {
            IsCatalog = false;
        }

        public void Reset()
        {
            for (var i = 0; i <= MaxRegister + 5; i++)
            {
                _r[i].Clear();
            }

            for (var i = 0; i < 58; i++)
                this._flag[i] = false;
            if (MarketPlace.IsAvtivated("Printer"))
            {
                _flag[21] = _flag[55] = true;
            }
            this._sbAlpha = new StringBuilder(string.Empty);
            Precision = 4;
            UsedFormat = Format.Fix;
            IsRunning = false;
            IsCatalog = false;
            IsCatalogRunning = false;
            IsUpn = true;
            KeyAssignment.Clear();
            StartUserKeys();
            OnPropertyChanged();
        }

        public ActionModel GetActionModel(string text)
        {
            ActionModel result = null;
            if (this.Programs.ContainsKey(text))
            {
                result = this.Programs[text];
            }
            else
            {
                result = ActionModels.GetExtensionActionModel(text);
            }

            if (result == null)
            {
                result = ActionModels.GetActionModel(text);
            }

            if (result == null) SetFlag(50);

            return result;
        }

        public void SetFlag(int index)
        {
            this._flag[index] = true;
            if (index == 0)
            {
                OnPropertyChanged("Flag0");
            }
            else if (index == 1)
            {
                OnPropertyChanged("Flag1");
            }
            else if (index == 2)
            {
                OnPropertyChanged("Flag2");
            }
            else if (index == 3)
            {
                OnPropertyChanged("Flag3");
            }
            else if (index == 4)
            {
                OnPropertyChanged("Flag4");
            }
            else if (index == 23)
            {
                OnPropertyChanged("AlphaIn");
            }
            else if (index == 27)
            {
                OnPropertyChanged("User");
            }
            else if (index == 42)
            {
                OnPropertyChanged("Grad");
            }
            else if (index == 43)
            {
                OnPropertyChanged("Rad");
            }
            else if (index == 48)
            {
                OnPropertyChanged("Alpha");
            }
            else if (index == 50)
            {
                OnPropertyChanged("Flag50");
            }
            else if (index == 52)
            {
                OnPropertyChanged("Prgm");
            }
            else if (index == 56)
            {
                OnPropertyChanged("Hex");
            }
            else if (index == 57)
            {
                OnPropertyChanged("Bin");
            }
        }
        public void AddKeyAssignment(ActionModel action, int button)
        {
            var keyAssignmentModel = new KeyAssignmentModel(KeyAssignment.Count + 1, action, button, false);
            if (keyAssignmentModel.Action == null) return;
            var hit = false;
            for (int i = 0; !hit && (i < KeyAssignment.Count); i++)
                if (keyAssignmentModel.Button == KeyAssignment[i].Button)
                {
                    hit = true;
                    KeyAssignment[i] = keyAssignmentModel;
                }
            if (!hit)
                KeyAssignment.Add(keyAssignmentModel);
            OnPropertyChanged("Keys");
            return;
        }

        public IEnumerable<KeyAssignmentModel> GetKeyAssignments()
        {
            return KeyAssignment;
        }

        public void AddAlpha(char c)
        {
            if (!IsFlagSet(23)) ClearAlpha();
            SetFlag(23);
            this._sbAlpha.Append(c);
            OnPropertyChanged(nameof(Display2));
        }

        public void AddAlpha(string value)
        {
            this._sbAlpha.Append(value);
        }


        public void SetAlpha(string value)
        {
            this._sbAlpha = new StringBuilder(value);
        }

        public void ClearAlpha()
        {
            this._sbAlpha = new StringBuilder(string.Empty);
        }

        public void RemoveAlpha()
        {
            if (this._sbAlpha.Length > 0)
            {
                this._sbAlpha.Remove(this._sbAlpha.Length - 1, 1);
            }
        }

        public void ClearFlag(int index)
        {
            this._flag[index] = false;
            if (index == 0)
            {
                OnPropertyChanged("Flag0");
            }
            else if (index == 1)
            {
                OnPropertyChanged("Flag1");
            }
            else if (index == 2)
            {
                OnPropertyChanged("Flag2");
            }
            else if (index == 3)
            {
                OnPropertyChanged("Flag3");
            }
            else if (index == 4)
            {
                OnPropertyChanged("Flag4");
            }
            else if (index == 27)
            {
                OnPropertyChanged("User");
            }
            else if (index == 42)
            {
                OnPropertyChanged("Grad");
            }
            else if (index == 43)
            {
                OnPropertyChanged("Rad");
            }
            else if (index == 48)
            {
                ClearFlag(23);
                OnPropertyChanged("Alpha");
            }
            else if (index == 50)
            {
                OnPropertyChanged("Flag50");
            }
            else if (index == 52)
            {
                OnPropertyChanged("Prgm");
            }
            else if (index == 56)
            {
                OnPropertyChanged("Hex");
            }
            else if (index == 57)
            {
                OnPropertyChanged("Bin");
            }
        }

        public bool IsFlagSet(int index)
        {
            return this._flag[index];
        }

        public void ClearX()
        {
            if (this._view)
            {
                SetValue(this._x.Value);
            }
            else
            {
                X = 0;
            }
            Foramula = string.Empty;
        }

        internal int Indirect(int index)
        {
            return (int)_r[index].Value;
        }

        public bool AddOperator(ActionModel newOperator)
        {
            IsEqual = false;
            var testOperator = newOperator as UnaryOperationModel;
            if (testOperator != null)
            {
                if (IsEqual || isUnary)
                    Foramula = string.Empty;
                if (newOperator.TextD.EndsWith("X"))
                    Foramula += newOperator.TextD.Substring(0, newOperator.TextD.Length - 1) + X.ToString();
                else
                    Foramula += newOperator.Text + "(" + X.ToString() + ")";
                newOperator.Execute(true);
                isUnary = true;
                return false;
            }
            isUnary = false;
            if (newOperator != null)
            {
                if (this.newOperator != null)
                {
                    if (((newOperator.Text.CompareTo("x") == 0) || (newOperator.Text.CompareTo("÷") == 0))
                        && ((this.newOperator.Text.CompareTo("+") == 0) || (this.newOperator.Text.CompareTo("-") == 0)))
                    {
                        lastOperator = this.newOperator;
                        Foramula = _y.Value.ToString() + " " + this.newOperator.Text + " " + X.ToString() + " " + newOperator.Text + " ";
                    }
                    else
                    {
                        this.newOperator.Execute(true);
                        if (lastOperator != null)
                        {
                            lastOperator.Execute(true);
                            lastOperator = null;
                            Foramula = X.ToString() + " " + newOperator.Text + " ";
                        }
                        else
                            Foramula = X.ToString() + " " + newOperator.Text + " ";
                    }
                }
                else
                    Foramula = X.ToString() + " " + newOperator.Text + " ";
            }
            this.newOperator = newOperator;
            return true;
        }

        public bool Equals()
        {
            var result = false;
            IsEqual = true;
            if (newOperator != null)
            {
                newOperator.Execute(true);
                newOperator = null;
                if (lastOperator != null)
                {
                    lastOperator.Execute(true);
                    lastOperator = null;
                }
                return true;
            }
            return result;
        }

        public void StopCatalog()
        {
            IsCatalogRunning = false;
        }


        internal string GetStack()
        {
            return string.Format("X:{0} Y:{1} Z:{2} T:{3} L:{4}", _x.Value, _y.Value, _z.Value, _t.Value, _l.Value);
        }

        internal bool NoStackLift { get; set; }

        internal void StackDown(double value)
        {
            this._t.Set(this._z);
            this._z.Set(this._y);
            this._y.Set(this._x);
            X = value;
        }

        internal void StackRoundDown()
        {
            var value = this._t.Value;
            this._t.Set(this._z);
            this._z.Set(this._y);
            this._y.Set(this._x);
            X = value;
        }

        internal void StackRoundUp()
        {
            var help = this._x;
            X = this._y.Value;
            this._y.Set(this._z);
            this._z.Set(this._t);
            this._t.Set(help);
        }

        internal double StackUp()
        {
            var result = this._x.Value;
            this._x.Set(this._y);
            this._y.Set(this._z);
            this._z.Set(this._t);
            return result;
        }

        private void SetValue(double value)
        {
            this._dataError = false;
            this._rangeError = false;
            this._flagSet = false;
            this._flagCleared = false;
            this._view = false;
            this._value = value;
            if (value == 0)
            {
                this._exponent = 0;
            }
            else
            {
                var help = Math.Log10(Math.Abs(value));
                if (help < 0)
                {
                    help--;
                }

                this._exponent = (int)help;
                if (UsedFormat == Format.Eng)
                {
                    this._exponent /= 3;
                    this._exponent *= 3;
                }
            }

            this._mantisse = value / Math.Pow(10, this._exponent);
            if (IsFlagSet(57))
            {
                OnPropertyChanged("Bytes");
            }

            OnPropertyChanged(nameof(Display1));
            OnPropertyChanged(nameof(Display2));
            OnPropertyChanged(nameof(Display3));
            OnPropertyChanged(nameof(Display4));
        }

        private void NextEntry()
        {
            if (IsCatalog)
            {
                var stop = !this.actionModelEnumerator.MoveNext();
                while (!stop && ((this.actionModelEnumerator.Current.GetType() == typeof(AlphaActionModel)) || (this.actionModelEnumerator.Current.GetType() == typeof(DigitActionModel)) || !MarketPlace.IsAvtivated(actionModelEnumerator.Current.Option)))
                {
                    stop = !this.actionModelEnumerator.MoveNext();
                }

                if (stop)
                {
                    EndCatalog();
                }
                else
                {
                    if (actionModelEnumerator.Current.Accessory != lastAccessory)
                    {
                        lastAccessory = actionModelEnumerator.Current.Accessory;
                        Printer.Print("-" + lastAccessory + "-", PrinterModel.Mode.TRACE);
                    }
                    Printer.Print(Display2, PrinterModel.Mode.TRACE);
                }

                OnPropertyChanged(nameof(Display2));
            }
            else if (IsPrinting)
            {
                //if (programLine < runningProgram.Lines.Count)
                //{
                //    string line = runningProgram.Lines[programLine].StartsWith("LBL\t") ? "♦" : " ";
                //    line += runningProgram.Lines[programLine];
                //    if (programLine < 98)
                //        Printer.PrintMan(" " + (programLine + 2).ToString("D2") + line);
                //    else
                //        Printer.PrintMan((programLine + 2).ToString("D3") + line);
                //}
                programLine++;
                if (programLine >= programEndLine)
                {
                    if (programLine < 98)
                        Printer.PrintMan(" " + (programLine + 2).ToString("D2") + " END");
                    else
                        Printer.PrintMan((programLine + 2).ToString("D3") + " END");
                    IsPrinting = false;
                    //if (_timer != null)
                    //    _timer.Stop();
                }
            }
            //else if (runningProgram != null)
            //    runningProgram.ExecuteStep();
        }

        private void CreateStandartAction()
        {
            bool useAppResource = ((App)App.Current).UseAppResource; ;
            ActionModel action = new BinaryOperationModel(this, (a, b) => a + b, "+") { IsUpn = false, Description = !useAppResource ? "" : AppResources.HelpAdd};
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => a - b, "-") { IsUpn = false, Description = !useAppResource ? "" : AppResources.HelpSub};
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => a * b, "x") { IsUpn = false ,Description = !useAppResource ? "" : AppResources.HelpMul};
            action.Add("*");
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => a / b, () => NotZero(this._x.Value), "÷") { IsUpn = false , Description = !useAppResource ? "" : AppResources.HelpDiv};
            ActionModels.Add("/", action);
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => Math.Pow(10, x), "10˟") { IsLogaritmic = true, Description = !useAppResource ? "" : AppResources.Help10PX};
            ActionModels.Add(action.Text, action);
            action.Add("10↑X");
            action = new UnaryOperationModel(this, x => Math.Pow(Math.E, x), "e˟") { IsLogaritmic = true, Description = !useAppResource ? "" : AppResources.HelpEPX};
            action.Add("E↑X");
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, Math.Abs, "ABS") {Description = !useAppResource ? "" : AppResources.HelpAbs};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => Printer.PrintBuffer(true), "ADV") { Option = "Printer" , Description = !useAppResource ? "" : AppResources.HelpAVD };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => ClearFlag(48), "AOFF") { Description = !useAppResource ? "" : AppResources.HelpAOFF };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => SetFlag(48), "AON") { Description = !useAppResource ? "" : AppResources.HelpAON };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, v => AddAlpha(IsFlagSet(48) || _r[v].IsAlpha ? _r[v].Alpha : _r[v].Value.ToString("F" + Precision, CultureInfo.InvariantCulture)), true, "ARCL") { Description = !useAppResource ? "" : AppResources.HelpARCL };
            ActionModels.Add(action.Text, action);
            action = new InvokeActionModel("APPEND") { Description = !useAppResource ? "" : AppResources.HelpAPPEND };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => this._sbAlpha.Remove(0, 6), "ASHF") { Description = !useAppResource ? "" : AppResources.HelpASHF };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                DisplayA = this._r[v].Alpha;
                this._view = true;
                Printer.PrintMan(DisplayA);
            }, true, "AVIEW") { Description = !useAppResource ? "" : AppResources.HelpAVIEW };
            ActionModels.Add(action.Text, action);
            action = new TrigeometricOperatorModel(this, Math.Acos, true, "COS´¹") { Description = !useAppResource ? "" : AppResources.HelpACOS };
            ActionModels.Add(action.Text, action);
            action.Add("ACOS");
            action = new TrigeometricOperatorModel(this, Math.Asin, true, "SIN´¹") { Description = !useAppResource ? "" : AppResources.HelpASIN };
            ActionModels.Add(action.Text, action);
            action.Add("ASIN");
            action = new InvokeActionModel("ASN") {Description = !useAppResource ? "" : AppResources.HelpASN};
            ActionModels.Add(action.Text, action);
            action = new TrigeometricOperatorModel(this, Math.Atan, true, "TAN´¹") { Description = !useAppResource ? "" : AppResources.HelpATAN };
            ActionModels.Add(action.Text, action);
            action.Add("ATAN");
            action = new SelectActionModel(this, v =>
            {
                if (this._r.Length > v)
                {
                    this._r[v].Alpha = DisplayA.Length < 6 ? DisplayA : this.DisplayA.Substring(0, 6);
                }
            }, true, "ASTO") { Description = !useAppResource ? "" : AppResources.HelpASTO };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                Tone(3);
                Tone(4);
                Tone(5);
                Tone(6);
            }, "BEEP") { Description = !useAppResource ? "" : AppResources.HelpBEEP };
            ActionModels.Add(action.Text, action);
            action = new InvokeActionModel("←") { Description = !useAppResource ? "" : AppResources.HelpBACK };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, StartCatalog, (v) => IsBetween(v, 1, 3), false, "CATALOG") { IsRegister = false, IsUpn = false, Description = !useAppResource ? "" : AppResources.HelpCAT};
            ActionModels.Add(action.Text, action);
            action = new InvokeActionModel("CHS") { Description = !useAppResource ? "" : AppResources.HelpCHS };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                foreach (var register in this._r)
                {
                    register.Clear();
                }
                Foramula = string.Empty;
                foreach (var collection in Values)
                {
                    collection.Clear();
                }
            }, "CLRG") { Description = !useAppResource ? "" : AppResources.HelpCLRG };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => { this._sbAlpha = new StringBuilder(string.Empty); }, "CLA") { Description = !useAppResource ? "" : AppResources.HelpCLA };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                X = 0;
                this._y.Value = 0;
                this._z.Value = 0;
                this._t.Value = 0;
                Foramula = string.Empty;
            }, "CLST") {Description = !useAppResource ? "" : AppResources.HelpClearStack};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                if (this._view)
                {
                    SetValue(this._x.Value);
                }
                else
                {
                    var noStackLift = NoStackLift;
                    X = 0;
                    NoStackLift = noStackLift;
                }
                Foramula = string.Empty;
            }, "CLX")
            { IsUpn = false ,Description = !useAppResource ? "" : AppResources.HelpClearX};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => IsPrompt = true, "CLD") { Option = "Programming" , Description = !useAppResource ? "" : AppResources.HelpClearProgram};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, ClearFlag, (v) => IsBetween(v, 0, 29), true, "CF") { IsRegister = false, IsFlag = true ,Description = !useAppResource ? "" : AppResources.HelpClearFlag};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                Values[0].Insert(0, _x.Value);
                Values[1].Insert(0, _y.Value);
                FillStatisticRegister();
            }, "Σ+")
            { Option = "Statistic", Description = !useAppResource ? "" : AppResources.HelpStatAdd };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                Values[0].Remove(_x.Value);
                Values[1].Remove(_y.Value);
                FillStatisticRegister();
            }, "Σ-")
            { Option = "Statistic" , Description = !useAppResource ? "" : AppResources.HelpStatRemove};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                for (int i = 0; i < 13; i++)
                    this._r[this.statisticRegister + i].Clear();
                foreach (var collection in Values)
                {
                    collection.Clear();
                }
                this.X = 0;
            }, "CLΣ")
            { Option = "Statistic" , Description = !useAppResource ? "" : AppResources.HelpStatClear};
            ActionModels.Add(action.Text, action);
            action = new TrigeometricOperatorModel(this, Math.Cos, "COS") {Description = !useAppResource ? "" : AppResources.HelpCOS};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                var end = (_r[v].Value - (int)_r[v].Value) * 1000;
                var inc = (end - (int)end) * 100;
                if (inc == 0) inc = 1;
                this._r[v].Value -= inc;
                _flagSet = (int)(_r[v].Value) > end;
            }, IsRegister, true, "DSE")
            { IsCondition = true, Option = "Programming",Description = !useAppResource ? "" : AppResources.HelpDSE};
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, FromOct, IsOct, "DEC") { IsConversion = true, IsUpn = true, Description = !useAppResource ? "" : AppResources.HelpDEC};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                ClearFlag(42);
                ClearFlag(43);
            }, "DEG") {Description = !useAppResource ? "" : AppResources.HelpDEG};
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => x / 180.0 * Math.PI, "D-R") { IsConversion = true, IsUpn = true, Description = !useAppResource ? "" : AppResources.HelpDTR};
            ActionModels.Add(action.Text, action);
            action = new ActionModel("END") { Option = "Programming", Description = !useAppResource ? "" : AppResources.HelpEND};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                UsedFormat = Format.Eng;
                Precision = v;
                SetValue(X);
            }, false, "ENG") {IsFormat = true,Description = !useAppResource ? "" : AppResources.HelpENG };
            ActionModels.Add(action.Text, action);
            action = new InvokeActionModel("EEX") {Description = !useAppResource ? "" : AppResources.HelpEEX};
            ActionModels.Add(action.Text, action);
            action = new InvokeActionModel("ENTER↑") { IsUpn = false, Description = !useAppResource ? "" : AppResources.HelpENTER};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                var help = this._y;
                this._y = this._x;
                this._x = help;
                SetValue(help.Value);
            }, "X↔Y") {Description = !useAppResource ? "" : AppResources.HelpXTY};
            ActionModels.Add(action.Text, action);
            ActionModels.Add("X<>Y", action);
            action = new SelectActionModel(this, (v) =>
            {
                var help = this._r[v];
                this._r[v] = this._x;
                this._x = help;
                SetValue(help.Value);
            }, true, "X<>");
            ActionModels.Add(action.Text, action);
            action = new InvokeActionModel("XEQ") {Description = !useAppResource ? "" : AppResources.HelpXEQ};
            action.Add("XROM");
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, Math.Pow, "Y˟") {Description = !useAppResource ? "" : AppResources.HelpYPX};
            action.Add("Y↑X");
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => Math.Pow(Math.E, x) - 1, "E↑X-1") { IsLogaritmic = true ,Description = !useAppResource ? "" : AppResources.HelpEPX1};
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x =>
            {
                var help = (int)this._x.Value;
                double result = help;
                for (var i = help - 1; i > 1; i--)
                {
                    result *= i;
                }

                return result;
            }, () => IsGreaterEqual(this._x.Value, 0) && IsLowerThan(this._x.Value, 70) && IsInteger(this._x.Value), "FACT")
            { IsUpn = true ,Description = !useAppResource ? "" : AppResources.HelpFACT};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                this._flagSet = !this._flag[v];
                this._flagCleared = this._flag[v];
                SetFlag(50);
            }, (v) => IsBetween(v, 0, 57), true, "FC?")
            {
                IsCondition = true,
                IsRegister = false,
                IsFlag = true,
                Description = !useAppResource ? "" : AppResources.HelpFC
            };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                this._flagSet = !this._flag[v];
                this._flagCleared = this._flag[v];
                SetFlag(50);
                this._flag[v] = false;
            }, (v) => IsBetween(v, 0, 29), true, "FC?C")
            {
                IsCondition = true,
                IsRegister = false,
                IsFlag = true,
                Description = !useAppResource ? "" : AppResources.HelpFCC
            };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                UsedFormat = Format.Fix;
                Precision = v;
                if (Precision < 0) Precision = 0;
                if (Precision > 9) Precision = 9;
                SetValue(X);
            }, false, "FIX")
            {
                IsFormat = true
            };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                this._flagSet = this._flag[v];
                this._flagCleared = !this._flag[v];
                SetFlag(50);
            }, (v) => IsBetween(v, 0, 57), true, "FS?")
            {
                IsCondition = true,
                IsRegister = false,
                IsFlag = true,
                Description = !useAppResource ? "" : AppResources.HelpFS
            };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                this._flagSet = this._flag[v];
                this._flagCleared = !this._flag[v];
                SetFlag(50);
                this._flag[v] = false;
            }, (v) => IsBetween(v, 0, 29), true, "FS?C")
            {
                IsCondition = true,
                IsRegister = false,
                IsFlag = true,
                Description = !useAppResource ? "" : AppResources.HelpFSC
            };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, (x) =>
            {
                var help = (int)x;
                return x - help;
            }, "FRC") {Description = !useAppResource ? "" : AppResources.HelpFRC};
            ActionModels.Add(action.Text, action);
            //action = new ProgramActionModel(this, (v) => { if (runningProgram != null) runningProgram.GotoLable(v); }, "GTO") { Option = "Programming", IsRegister = false };
            //ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, NextEntry, "SST") { Description = !useAppResource ? "" : AppResources.HelpSST };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                if (IsCatalog)
                {
                    PreviusEntry();
                }
            }, "BST")
            { Description = !useAppResource ? "" : AppResources.HelpBST };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                SetFlag(42);
                ClearFlag(43);
                return;
            }, "GRAD") {Description = !useAppResource ? "" : AppResources.HelpGRAD};
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, ToHms, "HMS") { IsConversion = true, IsUpn = true ,Description = !useAppResource ? "" : AppResources.HelpHMS};
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => ToHms(FromHms(a) + FromHms(b)), "HMS+") {Description = !useAppResource ? "" : AppResources.HelpHMSP};
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => ToHms(FromHms(a) - FromHms(b)), "HMS-") { Description = !useAppResource ? "" : AppResources.HelpHMSM };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, FromHms, "HR") { IsConversion = true, IsUpn = true, Description = !useAppResource ? "" : AppResources.HelpHR };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                var end = Math.Round((_r[v].Value - (int)_r[v].Value) * 1000);
                var inc = (end - (int)end) * 100;
                if (inc == 0) inc = 1;
                this._r[v].Value += inc;
                _flagSet = (int)(_r[v].Value) <= end;
            }, IsRegister, true, "ISG")
            { IsCondition = true, Option = "Programming",Description = !useAppResource ? "" : AppResources.HelpISG};
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, (x) => (int)x, "INT") {Description = !useAppResource ? "" : AppResources.HelpINT};
            ActionModels.Add(action.Text, action);
            //action = new ProgramActionModel(this, null, "LBL") { Option = "Programming", IsRegister = false };
            //ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, Math.Log10, () => IsGreaterThan(this._x.Value, 0), "LOG") { IsLogaritmic = true, Description = !useAppResource ? "" : AppResources.HelpLOG};
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, Math.Log, () => IsGreaterThan(this._x.Value, 0), "LN") { IsLogaritmic = true, Description = !useAppResource ? "" : AppResources.HelpLN };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => 1 + Math.Log(x), () => IsGreaterThan(this._x.Value, 0), "LN1+X") { IsLogaritmic = true, IsUpn = true, Description = !useAppResource ? "" : AppResources.HelpLN1 };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () => StackDown(this._l.Value), "LASTX") { Description = !useAppResource ? "" : AppResources.HelpLASTX }; ;
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this.X = this._r[this.statisticRegister].Value / this._r[this.statisticRegister + 5].Value;
                this._y.Value = this._r[this.statisticRegister + 2].Value / this._r[this.statisticRegister + 5].Value;
            }, "MEAN")
            { Option = "Statistic" ,Description = !useAppResource ? "" : AppResources.HelpMEAN};
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => (b == 0) ? a : a % b, "MOD") { Description = !useAppResource ? "" : AppResources.HelpMOD }; ;
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, ToOct, () => IsInteger(X) && IsBetween(X, -1073741823, 1073741823), "OCT") { IsConversion = true, IsUpn = true, Description = !useAppResource ? "" : AppResources.HelpOCT};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, Pause, "PSE") {Description = !useAppResource ? "" : AppResources.HelpPSE};
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationPModel(this, (a, b) => a * b / 100, "%") { Description = !useAppResource ? "" : AppResources.HelpPer };
            ActionModels.Add(action.Text, action);
            action = new BinaryOperationModel(this, (a, b) => (b - a) * 100 / a, () => NotZero(this._y.Value), "%CH") { Description = !useAppResource ? "" : AppResources.HelpPerC };
            ActionModels.Add(action.Text, action);
            action = new ComplexOperationModel(this, FromPolarY, FromPolarX, "P→R") { IsConversion = true, FromAngle = true, Description = !useAppResource ? "" : AppResources.HelpDTR};
            ActionModels.Add(action.Text, action);
            action.Add("P-R") ;
            action = new ModelActionModel(this, Prompt, "PROMPT") {Description = !useAppResource ? "" : AppResources.HelpPROMPT};
            ActionModels.Add(action.Text, action);
            action = new ActionModel("OFF") {Description = !useAppResource ? "" : AppResources.HelpOFF};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                ClearFlag(42);
                SetFlag(43);
            }, "RAD"){ Description = !useAppResource ? "" : AppResources.HelpRAD };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => x / Math.PI * 180.0, "R-D") { IsConversion = true, IsUpn = true,Description = !useAppResource ? "" : AppResources.HelpRTD};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) => { StackDown(_r[v].Value); }, IsRegister, true, "RCL") { Description = !useAppResource ? "" : AppResources.HelpRCL };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => 1 / x, () => NotZero(this._x.Value), "1/X") { IsUpn = true, Description = !useAppResource ? "" : AppResources.HelpRTD };
            ActionModels.Add(action.Text, action);
            action = new ComplexOperationModel(this, ToPolarO, ToPolarR, "R→P") { IsConversion = true, ToAngle = true, Description = !useAppResource ? "" : AppResources.HelpRTP };
            ActionModels.Add(action.Text, action);
            action.Add("R-P");
            action = new ModelActionModel(this, StackRoundDown, "R↓") {Description = !useAppResource ? "" : AppResources.HelpRD};
            action.Add("RDN");
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, StackRoundUp, "R↑") { Description = !useAppResource ? "" : AppResources.HelpRU };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, RunStop, "R/S") { Description = !useAppResource ? "" : AppResources.HelpRS };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                if (callStack.Count > 0)
                {
                    var index = callStack.Count - 1;
                    runningProgram = callStack[index].Program;
                    runningProgram.GotoLine(callStack[index].Step);
                    callStack.RemoveAt(index);
                }
                else if (runningProgram != null) runningProgram.Return();
            }, "RTN")
            { Option = "BaseProgramming", IsRegister = false, Description = !useAppResource ? "" : AppResources.HelpRS };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x =>
            {
                if (UsedFormat == Format.Fix)
                {
                    return Math.Round(x, this._precision);
                }
                else
                {
                    var help = Math.Round(this._mantisse * Math.Pow(10, this._precision));
                    this._mantisse = help / Math.Pow(10, this._precision);
                    return this._mantisse * Math.Pow(10, this._exponent);
                }
            }, "RND") { Description = !useAppResource ? "" : AppResources.HelpRND };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, SetFlag, (v) => IsBetween(v, 0, 29), true, "SF") { IsRegister = false, IsFlag = true, Description = !useAppResource ? "" : AppResources.HelpSF};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                UsedFormat = Format.Sci;
                Precision = v;
                SetValue(X);
            }, (v) => IsBetween(v, 0, 9), false, "SCI")
            {
                IsFormat = true,
                Description = !useAppResource ? "" : AppResources.HelpSCI
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                if (this._x.IsAlpha)
                {
                    X = 0;
                }
                else if (X < 0)
                {
                    X = -1;
                }
                else
                {
                    X = 1;
                }
            }, "SIGN") {Description = !useAppResource ? "" : AppResources.HelpSIGN};
            ActionModels.Add(action.Text, action);
            action = new TrigeometricOperatorModel(this, Math.Sin, "SIN") { Description = !useAppResource ? "" : AppResources.HelpSIN };
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, x => x * x, "X²") { Description = !useAppResource ? "" : AppResources.HelpX2 };
            action.Add("X↑2");
            ActionModels.Add(action.Text, action);
            action = new UnaryOperationModel(this, Math.Sqrt, () => IsGreaterEqual(this._x.Value, 0), "√x") { Description = !useAppResource ? "" : AppResources.HelpSQRT };
            ActionModels.Add(action.Text, action);
            action.Add("SQRT");
            action = new SelectActionModel(this, ReSize, (v) => IsBetween(v, 0, 999), true, "SIZE") { IsRegister = false, Select3 = true, Description = !useAppResource ? "" : AppResources.HelpSIZE};
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                if (this._r[this.statisticRegister + 5].Value < 2)
                {
                    this._dataError = true;
                    SetFlag(50);
                }
                else
                {
                    this.X = Sdev(this._r[this.statisticRegister].Value, this._r[this.statisticRegister + 1].Value, (int)this._r[this.statisticRegister + 5].Value);
                    this._y.Value = Sdev(this._r[this.statisticRegister + 2].Value, this._r[this.statisticRegister + 3].Value, (int)this._r[this.statisticRegister + 6].Value);
                }
            }, "SDEV")
            { Option = "Statistic", Description = !useAppResource ? "" : AppResources.HelpSDEV};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) => statisticRegister = v, (v) => IsBetween(v, 0, MaxRegister - 6), true, "ΣREG") { Option = "Statistic" };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, RunStop, "STOP") {Description = !useAppResource ? "" : AppResources.HelpSTOP};
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) => { this._r[v].Value = this._x.Value; }, IsRegister, true, "STO") { Description = !useAppResource ? "" : AppResources.HelpSTO };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) => { this._r[v].Value += this._x.Value; }, IsRegister, true, "STO +") { Description = !useAppResource ? "" : AppResources.HelpSTP };
            ActionModels.Add(action.Text, action);
            action.Add("ST+");
            action = new SelectActionModel(this, (v) => { this._r[v].Value /= this._x.Value; }, (v) => IsRegister(v) && NotZero(this._x.Value), true, "STO /") { Description = !useAppResource ? "" : AppResources.HelpSTD };
            ActionModels.Add(action.Text, action);
            action.Add("ST/");
            action = new SelectActionModel(this, (v) => { this._r[v].Value *= this._x.Value; }, IsRegister, true, "STO x") { Description = !useAppResource ? "" : AppResources.HelpSTM };
            action.Add("ST*");
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) => { this._r[v].Value -= this._x.Value; }, IsRegister, true, "STO -") { Description = !useAppResource ? "" : AppResources.HelpSTS };
            ActionModels.Add(action.Text, action);
            action.Add("ST-");
            action = new TrigeometricOperatorModel(this, Math.Tan, "TAN") { Description = !useAppResource ? "" : AppResources.HelpTAN };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, Tone, (v) => IsBetween(v, 0, 9), false, "TONE") { Description = !useAppResource ? "" : AppResources.HelpTONE };
            ActionModels.Add(action.Text, action);
            action = new SelectActionModel(this, (v) =>
            {
                SetValue(this._r[v].Value);
                this._view = true;
                Printer.PrintMan(Display1 + Display2 + Display3 + Display4);
            }, true, "VIEW") { Description = !useAppResource ? "" : AppResources.HelpVIEW };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value == this._y.Value;
                this._flagCleared = this._x.Value != this._y.Value;
                SetFlag(50);
            }, "X=Y?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXEY
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value == 0;
                this._flagCleared = this._x.Value != 0;
                SetFlag(50);
            }, "X=0?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXEN
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value > this._y.Value;
                this._flagCleared = this._x.Value <= this._y.Value;
                SetFlag(50);
            }, "X>Y?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXGY
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value > 0;
                this._flagCleared = this._x.Value <= 0;
                SetFlag(50);
            }, "X>0?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXGN
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value < this._y.Value;
                this._flagCleared = this._x.Value >= this._y.Value;
                SetFlag(50);
            }, "X<Y?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXLY
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value < 0;
                this._flagCleared = this._x.Value >= 0;
                SetFlag(50);
            }, "X<0?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXLY
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value <= this._y.Value;
                this._flagCleared = this._x.Value > this._y.Value;
                SetFlag(50);
            }, "X≤Y?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXLEY
            };
            ActionModels.Add(action.Text, action);
            ActionModels.Add("X<=Y?", action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value <= 0;
                this._flagCleared = this._x.Value > 0;
                SetFlag(50);
            }, "X≤0?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXLEN
            };
            ActionModels.Add(action.Text, action);
            ActionModels.Add("X<=0?", action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value != this._y.Value;
                this._flagCleared = this._x.Value == this._y.Value;
                SetFlag(50);
            }, "X≠Y?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXUY
            };
            ActionModels.Add(action.Text, action);
            action = new ModelActionModel(this, () =>
            {
                this._flagSet = this._x.Value != this._y.Value;
                this._flagCleared = this._x.Value == this._y.Value;
                SetFlag(50);
            }, "X≠0?")
            {
                IsCondition = true,
                IsLogical = true,
                Description = !useAppResource ? "" : AppResources.HelpXUN
            };
            ActionModels.Add(action.Text, action);

            action = new ModelActionModel(this, () => StackDown(Math.PI), "π") {Description = !useAppResource ? "" : AppResources.HelpPI};
            ActionModels.Add(action.Text, action);
            action.Add("PI");
            action = new InvokeActionModel(".") { IsUpn = false };
            ActionModels.Add(action.Text, action);
            for (var i = 0; i < 10; i++)
            {
                action = new DigitActionModel(i);
                ActionModels.Add(action.Text, action);
            }
            for (var i = 10; i < 16; i++)
            {
                action = new DigitActionModel(((char)('A' + i - 10)).ToString(), i);
                ActionModels.AddExtension(action.Text, "FORMAT", action);
            }

            for (var i = 0; i < 26; i++)
            {
                action = new AlphaActionModel((char)('A' + i));
                ActionModels.Add(action.TextD, action);
            }

            for (var i = 0; i < 6; i++)
            {
                action = new AlphaActionModel((char)('a' + i));
                ActionModels.Add(action.TextD, action);
            }

            for (var i = 0; i < 10; i++)
            {
                action = new AlphaActionModel((char)('0' + i));
                ActionModels.Add(action.TextD, action);
            }

            action = new AlphaActionModel("SPACE", ' ');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('↑');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('Δ');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('Σ');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('%');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('-');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('≠');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('*');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('/');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('+');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('<');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('>');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('=');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('?');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel(':');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel(',');
            ActionModels.Add(action.TextD, action);
            action = new AlphaActionModel('.');
            ActionModels.Add(action.TextD, action);
            action = new SelectActionModel(this, (v) =>
            {
                Precision = v;
                UsedFormat = Format.Hex;
                SetValue(X);
            }, (v) => IsBetween(v, 1, 8), false, "HEX")
            { IsFormat = true };
            ActionModels.AddExtension(action.Text, "FORMAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                Messenger.ShowDialogAsync(AppResources.NotYet);
                //Precision = v;
                //UsedFormat = Format.Bin;
                //SetValue(X);
            }, (v) => IsBetween(v, 1, 8), false, "BIN")
            { IsFormat = true };
            ActionModels.AddExtension(action.Text, "FORMAT", action);

            action = new ModelActionModel(this, () =>
            {
                if (!isUpn)
                {
                    IsUpn = true;
                    X = 0;
                    this._y.Value = 0;
                    this._z.Value = 0;
                    this._t.Value = 0;
                    Foramula = string.Empty;
                }
            }, "UPN")
            { IsUpn = false };
            ActionModels.AddExtension(action.Text, "NOUPN", action);

            action = new ModelActionModel(this, () =>
            {
                if (UsedFormat == Format.Bin)
                {
                    Messenger.ShowDialogAsync(AppResources.NotBin);
                }
                else if (UsedFormat == Format.Hex)
                {
                    Messenger.ShowDialogAsync(AppResources.NotHex);
                }
                else if (isUpn)
                {
                    IsUpn = false;
                    X = 0;
                    this._y.Value = 0;
                    this._z.Value = 0;
                    this._t.Value = 0;
                    Foramula = string.Empty;
                }
            }, "NOUPN")
            { IsUpn = false };
            ActionModels.AddExtension(action.Text, "NOUPN", action);
            action = new BinaryOperationModel(this, (a, b) => Cnk(a,b), "CNK");
            ActionModels.AddExtension(action.Text,"CNK", action);
            action = new ModelActionModel(this, () =>
            {
                if (Values[0].Count < 2)
                {
                    this._dataError = true;
                    SetFlag(50);
                }
                else
                {
                    var mid1 = (Values[0].Count - 1) / 2;
                    var mid2 = Values[0].Count / 2;
                    if (mid1 == mid2)
                        this.X = Values[0].GetSortedAt(mid2);
                    else
                        this.X = (Values[0].GetSortedAt(mid1) + Values[0].GetSortedAt(mid2)) / 2;
                }
            }, "MEDIAN")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                if (Values[0].Count < 2)
                {
                    this._dataError = true;
                    SetFlag(50);
                }
                else
                {
                    var ds = new SortedCollection<double>();
                    var mid = Values[0].Count / 2 - 1;
                    var mid2 = (Values[0].Count - 1) / 2;
                    var mean = this._r[this.statisticRegister].Value / this._r[this.statisticRegister + 5].Value;
                    for (var index = 0; index < Values[0].Count; index++)
                    {
                        ds.Insert(0, Math.Abs(mean - Values[0][index]));
                    }
                    if (mid == mid2)
                        this.X = ds.GetSortedAt(mid);
                    else
                        this.X = (ds.GetSortedAt(mid) + ds.GetSortedAt(mid2)) / 2;
                }
            }, "MAD")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                foreach (var value in Values)
                {
                    value.Clear();
                }
            }, "CLΣΣ")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                var help = new SortedCollection<double>();
                foreach (var value in Values[0])
                {
                    help.Add(value);
                }
                Values[0].Clear();
                foreach (var value in Values[v])
                {
                    Values[0].Add(value);
                }
                Values[v].Clear();
                foreach (var value in help)
                {
                    Values[v].Add(value);
                }
                FillStatisticRegister();
            }, (v) => (v >= 0) && (v < maxValues - 1), true, "ΣX↔")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            ActionModels.AddExtension("ΣX<>", "RSTAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                var help = new SortedCollection<double>();
                foreach (var value in Values[1])
                {
                    help.Add(value);
                }
                Values[1].Clear();
                foreach (var value in Values[v])
                {
                    Values[1].Add(value);
                }
                Values[v].Clear();
                foreach (var value in help)
                {
                    Values[v].Add(value);
                }
                FillStatisticRegister();
            }, (v) => (v >= 0) && (v < maxValues - 1), true, "ΣY↔")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            ActionModels.AddExtension("ΣY<>", "RSTAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                Values[v].Add(X);
                FillStatisticRegister();
                X = Values[v].Count;
            }, (v) => (v >= 0) && (v < maxValues - 1), true, "Σ+N")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) => StackDown(Statistic.GetTDistribution(this._r[v].Value, this._r[v + 1].Value, false)), (v) => (v >= 0) && (v < maxValues - 1), true, "TS") { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) => StackDown(Statistic.GetTDistribution(this._r[v].Value, this._r[v + 1].Value, true)), (v) => (v >= 0) && (v < maxValues - 1), true, "TD") { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                if (_r[statisticRegister + 7].Value > _r[statisticRegister + 8].Value)
                    StackDown(Statistic.GetFDistribution(_r[statisticRegister + 13].Value, _r[statisticRegister + 5].Value, _r[statisticRegister + 6].Value));
                else
                    StackDown(Statistic.GetFDistribution(_r[statisticRegister + 13].Value, _r[statisticRegister + 6].Value, _r[statisticRegister + 5].Value));
            }, "FD")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) => StackDown(Statistic.GetChiDistribution(this._r[v].Value, this._r[v + 1].Value)), (v) => (v >= 0) && (v < maxValues - 1), true, "CHI↑2") { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) => StackDown(Values[0][Values[0].Count - v - 1]), (v) => IsBetween(v, 0, Values[0].Count - 1), true, "GETΣX") { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) => StackDown(Values[1][Values[0].Count - v - 1]), (v) => IsBetween(v, 0, Values[1].Count - 1), true, "GETΣY") { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                Values[0].RemoveAt(Values[0].Count - v - 1);
                if (v < Values[1].Count)
                    Values[1].RemoveAt(Values[1].Count - v - 1);
                FillStatisticRegister();
            }, (v) => IsBetween(v, 0, Values[0].Count - 1), true, "DELΣ")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                var b = _r[statisticRegister + 9].Value / _r[statisticRegister + 7].Value;
                var r = _r[statisticRegister + 9].Value / Math.Sqrt(_r[statisticRegister + 7].Value * _r[statisticRegister + 8].Value);
                StackDown(r * r);
                StackDown(_r[statisticRegister + 2].Value / _r[statisticRegister + 6].Value - _r[statisticRegister].Value / _r[statisticRegister + 5].Value * b);
                StackDown(b);
            }, "REG")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                var b = _r[statisticRegister + 9].Value / _r[statisticRegister + 8].Value;
                var r = _r[statisticRegister + 9].Value / Math.Sqrt(_r[statisticRegister + 7].Value * _r[statisticRegister + 8].Value);
                StackDown(r * r);
                StackDown(_r[statisticRegister].Value / _r[statisticRegister + 5].Value - _r[statisticRegister + 2].Value / _r[statisticRegister + 6].Value * b);
                StackDown(b);
            }, "IREG")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                var entry = new PrinterRegresionEntry();
                entry.Bx = _r[statisticRegister + 8].Value / _r[statisticRegister + 6].Value;
                entry.By = _r[statisticRegister + 8].Value / _r[statisticRegister + 7].Value;
                entry.Ax = _r[statisticRegister + 2].Value / _r[statisticRegister + 5].Value - _r[statisticRegister].Value / _r[statisticRegister + 5].Value * entry.Bx;
                entry.Ay = _r[statisticRegister].Value / _r[statisticRegister + 5].Value - _r[statisticRegister + 2].Value / _r[statisticRegister + 5].Value * entry.Ay;
                var r = _r[statisticRegister + 8].Value / Math.Sqrt(_r[statisticRegister + 6].Value * _r[statisticRegister + 7].Value);
                entry.R2 = r * r;
                StackDown(entry.R2);
                StackDown(entry.Ax);
                StackDown(entry.Bx);
                StackDown(entry.Ay);
                StackDown(entry.By);
                Printer.Entrys.Add(entry);
                OnPropertyChanged(nameof(Printer));
            }, "PREG")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () =>
            {
                for (int index = maxValues; index > 0; index--)
                    if (Values[index - 1].Count > 0)
                    {
                        StackDown(index);
                        break;
                    }
            }, "COUNT")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) => { StackDown(Values[v].Count); }, (v) => IsBetween(v, 0, maxValues - 1), true, "COUNTN") { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                if (Values[v].Count < 1)
                {
                    this._dataError = true;
                    SetFlag(50);
                }
                else
                {
                    StackDown(Values[v].Sum() / Values[v].Count);
                }
            }, (v) => IsBetween(v, 0, maxValues - 1), true, "MEANN")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new SelectActionModel(this, (v) =>
            {
                if (Values[v].Count < 2)
                {
                    this._dataError = true;
                    SetFlag(50);
                }
                else
                {
                    var x = Values[v].Sum() / Values[v].Count;
                    var s = 0.0;
                    foreach (var value in Values[v])
                    {
                        s += (value - x) * (value - x);
                    }
                    StackDown(Math.Sqrt(s / (Values[v].Count - 1)));
                }
            }, (v) => IsBetween(v, 0, maxValues - 1), true, "SDEVN")
            { Option = "RobustStatistic" };
            ActionModels.AddExtension(action.Text, "RSTAT", action);
            action = new ModelActionModel(this, () => { Prx(); }, "PRX") { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () =>
            {
                Printer.PrintMan(DisplayA);
                OnPropertyChanged(nameof(Printer));
            }, "PRA")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () =>
            {
                PrintRegister(0, MaxRegister);
                OnPropertyChanged(nameof(Printer));
            }, "PRREG")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () =>
            {
                var start = (int)X;
                var stop = (int)((X - start) * 1000.0);
                PrintRegister(start, stop);
                OnPropertyChanged(nameof(Printer));
            }, "PRREGX")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () =>
            {
                PrintStatisticRegister();
                OnPropertyChanged(nameof(Printer));
            }, "PRΣ")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () =>
            {
                PrintStackRegister();
                OnPropertyChanged(nameof(Printer));
            }, "PRSTK")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new InvokeActionModel("PRP"){ Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new SelectActionModel(this, ListProgram, (v) => (runningProgram != null) && (programLine < runningProgram.Lines.Count), true, "LIST");
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () =>
            {
                PrintFlags();
                OnPropertyChanged(nameof(Printer));
            }, "PRFLAGS")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(
               this,
               () =>
               {
                   Printer.AddToPrintBuffer(IsFlagSet(13) ? _sbAlpha.ToString().ToLower() : _sbAlpha.ToString());
                   OnPropertyChanged(nameof(Printer));
               },
               "ACA")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(
               this,
               () =>
               {
                   string exponent = Display3 + Display4;
                   if (exponent.Length == 0)
                       Printer.AddToPrintBuffer(Display1 + Display2);
                   else
                       Printer.AddToPrintBuffer(Display1 + Display2 + " " + exponent);
                   OnPropertyChanged(nameof(Printer));
               },
               "ACX")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this,
                () =>
                {
                    if ((_x.Value > 0) && (_x.Value <= printChar.Length + 1))
                        Printer.AddToPrintBuffer(printChar[(int)_x.Value - 1].ToString());
                },
                "ACCHR");
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this, () => Printer.PrintBuffer(), "PRBUF") { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(
               this,
               () =>
               {
                   for (int i = 0; i < _x.Value; i++)
                       Printer.AddToPrintBuffer(" ");
                   OnPropertyChanged(nameof(Printer));
               },
               "SKPCHR")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(this,
                () =>
                {
                    if ((_x.Value >= 0) && (_x.Value <= 127))
                        Printer.AddColumn((byte)_x.Value);
                },
                "ACCOL");
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(
               this,
               () =>
               {
                   Printer.SkipColumns((int)_x.Value);
                   OnPropertyChanged(nameof(Printer));
               },
               "SKPCOL")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(
               this,
               () =>
               {
                   var size = (int)_r[2].Value;
                   Printer.PlotLine(_x.Value, _r[0].Value, _r[1].Value, size, (_r[2].Value - size) * 1000);
                   OnPropertyChanged(nameof(Printer));
               },
               "REGPLOT")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);
            action = new ModelActionModel(
               this,
               () =>
               {
                   //                   Printer.SkipColumns((int)_x.Value);
                   OnPropertyChanged(nameof(Printer));
               },
               "SKTPLOT")
            { Option = "Printer" };
            ActionModels.AddExtension(action.Text, "PRINTER", action);

        }

        private void StartCatalog(int value)
        {
            if (IsRunning)
            {
                return;
            }

            if (value == 1)
            {
                this.actionModelEnumerator = this.Programs.Values.GetEnumerator();
            }
            else
            {
                this.actionModelEnumerator = ActionModels.GetEnumerator(value);
            }

            if (!this.actionModelEnumerator.MoveNext())
            {
                return;
            }

            IsCatalog = true;
            IsCatalogRunning = true;
            lastAccessory = string.Empty;
            Printer.Print("CAT " + value, PrinterModel.Mode.TRACE, true);
            if ((this.actionModelEnumerator.Current.GetType() == typeof(AlphaActionModel)) || (this.actionModelEnumerator.Current.GetType() == typeof(DigitActionModel)))
                NextEntry();
            Device.StartTimer(timerSpeed, TimerOnTick);
        }

        public void RestartCatalog()
        {
            IsCatalogRunning = true;
            Device.StartTimer(timerSpeed,TimerOnTick);
        }

        public void ChangeCatalogSpeed(bool slow)
        {
            timerRestart = true;
            Device.StartTimer(slow ? slowTimerSpeed :timerSpeed, TimerOnTick);
        }

        internal bool TimerOnTick()
        {
            if (timerRestart)
            {
                timerRestart = false;
                return false;
            }
            if (IsRunning)
            {
                if (pauseCount > 0)
                    pauseCount--;
                else
                {
                    runningProgram.ExecuteStep();
                    programStep++;
                    if (programStep > 16)
                        programStep = 0;
                    OnPropertyChanged("ProgramStep");
                }
                return true;
            }
            if (IsCatalogRunning || IsPrinting)
            {
                NextEntry();
                return true;
            }
            return false;
        }

        private bool NotZero(double value)
        {
            if (value != 0)
            {
                return true;
            }

            this._dataError = true;
            SetFlag(50);
            OnPropertyChanged(nameof(Display2));
            return false;
        }


        private void StartUserKeys()
        {
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("HMS+"), 0x0E));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("HMS"), 0x1E));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("FACT"), 0x2E));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("CNK"), 0x3E));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("HMS-"), 0x0D));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("HR"), 0x1D));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("MEAN"), 0x2D));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("SDEV"), 0x3D));
            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("HEX"), 0x0F));
//            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("BIN"), 0x10));
//            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("UPN"), 0x3B));
//            keyAssignment.Add(new KeyAssignmentModel(keyAssignment.Count + 1, GetActionModel("NOUPN"), 0x3C));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("MOD"), 0x0E));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("A"), 0x1E));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("B"), 0x2E));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("C"), 0x3E));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("HMS-"), 0x0D));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("D"), 0x1D));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("E"), 0x2D));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("F"), 0x3D));
            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("HEX"), 0x0F));
//            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("BIN"), 0x10));
//            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("UPN"), 0x3B));
//            keyAssignmentHex.Add(new KeyAssignmentModel(keyAssignmentHex.Count + 1, GetActionModel("NOUPN"), 0x3C));
        }

        private bool IsBetween(double value, double min, double max)
        {
            if ((value >= min) && (value <= max))
            {
                return true;
            }

            this._dataError = true;
            SetFlag(50);
            return false;
        }

        private bool IsOct()
        {
            var help = (int)X;
            if (IsInteger(X) && IsBetween(help, -7777777777, 7777777777))
            {
                bool OK = true;
                while (OK && (help != 0))
                {
                    OK = (help % 10) < 8;
                    help /= 10;
                }
                if (OK) return true;
            }

            this._dataError = true;
            SetFlag(50);
            return false;
        }

        private bool IsGreaterEqual(double value, double limit)
        {
            if (value >= limit)
            {
                return true;
            }

            this._dataError = true;
            SetFlag(50);
            return false;
        }

        private bool IsGreaterThan(double value, double limit)
        {
            if (value > limit)
            {
                return true;
            }

            this._dataError = true;
            SetFlag(50);
            return false;
        }

        private bool IsInteger(double value)
        {
            var help = (int)value;
            if (help == value)
            {
                return true;
            }

            this._dataError = true;
            SetFlag(50);
            return false;
        }

        private bool IsLowerThan(double value, double limit)
        {
            if (value < limit)
            {
                return true;
            }

            this._rangeError = true;
            SetFlag(50);
            return false;
        }

        internal bool IsRegister(int value)
        {
            if ((value >= 0) && (value < MaxRegister + 6))
            {
                return true;
            }

            SetFlag(50);
            return false;
        }

        internal Register GetRegister(int value)
        {
            if ((value >= 0) && (value < MaxRegister + 6))
                return _r[value];
            return null;
        }

        internal void Aview()
        {
            IsPrompt = true;
            promptSchift = 0;
            OnPropertyChanged("Aview");
        }

        private static double FromHms(double value)
        {
            var h = (int)value;
            value = (value - h) * 100;
            var m = (int)value;
            value = (value - m) * 100;
            value /= 60;
            value += m;
            value /= 60;
            value += h;
            return value;
        }

        private static double ToHms(double value)
        {
            var h = (int)value;
            value = (value - h) * 60;
            var m = (int)value;
            value = (value - m) * 60;
            value /= 100;
            value += m;
            value /= 100;
            value += h;
            return value;
        }

        private static double ToOct(double value)
        {
            var input = (int)value;
            var result = 0;
            for (var count = 0; input != 0; count++)
            {
                result += (input % 8) * (int)Math.Pow(10, count);
                input /= 8;
            }
            return result;
        }

        private static double FromOct(double value)
        {
            var input = (int)value;
            var result = 0;
            for (var count = 0; input != 0; count++)
            {
                result += (input % 10) * (int)Math.Pow(8, count);
                input /= 10;
            }
            return result;
        }

        private static double FromPolarX(double r, double o)
        {
            var help = r;
            return Math.Cos(o) * r;
        }

        private static double FromPolarY(double r, double o)
        {
            return Math.Sin(o) * r;
        }

        private static double ToPolarO(double x, double y)
        {
            var result = Math.PI / 2;
            if (x != 0)
                result = Math.Atan(y / x);
            if (x < 0)
            {
                if (y < 0)
                    result = result - Math.PI;
                else result = Math.PI + result;
            }
            return result;
        }

        private static double ToPolarR(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        private static double Sdev(double x, double xx, int n, bool sqrt = true)
        {
            var result = (xx - x * x / n) / (n - 1);
            if (!sqrt) return result;
            return Math.Sqrt(result);
        }

        private static double Cnk(double x, double y)
        {
            double result = 1;
            long k = (int)y;
            if (k == y)
                for (var j = 1; j <= k; j++)
                    result *= (x + 1 - j) / j;
            return result;
        }

        private static void Tone(int value)
        {
            SoundPlayer.PlaySound();
        }

        internal void Pause()
        {
            if (IsRunning)
            {
                pauseCount = (int)(timePause / timePerStep);
                OnPropertyChanged("ProgramStep");
            }
        }

        internal void Prompt()
        {
            if (IsRunning)
            {
                IsPrompt = true;
                promptSchift = 0;
                StopProgram();
            }
            Printer.Print(DisplayA);
        }


        internal void PrintRegister(int from, int to)
        {
            for (var index = from; index <= to; index++)
            {
                string sLine = "R" + index.ToString("000") + " =  ";
                if (_r[index].IsAlpha)
                    Printer.Print(sLine + "\"" + _r[index].Alpha + "\"");
                if (!_r[index].IsAlpha || _r[index].Value != 0)
                    Printer.Print(sLine + _r[index].Value.ToString("F4"));
            }
        }

        internal void PrintStatisticRegister()
        {
            string sLine = "ΣX   =  ";
            Printer.Print(sLine + _r[statisticRegister].Value.ToString("F4"));
            sLine = "ΣX↑2 =  ";
            Printer.Print(sLine + _r[statisticRegister + 1].Value.ToString("F4"));
            sLine = "ΣY   =  ";
            Printer.Print(sLine + _r[statisticRegister + 2].Value.ToString("F4"));
            sLine = "ΣY↑2 =  ";
            Printer.Print(sLine + _r[statisticRegister + 3].Value.ToString("F4"));
            sLine = "ΣXY  =  ";
            Printer.Print(sLine + _r[statisticRegister + 4].Value.ToString("F4"));
            if (!MarketPlace.IsAvtivated("RobustStatistic"))
            {
                sLine = "N    =  ";
                Printer.Print(sLine + _r[statisticRegister + 5].Value.ToString("F4"));
            }
            else
            {
                sLine = "Nx   =  ";
                Printer.Print(sLine + _r[statisticRegister + 5].Value.ToString("F4"));
                sLine = "Ny   =  ";
                Printer.Print(sLine + _r[statisticRegister + 6].Value.ToString("F4"));
                sLine = "SX↑2 =  ";
                Printer.Print(sLine + _r[statisticRegister + 7].Value.ToString("F4"));
                sLine = "SY↑2 =  ";
                Printer.Print(sLine + _r[statisticRegister + 8].Value.ToString("F4"));
                sLine = "SXY↑2=  ";
                Printer.Print(sLine + _r[statisticRegister + 9].Value.ToString("F4"));
                sLine = "d    =  ";
                Printer.Print(sLine + _r[statisticRegister + 10].Value.ToString("F4"));
            }
        }

        internal void PrintStackRegister()
        {
            var sLine = "T=  ";
            Printer.Print(sLine + _t.Value.ToString("F4"));
            sLine = "Z=  ";
            Printer.Print(sLine + _z.Value.ToString("F4"));
            sLine = "Y=  ";
            Printer.Print(sLine + _y.Value.ToString("F4"));
            sLine = "X=  ";
            Printer.Print(sLine + _x.Value.ToString("F4"));

        }

        internal void PrintFlags()
        {
            var sLine = "STATUS";
            Printer.Print(sLine);
            sLine = "SIZE=  ";
            Printer.Print(sLine + maxValues);
            sLine = "Σ=  ";
            Printer.Print(sLine + statisticRegister);
            if (IsFlagSet(42))
                sLine = "GRAD";
            else if (IsFlagSet(43))
                sLine = "RAD";
            else
                sLine = "DEG";
            Printer.Print(sLine);
            switch (UsedFormat)
            {
                case Format.Fix:
                    sLine = "FIX ";
                    break;
                case Format.Sci:
                    sLine = "SCI ";
                    break;
                case Format.Eng:
                    sLine = "ENG ";
                    break;
                case Format.Hex:
                    sLine = "HEX ";
                    break;
                case Format.Bin:
                    sLine = "BIN ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Printer.Print(sLine + Precision);
            Printer.Print("");
            sLine = "FLAGS";
            Printer.Print(sLine);
            for (int i = 0; i < 58; i++)
            {
                sLine = "F " + i.ToString("D2");
                if (IsFlagSet(i))
                    Printer.Print(sLine + "  SET");
                else
                    Printer.Print(sLine + "  CLEAR");
            }
        }

        internal void Prx(bool man = true, bool marker = true)
        {
            string exponent = Display3 + Display4;
            if (exponent.Length == 0)
                Printer.PrintMan(Display1 + Display2, true, man, marker);
            else
                Printer.PrintMan(Display1 + Display2 + " " + exponent, true, man, marker);
            OnPropertyChanged(nameof(Printer));
        }

        internal void StartProgram(ProgramEntry programEntry = null)
        {
            //var program = programEntry as ProgramModel;
            //if (program != null)
            //{
            //    this.runningProgram = program;
            //}
            //else
            //{
            //    var link = programEntry as ProgramLink;
            //    if (link != null)
            //    {
            //        runningProgram = link.GetProgram();
            //    }
            //}
            callStack.Clear();
            IsRunning = true;
            //if (_timer != null)
            //    this._timer.Start();
            OnPropertyChanged("Prgm");
        }

        internal void PrintProgram(ActionModel action = null)
        {
            //var program = action as ProgramModel;
            //if (program == null) return;
            //runningProgram = program;
            IsPrinting = true;
            programLine = 0;
            //programEndLine = runningProgram.Lines.Count;
            //Printer.PrintMan(" 01♦LBL \"" + program.TextD + "\"");
            //if (_timer != null)
            //    _timer.Start();
        }

        internal void ListProgram(int lines)
        {
            if (runningProgram == null) return;
            IsPrinting = true;
            //programEndLine = Math.Min(programLine + lines, runningProgram.Lines.Count);
            //if (_timer != null)
            //    _timer.Start();
        }

        internal void StopProgram()
        {
            IsRunning = false;
            OnPropertyChanged("Prgm");
        }

        internal void RunStop()
        {
            if (IsCatalog)
            {
                if (IsCatalogRunning)
                    StopCatalog();
                else
                    RestartCatalog();
            }
            else if (IsRunning)
                StopProgram();
            else if (IsPrinting)
            {
                IsPrinting = false;
            }
            else if (runningProgram != null)
            {
                IsPrompt = false;
                StartProgram();
            }
        }

        internal bool GoToLable(string lable)
        {
            return false;
        }

        internal void PuschCallStack(ProgramModel program, int step)
        {
            callStack.Add(new CallStackEntry(program, step));
        }

        private void FillStatisticRegister()
        {
            for (int index = 0; index < ((robustStatistic) ? 12 : 6); index++)
                _r[statisticRegister + index].Value = 0;
            this.X = this._r[this.statisticRegister + 5].Value = Values[0].Count;
            if (robustStatistic)
                this._r[this.statisticRegister + 6].Value = Values[1].Count;
            foreach (var value in Values[0])
            {
                _r[this.statisticRegister].Value += value;
                _r[this.statisticRegister + 1].Value += value * value;
            }
            if (robustStatistic)
                _r[statisticRegister + 7].Value = Sdev(_r[this.statisticRegister].Value, _r[this.statisticRegister + 1].Value, (int)_r[this.statisticRegister + 5].Value, false);
            foreach (var value in Values[1])
            {
                _r[this.statisticRegister + 2].Value += value;
                _r[this.statisticRegister + 3].Value += value * value;
            }
            if (robustStatistic)
                _r[statisticRegister + 8].Value = Sdev(_r[this.statisticRegister + 2].Value, _r[this.statisticRegister + 3].Value, (int)_r[this.statisticRegister + 6].Value, false);
            var max = Math.Min(Values[0].Count, Values[1].Count);
            var d2 = 0.0;
            for (int index = 0; index < max; index++)
            {
                _r[this.statisticRegister + 4].Value += Values[0][index] * Values[1][index];
                if (!robustStatistic) continue;
                _r[statisticRegister + 9].Value += (_r[this.statisticRegister].Value / _r[this.statisticRegister + 5].Value - Values[0][index]) * (_r[this.statisticRegister + 2].Value / _r[this.statisticRegister + 6].Value - Values[1][index]);
                _r[statisticRegister + 10].Value += Values[0][index] - Values[1][index];
                d2 += (Values[0][index] - Values[1][index]) * (Values[0][index] - Values[1][index]);
            }
            if (!robustStatistic) return;
            for (int index = 0; index < max; index++)
            {
                _r[statisticRegister + 11].Value += (Values[0][index] - Values[1][index] - _r[statisticRegister + 10].Value / max) * (Values[0][index] - Values[1][index] - _r[statisticRegister + 10].Value / max);
            }
            _r[statisticRegister + 11].Value = Sdev(_r[statisticRegister + 10].Value, d2, max, false);
            _r[statisticRegister + 9].Value /= max - 1;
            _r[statisticRegister + 10].Value /= max;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
