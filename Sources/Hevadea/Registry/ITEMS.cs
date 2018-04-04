﻿using System.Collections.Generic;
using Hevadea.Entities.Components;
using Hevadea.Framework.Graphic.SpriteAtlas;
using Hevadea.Items;
using Hevadea.Items.Materials;
using Hevadea.Items.Tags;
using Microsoft.Xna.Framework;

namespace Hevadea.Registry
{
    public static class ITEMS
    {
        public static readonly List<Item> ById = new List<Item>();

        public static Material WoodMaterial;
        public static Material IronMaterial;
        public static Material GoldMaterial;

        public static Item IRON_ORE;
        public static Item COAL;
        public static Item WOOD_LOG;
        public static Item WOOD_PLANK;
        public static Item WOOD_STICK;
        public static Item PINE_CONE;
        public static Item STONE;
        
        public static Item WOOD_FLOOR;
        public static Item WOOD_WALL;
        public static Item GRASS_PATCH;
        public static Item SAND;

        public static Item CHEST;
        public static Item TORCH;
        public static Item CRAFTING_BENCH;
        public static Item FURNACE;

        public static Item BELT;
        public static Item TNT;
        public static Item RAW_FISH;
        public static Item LIGHTER;
        
        public static void Initialize()
        {
            WoodMaterial = new BaseMaterial(2f);
            IronMaterial = new BaseMaterial(4f);
            GoldMaterial = new BaseMaterial(8f);

            IRON_ORE       = new Item("iron_ore",       new Sprite(Ressources.TileItems, new Point(8, 0)));  
            COAL           = new Item("coal",           new Sprite(Ressources.TileItems, new Point(6, 2)));
            STONE          = new Item("stone",          new Sprite(Ressources.TileItems, new Point(7, 0)));
            PINE_CONE      = new Item("pine_cone",      new Sprite(Ressources.TileItems, new Point(5, 2)));
            WOOD_LOG       = new Item("wood_log",       new Sprite(Ressources.TileItems, 6));
            WOOD_PLANK     = new Item("wood_plank",     new Sprite(Ressources.TileItems, new Point(6, 1)));
            WOOD_STICK     = new Item("wood_stick",     new Sprite(Ressources.TileItems, 5));

            WOOD_FLOOR     = new Item("wood_floor",     new Sprite(Ressources.TileItems, new Point(7, 5)));
            WOOD_WALL      = new Item("wood_wall",      new Sprite(Ressources.TileItems, new Point(7, 4)));
            GRASS_PATCH    = new Item("grass_patch",    new Sprite(Ressources.TileItems, new Point(7, 2)));
            SAND           = new Item("sand",           new Sprite(Ressources.TileItems, new Point(7, 3)));
            
            CHEST          = new Item("chest",          new Sprite(Ressources.TileEntities, new Point(0, 1)));
            CRAFTING_BENCH = new Item("crafting_bench", new Sprite(Ressources.TileEntities, new Point(1, 0)));
            FURNACE        = new Item("furnace",        new Sprite(Ressources.TileEntities, new Point(1, 1)));
            TORCH          = new Item("torch",          new Sprite(Ressources.TileEntities, new Point(4, 0)));
            
            BELT = new Item("belt", new Sprite(Ressources.TileItems, new Point(9, 0)));
            TNT = new Item("tnt", new Sprite(Ressources.TileItems, new Point(0, 0)));
            RAW_FISH = new Item("raw_fish", new Sprite(Ressources.TileEntities, new Point(11, 0)));
            LIGHTER = new Item("lighter", new Sprite(Ressources.TileEntities, new Point(4, 0)));
        }

        public static void AttachTags()
        {
            CHEST.AddTag(new PlaceEntity(ENTITIES.CHEST));
            CRAFTING_BENCH.AddTag(new PlaceEntity(ENTITIES.CRAFTING_BENCH));
            FURNACE.AddTag(new PlaceEntity(ENTITIES.FURNACE));
            TORCH.AddTag(new PlaceEntity(ENTITIES.TORCH));
            
            WOOD_FLOOR.AddTag(new PlaceTile(TILES.WOOD_FLOOR){CanBePlaceOn = {TILES.DIRT}});
            WOOD_WALL.AddTag(new PlaceTile(TILES.WOOD_WALL){CanBePlaceOn = {TILES.DIRT}});
            GRASS_PATCH.AddTag(new PlaceTile(TILES.GRASS){CanBePlaceOn = {TILES.DIRT}});
            SAND.AddTag(new PlaceTile(TILES.SAND){CanBePlaceOn = {TILES.DIRT}});
            
            BELT.AddTag(new PlaceEntity(ENTITIES.BELT));
            TNT.AddTag(new PlaceEntity(ENTITIES.TNT));
            LIGHTER.AddTag(new ActionItemTag()
            {
                Action = (user, pos) =>
                {
                    foreach (var e in user.Level.GetEntityOnTile(pos))
                    {
                        if (e.HasComponent<Burnable>())
                            e.GetComponent<Burnable>().IsBurnning = true;
                    }
                }
            });

            return;
        }
    }
}