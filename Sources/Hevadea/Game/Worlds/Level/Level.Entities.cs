﻿using System.Collections.Generic;
using System.Linq;
using Hevadea.Game.Entities;
using Hevadea.Game.Registry;
using Hevadea.Game.Tiles;
using Microsoft.Xna.Framework;

namespace Hevadea.Game.Worlds
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
            entity.SetPosition(tx * ConstVal.TileSize + (ConstVal.TileSize / 2 - entity.Width / 2) + offX, ty * ConstVal.TileSize + (ConstVal.TileSize / 2 - entity.Height / 2) + offY);
            return entity;
        }

        public Entity SpawnEntity(EntityBlueprint blueprint, int tx, int ty, float offX = 0f, float offY = 0f)
        {
            var entity = blueprint.Build();
            SpawnEntity(entity, tx, ty, offX, offY);
            return entity;
        }
        
        public void AddEntity(Entity e)
        {
            e.Removed = false;
            if (!_entities.Contains(e)) _entities.Add(e);

            e.Initialize(this, _world, _game);
            AddEntityToTile(e.GetTilePosition(), e);
        }

        public void RemoveEntity(Entity e)
        {
            _entities.Remove(e);
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
            var result = new List<Entity>();


            if (tx < Width && ty < Height && tx >= 0 && ty >= 0) result.AddRange(_entitiesOnTiles[tx, ty]);

            return result;
        }

        public List<Entity> GetEntitiesOnArea(Rectangle area)
        {
            var result = new List<Entity>();

            var beginX = area.X / ConstVal.TileSize - 1;
            var beginY = area.Y / ConstVal.TileSize - 1;

            var endX = (area.X + area.Width) / ConstVal.TileSize + 1;
            var endY = (area.Y + area.Height) / ConstVal.TileSize + 1;


            for (var x = beginX; x < endX; x++)
            for (var y = beginY; y < endY; y++)
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height) continue;
                var entities = _entitiesOnTiles[x, y];
                result.AddRange(entities.Where(i => i.IsColliding(area)));
            }

            return result;
        }
    }
}