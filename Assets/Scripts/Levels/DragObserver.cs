using Events;
using Player.ActionHandlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Levels
{
    public class DragObserver : MonoBehaviour
    {
        private ClickHandler _clickHandler;

        private void Awake()
        {
            _clickHandler = ClickHandler.Instance;
            
            _clickHandler.DragStartEvent += OnDragStart;
        }

        private void OnDestroy()
        {
            _clickHandler.DragStartEvent -= OnDragStart;
        }
        
        private void OnDragStart(Vector3 position)
        {
            if (!EventSystem.current.IsPointerOverGameObject()
                && EventSystem.current.currentSelectedGameObject == null)
            {
                EventsController.Fire(new EventModels.Game.DragStarted());
            }
        }
    }
}