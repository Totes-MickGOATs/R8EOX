using System.Collections.Generic;
using UnityEngine;

namespace R8EOX.Menu.Internal
{
    internal class MenuNavigator
    {
        private readonly Stack<MenuScreen> screenStack = new Stack<MenuScreen>();
        private readonly float transitionDuration;
        private bool isTransitioning;
        private float transitionEndTime;

        internal MenuScreen CurrentScreen =>
            screenStack.Count > 0 ? screenStack.Peek() : null;

        internal int Depth => screenStack.Count;

        internal MenuNavigator(float transitionDuration)
        {
            this.transitionDuration = transitionDuration;
        }

        // Must be called each frame by the owning MonoBehaviour so the
        // transition lock is released once the fade duration has elapsed.
        internal void Tick()
        {
            if (isTransitioning && Time.unscaledTime >= transitionEndTime)
                isTransitioning = false;
        }

        internal void PushScreen(MenuScreen screen)
        {
            if (screen == null || isTransitioning) return;

            isTransitioning = true;
            transitionEndTime = Time.unscaledTime + transitionDuration;

            if (screenStack.Count > 0)
            {
                var current = screenStack.Peek();
                current.Hide(transitionDuration);
            }

            screenStack.Push(screen);
            screen.Show(transitionDuration);
        }

        internal void PopScreen()
        {
            if (screenStack.Count <= 1 || isTransitioning) return;

            isTransitioning = true;
            transitionEndTime = Time.unscaledTime + transitionDuration;

            var leaving = screenStack.Pop();
            leaving.Hide(transitionDuration);

            if (screenStack.Count > 0)
            {
                var returning = screenStack.Peek();
                returning.Show(transitionDuration);
            }
        }

        internal void PopToRoot()
        {
            if (screenStack.Count <= 1 || isTransitioning) return;

            isTransitioning = true;
            transitionEndTime = Time.unscaledTime + transitionDuration;

            while (screenStack.Count > 1)
            {
                var leaving = screenStack.Pop();
                leaving.HideImmediate();
            }

            if (screenStack.Count > 0)
            {
                var root = screenStack.Peek();
                root.Show(transitionDuration);
            }
        }

        internal void ReplaceScreen(MenuScreen screen)
        {
            if (screen == null || isTransitioning) return;

            isTransitioning = true;
            transitionEndTime = Time.unscaledTime + transitionDuration;

            if (screenStack.Count > 0)
            {
                var leaving = screenStack.Pop();
                leaving.Hide(transitionDuration);
            }

            screenStack.Push(screen);
            screen.Show(transitionDuration);
        }

        internal void Clear()
        {
            while (screenStack.Count > 0)
            {
                var screen = screenStack.Pop();
                screen.HideImmediate();
            }
        }
    }
}
