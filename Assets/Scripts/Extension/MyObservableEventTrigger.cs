using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

namespace Main.Extension
{
    public class MyObservableEventTrigger : ObservableEventTrigger
    {
        public IObservable<PointerEventData> OnLongTapAsObservable(float time)
        {
            return OnPointerDownAsObservable()
                .Throttle(TimeSpan.FromSeconds(time))
                .TakeUntil(OnPointerExitAsObservable())
                .TakeUntil(OnPointerUpAsObservable())
                .RepeatUntilDestroy(this);
        }
    }
}
