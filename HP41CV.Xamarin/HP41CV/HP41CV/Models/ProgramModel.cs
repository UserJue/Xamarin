using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace HP41CV.Models
{
    public class ProgramModel : ProgramEntry
    {
        public enum SourceType
        {
            NoNumber,
            Number2,
            Number3
        }
        private Collection<ProgramStepModel> steps;
        private int ActualStep;
        private Dictionary<string, int> lableDictionary;
        private CalculatorModel model;

        public ProgramModel(CalculatorModel model, string lable)
            : base(model, lable)
        {
            this.model = model;
            steps = new Collection<ProgramStepModel>();
            Lines = new Collection<string>();
            lableDictionary = new Dictionary<string, int>();
            lableDictionary.Add(lable, 0);
            ActualStep = 0;
        }

        public void AddStep(ActionModel model)
        {
            steps.Add(new ProgramStepModel(model));
            Lines.Add(model.Text);
        }

        public void AddStep(ActionModel model, int value, bool indirect = false)
        {
            steps.Add(new ProgramStepSelectValueModel((SelectActionModel)model, value, indirect));
            if (model != null)
                Lines.Add(model.Text + "\t" + value);
            else
                Lines.Add(value.ToString());
        }

        public void AddStep(ActionModel model, string value, bool ind)
        {
            var invokeActionModel = model as InvokeActionModel;
            if (invokeActionModel != null)
            {
                if (ind)
                {
                    int index;
                    if (int.TryParse(value, out index) && Model.IsRegister(index))
                        steps.Add(new ProgramStepExecuteModel(this, invokeActionModel, Model.GetRegister(index)));
                }
                else
                    steps.Add(new ProgramStepExecuteModel(this, invokeActionModel, value));
            }
            else
                steps.Add(new ProgramStepActionModel((ProgramActionModel)model, value));
            if (model != null)
                Lines.Add(model.Text + "\t" + value);
            else
                Lines.Add(value.ToString());
        }

        public void AddStep(string line)
        {
            steps.Add(new ProgramStepValueModel(Model, line));
            Lines.Add('ᵀ' + line);
        }

        public void AddStep(double value)
        {
            steps.Add(new ProgramStepValueModel(Model, value));
            Lines.Add(value.ToString());
        }

        public void AddStep(ProgramStepValueModel.Actions action)
        {
            steps.Add(new ProgramStepValueModel(Model, action));
            Lines.Add(action.ToString());
        }

        public void AddLable(ActionModel model, string value)
        {
            bool global;

            var lable = GetLable(value, out global);
            Lines.Add(model.Text + "\t" + value);
            if (global)
            {
                if (!Model.Programs.ContainsKey(lable))
                    Model.Programs.Add(lable, new ProgramLink(this, Model, lable));
            }
            if (!lableDictionary.ContainsKey(lable))
                lableDictionary.Add(lable, Math.Max(0, steps.Count - 1));
        }

        private static string GetLable(string value, out bool global)
        {
            if (value[0] == '\"')
            {
                global = true;
                var parts = value.Substring(1).Split('\"');
                return parts[0];
            }
            if (value[0] == 'ᵀ')
            {
                global = true;
                return value.Substring(1);
            }
            global = false;
            return value;
        }

        public bool GotoLable(string value, bool next = false)
        {
            bool global;
            var lable = GetLable(value, out global);
            if (!lableDictionary.ContainsKey(lable))
            {
                if (!global) return false;
                if (next) ActualStep++;
                return model.GoToLable(value);
            }
            ActualStep = lableDictionary[lable];
            if (next) ActualStep++;
            return true;
        }

        public void GotoLine(int step)
        {
            ActualStep = step;
        }

        public bool ExecuteLable(string value, bool next = false)
        {
            model.PuschCallStack(this, ActualStep);
            return GotoLable(value, next);
        }

        public Collection<string> Lines { get; private set; }

        public override void Execute(bool noUpn)
        {
            ActualStep = 0;
            Model.StartProgram(this);
        }

        public void Return()
        {
            ActualStep = 0;
            Model.StopProgram();
        }

        public void ExecuteStep()
        {
            if (ActualStep < steps.Count)
            {
                steps[ActualStep].Execute();
                Model.ClearFlag(50);
                if (steps[ActualStep].IsCondition && !Model.FlagSet)
                    ActualStep += 2;
                else
                    ActualStep++;
            }
            else
            {
                ActualStep = 0;
                Model.StopProgram();
            }
        }

        public int Load(IList<string> lines, int pos, out bool ok, SourceType sourceType = SourceType.NoNumber)
        {
            int help = 0;
            int intvalue;
            double value;
            ok = false;
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.InvariantCulture;
            for (; pos < lines.Count; pos++)
            {
                string line;
                switch (sourceType)
                {
                    case SourceType.Number2:
                        line = lines[pos].Substring(3);
                        break;
                    case SourceType.Number3:
                        line = lines[pos].Substring(4);
                        break;
                    default:
                        line = lines[pos];
                        break;
                }
                if (pos == 93)
                    ;
                if ((line.StartsWith("EndProgram")) || (line == "END"))
                {
                    ok = true;
                    break;

                }
                if (line[0] == 'ᵀ')
                {
                    AddStep(line.Substring(1));
                    continue;
                }
                if (line[0] == '\"')
                {
                    var part = line.Substring(1).Split('\"');
                    AddStep(part[0]);
                    continue;
                }
                if ((line == "AVIEW") || (line == "AVIEW "))
                {
                    AddStep(ProgramStepValueModel.Actions.AVIEW);
                    continue;
                }
                if (int.TryParse(line, out intvalue))
                {
                    AddStep(intvalue);
                    continue;
                }
                if (double.TryParse(line, style, culture, out value))
                {
                    AddStep(value);
                    continue;
                }
                if (line == "STAT")
                {
                    AddStep(Model.statisticRegister);
                    continue;
                }
                var parts = line.Split(new char[] { '\t', ' ' });
                var step = Model.GetActionModel(parts[0]);
                if (step != null)
                {
                    if (parts.Length > 1)
                    {
                        if (parts[0].StartsWith("LBL"))
                        {
                            if (parts[1][0] == '\"')
                                AddLable(step, "ᵀ" + parts[1].Substring(1, parts[1].Length - 2));
                            else
                                AddLable(step, parts[1]);
                            continue;
                        }
                        if (parts[0].StartsWith("GTO") || parts[0].StartsWith("XEQ") || parts[0].StartsWith("XROM"))
                        {
                            var ind = false;
                            var index = 1;
                            if (parts[index] == "IND")
                            {
                                index++;
                                ind = true;
                            }
                            if (parts[index][0] == '\"')
                                AddStep(step, "ᵀ" + parts[index].Substring(1, parts[1].Length - 2), ind);
                            else
                                AddStep(step, parts[index], ind);
                            continue;
                        }
                        if (int.TryParse(parts[1], out help))
                        {
                            AddStep(step, help);
                            continue;
                        }
                        if (parts[1] == "X")
                        {
                            AddStep(step, Model.MaxRegister + 1);
                            continue;
                        }
                        if (parts[1] == "Y")
                        {
                            AddStep(step, Model.MaxRegister + 2);
                            continue;
                        }
                        if (parts[1] == "Z")
                        {
                            AddStep(step, Model.MaxRegister + 3);
                            continue;
                        }
                        if (parts[1] == "T")
                        {
                            AddStep(step, Model.MaxRegister + 4);
                            continue;
                        }
                        if (parts[1] == "L")
                        {
                            AddStep(step, Model.MaxRegister + 5);
                            continue;
                        }
                        if ((parts[1] == "STAT") && double.TryParse(parts[2], out value))
                        {
                            AddStep(step, Model.statisticRegister + (int)value);
                            continue;
                        }
                        if (parts[1] == "IND")
                        {
                            if ((parts[2] == "STAT") && double.TryParse(parts[3], out value))
                                AddStep(step, Model.statisticRegister + (int)value, true);
                            else if (double.TryParse(parts[2], out value))
                                AddStep(step, (int)value, true);
                            continue;
                        }
                    }
                    AddStep(step);
                }
                else
                {
                    ;
                }
            }
            return pos;
        }

        public IList<string> Save()
        {
            var result = new List<string>();
            result.Add("Program=" + Text);
            result.AddRange(Lines);
            result.Add("EndProgram");
            return result;
        }

        public static bool IsProgramStart(string line, out ProgramModel.SourceType sourceType, out string lable)
        {
            if (line.StartsWith("Program="))
            {
                var len = Math.Min(7, line.Length - "Program=".Length);
                lable = line.Substring("Program=".Length, len);
                sourceType = ProgramModel.SourceType.NoNumber;
            }
            else if (line.StartsWith("01 LBL \"") || line.StartsWith("01♦LBL \""))
            {
                var parts = line.Split('\"');
                lable = parts[1];
                sourceType = ProgramModel.SourceType.Number2;
            }
            else if (line.StartsWith(" 01 LBL \"") || line.StartsWith("001 LBL \"") || line.StartsWith(" 01♦LBL \"") || line.StartsWith("001♦LBL \""))
            {
                var parts = line.Split('\"');
                lable = parts[1];
                sourceType = ProgramModel.SourceType.Number3;
            }
            else
            {
                lable = string.Empty;
                sourceType = ProgramModel.SourceType.NoNumber;
                return false;
            }
            if (lable.Length > 7)
                lable = lable.Substring(0, 7);
            return true;
        }
    }
}
