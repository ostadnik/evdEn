using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel;

namespace evdEn
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class MenuEntry
    {
        #region Fields

        protected float selectionFade;
        protected string caption = string.Empty;
        protected string text = string.Empty;
        protected string toolTip = string.Empty;
        protected Vector2 position = Vector2.Zero;
        protected Texture2D activeTexture = null;
        protected Texture2D passiveTexture = null;
        protected Texture2D readonlyTexture = null;
        protected bool readOnly = false;

        #endregion

        #region Properties

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Texture2D ActiveTexture
        {
            get { return activeTexture; }
            set { activeTexture = value; }
        }

        public Texture2D PassiveTexture
        {
            get { return passiveTexture; }
            set { passiveTexture = value; }
        }

        public Texture2D ReadonlyTexture
        {
            get { return readonlyTexture; }
            set { readonlyTexture = value; }
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;
        public event EventHandler<PlayerIndexEventArgs> ToLeft;
        public event EventHandler<PlayerIndexEventArgs> ToRight;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        protected internal virtual void OnLeftEntry(PlayerIndex playerIndex)
        {
            if (ToLeft != null)
                ToLeft(this, new PlayerIndexEventArgs(playerIndex));
        }

        protected internal virtual void OnRightEntry(PlayerIndex playerIndex)
        {
            if (ToRight != null)
                ToRight(this, new PlayerIndexEventArgs(playerIndex));
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string caption)
        {
            this.caption = caption;
            this.text = string.Empty;
        }
        
        public MenuEntry(string caption, bool ReadOnly)
        {
            this.caption = caption;
            this.text = string.Empty;
            this.ReadOnly = ReadOnly;
        }

        public MenuEntry(string caption, string text)
        {
            this.caption = caption;
            this.text = text;
        }

        public MenuEntry(string caption, string text, bool ReadOnly)
        {
            this.caption = caption;
            this.text = text;
            this.ReadOnly = ReadOnly;
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected,
                                                      GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, Vector2 position,
                                 bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? evdEnUI.MenuSelectedColor : evdEnUI.MenuColor;
            if (ReadOnly) color = evdEnUI.MenuReadonlyColor;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = evdEnUI.fntMenuLarge;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);
            
            Vector2 textSize = font.MeasureString(text);

            if (readOnly)
            {
                if (null != ReadonlyTexture)
                {
                    spriteBatch.Draw(ReadonlyTexture,
                                     new Vector2(position.X - (ReadonlyTexture.Width / 2), 
                                                 position.Y - (ReadonlyTexture.Height / 2)),
                                     Color.White);
                }
                else if (null != screen.ReadonlyTexture)
                {
                    spriteBatch.Draw(screen.ReadonlyTexture,
                                     new Vector2(position.X - (screen.ReadonlyTexture.Width / 2),
                                                 position.Y - (screen.ReadonlyTexture.Height / 2)),
                                     Color.White);
                }
            }
            else if (isSelected)
            {
                if (null != ActiveTexture)
                {
                    spriteBatch.Draw(ActiveTexture,
                                     new Vector2(position.X - (ActiveTexture.Width / 2),
                                                 position.Y - (ActiveTexture.Height / 2)),
                                     Color.White);
                }
                else if (null != screen.ActiveTexture)
                {
                    spriteBatch.Draw(screen.ActiveTexture,
                                     new Vector2(position.X - (screen.ActiveTexture.Width / 2),
                                                 position.Y - (screen.ActiveTexture.Height / 2)),
                                     Color.White);
                }
            }
            else
            {
                if (null != PassiveTexture)
                {
                    spriteBatch.Draw(PassiveTexture,
                                     new Vector2(position.X - (PassiveTexture.Width / 2),
                                                 position.Y - (PassiveTexture.Height / 2)),
                                     Color.White);
                }
                else if (null != screen.PassiveTexture)
                {
                    spriteBatch.Draw(screen.PassiveTexture,
                                     new Vector2(position.X - (screen.PassiveTexture.Width / 2),
                                                 position.Y - (screen.PassiveTexture.Height / 2)),
                                     Color.White);
                }
            }
            if(!string.IsNullOrEmpty(caption))
                spriteBatch.DrawString(font, caption, position, color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);
            if (!string.IsNullOrEmpty(text))
                spriteBatch.DrawString(font, text, new Vector2(position.X + screen.LabelWidth, position.Y), color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);

        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            if (null != PassiveTexture)
            {
                return Math.Max(screen.ScreenManager.Font.LineSpacing, PassiveTexture.Height);
            }
            else if (null != screen.PassiveTexture)
            {
                return Math.Max(screen.ScreenManager.Font.LineSpacing, screen.PassiveTexture.Height);
            }
            else 
                return screen.ScreenManager.Font.LineSpacing;
        }
        
        #endregion
    }

    public class MenuEntryNumber : MenuEntry
    {
        protected virtual void UpdateText()
        {
            Text = _value.ToString();
        }

        protected int _value = 0;
        protected int min = 0;
        protected int max = 100;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value > max) _value = max;
                else if (value < min) _value = min;
                else _value = value;
                UpdateText();
            }
        }

        public int Max
        {
            get { return max; }
            set
            {
                if (value < min) max = min;
                else max = value;
                Value = _value;
            }
        }

        public int Min
        {
            get { return min; }
            set
            {
                if (value > max) min = max;
                else min = value;
                Value = _value;
            }
        }

        public MenuEntryNumber(string caption)
            : base(caption)
        {
            UpdateText();
        }

        public MenuEntryNumber(string caption, int min, int max)
            : base(caption)
        {
            this.min = min;
            this.max = max;
            UpdateText();
        }

        public MenuEntryNumber(string caption, int min, int max, int value)
            : base(caption)
        {
            this.min = min;
            this.max = max;
            this._value = value;
            UpdateText();
        }

        protected internal override void OnLeftEntry(PlayerIndex playerIndex)
        {
            _value--;
            if (_value < min) _value = min;
            UpdateText();
            base.OnLeftEntry(playerIndex);
        }

        protected internal override void OnRightEntry(PlayerIndex playerIndex)
        {
            _value++;
            if (_value > max) _value = max;
            UpdateText();
            base.OnRightEntry(playerIndex);
        }

        protected internal override void OnSelectEntry(PlayerIndex playerIndex)
        {
            _value++;
            if (_value > max) _value = max;
            UpdateText();
            base.OnSelectEntry(playerIndex);
        }
    }

    public class MenuEntryCheckBox : MenuEntry
    {

        private void UpdateText()
        {
            Text = _value ? "on" : "off";
        }

        bool _value = false;

        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                UpdateText();
            }
        }

        public MenuEntryCheckBox(string caption)
            : base(caption)
        {
            UpdateText();
        }

        public MenuEntryCheckBox(string caption, bool value)
            : base(caption)
        {
            this._value = value;
            UpdateText();
        }

        protected internal override void OnSelectEntry(PlayerIndex playerIndex)
        {
            _value = !_value;
            UpdateText();
            base.OnSelectEntry(playerIndex);
        }

        protected internal override void OnLeftEntry(PlayerIndex playerIndex)
        {
            _value = !_value;
            UpdateText();
            base.OnLeftEntry(playerIndex);
        }

        protected internal override void OnRightEntry(PlayerIndex playerIndex)
        {
            _value = !_value;
            UpdateText();
            base.OnLeftEntry(playerIndex);
        }
    }

    public class MenuEntryList : MenuEntry
    {
        private void UpdateText()
        {
            Text = (selectedIndex<0) ? "" : list[selectedIndex];
        }

        List<string> list = new List<string>();
        int selectedIndex = -1;

        public string Value
        {
            get { return (selectedIndex < 0) ? "" : list[selectedIndex]; }
            set
            {
                bool found = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Equals(value))
                    {
                        selectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (!found) selectedIndex = -1;
                UpdateText();
            }
        }

        public MenuEntryList(string caption, List<string> list)
            : base(caption)
        {
            this.list = list;
            UpdateText();
        }

        public MenuEntryList(string caption, List<string> list, string selected)
            : base(caption)
        {
            this.list = list;
            this.Value = selected;
            UpdateText();
        }

        protected internal override void OnLeftEntry(PlayerIndex playerIndex)
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = list.Count - 1;
            UpdateText();
            base.OnLeftEntry(playerIndex);
        }

        protected internal override void OnRightEntry(PlayerIndex playerIndex)
        {
            selectedIndex++;
            if (selectedIndex >= list.Count)
                selectedIndex = 0;
            UpdateText();
            base.OnRightEntry(playerIndex);
        }

        protected internal override void OnSelectEntry(PlayerIndex playerIndex)
        {
            selectedIndex++;
            if (selectedIndex >= list.Count)
                selectedIndex = 0;
            UpdateText();
            base.OnSelectEntry(playerIndex);
        }
    }

    public class MenuEntryHSlider : MenuEntryNumber
    {

        Point bounds = new Point(0, 2);
        
        [
        Category("Layout"),
        Description("Specifies the boundaries of the slider movement. From X to Y on horizontal line. min(y-x)>=2!")
        ]
        public Point Bounds
        {
            get { return bounds; }
            set
            {
                if (value.X + 2 >= value.Y)
                {
                    bounds = new Point(0, 2);
                }
                else
                {
                    bounds = value;
                }
                UpdateText();
            }
        }

        #region constructors
        public MenuEntryHSlider(string caption)
            : base(caption)
        {
            UpdateText();
            Bounds = evdEnUI.hScrollBounds;
        }

        public MenuEntryHSlider(string caption, int min, int max)
            : base(caption, min, max)
        {
            UpdateText();
            Bounds = evdEnUI.hScrollBounds;
        }

        public MenuEntryHSlider(string caption, int min, int max, int value)
            : base(caption, min, max, value)
        {
            UpdateText();
            Bounds = evdEnUI.hScrollBounds;
        }
        #endregion

        protected override void UpdateText()
        {
            base.UpdateText();
            CalculateSliderPosition();
        }

        int sliderPos;
        protected virtual void CalculateSliderPosition()
        {
            if (_value == min)
            {
                sliderPos = bounds.X;
            }
            else if (_value == max)
            {
                sliderPos = bounds.Y;
            }
            else
            {
                float rel = ((float)_value - (float)min) / ((float)max - (float)min);
                sliderPos = (int)(bounds.X + 1 + (bounds.Y - bounds.X - 2) * rel);
            }
        }

        public override void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
        {
            // 
            if (evdEnUI.hScrollSlideTexture == null || evdEnUI.hScrollRailTexture == null)
            {
                base.Draw(screen, position, isSelected, gameTime);
                return;
            }

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? evdEnUI.MenuSelectedColor : evdEnUI.MenuColor;
            if (ReadOnly) color = evdEnUI.MenuReadonlyColor;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = evdEnUI.fntMenuLarge;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            Vector2 textSize = font.MeasureString(text);

            spriteBatch.Draw(evdEnUI.hScrollRailTexture,
                new Vector2(position.X + screen.LabelWidth,
                    position.Y - (evdEnUI.hScrollRailTexture.Height / 2)),
                    Color.White);
            spriteBatch.Draw(evdEnUI.hScrollSlideTexture,
                new Vector2(position.X + sliderPos + screen.LabelWidth,
                    position.Y - (evdEnUI.hScrollRailTexture.Height / 2)),
                    Color.White);

            if (readOnly)
            {
                if (null != ReadonlyTexture)
                {
                    spriteBatch.Draw(ReadonlyTexture,
                                     new Vector2(position.X - (ReadonlyTexture.Width / 2),
                                                 position.Y - (ReadonlyTexture.Height / 2)),
                                     Color.White);
                }
                else if (null != screen.ReadonlyTexture)
                {
                    spriteBatch.Draw(screen.ReadonlyTexture,
                                     new Vector2(position.X - (screen.ReadonlyTexture.Width / 2),
                                                 position.Y - (screen.ReadonlyTexture.Height / 2)),
                                     Color.White);
                }
            }
            else if (isSelected)
            {
                if (null != ActiveTexture)
                {
                    spriteBatch.Draw(ActiveTexture,
                                     new Vector2(position.X - (ActiveTexture.Width / 2),
                                                 position.Y - (ActiveTexture.Height / 2)),
                                     Color.White);
                }
                else if (null != screen.ActiveTexture)
                {
                    spriteBatch.Draw(screen.ActiveTexture,
                                     new Vector2(position.X - (screen.ActiveTexture.Width / 2),
                                                 position.Y - (screen.ActiveTexture.Height / 2)),
                                     Color.White);
                }
            }
            else
            {
                if (null != PassiveTexture)
                {
                    spriteBatch.Draw(PassiveTexture,
                                     new Vector2(position.X - (PassiveTexture.Width / 2),
                                                 position.Y - (PassiveTexture.Height / 2)),
                                     Color.White);
                }
                else if (null != screen.PassiveTexture)
                {
                    spriteBatch.Draw(screen.PassiveTexture,
                                     new Vector2(position.X - (screen.PassiveTexture.Width / 2),
                                                 position.Y - (screen.PassiveTexture.Height / 2)),
                                     Color.White);
                }
            }
            if (!string.IsNullOrEmpty(caption))
                spriteBatch.DrawString(font, caption, position, color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);
            /*
            if (!string.IsNullOrEmpty(text))
                spriteBatch.DrawString(font, text, new Vector2(position.X + screen.LabelWidth, position.Y), color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);
            */
        }
    }

}
