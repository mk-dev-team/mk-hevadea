﻿using Hevadea.Framework;
using Hevadea.Framework.Graphic;
using Hevadea.Framework.Utils;
using Hevadea.GameObjects.Entities.Components.Ai.Actions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Hevadea.GameObjects.Entities.Components.Ai.Behaviors
{
    public class BehaviorEnemy : BehaviorAnimal
    {
        public float AgroRange { get; set; } = 5f;
        public float FollowRange { get; set; } = 7f;
        public float ChanceToAgro { get; set; } = 0.5f;
        public float MoveSpeedAgro { get; set; } = 0.5f;

        public Entity Target { get; private set; } = null;

        private List<Entity> _targets;

        public override void IaAborted(Agent agent, AgentAbortReason why)
        {
            base.Update(agent, null);
        }

        public override void Update(Agent agent, GameTime gameTime)
        {
            if (Target != null &&
                Target.Level == agent.Owner.Level &&
                Mathf.Distance(agent.Owner.Position, Target.Position) > FollowRange * 16 && !agent.IsBusy())

            {
                agent.Abort(AgentAbortReason.TagetLost);
                Target = null;
            }

            if (!agent.IsBusy())
            {
                if (Target == null)
                {
                    var game = agent.Owner.Game;

                    if (game.LocalPlayer.Entity != null &&
                        game.LocalPlayer.Entity.Level == agent.Owner.Level &&
                        Mathf.Distance(game.LocalPlayer.Entity.Position, agent.Owner.Position) < AgroRange * 16 &&
                        Rise.Rnd.NextFloat() < ChanceToAgro)
                    {
                        Target = game.LocalPlayer.Entity;
                    }
                }

                if (Target != null)
                {
                    agent.MoveTo(Target.GetTilePosition(), MoveSpeedAgro, true, (int)(FollowRange + 4));
                }
            }

            if (!agent.IsBusy())
                base.Update(agent, gameTime);
        }

        public override void DrawDebug(SpriteBatch spriteBatch, Agent agent, GameTime gameTime)
        {
            spriteBatch.DrawCircle(agent.Owner.Position, AgroRange * Game.Unit, 24, Target == null ? Color.Green : Color.Red);
            spriteBatch.DrawCircle(agent.Owner.Position, FollowRange * Game.Unit, 24, Color.White * 0.5f);
        }
    }
}