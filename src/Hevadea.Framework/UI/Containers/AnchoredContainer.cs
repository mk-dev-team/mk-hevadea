﻿using Hevadea.Framework.Utils;
using Microsoft.Xna.Framework;
using System;

namespace Hevadea.Framework.UI.Containers
{
    [Obsolete]
    public class AnchoredContainer : Container
    {
        public override void Layout()
        {
            foreach (var c in Childrens)
            {
                var position = UnitHost.Location + UnitHost.GetAnchorPoint(c.Anchor) - c.UnitBound.GetAnchorPoint(c.Origine) + c.UnitOffset;
                c.UnitBound = new Rectangle(position, c.UnitBound.Size);
            }
        }
    }
}