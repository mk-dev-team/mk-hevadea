﻿using Hevadea.Framework.Utils;
using Hevadea.GameObjects.Entities;
using Hevadea.GameObjects.Entities.Blueprints;
using Hevadea.GameObjects.Entities.Components;
using Hevadea.GameObjects.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Hevadea.Worlds.Level
{
    public partial class Level
    {
        public void AddEntity(Entity e, float x, float y)
        {
            AddEntity(e);
            e.SetPosition(x, y);
        }

        public Entity SpawnEntity(Entity entity, int tx, int ty, float offX = 0f, float offY = 0f)
        {
            AddEntity(entity);
            entity.SetPosition(tx * Constant.TileSize + (Constant.TileSize / 2) + offX, ty * Constant.TileSize + (Constant.TileSize / 2) + offY);
            return entity;
        }

        public Entity SpawnEntity(EntityBlueprint blueprint, int tx, int ty, float offX = 0f, float offY = 0f)
        {
            var entity = blueprint.Construct();
            SpawnEntity(entity, tx, ty, offX, offY);
            return entity;
        }
        
        public void AddEntity(Entity e)
        {
            e.Removed = false;
            e.Level = this;
            if (!Entities.Contains(e)) Entities.Add(e);
            AddEntityToTile(e.GetTilePosition(), e);

            if (IsInitialized)
                e.Initialize(this, _world, _game);
        }

        public void RemoveEntity(Entity e)
        {
            Entities.Remove(e);
            RemoveEntityFromTile(e.GetTilePosition(), e);
            e.Removed = true;
        }

        public void AddEntityToTile(TilePosition p, Entity e)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height) return;
            _entitiesOnTiles[p.X, p.Y].Add(e);
        }

        public void RemoveEntityFromTile(TilePosition p, Entity e)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height) return;
            _entitiesOnTiles[p.X, p.Y].Remove(e);
        }

        internal List<Entity> GetEntityOnTile(TilePosition selectedTile)
        {
            return GetEntityOnTile(selectedTile.X, selectedTile.Y);
        }

        public List<Entity> GetEntityOnTile(int tx, int ty)
        {
            if (tx < Width && ty < Height && tx >= 0 && ty >= 0) return _entitiesOnTiles[tx, ty].ToList();
            return new List<Entity>();
        }

        public List<Entity> GetEntitiesOnArea(RectangleF area)
        {
            var result = new List<Entity>();

            var beginX = area.X / Constant.TileSize - 1;
            var beginY = area.Y / Constant.TileSize - 1;

            var endX = (area.X + area.Width) / Constant.TileSize + 1;
            var endY = (area.Y + area.Height) / Constant.TileSize + 1;


            for (int x = (int)beginX; x < endX; x++)
            for (int y = (int)beginY; y < endY; y++)
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height) continue;
                var entities = _entitiesOnTiles[x, y];

                result.AddRange(entities.Where(i => i.GetComponent<Colider>()?.GetHitBox().IntersectsWith(area) ?? area.Contains(i.Position)));
            }

            return result;
        }

        public List<Entity> GetEntitiesOnArea(Rectangle area)
        {
            return GetEntitiesOnArea(new RectangleF(area.X, area.Y, area.Width, area.Height));
        }

        public List<Entity> GetEntitiesOnRadius(float cx, float cy, float radius)
        {
            var entities = GetEntitiesOnArea(new RectangleF(cx - radius, cy - radius, radius * 2, radius * 2));
            var result = new List<Entity>();

            foreach (var e in Entities)
            {
                if (Mathf.Distance(e.X, e.Y, cx, cy) <= radius) result.Add(e);
            }

            return result;
        }
    }
}