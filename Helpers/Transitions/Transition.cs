using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bEngine;
using bEngine.Graphics;

namespace bEngine.Helpers.Transitions
{
    public enum TransitionState { NotInitialized, FadeOut, Halfway, FadeIn, Finished };

    // Base transition class
    // Sets the interface that will be used
    public class Transition
    {
        public bGame game;
        public TransitionState currentState;

        public Transition(bGame game)
        {
            this.game = game;
            currentState = TransitionState.NotInitialized;
        }

        virtual public bool finished()
        {
            return currentState == TransitionState.Finished;
        }

        virtual public bool expectingWorldChange()
        {
            return currentState == TransitionState.Halfway;
        }

        virtual public void worldChanged()
        {
            if (currentState == TransitionState.Halfway)
                currentState = TransitionState.FadeIn;
        }

        virtual public void update()
        {

        }

        virtual public void render(SpriteBatch sb)
        {

        }
    }

    // No transition
    public class NilTransition : Transition
    {
        bool changed;

        public NilTransition(bGame game)
            : base(game)
        {
            changed = false;
        }

        public override void worldChanged()
        {
            base.worldChanged();

            changed = true;
        }

        public override bool expectingWorldChange()
        {
            return !changed;
        }

        public override bool finished()
        {
            return changed;
        }
    }

    // Fade to given color during amount of time
    public class FadeToColor : Transition
    {
        Color color;
        float dAlpha;
        int stepsEachPhase;
        int timer;

        Texture2D image;

        public FadeToColor(bGame game, Color color, int stepsEachPhase = 10) : base(game)
        {
            this.color = color;
            color.A = 0;
            this.stepsEachPhase = stepsEachPhase;
            image = bDummyRect.sharedDummyRect(game);
        }

        public override bool expectingWorldChange()
        {
            return (base.expectingWorldChange() && timer == -2);
        }

        public override void worldChanged()
        {
            // Perform fadein init routine
            color.A = 255;
            timer = stepsEachPhase;
            dAlpha = 256f / stepsEachPhase;
            currentState = TransitionState.FadeIn;

            base.worldChanged();
        }

        override public void update()
        {
            switch (currentState)
            {
                case TransitionState.NotInitialized:
                    // Perform fadeout init routine
                    color.A = 0;
                    timer = stepsEachPhase;
                    dAlpha = 256f / stepsEachPhase;
                    currentState = TransitionState.FadeOut;
                    break;
                case TransitionState.FadeOut:
                    timer--;
                    color.A += (byte) dAlpha;
                    if (timer <= 0)
                    {
                        timer = -1;
                        currentState = TransitionState.Halfway;
                    }
                    break;
                case TransitionState.Halfway:
                    color.A = 255;
                    // Reinit timer to wait a little in full tinted screen
                    if (timer == -1)
                    {
                        timer = stepsEachPhase / 2;
                    }
                    else
                    {
                        // and wait!
                        if (--timer < 0)
                        {
                            timer = -2;
                        }
                    }
                    break;
                case TransitionState.FadeIn:
                    timer--;
                    color.A -= (byte) dAlpha;
                    if (timer <= 0)
                        currentState = TransitionState.Finished;
                    break;
                case TransitionState.Finished:
                    color.A = 0;
                    break;
            }
        }

        override public void render(SpriteBatch sb)
        {
            if (currentState == TransitionState.NotInitialized)
                color.A = 0;
            Console.WriteLine(color.A);
            sb.Draw(image, new Rectangle(0, 0, game.GraphicsDevice.DisplayMode.Width, 
                                         game.GraphicsDevice.DisplayMode.Height), color);
        }
    }

    // Immediately change to given color during amount of time
    public class BlinkToColor : Transition
    {
        Color color;
        int stepsEachPhase;
        int timer;

        Texture2D image;

        public BlinkToColor(bGame game, Color color, int stepsEachPhase = 5)
            : base(game)
        {
            this.color = color;
            color.A = 255;
            this.stepsEachPhase = stepsEachPhase;
            image = bDummyRect.sharedDummyRect(game);
        }

        public override bool expectingWorldChange()
        {
            return (base.expectingWorldChange());
        }

        public override void worldChanged()
        {
            // Perform fadein init routine
            color.A = 255;
            timer = stepsEachPhase;
            currentState = TransitionState.FadeIn;

            base.worldChanged();
        }

        override public void update()
        {
            color.A = 255;
            switch (currentState)
            {
                case TransitionState.NotInitialized:
                    // Perform fadeout init routine
                    timer = stepsEachPhase;
                    currentState = TransitionState.FadeOut;
                    break;
                case TransitionState.FadeOut:
                    timer--;
                    if (timer <= 0)
                    {
                        timer = -1;
                        currentState = TransitionState.Halfway;
                    }
                    break;
                case TransitionState.Halfway:
                    break;
                case TransitionState.FadeIn:
                    timer--;
                    if (timer <= 0)
                        currentState = TransitionState.Finished;
                    break;
                case TransitionState.Finished:
                    color.A = 0;
                    break;
            }
        }

        override public void render(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(0, 0, game.GraphicsDevice.DisplayMode.Width,
                                         game.GraphicsDevice.DisplayMode.Height), color);
        }
    }
}