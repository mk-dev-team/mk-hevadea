﻿using Hevadea.Framework.Utils;

namespace Hevadea.Game.Registry
{
    public static class REGISTRY
    {
        public static void Initialize()
        {
            Logger.Log("Initializing game registery.");
            
            TILES.Initialize();
            ENTITIES.Initialize();
            ITEMS.Initialize();
            RECIPIES.InitializeHandCraftedRecipe();
            LEVELS.Initialize();
            GENERATOR.Initialize();
            TILES.AttachRender();
            TILES.AttachTags();
            ITEMS.AttachTags();
        }
    }
}