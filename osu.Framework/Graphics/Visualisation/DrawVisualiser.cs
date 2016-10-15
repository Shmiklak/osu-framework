﻿// Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Linq;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Drawables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Threading;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics.Primitives;

namespace osu.Framework.Graphics.Visualisation
{
    public class DrawVisualiser : Container
    {
        private TreeContainer treeContainer;

        public DrawVisualiser()
        {
            RelativeSizeAxes = Axes.Both;
        }

        private VisualisedDrawable targetVD;

        private Drawable target;
        public Drawable Target
        {
            get { return target; }
            set
            {
                if (targetVD != null)
                    treeContainer.Remove(targetVD);

                target = value;
                targetVD = new VisualisedDrawable(target);
                treeContainer.Add(targetVD);
            }
        }


        public override void Load(BaseGame game)
        {
            base.Load(game);

            Add(treeContainer = new TreeContainer()
            {
                BeginRun = delegate { Scheduler.AddDelayed(runUpdate, 200, true); }
            });
        }

        private void runUpdate()
        {
            if (Target == null) return;

            visualise(Target, targetVD);
        }

        private void visualise(Drawable d, VisualisedDrawable vis)
        {
            if (d == this) return;

            vis.CheckExpiry();

            var drawables = vis.Flow.Children.Cast<VisualisedDrawable>();
            foreach (var dd in drawables)
            {
                if (!dd.CheckExpiry())
                    visualise(dd.Target, dd);
            }

            Container dContainer = d as Container;
            if (dContainer == null) return;

            foreach (var c in dContainer.Children)
            {
                var dr = drawables.FirstOrDefault(x => x.Target == c);

                if (dr == null)
                {
                    dr = new VisualisedDrawable(c)
                    {
                        Selected = onSelect
                    };
                    vis.Flow.Add(dr);
                }

                visualise(c, dr);
            }
        }

        private InfoOverlay overlay;

        private void onSelect(VisualisedDrawable obj)
        {
            if (overlay != null)
                Remove(overlay);

            Add(overlay = new InfoOverlay(obj.Target));
        }

        class InfoOverlay : Container
        {
            private Drawable target;

            private Box tl, tr, bl, br;

            public InfoOverlay(Drawable target)
            {
                this.target = target;
                target.OnInvalidate += update;

                RelativeSizeAxes = Axes.Both;

                Children = new Drawable[]
                {
                    tl = new Box
                    {
                        Size = new Vector2(5),
                        Colour = Color4.Red,
                    },
                    tr = new Box
                    {
                        Size = new Vector2(5),
                        Colour = Color4.Red,
                    },
                    bl = new Box
                    {
                        Size = new Vector2(5),
                        Colour = Color4.Red,
                    },
                    br = new Box
                    {
                        Size = new Vector2(5),
                        Colour = Color4.Red,
                    }
                };
            }

            public override void Load(BaseGame game)
            {
                base.Load(game);
                update();
            }

            private void update()
            {
                Quad q = target.ScreenSpaceDrawQuad * DrawInfo.MatrixInverse;

                tl.Position = q.TopLeft;
                tr.Position = q.TopRight;
                bl.Position = q.BottomLeft;
                br.Position = q.BottomRight;
            }
        }
    }
}
