﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blish_HUD.BHUDControls.Hotkeys {
    public class HotkeyAssigner : Control {

        private const int UNIVERSAL_PADDING = 2;

        private const int DOUBLE_CLICK_THRESHOLD = 600;

        private int _nameWidth;
        public int NameWidth {
            get => _nameWidth;
            set {
                if (_nameWidth == value) return;

                _nameWidth = value;
                
                OnPropertyChanged();
            }
        }

        private Rectangle NameRegion => new Rectangle(0, 0, this.NameWidth, this.Height);
        private Rectangle HotkeyRegion => new Rectangle(this.NameRegion.Width + UNIVERSAL_PADDING, 0, this.Width - this.NameRegion.Width - UNIVERSAL_PADDING, this.Height);

        private bool _mouseOverHotkey = false;
        private bool MouseOverHotkey {
            get => _mouseOverHotkey;
            set => SetProperty(ref _mouseOverHotkey, value);
        }

        private DateTime lastClickTime;

        private Hotkey HotkeyDefinition;

        public HotkeyAssigner(Hotkey hotkey) {
            HotkeyDefinition = hotkey;

            this.Size = new Point(256, 16);

            lastClickTime = DateTime.MinValue;

            this.MouseMoved += HotkeyAssigner_OnMouseMoved;
            this.MouseLeft += HotkeyAssigner_OnMouseLeft;
            this.LeftMouseButtonReleased += HotkeyAssigner_OnLeftMouseButtonReleased;
        }

        private void HotkeyAssigner_OnLeftMouseButtonReleased(object sender, MouseEventArgs e) {
            // This is used to make it require a double-click to open the assignment window instead of just a single-click
            if (DateTime.Now.Subtract(lastClickTime).TotalMilliseconds < DOUBLE_CLICK_THRESHOLD)
                SetupNewAssignmentWindow();
            else
                lastClickTime = DateTime.Now;
        }

        private void SetupNewAssignmentWindow() {
            var newHkAssign = new HotkeyAssignmentWindow(HotkeyDefinition);
            newHkAssign.Location = new Point(Graphics.WindowWidth / 2 - newHkAssign.Width / 2, Graphics.WindowHeight / 2 - newHkAssign.Height / 2);
            newHkAssign.Parent = Graphics.SpriteScreen;

        }

        private void HotkeyAssigner_OnMouseLeft(object sender, MouseEventArgs e) {
            this.MouseOverHotkey = false;
        }

        private void HotkeyAssigner_OnMouseMoved(object sender, MouseEventArgs e) {
            var relPos = e.MouseState.Position - this.AbsoluteBounds.Location;

            this.MouseOverHotkey = this.HotkeyRegion.Contains(relPos);
        }

        protected override CaptureType CapturesInput() { return CaptureType.Filter | CaptureType.Mouse; }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            // Draw first white panel
            spriteBatch.Draw(
                             ContentService.Textures.Pixel,
                             this.NameRegion,
                             Color.White * 0.15f
                             );

            // Draw name shadow
            Blish_HUD.Utils.DrawUtil.DrawAlignedText(
                                                spriteBatch,
                                                Content.DefaultFont14,
                                                HotkeyDefinition.Name,
                                                this.NameRegion.OffsetBy(UNIVERSAL_PADDING + 1, 1),
                                                Color.Black,
                                                DrawUtil.HorizontalAlignment.Left,
                                                DrawUtil.VerticalAlignment.Middle
                                               );

            // Draw name
            Blish_HUD.Utils.DrawUtil.DrawAlignedText(spriteBatch,
                                                Content.DefaultFont14,
                                                HotkeyDefinition.Name,
                                                this.NameRegion.OffsetBy(UNIVERSAL_PADDING, 0),
                                                Color.White,
                                                DrawUtil.HorizontalAlignment.Left,
                                                DrawUtil.VerticalAlignment.Middle
                                               );

            // Draw white panel for hotkey
            spriteBatch.Draw(
                             ContentService.Textures.Pixel,
                             this.HotkeyRegion,
                             Color.White * (this.MouseOverHotkey ? 0.20f : 0.15f)
                            );

            // Easy way to get a string representation of the hotkeys
            string hotkeyRep = string.Join(" + ", HotkeyDefinition.Keys);

            // Draw hotkey shadow
            Blish_HUD.Utils.DrawUtil.DrawAlignedText(
                                                spriteBatch,
                                                Content.DefaultFont14,
                                                hotkeyRep,
                                                this.HotkeyRegion.OffsetBy(1, 1),
                                                Color.Black,
                                                DrawUtil.HorizontalAlignment.Center,
                                                DrawUtil.VerticalAlignment.Middle
                                               );

            // Draw hotkey
            Blish_HUD.Utils.DrawUtil.DrawAlignedText(spriteBatch,
                                                Content.DefaultFont14,
                                                hotkeyRep,
                                                this.HotkeyRegion,
                                                Color.White,
                                                DrawUtil.HorizontalAlignment.Center,
                                                DrawUtil.VerticalAlignment.Middle
                                               );


        }

    }
}
