using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    public class Swipe
    {
        static Vector2[] compass = new Vector2[] { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        const float DIRRECTION_EPS = 0.7f;
        const float sqrDIRRECTION_EPS = DIRRECTION_EPS * DIRRECTION_EPS;

        public Vector2 start;
        public Vector2 end;
        public float time;

        public Vector2 Vector => end - start;
        public float Magnitude => Vector.magnitude;
        public float SqrMagnitude => Vector.sqrMagnitude;
        public Vector2 Direction => Vector.normalized;

        public bool IsRight => Vector2.Dot(Direction, Vector2.right) >= DIRRECTION_EPS;
        public bool IsLeft => Vector2.Dot(Direction, Vector2.left) >= DIRRECTION_EPS;
        public bool IsUp => Vector2.Dot(Direction, Vector2.up) >= DIRRECTION_EPS;
        public bool IsDown => Vector2.Dot(Direction, Vector2.down) >= DIRRECTION_EPS;

        public Vector2 EightDirection
        {
            get
            {
                var d = Direction;
                d.x = Mathf.RoundToInt(d.x);
                d.y = Mathf.RoundToInt(d.y);
                d.Normalize();
                return d;
            }
        }

        public Vector2 FourDirection
        {
            get
            {
                var max = float.MinValue;
                var dir = Direction;
                Vector2 res = Vector2.zero;

                for (int i = 0; i < compass.Length; i++)
                {
                    var c = compass[i];
                    var dot = Vector2.Dot(dir, c);
                    if ( dot > max)
                    {
                        res = c;
                        max = dot;
                    }
                }

                return res;
            }
        }

        public bool IsHorizontal
        {
            get
            {
                var dot = Math.Abs(Vector2.Dot(Direction, Vector2.right));
                return dot >= DIRRECTION_EPS;
            }
        }

        public bool IsVertical
        {
            get
            {
                var dot = Math.Abs(Vector2.Dot(Direction, Vector2.up));
                return dot >= DIRRECTION_EPS;
            }
        }

        public bool IsHorOrVert
        {
            get
            {
                var cos = Math.Abs(Vector2.Dot(Direction, Vector2.right));
                float sqrSin =  1 - cos * cos ;
                return cos >= DIRRECTION_EPS || sqrSin > sqrDIRRECTION_EPS;
            }
        }

    }

    [DisallowMultipleComponent]
    public class ObservableSwipeTrigger : ObservableTriggerBase, IEventSystemHandler, IBeginDragHandler, IDragHandler
    {
        Subject<Swipe> onSwipe;
        Swipe swipe;
        float startTime;

        float minLength = -1;
        float maxTime = float.MaxValue;
        bool single = false;
        bool isDispatched = false;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            swipe = new Swipe();
            swipe.start = Input.mousePosition;
            swipe.time = 0;
            startTime = Time.realtimeSinceStartup;
            isDispatched = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            swipe.end = Input.mousePosition;
            swipe.time = Time.realtimeSinceStartup - startTime;
            if (!isDispatched && swipe.SqrMagnitude >= minLength && swipe.time <= maxTime)
            {
                OnSwipe(swipe);
                isDispatched = single;
            }
        }

        void OnSwipe(Swipe eventData)
        {
            if (onSwipe != null) onSwipe.OnNext(eventData);
        }

        public IObservable<Swipe> OnSwipeAsObservable()
        {
            return onSwipe ?? ( onSwipe = new Subject<Swipe>() );
        }

        public ObservableSwipeTrigger SwipWithLength(float minLength)
        {
            this.minLength = minLength;
            return this;
        }

        public ObservableSwipeTrigger SwipeWithTime(float maxTime)
        {
            this.maxTime = maxTime;
            return this;
        }

        public ObservableSwipeTrigger SwipeAsSingel()
        {
            single = true;
            return this;
        }

        public ObservableSwipeTrigger OnSwipeAsRepeated()
        {
            single = false;
            return this;
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onSwipe != null)
            {
                onSwipe.OnCompleted();
            }
        }
    }
}
