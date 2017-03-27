using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using HP41CV.Annotations;
using HP41CV.Helpers;
using HP41CV.Models;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private bool useTimer;

        private bool isInit = false;

        private TimeSpan timerSpeed = TimeSpan.FromMilliseconds(500);

        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorModel Model { get; private set; }

        private ButtonViewModel equalButtonViewModel;

        private ButtonViewModel spaceButtonViewModel;

        private int _afterDecimals;

        private bool _asn;

        private bool _asn1;

        private ActionModel _asnAction;

        private bool _compleate;

        private bool _compleateExp1;

        private bool _compleateExp2;

        private StringBuilder _display2;

        private string _display4;

        private bool _dot;

        private int _inputExValue;

        private long _inputValue;

        private StringBuilder foramula;

        private int numberPos;

        private Dictionary<int, KeyModel> _keyDictionary;

        private string _keyDisplay;

        private bool _minus;

        private bool _minusEx;

        private bool _select1;

        private bool _select2;

        private bool _select3;

        private string _selectDisplay;

        private bool _selectStack;

        private int _selectionValue;

        private bool _shift;

        private bool _showKey;

        private bool _xeq;

        private bool _prp;

        public double Width
        {
            get { return width; }
            set
            {
                if (value.Equals(width)) return;
                width = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                OnPropertyChanged();
            }
        }

        public Action ShowHelpAction;

        private bool isVisible;
        private double width;

        public string Foramula
        {
            get
            {
                return foramula.ToString();
            }
        }

        public string Display1
        {
            get
            {
                return (this.Model.IsFlagSet(48) || this._select1 || this.Model.IsFlagSet(50) || this.Model.IsFlagSet(56))
                           ? string.Empty
                           : this._compleate ? this.Model.Display1 : (this._minus) ? "-" : string.Empty;
            }
        }

        public string Display2
        {
            get
            {
                return (_showKey || _xeq || _asn || _prp)
                    ? this._keyDisplay
                    : this.Model.IsFlagSet(50)
                        ? this.Model.Display2
                        : (this._select1)
                            ? this._selectDisplay
                            : (this.Model.IsFlagSet(48))
                                ? this.Model.DisplayA
                                : (!this._compleate)
                                    ? (this._display2.Length > 12)
                                        ? this._display2.ToString().Substring(this._display2.Length - 12, 12)
                                        : this._display2.ToString()
                                    : Model.IsProgramActiv || Model.IsPrompt
                                        ? Model.DisplayP
                                        : this.Model.Display2;
            }
        }

        public string Display2Margin
        {
            get
            {
                return ((this.Model.IsFlagSet(56) || this.Model.IsFlagSet(57)) && this.Model.Precision == 8) ? "0,10,10,0" : "44,10,10,0";
            }
        }

        public string Display3
        {
            get
            {
                return (this._select1 || this.Model.IsFlagSet(48) || this.Model.IsFlagSet(50) || this.Model.IsFlagSet(57))
                           ? string.Empty
                           : this._compleate ? this.Model.Display3 : (this._minusEx) ? "-" : string.Empty;
            }
        }

        public string Display4
        {
            get
            {
                return (this._select1 || this.Model.IsFlagSet(48) || this.Model.IsFlagSet(50) || this.Model.IsFlagSet(57))
                           ? string.Empty
                           : this._compleate ? this.Model.Display4 : this._display4;
            }
        }

        public string DisplayC1
        {
            get
            {
                return this._select3 ? "___" : this._select2 ? "__" : (this._select1 || (!this._compleate && this._compleateExp1)) ? "_" : string.Empty;
            }
        }

        public string DisplayC2
        {
            get
            {
                return (this._select1 || this._compleateExp1) ? string.Empty : "_";
            }
        }

        private bool IsBin()
        {
            return this.Model.IsFlagSet(57) && !this._showKey && !this.Model.IsFlagSet(50) && !this.Model.IsFlagSet(48) && this._compleate && !this._select1;
        }

        private bool Asn
        {
            get
            {
                return this._asn;
            }

            set
            {
                this._asn = value;
                if (this.Model.IsFlagSet(57))
                {
                    OnPropertyChanged("BinVisible");
                    OnPropertyChanged("NumVisible");
                }
            }
        }

        public void GetValue(bool compleate = true)
        {
            double value = this._inputValue;
            for (var i = 0; i < this._afterDecimals; i++)
            {
                value /= 10;
            }

            if (this._minus)
            {
                value *= -1;
            }

            if (this._inputExValue > 0)
            {
                if (this._minusEx)
                {
                    this._inputExValue *= -1;
                }

                value *= Math.Pow(10, this._inputExValue);
            }

            if (compleate)
            {
                if (!Compleate)
                {
                    Model.X = value;
                    Compleate = true;
                }
            }
            else
            {
                if (Compleate && !Model.NoStackLift)
                    Model.StackDown(value);
                else
                    Model.X = value;
            }


        }

        private bool Compleate
        {
            get
            {
                return this._compleate;
            }

            set
            {
                this.Model.ClearFlag(50);
                this._compleate = value;
                if (this._compleate)
                {
                    Minus = false;
                    MinusEx = false;
                    this._inputValue = 0;
                    this._inputExValue = 0;
                    this._display4 = string.Empty;
                    this._afterDecimals = 0;
                    this._dot = false;
                }

                this._compleateExp1 = true;
                OnPropertyChanged(nameof(DisplayC1));
                OnPropertyChanged(nameof(DisplayC2));
                if (this.Model.IsFlagSet(57))
                {
                    OnPropertyChanged("BinVisible");
                    OnPropertyChanged("NumVisible");
                }
            }
        }

        private bool CompleateExp
        {
            get
            {
                return this._compleateExp1;
            }

            set
            {
                this._compleateExp1 = value;
                if (!this._compleateExp1)
                {
                    this._compleateExp2 = false;
                }

                OnPropertyChanged(nameof(DisplayC1));
                OnPropertyChanged(nameof(DisplayC2));
                if (this.Model.IsFlagSet(57))
                {
                    OnPropertyChanged("BinVisible");
                    OnPropertyChanged("NumVisible");
                }
            }
        }

        private bool Minus
        {
            set
            {
                this._minus = value;
                OnPropertyChanged(nameof(Display1));
            }
        }

        private bool MinusEx
        {
            set
            {
                this._minusEx = value;
                OnPropertyChanged(nameof(Display3));
            }
        }

        private bool Select
        {
            set
            {
                if (value)
                {
                    this._selectionValue = 0;
                    this._select1 = true;
                }
                else
                {
                    this._select1 = false;
                    this._select2 = false;
                    this._select3 = false;
                }

                this._selectStack = false;
                this.Model.ClearFlag(50);
                OnPropertyChanged(nameof(DisplayC1));
                OnPropertyChanged(nameof(Display1));
                OnPropertyChanged(nameof(Display2));
                if (this.Model.IsFlagSet(57))
                {
                    OnPropertyChanged("BinVisible");
                    OnPropertyChanged("NumVisible");
                }
            }
        }

        private bool Select2
        {
            get
            {
                return this._select2;
            }

            set
            {
                this._select2 = value;
                OnPropertyChanged(nameof(DisplayC1));
                OnPropertyChanged(nameof(Display2));
            }
        }

        private bool Select3
        {
            get
            {
                return this._select3;
            }

            set
            {
                this._select3 = value;
                OnPropertyChanged(nameof(DisplayC1));
                OnPropertyChanged(nameof(Display2));
            }
        }

        public bool Shift
        {
            get
            {
                return this._shift;
            }

            set
            {
                this._shift = value;
                OnPropertyChanged("ShiftVisibel");
            }
        }

        public bool AlphaVisibel => !isInit || this.Model.IsFlagSet(48);
        public bool Flag0Visibel => !isInit || this.Model.IsFlagSet(0);
        public bool Flag1Visibel => !isInit || this.Model.IsFlagSet(1);
        public bool Flag2Visibel => !isInit || this.Model.IsFlagSet(2);
        public bool Flag3Visibel => !isInit || this.Model.IsFlagSet(3);
        public bool Flag4Visibel => !isInit || this.Model.IsFlagSet(4);

        public bool GradVisibel => !isInit || this.Model.IsFlagSet(42);
        public bool NumVisible => !isInit || !IsBin();
        public bool ProgVisibel => !isInit || this.Model.IsFlagSet(52);
        public bool RadVisibel => !isInit || this.Model.IsFlagSet(43);
        public bool ShiftVisibel => !isInit || this._shift;
        public bool UserVisibel => !isInit || this.Model.IsFlagSet(27);
        public bool BinVisible => !isInit || IsBin();
        public bool FormulaVisible => !isInit || Model.IsUpn;
        public bool Byte2Visible => !isInit || (IsBin() && Model.Precision >= 2);
        public bool Byte3Visible => !isInit || (IsBin() && Model.Precision >= 3);
        public bool Byte4Visible => !isInit || (IsBin() && Model.Precision >= 4);

        public Collection<ToggleViewModel> Toggles { get; private set; }
        public Collection<ButtonViewModel> Buttons { get; private set; }

        public Color[] Bits { get; private set; }


        public CalculatorViewModel() : this(new CalculatorModel())
        {
        }

        public CalculatorViewModel(CalculatorModel model, bool useTimer = true)
        {
            Width = 440;
            Model = model;
            Model.PropertyChanged += ModelOnPropertyChanged;
            SetInvokeActions();
            CreateButtons();
            AssignKeys();
            AssignKeysButtonDictionary();

            Bits = new Color[32];

            this._display2 = new StringBuilder("0.0000");
            foramula = new StringBuilder(string.Empty);
            numberPos = 0;

            this._compleate = true;
            this._compleateExp1 = true;
            this._inputValue = 0;
            this._inputExValue = 0;
            this._afterDecimals = 0;

            this._dot = false;
            this._shift = false;
            this._minus = false;
            this._minusEx = false;
            Select = false;
            this._select2 = false;
            this._select3 = false;
            this._selectStack = false;

            Toggles = new Collection<ToggleViewModel>();
            Toggles.Add(new ToggleViewModel("HELP",() => {ShowHelpAction?.Invoke();}));
            Toggles.Add(new ToggleViewModel("USER",
                () =>
                {
                    if (this.Model.IsFlagSet(27))
                    {
                        this.Model.ClearFlag(27);
                    }
                    else
                    {
                        this.Model.SetFlag(27);
                    }
                }));
            Toggles.Add(new ToggleViewModel("PRGM", () => {Messenger.ShowDialogAsync(AppResources.NotLight);}));
            Toggles.Add(new ToggleViewModel("ALPHA",
                () =>
                {
                    if (this.Model.IsFlagSet(48))
                    {
                        this.Model.ClearFlag(48);
                    }
                    else
                    {
                        this.Model.SetFlag(48);
                    }
                }));
            DisplayChanged();
        }

        private void CreateButtons()
        {
            Buttons = new Collection<ButtonViewModel>();
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x01,
                    0x09,
                    new KeyModel(this.Model.GetActionModel("Σ+"), this.Model.GetActionModel("Σ-")),
                    new KeyModel(this.Model.GetActionModel("ᵀA"), this.Model.GetActionModel("ᵀa"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x11,
                    0x19,
                    new KeyModel(this.Model.GetActionModel("1/X"), this.Model.GetActionModel("Y↑X")),
                    new KeyModel(this.Model.GetActionModel("ᵀB"), this.Model.GetActionModel("ᵀb"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x21,
                    0x29,
                    new KeyModel(this.Model.GetActionModel("SQRT"), this.Model.GetActionModel("X↑2")),
                    new KeyModel(this.Model.GetActionModel("ᵀC"), this.Model.GetActionModel("ᵀc"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x31,
                    0x39,
                    new KeyModel(this.Model.GetActionModel("LOG"), this.Model.GetActionModel("10↑X")),
                    new KeyModel(this.Model.GetActionModel("ᵀD"), this.Model.GetActionModel("ᵀd"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x41,
                    0x49,
                    new KeyModel(this.Model.GetActionModel("LN"), this.Model.GetActionModel("E↑X")),
                    new KeyModel(this.Model.GetActionModel("ᵀE"), this.Model.GetActionModel("ᵀe"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x02,
                    0x0A,
                    new KeyModel(this.Model.GetActionModel("X<>Y"), this.Model.GetActionModel("CLΣ")),
                    new KeyModel(this.Model.GetActionModel("ᵀF"), this.Model.GetActionModel("ᵀΣ"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x12,
                    0x1A,
                    new KeyModel(this.Model.GetActionModel("R↓"), this.Model.GetActionModel("%")),
                    new KeyModel(this.Model.GetActionModel("ᵀG"), this.Model.GetActionModel("ᵀ%"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x22,
                    0x2A,
                    new KeyModel(this.Model.GetActionModel("SIN"), this.Model.GetActionModel("ASIN")),
                    new KeyModel(this.Model.GetActionModel("ᵀH"), this.Model.GetActionModel("ᵀ≠"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x32,
                    0x3A,
                    new KeyModel(this.Model.GetActionModel("COS"), this.Model.GetActionModel("ACOS")),
                    new KeyModel(this.Model.GetActionModel("ᵀI"), this.Model.GetActionModel("ᵀ<"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x42,
                    0x4A,
                    new KeyModel(this.Model.GetActionModel("TAN"), this.Model.GetActionModel("ATAN")),
                    new KeyModel(this.Model.GetActionModel("ᵀJ"), this.Model.GetActionModel("ᵀ>"))));
            Buttons.Add(new ShiftButtonViewModel(this, 0x03, 0x0B));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x13,
                    0x1B,
                    new KeyModel(this.Model.GetActionModel("XEQ"), this.Model.GetActionModel("ASN")),
                    new KeyModel(this.Model.GetActionModel("ᵀK"), this.Model.GetActionModel("APPEND"))));
            Buttons.
                Add(
                new ButtonViewModel(
                    this,
                    0x23,
                    0x2B,
                    new KeyModel(this.Model.GetActionModel("STO"), this.Model.GetActionModel("LBL")),
                    new KeyModel(this.Model.GetActionModel("ᵀL"), this.Model.GetActionModel("ASTO"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x33,
                    0x3B,
                    new KeyModel(this.Model.GetActionModel("RCL"), this.Model.GetActionModel("GTO")),
                    new KeyModel(this.Model.GetActionModel("ᵀM"), this.Model.GetActionModel("ARCL"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x43,
                    0x4B,
                    new KeyModel(this.Model.GetActionModel("SST"), this.Model.GetActionModel("BST")),
                    new KeyModel(this.Model.GetActionModel("SST"), this.Model.GetActionModel("BST"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x04,
                    0x0C,
                    new KeyModel(this.Model.GetActionModel("ENTER↑"), this.Model.GetActionModel("CATALOG")),
                    new KeyModel(this.Model.GetActionModel("ᵀN"), this.Model.GetActionModel("ᵀ↑")))
                {
                    Width = 120,
                });
            equalButtonViewModel = Buttons[Buttons.Count - 1];
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x24,
                    0x2C,
                    new KeyModel(this.Model.GetActionModel("CHS"), this.Model.GetActionModel("ISG")),
                    new KeyModel(this.Model.GetActionModel("ᵀO"), this.Model.GetActionModel("ᵀΔ"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x34,
                    0x3C,
                    new KeyModel(this.Model.GetActionModel("EEX"), this.Model.GetActionModel("RTN")),
                    new KeyModel(this.Model.GetActionModel("ᵀP"), this.Model.GetActionModel("ᵀS"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x44,
                    0x4C,
                    new KeyModel(this.Model.GetActionModel("←"), this.Model.GetActionModel("CLX")),
                    new KeyModel(this.Model.GetActionModel("←"), this.Model.GetActionModel("CLA"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x05,
                    0x0D,
                    new KeyModel(this.Model.GetActionModel("-"), this.Model.GetActionModel("X=Y?")),
                    new KeyModel(this.Model.GetActionModel("ᵀQ"), this.Model.GetActionModel("ᵀ-"))));
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x15,
                    0x1D,
                    new KeyModel(this.Model.GetActionModel("7"), this.Model.GetActionModel("SF")),
                    new KeyModel(this.Model.GetActionModel("ᵀR"), this.Model.GetActionModel("ᵀ7")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x25,
                    0x2D,
                    new KeyModel(this.Model.GetActionModel("8"), this.Model.GetActionModel("CF")),
                    new KeyModel(this.Model.GetActionModel("ᵀS"), this.Model.GetActionModel("ᵀ8")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x35,
                    0x3D,
                    new KeyModel(this.Model.GetActionModel("9"), this.Model.GetActionModel("FS?")),
                    new KeyModel(this.Model.GetActionModel("ᵀT"), this.Model.GetActionModel("ᵀ9")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x06,
                    0x0E,
                    new KeyModel(this.Model.GetActionModel("+"), this.Model.GetActionModel("X≤Y?")),
                    new KeyModel(this.Model.GetActionModel("ᵀU"), this.Model.GetActionModel("ᵀ+")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x16,
                    0x1E,
                    new KeyModel(this.Model.GetActionModel("4"), this.Model.GetActionModel("BEEP")),
                    new KeyModel(this.Model.GetActionModel("ᵀV"), this.Model.GetActionModel("ᵀ4")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x26,
                    0x2E,
                    new KeyModel(this.Model.GetActionModel("5"), this.Model.GetActionModel("P→R")),
                    new KeyModel(this.Model.GetActionModel("ᵀW"), this.Model.GetActionModel("ᵀ5")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x36,
                    0x3E,
                    new KeyModel(this.Model.GetActionModel("6"), this.Model.GetActionModel("R→P")),
                    new KeyModel(this.Model.GetActionModel("ᵀX"), this.Model.GetActionModel("ᵀ6")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x07,
                    0x0F,
                    new KeyModel(this.Model.GetActionModel("x"), this.Model.GetActionModel("X>Y?")),
                    new KeyModel(this.Model.GetActionModel("ᵀY"), this.Model.GetActionModel("ᵀ*")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x17,
                    0x1F,
                    new KeyModel(this.Model.GetActionModel("1"), this.Model.GetActionModel("FIX")),
                    new KeyModel(this.Model.GetActionModel("ᵀZ"), this.Model.GetActionModel("ᵀ1")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x27,
                    0x2F,
                    new KeyModel(this.Model.GetActionModel("2"), this.Model.GetActionModel("SCI")),
                    new KeyModel(this.Model.GetActionModel("ᵀ="), this.Model.GetActionModel("ᵀ2")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x37,
                    0x3F,
                    new KeyModel(this.Model.GetActionModel("3"), this.Model.GetActionModel("ENG")),
                    new KeyModel(this.Model.GetActionModel("ᵀ?"), this.Model.GetActionModel("ᵀ3")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x08,
                    0x10,
                    new KeyModel(this.Model.GetActionModel("÷"), this.Model.GetActionModel("X=0?")),
                    new KeyModel(this.Model.GetActionModel("ᵀ:"), this.Model.GetActionModel("ᵀ/")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x18,
                    0x20,
                    new KeyModel(this.Model.GetActionModel("0"), this.Model.GetActionModel("PI")),
                    new KeyModel(this.Model.GetActionModel("ᵀSPACE"), this.Model.GetActionModel("ᵀ0")))
                );
            spaceButtonViewModel = Buttons[Buttons.Count - 1];
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x28,
                    0x30,
                    new KeyModel(this.Model.GetActionModel("."), this.Model.GetActionModel("LASTX")),
                    new KeyModel(this.Model.GetActionModel("ᵀ,"), this.Model.GetActionModel("ᵀ.")))
                );
            Buttons.Add(
                new ButtonViewModel(
                    this,
                    0x38,
                    0x40,
                    new KeyModel(this.Model.GetActionModel("R/S"), this.Model.GetActionModel("VIEW")),
                    new KeyModel(this.Model.GetActionModel("R/S"), this.Model.GetActionModel("AVIEW")))
                );
            this._keyDictionary = new Dictionary<int, KeyModel>();
            foreach (var buttonViewModel in Buttons)
            {
                this._keyDictionary.Add(buttonViewModel.IndexMain, buttonViewModel.MainModel);
            }
        }

        private void SetInvokeActions()
        {
            var actionModel = this.Model.GetActionModel("EEX") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction(
                    (o) =>
                    {
                        if (this.Model.IsFlagSet(56))
                        {
                            Messenger.ShowDialogAsync(AppResources.NotHex);
                        }
                        else
                        {
                            if (this._compleate)
                            {
                                this._inputValue = 1;
                                GetValue(false);
                                Compleate = false;
                            }

                            CompleateExp = false;
                            if (!Model.IsUpn)
                            {
                                foramula.Append("E");
                                OnPropertyChanged(nameof(Foramula));
                            }
                        }
                    });
            }

            actionModel = this.Model.GetActionModel("ENTER↑") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction(
                    (o) =>
                    {
                        if (Model.IsUpn)
                        {
                            if (!Compleate)
                            {
                                GetValue();
                            }
                            this.Model.StackDown(this.Model.X);
                            Model.NoStackLift = true;
                        }
                        else
                        {
                            GetValue();
                            //if (Model.Equals())
                            //    foramula.Append(" = ");
                            OnPropertyChanged("Foramula");
                        }

                        return;
                    });
            }

            actionModel = this.Model.GetActionModel("APPEND") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction(
                    (o) =>
                    {
                        Model.SetFlag(23);
                    });
            }

            actionModel = this.Model.GetActionModel("ASN") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction( DoAsn);
            }

            actionModel = this.Model.GetActionModel("XEQ") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction( DoXeq);
            }

            actionModel = this.Model.GetActionModel("PRP") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction(DoPrp);
            }

            actionModel = this.Model.GetActionModel("CHS") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction(DoChs);
            }

            actionModel = this.Model.GetActionModel("←") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction( DoBack);
            }

            actionModel = this.Model.GetActionModel(".") as InvokeActionModel;
            if (actionModel != null)
            {
                actionModel.SetInvokeAction( AddDot);
            }

            foreach (var actionName in ActionModels.GetActions((m) => m.GetType() == typeof(AlphaActionModel)))
            {
                actionModel = this.Model.GetActionModel('ᵀ' + actionName) as AlphaActionModel;
                if (actionModel != null)
                {
                    actionModel.SetInvokeAction(AddAlpha);
                }
            }
            for (var i = 0; i < 16; i++)
            {
                actionModel = this.Model.GetActionModel((i < 10) ? i.ToString() : ((char)('A' + i - 10)).ToString()) as InvokeActionModel;
                if (actionModel != null)
                {
                    actionModel.SetInvokeAction(Digit);
                }
            }
        }

        private void UpdateKeys()
        {
            bool shift;
            foreach (var keyAssignmentModel in this.Model.GetKeyAssignments())
            {
                var button = GetButton(keyAssignmentModel.Button, out shift);
                if (button != null)
                {
                    button.SetAction(keyAssignmentModel.Action, shift, Model.IsFlagSet(48), Model.IsFlagSet(27));
                }
            }
        }

        private ButtonViewModel GetButton(int index, out bool shift)
        {
            foreach (var buttonViewModel in Buttons)
            {
                if (buttonViewModel.IndexMain == index)
                {
                    shift = false;
                    return buttonViewModel;
                }
                if (buttonViewModel.IndexShift == index)
                {
                    shift = true;
                    return buttonViewModel;
                }
            }
            shift = false;
            return null;
        }

        private void AssignKeys()
        {
            foreach (var keyAssignmentModel in this.Model.GetKeyAssignments())
            {
                var button = keyAssignmentModel.Button;
                var shift = ((button % 16) == 0) || ((button % 16) > 8);
                if (shift)
                {
                    button -= 8;
                }

                if (button >= 0)
                {
                    var help = this._keyDictionary[button];
                    if (shift)
                    {
                        help.UserShiftAction = keyAssignmentModel.Action;
                    }
                    else
                    {
                        help.UserMainAction = keyAssignmentModel.Action;
                    }

                    foreach (var buttonViewModel in Buttons)
                    {
                        buttonViewModel.SetAlpha(this.Model.IsFlagSet(48), this.Model.IsFlagSet(27));
                    }
                }
            }
        }

        private void AssignKeysButtonDictionary()
        {
            //    for (int i = 0; i < 10; i++)
            //        _keyButtonDictionary.Add(VirtualKey.A + i, Buttons[i]);
            //    _keyButtonDictionary.Add(VirtualKey.K, Buttons[11]);
            //    _keyButtonDictionary.Add(VirtualKey.L, Buttons[12]);
            //    _keyButtonDictionary.Add(VirtualKey.M, Buttons[13]);
            //    _keyButtonDictionary.Add(VirtualKey.N, Buttons[15]);
            //    _keyButtonDictionary.Add(VirtualKey.O, Buttons[16]);
            //    _keyButtonDictionary.Add(VirtualKey.P, Buttons[17]);
            //    _keyButtonDictionary.Add(VirtualKey.Back, Buttons[18]);
            //    for (int i = 19; i < 29; i++)
            //        _keyButtonDictionary.Add(VirtualKey.Q + i - 19, Buttons[i]);
            //    _keyButtonDictionary.Add(VirtualKey.Number0, Buttons[32]);
            //    _keyButtonDictionary.Add(VirtualKey.Number1, Buttons[28]);
            //    _keyButtonDictionary.Add(VirtualKey.Number2, Buttons[29]);
            //    _keyButtonDictionary.Add(VirtualKey.Number3, Buttons[30]);
            //    _keyButtonDictionary.Add(VirtualKey.Number4, Buttons[24]);
            //    _keyButtonDictionary.Add(VirtualKey.Number5, Buttons[25]);
            //    _keyButtonDictionary.Add(VirtualKey.Number6, Buttons[26]);
            //    _keyButtonDictionary.Add(VirtualKey.Number7, Buttons[20]);
            //    _keyButtonDictionary.Add(VirtualKey.Number8, Buttons[21]);
            //    _keyButtonDictionary.Add(VirtualKey.Number9, Buttons[22]);
            //    _keyButtonDictionary.Add(VirtualKey.Space, Buttons[32]);
            //    _keyButtonDictionary.Add(VirtualKey.Add, Buttons[23]);
            //    _keyButtonDictionary.Add(VirtualKey.Subtract, Buttons[19]);
            //    _keyButtonDictionary.Add((VirtualKey)189, Buttons[19]);
            //    _keyButtonDictionary.Add((VirtualKey)190, Buttons[31]);
            //    _keyButtonDictionary.Add(VirtualKey.Divide, Buttons[31]);
            //    _keyButtonDictionary.Add(VirtualKey.Multiply, Buttons[27]);
            //    _keyButtonDictionary.Add((VirtualKey)188, Buttons[33]);
            //    _keyButtonDictionary.Add(VirtualKey.Decimal, Buttons[33]);
            //    _keyButtonDictionary.Add((VirtualKey)19, Buttons[29]);
            //    _keyButtonDictionary.Add((VirtualKey)219, Buttons[30]);
        }

        public async void PerformKey(KeyModel key, int index, int indexShif)
        {
            //var action = key.GetAction(Shift, this.Model.IsFlagSet(27));
            //action?.Execute();
            OnPropertyChanged(nameof(Display2));
            OnPropertyChanged("BinVisible");
            OnPropertyChanged("NumVisible");
            //if (!this._abortKey)
            {
                //timerActive = false;
                if (this.Model.IsCatalog)
                {
                    if (index == 0x38)
                    {
                        if (this.Model.IsCatalogRunning)
                        {
                            this.Model.StopCatalog();
                        }
                        else
                        {
                            this.Model.RestartCatalog();
                        }

                        return;
                    }

                    if (this.Model.IsCatalogRunning)
                    {
                        this.Model.ChangeCatalogSpeed(false);
                        return;
                    }
                    if (index != 0x43)
                    {
                        Model.EndCatalog();
                    }
                }

                if (this._asn && !this.Model.IsFlagSet(48))
                {
                    if (this._asn1)
                    {
                        return;
                    }

                    if (Shift)
                    {
                        key.UserShiftAction = this._asnAction;
                    }
                    else
                    {
                        key.UserMainAction = this._asnAction;
                    }

                    this.Model.AddKeyAssignment(this._asnAction, Shift ? index + 8 : index);
                    foreach (var buttonViewModel in Buttons)
                    {
                        buttonViewModel.SetAlpha(this.Model.IsFlagSet(48), this.Model.IsFlagSet(27));
                    }

                    Asn = false;
                    OnPropertyChanged(nameof(Display2));
                    return;
                }

                if (this._selectStack)
                {
                    if (index == 0x36)
                    {
                        this._selectionValue = this.Model.MaxRegister + 1;
                    }
                    else if (index == 0x07)
                    {
                        this._selectionValue = this.Model.MaxRegister + 2;
                    }
                    else if (index == 0x17)
                    {
                        this._selectionValue = this.Model.MaxRegister + 3;
                    }
                    else if (index == 0x35)
                    {
                        this._selectionValue = this.Model.MaxRegister + 4;
                    }
                    else if (index == 0x23)
                    {
                        this._selectionValue = this.Model.MaxRegister + 5;
                    }
                    else
                    {
                        return;
                    }

                    Select = false;
                    this.Model.SelectAction.Execute(this._selectionValue);
                }
                else
                {
                    if (!Select2 || !SpecialSelect(key, index))
                    {
                        var newInput = !Compleate;

                        var viewModelaction = key.GetAction(Shift, this.Model.IsFlagSet(27)) as InvokeActionModel;
                        if (viewModelaction == null)
                        {
                            GetValue();
                            if (newInput)
                                Model.Prx(false, false);
                        }

                        var action = key.GetAction(Shift, this.Model.IsFlagSet(27));
                        if (action.Text == "ENTER↑")
                        {
                            action.Execute();
                            if (newInput)
                                Model.Prx(false, false);
                            Model.Printer.Print(action.Text, PrinterModel.Mode.NORM, true);
                        }
                        else
                        {
                            if (action.IsFunction && (action.Text != "XEQ"))
                                Model.Printer.Print(action.Text, PrinterModel.Mode.NORM, true);
                            action.Execute();
                            if (action.HasResult)
                                Model.Prx(false);
                        }
                    }
                }

                Shift = false;
            }
        }

        private bool Xeq
        {
            get
            {
                return this._xeq;
            }

            set
            {
                this._xeq = value;
                if (this.Model.IsFlagSet(57))
                {
                    OnPropertyChanged("BinVisible");
                    OnPropertyChanged("NumVisible");
                }
            }
        }

        private bool SpecialSelect(KeyModel key, int index)
        {
            if (index == 0x01)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 1", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(1);
                return true;
            }

            if (index == 0x11)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 2", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(2);
                return true;
            }

            if (index == 0x21)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 3", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(3);
                return true;
            }

            if (index == 0x31)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 4", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(4);
                return true;
            }

            if (index == 0x41)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 5", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(5);
                return true;
            }

            if (index == 0x02)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 6", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(6);
                return true;
            }

            if (index == 0x12)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 7", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(7);
                return true;
            }

            if (index == 0x22)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 8", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(8);
                return true;
            }

            if (index == 0x32)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 9", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(9);
                return true;
            }

            if (index == 0x42)
            {
                Select = false;
                this.Model.Printer.Print(Model.SelectAction.Text + " 10", PrinterModel.Mode.NORM, true);
                this.Model.SelectAction.Execute(10);
                return true;
            }

            if (key.GetAction(Shift, this.Model.IsFlagSet(27)).TextD == "+")
            {
                if (Model.SelectAction.Text == "STO")
                    Model.SelectAction = (SelectActionModel)this.Model.GetActionModel("STO +");
                return true;
            }

            if (key.GetAction(Shift, this.Model.IsFlagSet(27)).TextD == "-")
            {
                if (Model.SelectAction.Text == "STO")
                    this.Model.SelectAction = (SelectActionModel)this.Model.GetActionModel("STO -");
                return true;
            }

            if (key.GetAction(Shift, this.Model.IsFlagSet(27)).TextD == "x")
            {
                if (Model.SelectAction.Text == "STO")
                    this.Model.SelectAction = (SelectActionModel)this.Model.GetActionModel("STO x");
                return true;
            }

            if (key.GetAction(Shift, this.Model.IsFlagSet(27)).TextD == "/")
            {
                if (Model.SelectAction.Text == "STO")
                    this.Model.SelectAction = (SelectActionModel)this.Model.GetActionModel("STO /");
                return true;
            }

            return false;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Display1")
            {
                OnPropertyChanged(nameof(Display1));
            }
            else if (e.PropertyName == "Display2")
            {
                OnPropertyChanged(nameof(Display2));
            }
            else if (e.PropertyName == "Display3")
            {
                OnPropertyChanged(nameof(Display3));
            }
            else if (e.PropertyName == "Display4")
            {
                OnPropertyChanged(nameof(Display4));
            }
            else if (e.PropertyName == "Flag0")
            {
                OnPropertyChanged(nameof(Flag0Visibel));
            }
            else if (e.PropertyName == "Flag1")
            {
                OnPropertyChanged(nameof(Flag1Visibel));
            }
            else if (e.PropertyName == "Flag2")
            {
                OnPropertyChanged(nameof(Flag2Visibel));
            }
            else if (e.PropertyName == "Flag3")
            {
                OnPropertyChanged(nameof(Flag3Visibel));
            }
            else if (e.PropertyName == "Flag4")
            {
                OnPropertyChanged(nameof(Flag4Visibel));
            }
            else if (e.PropertyName == "Flag50")
            {
                DisplayChanged();
            }
            else if (e.PropertyName == "Rad")
            {
                OnPropertyChanged("RadVisibel");
            }
            else if (e.PropertyName == "Grad")
            {
                OnPropertyChanged("GradVisibel");
            }
            else if (e.PropertyName == "User")
            {
                DoUser();
            }
            else if (e.PropertyName == "Prgm")
            {
                DoPrgm();
            }
            else if (e.PropertyName == "Alpha")
            {
                DoAlpha();
            }
            else if ((e.PropertyName == "SelectAction") && (this.Model.SelectAction != null))
            {
                if (!Compleate)
                {
                    GetValue();
                }

                this._selectDisplay = this.Model.SelectAction.Text;
                Select3 = this.Model.SelectAction.Select3;
                Select2 = this.Model.SelectAction.Select2;
                Select = true;
            }
            else if (e.PropertyName == "Hex")
            {
                OnPropertyChanged("Display2Margin");
            }
            else if (e.PropertyName == "Bin")
            {
                OnPropertyChanged("Display2Margin");
                OnPropertyChanged("Foramula");
                OnPropertyChanged("NumVisible");
                OnPropertyChanged(nameof(BinVisible));
            }
            else if (e.PropertyName == "KeyHex")
            {
                UpdateKeys();
            }
            else if (e.PropertyName == "Bytes")
            {
                var help = new [] { (int)Math.Floor(this.Model.X) } ;
                var bits = new BitArray(help);
                for (var i = 0; i < 32; i++)
                    Bits[i] = bits[i] ? Color.Black : Color.White;
                //for (var i = 0; i < this.Model.Precision; i++)
                //{
                //    var viewModel = new ByteViewModel((byte) (help & 0xFF), i + 1, this.Model.Precision > 4);
                //    Bytes[i] = viewModel;
                //    help = help >> 8;
                //}
                OnPropertyChanged(nameof(Bits));
                OnPropertyChanged(nameof(Byte2Visible));
                OnPropertyChanged(nameof(Byte3Visible));
                OnPropertyChanged(nameof(Byte4Visible));
            }
            else if (e.PropertyName == "IsUpn")
            {
                this.Model.GetActionModel("ENTER↑").Text = (Model.IsUpn) ? "ENTER↑" : "=";
                equalButtonViewModel.SetAlpha(this.Model.IsFlagSet(48), this.Model.IsFlagSet(27));
                OnPropertyChanged("FormulaVisible");
            }
            else if (e.PropertyName == "Foramula")
            {
                foramula = new StringBuilder(Model.Foramula);
                numberPos = Model.Foramula.Length;
                OnPropertyChanged("Foramula");
            }
            else if (e.PropertyName == "Keys")
            {
                UpdateKeys();
            }
            else if (e.PropertyName == "ProgramStep")
            {
                DisplayChanged();
            }
            //else if (e.PropertyName == "Aview")
            //{
            //    if (useTimer)
            //        Device.StartTimer(timerSpeed,);
            //}
            else if (e.PropertyName == "Reset")
            {
                isInit = true;
                UpdateKeys();
                this.Model.GetActionModel("ENTER↑").Text = (Model.IsUpn) ? "ENTER↑" : "=";
                equalButtonViewModel.SetAlpha(this.Model.IsFlagSet(48), this.Model.IsFlagSet(27));
                OnPropertyChanged("FormulaVisible");
                OnPropertyChanged(nameof(Display1));
                OnPropertyChanged(nameof(Display2));
                OnPropertyChanged(nameof(Display3));
                OnPropertyChanged(nameof(Display4));
                OnPropertyChanged(nameof(Flag0Visibel));
                OnPropertyChanged(nameof(Flag1Visibel));
                OnPropertyChanged(nameof(Flag2Visibel));
                OnPropertyChanged(nameof(Flag3Visibel));
                OnPropertyChanged(nameof(Flag4Visibel));
                OnPropertyChanged(nameof(RadVisibel));
                OnPropertyChanged(nameof(GradVisibel));
                OnPropertyChanged(nameof(UserVisibel));
                OnPropertyChanged(nameof(AlphaVisibel));
                OnPropertyChanged(nameof(ProgVisibel));
                OnPropertyChanged(nameof(ShiftVisibel));
            }
        }

        private void DisplayChanged()
        {
            OnPropertyChanged(nameof(Display1));
            OnPropertyChanged(nameof(Display2));
            OnPropertyChanged(nameof(Display3));
            OnPropertyChanged(nameof(Display4));
            OnPropertyChanged(nameof(DisplayC1));
            OnPropertyChanged(nameof(DisplayC2));
            if (!Model.IsUpn)
                OnPropertyChanged(nameof(Foramula));
            OnPropertyChanged(nameof(Flag0Visibel));
            OnPropertyChanged(nameof(Flag1Visibel));
            OnPropertyChanged(nameof(Flag2Visibel));
            OnPropertyChanged(nameof(Flag3Visibel));
            OnPropertyChanged(nameof(RadVisibel));
            OnPropertyChanged(nameof(GradVisibel));
            OnPropertyChanged(nameof(UserVisibel));
            OnPropertyChanged(nameof(AlphaVisibel));
            OnPropertyChanged(nameof(ProgVisibel));
            OnPropertyChanged(nameof(ShiftVisibel));
            OnPropertyChanged(nameof(BinVisible));
        }

        private void DoPrgm()
        {
            if (!MarketPlace.IsAvtivated("BaseProgramming"))
            {
                Messenger.ShowDialogAsync(AppResources.NotLight);
                return;
            }

            DisplayChanged();
            OnPropertyChanged(nameof(UserVisibel));
            OnPropertyChanged(nameof(AlphaVisibel));
            OnPropertyChanged(nameof(ProgVisibel));
        }

        private void DoUser()
        {
            if (this.Model.IsFlagSet(27))
            {
                this.Model.ClearFlag(48);
            }
            else
            {
                ;
            }

            foreach (var buttonViewModel in Buttons)
            {
                buttonViewModel.SetAlpha(this.Model.IsFlagSet(48), this.Model.IsFlagSet(27));
            }

            DisplayChanged();
            OnPropertyChanged(nameof(UserVisibel));
            OnPropertyChanged(nameof(AlphaVisibel));
        }

        internal void Digit(object value)
        {
            int digit = (int) value;
            if (this._select1)
            {
                if (this._select3)
                {
                    this._selectionValue = digit;
                    this._selectDisplay += "  " + this._selectionValue;
                    Select3 = false;
                }
                else if (this._select2)
                {
                    this._selectionValue *= 10;
                    this._selectionValue += digit;
                    this._selectDisplay += " " + this._selectionValue;
                    Select2 = false;
                }
                else
                {
                    this._selectionValue *= 10;
                    this._selectionValue += digit;
                    Select = false;
//                    this.Model.Printer.Print(Model.SelectAction.Text + " " + _selectionValue, PrinterModel.Mode.NORM, true);
                    this.Model.SelectAction.Execute(this._selectionValue);
                }
            }
            else
            {
                char help;
                if (digit < 10)
                {
                    help = (char)('0' + digit);
                }
                else if (this.Model.IsFlagSet(56))
                {
                    help = (char)('A' + digit - 10);
                }
                else
                {
                    return;
                }



                if (CompleateExp)
                {
                    if (this._dot)
                    {
                        this._afterDecimals++;
                    }

                    if (this.Model.IsFlagSet(56))
                    {
                        this._inputValue *= 16;
                    }
                    else
                    {
                        this._inputValue *= 10;
                    }

                    this._inputValue += digit;
                    GetValue(false);
                    if (!this._compleate)
                    {
                        this._display2.Append(help);
                    }
                    else
                    {
                        Compleate = false;
                        this._minus = false;
                        this._minusEx = false;
                        this._display2 = new StringBuilder();
                        this._display2.Append(help);
                    }
                }
                else
                {
                    if (this._compleateExp2)
                    {
                        this._inputExValue *= 10;
                        this._inputExValue += digit;
                        GetValue();
                    }
                    else
                    {
                        this._inputExValue = digit;
                        this._compleateExp2 = true;
                    }

                    this._display4 = this._inputExValue.ToString();
                }
                if (!Model.IsUpn)
                {
                    if (Model.IsEqual)
                        foramula = new StringBuilder(help.ToString());
                    else
                        foramula.Append(help);
                    Model.IsEqual = false;
                }
            }

            DisplayChanged();
        }

        internal void AddAlpha(object value)
        {
            char c = (char) value;
            if (_xeq || _asn1 || _prp)
            {
                this._keyDisplay += c;
                OnPropertyChanged(nameof(Display2));
            }
            else
            {
                this.Model.AddAlpha(c);
            }
        }

        internal void AddDot(object o)
        {
            if (this._select1)
            {
                if (this._select2)
                {
                    this._selectDisplay += " ST";
                    this._selectStack = true;
                }
            }
            else
            {
                if (this.Model.IsFlagSet(56))
                {
                    Messenger.ShowDialogAsync(AppResources.NotHex);
                }
                else
                {
                    if (!this._compleate)
                    {
                        this._display2.Append(this.Model.IsFlagSet(28) ? "," : ".");
                    }
                    else
                    {
                        GetValue(false);
                        this._compleate = false;
                        this._display2 = new StringBuilder(this.Model.IsFlagSet(28) ? "," : ".");
                    }

                    if (!Model.IsUpn)
                    {
                        foramula.Append(this.Model.IsFlagSet(28) ? "," : ".");
                        OnPropertyChanged(nameof(Foramula));
                    }

                    this._dot = true;
                }
            }

            OnPropertyChanged(nameof(Display2));
            OnPropertyChanged(nameof(DisplayC1));
        }

        internal void DoAlpha()
        {
            if (this.Model.IsFlagSet(48))
            {
                GetValue();
            }
            else
            {
                Compleate = true;
                if (_xeq || _asn || _prp)
                {
                    Xeq = false;
                    this._asn1 = false;
                    var action = this.Model.GetActionModel(this._keyDisplay.Substring(4));
                    if (_prp)
                    {
                        Model.PrintProgram(action);
                        _prp = false;
                    }
                    else
                    {
                        if (this._asn)
                        {
                            this._asnAction = action;
                        }
                        else if (action != null)
                        {
                            Model.Printer.Print(_keyDisplay.Substring(4), PrinterModel.Mode.NORM, true);
                            action.Execute();
                        }
                    }
                }
            }

            foreach (var buttonViewModel in Buttons)
            {
                buttonViewModel.SetAlpha(this.Model.IsFlagSet(48), this.Model.IsFlagSet(27));
            }

            DisplayChanged();
            OnPropertyChanged(nameof(AlphaVisibel));
        }

        internal void DoAsn(object o)
        {
            if (this._asn)
            {
                Asn = this._asn1 = false;
            }
            else
            {
                this._asn1 = Asn = true;
                this._keyDisplay = "ASN ";
                OnPropertyChanged(nameof(Display2));
            }
        }

        private void DoBack(object o)
        {
            if (_xeq || _asn1 || _prp)
            {
                if (this._keyDisplay.Length > 4)
                {
                    this._keyDisplay = this._keyDisplay.Substring(0, this._keyDisplay.Length - 1);
                }
                else
                {
                    Xeq = this._asn1 = Asn = _prp = false;
                }

                OnPropertyChanged(nameof(Display2));
            }
            else
            {
                if (this.Model.IsFlagSet(50))
                {
                    this.Model.ClearFlag(50);
                    DisplayChanged();
                }
                else if (this.Model.IsFlagSet(48))
                {
                    this.Model.RemoveAlpha();
                    OnPropertyChanged(nameof(Display2));
                }
                else if ((!this._shift) && (!this._compleate) && (this._display2.Length > 0))
                {
                    this._display2.Remove(this._display2.Length - 1, 1);
                    this._inputValue /= 10;
                    OnPropertyChanged(nameof(Display2));
                    OnPropertyChanged(nameof(DisplayC1));
                    if (!Model.IsUpn)
                    {
                        foramula.Remove(foramula.Length - 1, 1);
                        OnPropertyChanged(nameof(Foramula));
                    }
                }
                else
                {
                    this.Model.ClearX();
                }
            }
        }

        internal void DoChs(object o)
        {
            if (!this._compleate)
            {
                if (this.Model.IsFlagSet(56))
                {
                    Messenger.ShowDialogAsync(AppResources.NotHex);
                }
                else
                {
                    if (!CompleateExp)
                    {
                        MinusEx = !this._minusEx;
                        if (!Model.IsUpn)
                        {
                            var pos = foramula.Length;
                            if (_compleateExp2)
                            {
                                pos--;
                                if (_compleateExp1) pos--;
                            }
                            if (this._minusEx)
                                foramula.Insert(pos, '-');
                            else
                                foramula.Remove(pos - 1, 1);
                            OnPropertyChanged(nameof(Foramula));
                        }
                    }
                    else
                    {
                        Minus = !this._minus;
                        if (!Model.IsUpn)
                        {
                            if (this._minus)
                                foramula.Insert(numberPos, '-');
                            else
                                foramula.Remove(numberPos, 1);
                            OnPropertyChanged(nameof(Foramula));
                        }
                    }
                }
            }
            else
            {
                this.Model.X = this.Model.X * -1;
                if (!Model.IsUpn)
                {
                    if (Model.IsEqual)
                        foramula = new StringBuilder(Model.X.ToString());
                    OnPropertyChanged(nameof(Foramula));
                }
            }
        }

        public void DoShift(object o)
        {
            if (this._select1)
            {
                Messenger.ShowDialogAsync(AppResources.NotYet);
            }
            else
            {
                Shift = !this._shift;
            }
        }

        internal void DoXeq(object o)
        {
            if (this._xeq)
            {
                this._xeq = false;
            }
            else
            {
                this._xeq = true;
                this._keyDisplay = "XEQ ";
                OnPropertyChanged(nameof(Display2));
            }
        }

        internal void DoPrp(object o)
        {
            if (_prp)
            {
                _prp = false;
            }
            else
            {
                _prp = true;
                this._keyDisplay = "PRP ";
                OnPropertyChanged(nameof(Display2));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
