﻿using Hevadea.Game;
using Hevadea.Scenes.Widgets;
using Microsoft.Xna.Framework;
using System.IO;
using Hevadea.Framework;
using Hevadea.Framework.Scening;
using Hevadea.Framework.UI;
using Hevadea.Framework.UI.Containers;
using Hevadea.Framework.UI.Widgets;

namespace Hevadea.Scenes
{
    public class SceneLoadGame : Scene
    {
        public override void Load()
        {         
            var backButton = new BackButton
            {
                UnitBound = new Rectangle(0, 0, 48, 48),
                Origine = Anchor.BottomLeft,
                Anchor = Anchor.BottomLeft,
                UnitOffset = new Point(0, -16)
            };
            backButton.MouseClick += sender => Rise.Scene.Switch(new MainMenu());

            
            var savesList = new WidgetFancyPanel
            {
                Anchor  = Anchor.Center,
                Origine = Anchor.Center,
                UnitBound = new Rectangle(0, 0, 720, 416),
                Padding = new Padding(16),
                Content = new DockContainer()
            };

            Container = new AnchoredContainer
            {
                Padding = new Padding(16),
                Childrens = {savesList, backButton}
            };

            var s = Directory.GetDirectories(Rise.Platform.GetStorageFolder() + "/Saves/");
            var list = (DockContainer) savesList.Content;
            foreach (var save in s)
            {
                list.AddChild(new Button {Text = save, Dock = Dock.Top}
                .RegisterMouseClickEvent(sender =>
                    {
                        var button = (Button) (sender);
                        Rise.Scene.Switch(new SceneWorldLoading(button.Text));
                    }));
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
        }

        public override void OnDraw(GameTime gameTime)
        {
        }

        public override void Unload()
        {
        }
    }
}