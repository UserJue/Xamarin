namespace HP41CV.Models
{
    public class AlphaActionModel : InvokeActionModel
    {
        private char _character;

        public AlphaActionModel(string text, char character)
            : base(null, text)
        {
            _character = character;
            TextD = 'ᵀ' + text;
            GetValue = false;
            IsFunction = false;
        }

        public AlphaActionModel(char character)
            : base(null, character.ToString())
        {
            _character = character;
            TextD = 'ᵀ' + character.ToString();
            IsFunction = false;
        }

        public override void Execute(bool noUpn)
        {
            InvokeAction?.Invoke(_character);
        }
    }
}
