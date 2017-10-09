using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
    // https://www.codeproject.com/Tips/631196/ComboBox-with-Suggest-Ability-based-on-Substring-S
    public class SuggestComboBoxCompanion
    {
        private readonly ComboBox comboBox;

        public SuggestComboBoxCompanion(ComboBox comboBox)
        {
            this.comboBox = comboBox;

            // set the standard rules:
            _filterRuleCompiled = s => s.ToLower().Contains(comboBox.Text.Trim().ToLower());
            _suggestListOrderRuleCompiled = s => s;
            _propertySelectorCompiled = collection => collection.Cast<string>();

            _suggLb.DataSource = _suggBindingList;
            _suggLb.Click += SuggLbOnClick;

            OnParentChanged(null, null);
            comboBox.ParentChanged += OnParentChanged;
            comboBox.TextChanged += ComboBox_TextChanged;
            comboBox.LostFocus += OnLostFocus;
            comboBox.SizeChanged += OnSizeChanged;
            comboBox.DropDown += OnDropDown;
            comboBox.PreviewKeyDown += OnPreviewKeyDown;
            comboBox.KeyDown += ComboBox_KeyDown;
        }

        #region fields and properties

        private readonly ListBox _suggLb = new ListBox { Visible = false, TabStop = false };
        private readonly BindingList<string> _suggBindingList = new BindingList<string>();
        private Expression<Func<ComboBox.ObjectCollection, IEnumerable<string>>> _propertySelector;
        private Func<ComboBox.ObjectCollection, IEnumerable<string>> _propertySelectorCompiled;
        private Expression<Func<string, string, bool>> _filterRule;
        private Func<string, bool> _filterRuleCompiled;
        private Expression<Func<string, string>> _suggestListOrderRule;
        private Func<string, string> _suggestListOrderRuleCompiled;

        public int SuggestBoxHeight
        {
            get { return _suggLb.Height; }
            set { if (value > 0) _suggLb.Height = value; }
        }
        /// <summary>
        /// If the item-type of the ComboBox is not string,
        /// you can set here which property should be used
        /// </summary>
        public Expression<Func<ComboBox.ObjectCollection, IEnumerable<string>>> PropertySelector
        {
            get { return _propertySelector; }
            set
            {
                if (value == null) return;
                _propertySelector = value;
                _propertySelectorCompiled = value.Compile();
            }
        }

        ///<summary>
        /// Lambda-Expression to determine the suggested items
        /// (as Expression here because simple lamda (func) is not serializable)
        /// <para>default: case-insensitive contains search</para>
        /// <para>1st string: list item</para>
        /// <para>2nd string: typed text</para>
        ///</summary>
        public Expression<Func<string, string, bool>> FilterRule
        {
            get { return _filterRule; }
            set
            {
                if (value == null) return;
                _filterRule = value;
                _filterRuleCompiled = item => value.Compile()(item, comboBox.Text);
            }
        }

        ///<summary>
        /// Lambda-Expression to order the suggested items
        /// (as Expression here because simple lamda (func) is not serializable)
        /// <para>default: alphabetic ordering</para>
        ///</summary>
        public Expression<Func<string, string>> SuggestListOrderRule
        {
            get { return _suggestListOrderRule; }
            set
            {
                if (value == null) return;
                _suggestListOrderRule = value;
                _suggestListOrderRuleCompiled = value.Compile();
            }
        }

        public Func<object, EventArgs, ListBox, object> OnClick { get; internal set; }

        #endregion

        /// <summary>
        /// the magic happens here ;-)
        /// </summary>
        /// <param name="e"></param>
        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!comboBox.Focused) return;

            _suggBindingList.Clear();
            _suggBindingList.RaiseListChangedEvents = false;
            _propertySelectorCompiled(comboBox.Items)
                .Where(_filterRuleCompiled)
                //.OrderBy(_suggestListOrderRuleCompiled)
                .ToList()
                .ForEach(_suggBindingList.Add);
            _suggBindingList.RaiseListChangedEvents = true;
            _suggBindingList.ResetBindings();

            _suggLb.Visible = _suggBindingList.Any();

            if (_suggBindingList.Count == 1 &&
                _suggBindingList.Single().Length == comboBox.Text.Trim().Length)
            {
                comboBox.Text = _suggBindingList.Single();
                comboBox.Select(0, comboBox.Text.Length);
                _suggLb.Visible = false;
            }
        }

        private List<Control> _parentChain = new List<Control>();

        private void BuildChain()
        {
            foreach (var item in _parentChain)
            {
                item.LocationChanged -= ControlLocationChanged;
                item.ParentChanged -= ControlParentChanged;
            }
            _parentChain.Clear();

            Control current = comboBox;

            while (current != null)
            {
                _parentChain.Add(current);
                current = current.Parent;
            }

            foreach (var item in _parentChain)
            {
                item.LocationChanged += ControlLocationChanged;
                item.ParentChanged += ControlParentChanged;
            }
        }

        void ControlParentChanged(object sender, EventArgs e)
        {
            BuildChain();
            ControlLocationChanged(sender, e);
        }

        void ControlLocationChanged(object sender, EventArgs e)
        {
            // Update Location of Form
            if (comboBox.Parent != null)
            {
                var screenLoc = comboBox.Parent.PointToScreen(comboBox.Location);
                var topParent = _parentChain[_parentChain.Count - 1];
                var relLoc = topParent.PointToClient(screenLoc);
                OnLocationChanged(relLoc);
            }
        }

        protected void OnLocationChanged(Point r)
        {
            _suggLb.Top = r.Y + comboBox.Height - 3;
            _suggLb.Left = r.X + 3;
        }

        /// <summary>
        /// suggest-ListBox is added to parent control
        /// (in ctor parent isn't already assigned)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParentChanged(object sender, EventArgs e)
        {
            if (comboBox.Parent != null)
            {
                ControlParentChanged(sender, e);
                var topParent = _parentChain[_parentChain.Count - 1];
                topParent.Controls.Add(_suggLb);
                topParent.Controls.SetChildIndex(_suggLb, 0);
                _suggLb.Top = comboBox.Top + comboBox.Height - 3;
                _suggLb.Left = comboBox.Left + 3;
                _suggLb.Width = comboBox.Width;
                _suggLb.Font = comboBox.Font;
            }
        }

        protected void OnLostFocus(object sender, EventArgs e)
        {
            // _suggLb can only getting focused by clicking (because TabStop is off)
            // --> click-eventhandler 'SuggLbOnClick' is called
            if (!_suggLb.Focused) //|| !comboBox.Focused)
                HideSuggBox();
        }

        protected void OnSizeChanged(object sender, EventArgs e)
        {
            _suggLb.Width = comboBox.Width;
        }

        private void SuggLbOnClick(object sender, EventArgs eventArgs)
        {
            comboBox.Text = _suggLb.Text;
            comboBox.Focus();

            if (this.OnClick != null)
            {
                OnClick(sender, eventArgs, _suggLb);
            }
            HideSuggBox();
        }

        private void HideSuggBox()
        {
            _suggLb.Visible = false;
        }

        protected void OnDropDown(object sender, EventArgs e)
        {
            HideSuggBox();
        }

        #region keystroke events

        private bool skip = false;

        /// <summary>
        /// if the suggest-ListBox is visible some keystrokes
        /// should behave in a custom way
        /// </summary>
        /// <param name="e"></param>
        protected void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                if (skip)
                {
                    skip = false;
                    return;
                }
                else
                {
                    skip = true;
                }

                if (!_suggLb.Visible)
                {
                    return;
                }
            }

            if (comboBox.DroppedDown)
            {
                comboBox.DroppedDown = false;
            }

            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (_suggLb.SelectedIndex < _suggBindingList.Count - 1)
                        _suggLb.SelectedIndex++;
                    return;
                case Keys.Up:
                    if (_suggLb.SelectedIndex > 0)
                        _suggLb.SelectedIndex--;
                    return;
                case Keys.Tab:
                case Keys.Enter:
                    comboBox.Text = _suggLb.Text;
                    comboBox.Select(0, comboBox.Text.Length);
                    HideSuggBox();
                    return;
                case Keys.Escape:
                    HideSuggBox();
                    return;
            }
        }

        private static readonly Keys[] KeysToHandle = new[]
            { Keys.Down, Keys.Up, Keys.Enter, Keys.Escape, Keys.Tab };

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            // the keystrokes of our interest should not be processed by base class:
            if (_suggLb.Visible && KeysToHandle.Contains(e.KeyCode))
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
