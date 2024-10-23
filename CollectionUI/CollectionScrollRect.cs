using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Alchemy.Meta.Collection
{
    public class CollectionScrollRect : ScrollRect
    {
        public event Action OnBeginDragScroll;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            OnBeginDragScroll?.Invoke();
        }
    }
}